using System.Threading;
using System.Threading.Tasks;

namespace PequenoExplorador.Application.Save
{
    public interface IFileStore
    {
        Task<bool> ExistsAsync(SaveFileKind kind, CancellationToken cancellationToken);
        Task<string> ReadTextAsync(SaveFileKind kind, CancellationToken cancellationToken);
        Task WriteTemporaryAsync(string content, CancellationToken cancellationToken);
        Task FlushTemporaryAsync(CancellationToken cancellationToken);
        void CommitTemporary(SaveCommitMode mode);
        Task DiscardTemporaryAsync();
        Task DeleteAllAsync(CancellationToken cancellationToken);
    }
}
