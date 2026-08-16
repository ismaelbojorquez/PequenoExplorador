using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Worlds
{
    public interface IWorldSession
    {
        WorldManifest ActiveWorld { get; }
        Task<WorldLoadResult> EnterAsync(WorldId worldId, CancellationToken cancellationToken);
        Task<WorldLoadResult> ReturnToCampAsync(CancellationToken cancellationToken);
        Task<WorldLoadResult> RetryAsync(CancellationToken cancellationToken);
    }
}
