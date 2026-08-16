using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Infrastructure.Save;
using PequenoExplorador.Tests.EditMode.Fixtures;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class SaveServiceTests
    {
        [Test]
        public async Task FirstRunCreatesDefaultSchemaV1()
        {
            var store = new InMemoryFileStore();
            LocalSaveService service = CreateService(store);

            await service.InitializeAsync(CancellationToken.None);
            SaveEnvelopeData envelope = new UnityJsonSaveSerializer().DeserializeEnvelope(store.Primary);

            Assert.That(service.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.DefaultCreated));
            Assert.That(service.Current.Stars, Is.Zero);
            Assert.That(service.Current.WorldIds, Is.Empty);
            Assert.That(service.Current.DiscoveryIds, Is.Empty);
            Assert.That(service.Current.CompletedMissionIds, Is.Empty);
            Assert.That(envelope.SchemaVersion, Is.EqualTo(1));
        }

        [Test]
        public async Task RoundTripRestoresProgressAndSettings()
        {
            var store = new InMemoryFileStore();
            LocalSaveService writer = CreateService(store);
            await writer.InitializeAsync(CancellationToken.None);
            var expected = new PlayerProgress(
                7,
                new[] { "world.jungle" },
                new[] { "discovery.test" },
                new[] { "mission.test" },
                new PlayerPreferences(GuidanceMode.MoreGuidance, false, true, false));

            Assert.That((await writer.SaveAsync(expected, CancellationToken.None)).IsSuccess, Is.True);
            LocalSaveService reader = CreateService(store);
            await reader.InitializeAsync(CancellationToken.None);

            AssertProgress(reader.Current, expected);
            Assert.That(reader.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.Loaded));
        }

        [TestCase(FileStoreFailurePoint.WriteTemporary)]
        [TestCase(FileStoreFailurePoint.FlushTemporary)]
        [TestCase(FileStoreFailurePoint.CommitTemporary)]
        public async Task FailureBeforeAtomicCommitPreservesPrimaryAndBackup(FileStoreFailurePoint point)
        {
            var store = new InMemoryFileStore();
            LocalSaveService service = CreateService(store);
            await service.InitializeAsync(CancellationToken.None);
            string primaryBefore = store.Primary;
            string backupBefore = store.Backup;
            store.FailurePoint = point;

            SaveOperationResult result = await service.SaveAsync(
                service.Current.WithStars(99),
                CancellationToken.None);

            Assert.That(result.Status, Is.EqualTo(SaveOperationStatus.Failed));
            Assert.That(store.Primary, Is.EqualTo(primaryBefore));
            Assert.That(store.Backup, Is.EqualTo(backupBefore));
            Assert.That(service.Current.Stars, Is.Zero);
        }

        [Test]
        public async Task TruncatedPrimaryRecoversBackupWithoutReplacingBackupWithCorruptData()
        {
            var store = new InMemoryFileStore();
            LocalSaveService writer = CreateService(store);
            await writer.InitializeAsync(CancellationToken.None);
            await writer.SaveAsync(writer.Current.WithStars(5), CancellationToken.None);
            string validBackup = store.Backup;
            store.SeedPrimary(store.Primary.Substring(0, store.Primary.Length / 2));

            LocalSaveService reader = CreateService(store);
            await reader.InitializeAsync(CancellationToken.None);

            Assert.That(reader.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.RecoveredBackup));
            Assert.That(reader.LastLoadResult.UserNotice, Is.EqualTo(SaveUserNotice.ProgressRecovered));
            Assert.That(reader.Current.Stars, Is.Zero, "Backup contains the previous committed checkpoint.");
            Assert.That(store.Backup, Is.EqualTo(validBackup));
            Assert.That(store.Primary, Is.Not.EqualTo(validBackup), "Repair advances technical save sequence.");
        }

        [Test]
        public async Task ChecksumMismatchRecoversBackup()
        {
            var store = new InMemoryFileStore();
            LocalSaveService writer = CreateService(store);
            await writer.InitializeAsync(CancellationToken.None);
            await writer.SaveAsync(writer.Current.WithStars(5), CancellationToken.None);
            store.SeedPrimary(store.Primary.Replace("\\\"stars\\\":5", "\\\"stars\\\":6"));

            LocalSaveService reader = CreateService(store);
            await reader.InitializeAsync(CancellationToken.None);

            Assert.That(reader.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.RecoveredBackup));
            Assert.That(reader.Current.Stars, Is.Zero);
        }

        [Test]
        public void InvalidPrimaryWithoutBackupFailsRecoverablyAndPreservesEvidence()
        {
            var store = new InMemoryFileStore();
            const string invalid = "{truncated";
            store.SeedPrimary(invalid);
            LocalSaveService service = CreateService(store);

            Assert.ThrowsAsync<SaveDataException>(async () =>
                await service.InitializeAsync(CancellationToken.None));
            Assert.That(store.Primary, Is.EqualTo(invalid));
            Assert.That(store.Backup, Is.Null);
        }

        [Test]
        public async Task LegacyV0MigratesStepByStepAndPreservesOriginalAsBackup()
        {
            var store = new InMemoryFileStore();
            var serializer = new UnityJsonSaveSerializer();
            string legacy = serializer.SerializeEnvelope(
                0,
                "{\"appVersion\":\"0.0.1\",\"stars\":9}");
            store.SeedPrimary(legacy);
            LocalSaveService service = CreateService(store);

            await service.InitializeAsync(CancellationToken.None);
            SaveEnvelopeData current = serializer.DeserializeEnvelope(store.Primary);

            Assert.That(service.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.Migrated));
            Assert.That(service.LastLoadResult.SourceSchemaVersion, Is.Zero);
            Assert.That(service.Current.Stars, Is.EqualTo(9));
            Assert.That(current.SchemaVersion, Is.EqualTo(1));
            Assert.That(store.Backup, Is.EqualTo(legacy));
        }

        [Test]
        public async Task FutureVersionIsReadOnlyAndNeverOverwritten()
        {
            var store = new InMemoryFileStore();
            string future = new UnityJsonSaveSerializer().SerializeEnvelope(99, "{\"future\":true}");
            store.SeedPrimary(future);
            LocalSaveService service = CreateService(store);

            await service.InitializeAsync(CancellationToken.None);
            SaveOperationResult attempted = await service.SaveAsync(
                PlayerProgress.CreateDefault().WithStars(3),
                CancellationToken.None);

            Assert.That(service.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.FutureVersion));
            Assert.That(service.IsReadOnly, Is.True);
            Assert.That(attempted.Status, Is.EqualTo(SaveOperationStatus.BlockedByFutureVersion));
            Assert.That(store.Primary, Is.EqualTo(future));
            Assert.That(store.Backup, Is.Null);
        }

        [Test]
        public async Task MissingMigrationFailsWithoutRewritingLegacyPrimary()
        {
            var store = new InMemoryFileStore();
            var serializer = new UnityJsonSaveSerializer();
            string legacy = serializer.SerializeEnvelope(
                0,
                "{\"appVersion\":\"0.0.1\",\"stars\":1}");
            store.SeedPrimary(legacy);
            var service = new LocalSaveService(
                store,
                "0.1.0-test",
                new RecordingLogger(),
                Array.Empty<ISaveMigration>());

            Assert.ThrowsAsync<SaveDataException>(async () =>
                await service.InitializeAsync(CancellationToken.None));
            Assert.That(store.Primary, Is.EqualTo(legacy));
            Assert.That(store.Backup, Is.Null);
        }

        [Test]
        public void CancellationBeforeCommitPreservesLastCommittedSave()
        {
            var store = new InMemoryFileStore();
            LocalSaveService service = CreateService(store);
            service.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
            string primaryBefore = store.Primary;
            using var cancellation = new CancellationTokenSource();
            store.BeforeFlush = cancellation.Cancel;

            Assert.CatchAsync<OperationCanceledException>(async () =>
                await service.SaveAsync(service.Current.WithStars(4), cancellation.Token));
            Assert.That(store.Primary, Is.EqualTo(primaryBefore));
            Assert.That(store.Backup, Is.Null);
        }

        [Test]
        public void SerializationIsDeterministicAndContainsNoPersonalOrRuntimeConfigurationFields()
        {
            var serializer = new UnityJsonSaveSerializer();
            PlayerProgress progress = PlayerProgress.CreateDefault().WithStars(2);

            string first = serializer.Serialize(progress, "0.1.0-test", 4);
            string second = serializer.Serialize(progress, "0.1.0-test", 4);

            Assert.That(first, Is.EqualTo(second));
            Assert.That(first, Does.Not.Contain("name"));
            Assert.That(first, Does.Not.Contain("birth"));
            Assert.That(first, Does.Not.Contain("age"));
            Assert.That(first, Does.Not.Contain("device"));
            Assert.That(first, Does.Not.Contain("location"));
            Assert.That(first, Does.Not.Contain("buildProfile"));
            Assert.That(first, Does.Not.Contain("featureFlag"));
            Assert.That(first, Does.Not.Contain("randomSeed"));
            Assert.That(first, Does.Not.Contain("sceneTransitionTimeout"));
            Assert.That(first, Does.Not.Contain("autosaveDebounce"));
        }

        [Test]
        public async Task ResetCreatesFreshDefaultAndRemovesOldBackup()
        {
            var store = new InMemoryFileStore();
            LocalSaveService service = CreateService(store);
            await service.InitializeAsync(CancellationToken.None);
            await service.SaveAsync(service.Current.WithStars(8), CancellationToken.None);

            SaveOperationResult reset = await service.ResetAsync(CancellationToken.None);

            Assert.That(reset.IsSuccess, Is.True);
            Assert.That(service.Current.Stars, Is.Zero);
            Assert.That(store.Primary, Is.Not.Null);
            Assert.That(store.Backup, Is.Null);
        }

        [Test]
        public async Task LocalFileStoreUsesOnlyKnownPortableNamesAndSupportsRepeatedReplace()
        {
            string parent = Path.Combine(Path.GetTempPath(), "PequenoExploradorSaveTests");
            string directory = Path.Combine(parent, Guid.NewGuid().ToString("N"));
            try
            {
                using var store = new LocalFileStore(directory);
                LocalSaveService service = CreateService(store);
                await service.InitializeAsync(CancellationToken.None);
                await service.SaveAsync(service.Current.WithStars(1), CancellationToken.None);
                await service.SaveAsync(service.Current.WithStars(2), CancellationToken.None);

                Assert.That(File.Exists(Path.Combine(directory, LocalFileStore.PrimaryFileName)), Is.True);
                Assert.That(File.Exists(Path.Combine(directory, LocalFileStore.BackupFileName)), Is.True);
                Assert.That(File.Exists(Path.Combine(directory, LocalFileStore.TemporaryFileName)), Is.False);
            }
            finally
            {
                string fullParent = Path.GetFullPath(parent) + Path.DirectorySeparatorChar;
                string fullDirectory = Path.GetFullPath(directory);
                if (fullDirectory.StartsWith(fullParent, StringComparison.Ordinal) && Directory.Exists(fullDirectory))
                {
                    Directory.Delete(fullDirectory, true);
                }
            }
        }

        private static LocalSaveService CreateService(IFileStore store)
        {
            return new LocalSaveService(
                store,
                "0.1.0-test",
                new RecordingLogger(),
                new ISaveMigration[] { new LegacyV0ToV1Migration() });
        }

        private static void AssertProgress(PlayerProgress actual, PlayerProgress expected)
        {
            Assert.That(actual.Stars, Is.EqualTo(expected.Stars));
            Assert.That(actual.WorldIds, Is.EqualTo(expected.WorldIds));
            Assert.That(actual.DiscoveryIds, Is.EqualTo(expected.DiscoveryIds));
            Assert.That(actual.CompletedMissionIds, Is.EqualTo(expected.CompletedMissionIds));
            Assert.That(actual.Preferences.GuidanceMode, Is.EqualTo(expected.Preferences.GuidanceMode));
            Assert.That(actual.Preferences.MusicEnabled, Is.EqualTo(expected.Preferences.MusicEnabled));
            Assert.That(actual.Preferences.SoundEffectsEnabled, Is.EqualTo(expected.Preferences.SoundEffectsEnabled));
            Assert.That(actual.Preferences.NarrationEnabled, Is.EqualTo(expected.Preferences.NarrationEnabled));
        }
    }
}
