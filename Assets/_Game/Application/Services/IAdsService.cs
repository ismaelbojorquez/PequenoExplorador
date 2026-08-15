using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Lifecycle;

namespace PequenoExplorador.Application.Services
{
    public interface IAdsService : IApplicationService
    {
        ServiceAvailability Availability { get; }

        Task<ServiceOperationResult> TryShowAsync(string placementId, CancellationToken cancellationToken);
    }
}
