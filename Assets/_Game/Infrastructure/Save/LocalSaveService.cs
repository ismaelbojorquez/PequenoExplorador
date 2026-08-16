using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Logging;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class LocalSaveService : ISaveService
    {
        public const int CurrentSchemaVersion = 4;

        private readonly IFileStore _fileStore;
        private readonly UnityJsonSaveSerializer _serializer;
        private readonly IReadOnlyDictionary<int, ISaveMigration> _migrations;
        private readonly IAppLogger _logger;
        private readonly string _appVersion;
        private readonly SemaphoreSlim _operation = new SemaphoreSlim(1, 1);
        private int _saveSequence;
        private bool _initialized;

        public LocalSaveService(
            IFileStore fileStore,
            string appVersion,
            IAppLogger logger,
            IEnumerable<ISaveMigration> migrations)
        {
            _fileStore = fileStore ?? throw new ArgumentNullException(nameof(fileStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (string.IsNullOrWhiteSpace(appVersion))
            {
                throw new ArgumentException("App version is required.", nameof(appVersion));
            }

            _appVersion = appVersion;
            _serializer = new UnityJsonSaveSerializer();
            ISaveMigration[] migrationArray = (migrations ?? throw new ArgumentNullException(nameof(migrations))).ToArray();
            if (migrationArray.Any(item => item == null || item.ToVersion != item.FromVersion + 1) ||
                migrationArray.Select(item => item.FromVersion).Distinct().Count() != migrationArray.Length)
            {
                throw new ArgumentException("Save migrations must be unique, non-null n-to-n+1 steps.", nameof(migrations));
            }

            _migrations = migrationArray.ToDictionary(item => item.FromVersion);
            Current = PlayerProgress.CreateDefault();
            LastLoadResult = new SaveLoadResult(Current, SaveLoadStatus.DefaultCreated, SaveUserNotice.None, 0);
        }

        public string ServiceId => "Save";
        public PlayerProgress Current { get; private set; }
        public SaveLoadResult LastLoadResult { get; private set; }
        public bool IsReadOnly { get; private set; }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            await _operation.WaitAsync(cancellationToken);
            try
            {
                if (_initialized)
                {
                    return;
                }

                await _fileStore.DiscardTemporaryAsync();
                LastLoadResult = await LoadCoreAsync(cancellationToken);
                Current = LastLoadResult.Progress;
                _initialized = true;
                _logger.Write(new AppLogEntry(
                    AppLogLevel.Info,
                    "Save",
                    "LoadCompleted",
                    LastLoadResult.Status.ToString()));
            }
            finally
            {
                _operation.Release();
            }
        }

        public void Shutdown()
        {
            // Writes are explicit checkpoints. Bootstrap gives the coordinator a bounded flush window.
        }

        public async Task<SaveOperationResult> SaveAsync(
            PlayerProgress progress,
            CancellationToken cancellationToken)
        {
            if (progress == null)
            {
                throw new ArgumentNullException(nameof(progress));
            }

            await _operation.WaitAsync(cancellationToken);
            try
            {
                EnsureInitialized();
                if (IsReadOnly)
                {
                    return new SaveOperationResult(
                        SaveOperationStatus.BlockedByFutureVersion,
                        "SaveFutureVersionReadOnly");
                }

                return await WriteProgressAsync(
                    progress,
                    SaveCommitMode.RotatePrimaryToBackup,
                    cancellationToken);
            }
            finally
            {
                _operation.Release();
            }
        }

        public async Task<SaveOperationResult> ResetAsync(CancellationToken cancellationToken)
        {
            await _operation.WaitAsync(cancellationToken);
            try
            {
                EnsureInitialized();
                if (IsReadOnly)
                {
                    return new SaveOperationResult(
                        SaveOperationStatus.BlockedByFutureVersion,
                        "SaveFutureVersionReadOnly");
                }

                await _fileStore.DeleteAllAsync(cancellationToken);
                _saveSequence = 0;
                PlayerProgress reset = PlayerProgress.CreateDefault();
                SaveOperationResult result = await WriteProgressAsync(
                    reset,
                    SaveCommitMode.PreserveBackup,
                    cancellationToken);
                if (result.IsSuccess)
                {
                    LastLoadResult = new SaveLoadResult(
                        reset,
                        SaveLoadStatus.DefaultCreated,
                        SaveUserNotice.None,
                        CurrentSchemaVersion);
                }

                return result;
            }
            finally
            {
                _operation.Release();
            }
        }

        private async Task<SaveLoadResult> LoadCoreAsync(CancellationToken cancellationToken)
        {
            bool hasPrimary = await _fileStore.ExistsAsync(SaveFileKind.Primary, cancellationToken);
            bool hasBackup = await _fileStore.ExistsAsync(SaveFileKind.Backup, cancellationToken);
            if (!hasPrimary && !hasBackup)
            {
                PlayerProgress created = PlayerProgress.CreateDefault();
                SaveOperationResult result = await WriteProgressAsync(
                    created,
                    SaveCommitMode.PreserveBackup,
                    cancellationToken);
                if (!result.IsSuccess)
                {
                    throw new SaveDataException(result.ErrorCode);
                }

                return new SaveLoadResult(
                    created,
                    SaveLoadStatus.DefaultCreated,
                    SaveUserNotice.None,
                    CurrentSchemaVersion);
            }

            if (hasPrimary)
            {
                try
                {
                    LoadedProgress primary = await ReadAsync(SaveFileKind.Primary, cancellationToken);
                    if (primary.IsFutureVersion)
                    {
                        return EnterFutureVersionMode(primary.SourceSchemaVersion);
                    }

                    ApplyLoaded(primary);
                    if (primary.WasMigrated)
                    {
                        SaveOperationResult migratedWrite = await WriteProgressAsync(
                            primary.Progress,
                            SaveCommitMode.RotatePrimaryToBackup,
                            cancellationToken);
                        if (!migratedWrite.IsSuccess)
                        {
                            throw new SaveDataException(migratedWrite.ErrorCode);
                        }
                    }

                    return new SaveLoadResult(
                        primary.Progress,
                        primary.WasMigrated ? SaveLoadStatus.Migrated : SaveLoadStatus.Loaded,
                        SaveUserNotice.None,
                        primary.SourceSchemaVersion);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    _logger.Write(new AppLogEntry(
                        AppLogLevel.Warning,
                        "Save",
                        "PrimaryRejected",
                        GetErrorCode(exception)));
                }
            }

            if (!hasBackup)
            {
                throw new SaveDataException("SavePrimaryInvalidNoBackup");
            }

            LoadedProgress backup = await ReadAsync(SaveFileKind.Backup, cancellationToken);
            if (backup.IsFutureVersion)
            {
                return EnterFutureVersionMode(backup.SourceSchemaVersion);
            }

            ApplyLoaded(backup);
            SaveOperationResult repair = await WriteProgressAsync(
                backup.Progress,
                SaveCommitMode.PreserveBackup,
                cancellationToken);
            if (!repair.IsSuccess)
            {
                _logger.Write(new AppLogEntry(
                    AppLogLevel.Warning,
                    "Save",
                    "PrimaryRepairDeferred",
                    repair.ErrorCode));
            }

            return new SaveLoadResult(
                backup.Progress,
                SaveLoadStatus.RecoveredBackup,
                SaveUserNotice.ProgressRecovered,
                backup.SourceSchemaVersion);
        }

        private async Task<LoadedProgress> ReadAsync(
            SaveFileKind kind,
            CancellationToken cancellationToken)
        {
            string serialized = await _fileStore.ReadTextAsync(kind, cancellationToken);
            SaveEnvelopeData envelope = _serializer.DeserializeEnvelope(serialized);
            if (envelope.SchemaVersion > CurrentSchemaVersion)
            {
                return LoadedProgress.Future(envelope.SchemaVersion);
            }

            _serializer.ValidateChecksum(envelope);
            int sourceVersion = envelope.SchemaVersion;
            int currentVersion = sourceVersion;
            string payload = envelope.Payload;
            while (currentVersion < CurrentSchemaVersion)
            {
                if (!_migrations.TryGetValue(currentVersion, out ISaveMigration migration))
                {
                    throw new SaveDataException("SaveMigrationMissingV" + currentVersion);
                }

                payload = migration.Migrate(payload);
                currentVersion = migration.ToVersion;
            }

            if (currentVersion != CurrentSchemaVersion)
            {
                throw new SaveDataException("SaveSchemaUnsupported");
            }

            DecodedSaveData decoded = _serializer.DeserializeCurrentPayload(payload);
            return LoadedProgress.Current(
                decoded.Progress,
                decoded.SaveSequence,
                sourceVersion,
                sourceVersion != CurrentSchemaVersion);
        }

        private async Task<SaveOperationResult> WriteProgressAsync(
            PlayerProgress progress,
            SaveCommitMode commitMode,
            CancellationToken cancellationToken)
        {
            int nextSequence = checked(_saveSequence + 1);
            string serialized = _serializer.Serialize(progress, _appVersion, nextSequence);
            try
            {
                await _fileStore.WriteTemporaryAsync(serialized, cancellationToken);
                await _fileStore.FlushTemporaryAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                _fileStore.CommitTemporary(commitMode);
                _saveSequence = nextSequence;
                Current = progress;
                return SaveOperationResult.Saved();
            }
            catch (OperationCanceledException)
            {
                await _fileStore.DiscardTemporaryAsync();
                throw;
            }
            catch (Exception exception)
            {
                await _fileStore.DiscardTemporaryAsync();
                string errorCode = GetErrorCode(exception);
                _logger.Write(new AppLogEntry(AppLogLevel.Error, "Save", "WriteFailed", errorCode));
                return new SaveOperationResult(SaveOperationStatus.Failed, errorCode);
            }
        }

        private SaveLoadResult EnterFutureVersionMode(int schemaVersion)
        {
            IsReadOnly = true;
            PlayerProgress safeDefault = PlayerProgress.CreateDefault();
            Current = safeDefault;
            _logger.Write(new AppLogEntry(
                AppLogLevel.Warning,
                "Save",
                "FutureVersionReadOnly",
                "Schema" + schemaVersion));
            return new SaveLoadResult(
                safeDefault,
                SaveLoadStatus.FutureVersion,
                SaveUserNotice.NewerSaveVersionDetected,
                schemaVersion);
        }

        private void ApplyLoaded(LoadedProgress loaded)
        {
            Current = loaded.Progress;
            _saveSequence = loaded.SaveSequence;
        }

        private void EnsureInitialized()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Save service must initialize before use.");
            }
        }

        private static string GetErrorCode(Exception exception)
        {
            return exception is SaveDataException saveData
                ? saveData.ErrorCode
                : exception.GetType().Name;
        }

        private sealed class LoadedProgress
        {
            private LoadedProgress(
                PlayerProgress progress,
                int saveSequence,
                int sourceSchemaVersion,
                bool wasMigrated,
                bool isFutureVersion)
            {
                Progress = progress;
                SaveSequence = saveSequence;
                SourceSchemaVersion = sourceSchemaVersion;
                WasMigrated = wasMigrated;
                IsFutureVersion = isFutureVersion;
            }

            public PlayerProgress Progress { get; }
            public int SaveSequence { get; }
            public int SourceSchemaVersion { get; }
            public bool WasMigrated { get; }
            public bool IsFutureVersion { get; }

            public static LoadedProgress Current(
                PlayerProgress progress,
                int saveSequence,
                int sourceSchemaVersion,
                bool wasMigrated)
            {
                return new LoadedProgress(progress, saveSequence, sourceSchemaVersion, wasMigrated, false);
            }

            public static LoadedProgress Future(int sourceSchemaVersion)
            {
                return new LoadedProgress(
                    PlayerProgress.CreateDefault(),
                    0,
                    sourceSchemaVersion,
                    false,
                    true);
            }
        }
    }
}
