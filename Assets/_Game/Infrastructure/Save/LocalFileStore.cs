using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Save;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class LocalFileStore : IFileStore, IDisposable
    {
        public const string PrimaryFileName = SaveFileNames.Primary;
        public const string BackupFileName = SaveFileNames.Backup;
        public const string TemporaryFileName = SaveFileNames.Temporary;

        private readonly string _directory;
        private readonly string _primaryPath;
        private readonly string _backupPath;
        private readonly string _temporaryPath;
        private FileStream _temporaryStream;

        public LocalFileStore(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentException("Save directory is required.", nameof(directory));
            }

            _directory = Path.GetFullPath(directory);
            _primaryPath = Path.Combine(_directory, PrimaryFileName);
            _backupPath = Path.Combine(_directory, BackupFileName);
            _temporaryPath = Path.Combine(_directory, TemporaryFileName);
        }

        public string DirectoryPath => _directory;

        public Task<bool> ExistsAsync(SaveFileKind kind, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(File.Exists(GetPath(kind)));
        }

        public async Task<string> ReadTextAsync(SaveFileKind kind, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var stream = new FileStream(
                GetPath(kind),
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,
                true);
            using var reader = new StreamReader(stream, Encoding.UTF8, true, 4096, false);
            string content = await reader.ReadToEndAsync();
            cancellationToken.ThrowIfCancellationRequested();
            return content;
        }

        public async Task WriteTemporaryAsync(string content, CancellationToken cancellationToken)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            cancellationToken.ThrowIfCancellationRequested();
            Directory.CreateDirectory(_directory);
            CloseTemporaryStream();
            _temporaryStream = new FileStream(
                _temporaryPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                4096,
                FileOptions.Asynchronous | FileOptions.WriteThrough);
            byte[] bytes = new UTF8Encoding(false).GetBytes(content);
            await _temporaryStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }

        public async Task FlushTemporaryAsync(CancellationToken cancellationToken)
        {
            if (_temporaryStream == null)
            {
                throw new InvalidOperationException("No temporary save is open.");
            }

            await _temporaryStream.FlushAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            _temporaryStream.Flush(true);
            CloseTemporaryStream();
        }

        public void CommitTemporary(SaveCommitMode mode)
        {
            if (_temporaryStream != null)
            {
                throw new InvalidOperationException("Temporary save must be flushed before commit.");
            }

            if (!File.Exists(_temporaryPath))
            {
                throw new FileNotFoundException("Temporary save is missing.", _temporaryPath);
            }

            if (!File.Exists(_primaryPath))
            {
                File.Move(_temporaryPath, _primaryPath);
                return;
            }

            string backup = mode == SaveCommitMode.RotatePrimaryToBackup ? _backupPath : null;
            File.Replace(_temporaryPath, _primaryPath, backup, true);
        }

        public Task DiscardTemporaryAsync()
        {
            CloseTemporaryStream();
            if (File.Exists(_temporaryPath))
            {
                File.Delete(_temporaryPath);
            }

            return Task.CompletedTask;
        }

        public Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CloseTemporaryStream();
            DeleteIfPresent(_temporaryPath);
            DeleteIfPresent(_backupPath);
            DeleteIfPresent(_primaryPath);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            CloseTemporaryStream();
        }

        private string GetPath(SaveFileKind kind)
        {
            switch (kind)
            {
                case SaveFileKind.Primary:
                    return _primaryPath;
                case SaveFileKind.Backup:
                    return _backupPath;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }

        private void CloseTemporaryStream()
        {
            _temporaryStream?.Dispose();
            _temporaryStream = null;
        }

        private static void DeleteIfPresent(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
