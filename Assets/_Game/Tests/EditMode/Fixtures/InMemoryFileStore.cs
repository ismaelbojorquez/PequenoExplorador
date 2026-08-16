using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Save;

namespace PequenoExplorador.Tests.EditMode.Fixtures
{
    public enum FileStoreFailurePoint
    {
        None = 0,
        WriteTemporary = 1,
        FlushTemporary = 2,
        CommitTemporary = 3
    }

    internal sealed class InMemoryFileStore : IFileStore
    {
        private string _temporary;

        public string Primary { get; private set; }
        public string Backup { get; private set; }
        public FileStoreFailurePoint FailurePoint { get; set; }
        public Action BeforeFlush { get; set; }

        public void SeedPrimary(string content)
        {
            Primary = content;
        }

        public void SeedBackup(string content)
        {
            Backup = content;
        }

        public Task<bool> ExistsAsync(SaveFileKind kind, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Get(kind) != null);
        }

        public Task<string> ReadTextAsync(SaveFileKind kind, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string value = Get(kind);
            if (value == null)
            {
                throw new FileNotFoundException();
            }

            return Task.FromResult(value);
        }

        public Task WriteTemporaryAsync(string content, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIf(FileStoreFailurePoint.WriteTemporary);
            _temporary = content;
            return Task.CompletedTask;
        }

        public Task FlushTemporaryAsync(CancellationToken cancellationToken)
        {
            BeforeFlush?.Invoke();
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIf(FileStoreFailurePoint.FlushTemporary);
            if (_temporary == null)
            {
                throw new InvalidOperationException("Temporary content is missing.");
            }

            return Task.CompletedTask;
        }

        public void CommitTemporary(SaveCommitMode mode)
        {
            ThrowIf(FileStoreFailurePoint.CommitTemporary);
            if (_temporary == null)
            {
                throw new InvalidOperationException("Temporary content is missing.");
            }

            if (mode == SaveCommitMode.RotatePrimaryToBackup && Primary != null)
            {
                Backup = Primary;
            }

            Primary = _temporary;
            _temporary = null;
        }

        public Task DiscardTemporaryAsync()
        {
            _temporary = null;
            return Task.CompletedTask;
        }

        public Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Primary = null;
            Backup = null;
            _temporary = null;
            return Task.CompletedTask;
        }

        private string Get(SaveFileKind kind)
        {
            return kind == SaveFileKind.Primary ? Primary : Backup;
        }

        private void ThrowIf(FileStoreFailurePoint expected)
        {
            if (FailurePoint == expected)
            {
                throw new IOException("Injected" + expected);
            }
        }
    }
}
