using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Save
{
    public interface ISaveService : IApplicationService
    {
        PlayerProgress Current { get; }
        SaveLoadResult LastLoadResult { get; }
        bool IsReadOnly { get; }
        Task<SaveOperationResult> SaveAsync(PlayerProgress progress, CancellationToken cancellationToken);
        Task<SaveOperationResult> ResetAsync(CancellationToken cancellationToken);
    }
}
