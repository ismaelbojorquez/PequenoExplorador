using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Lifecycle;

namespace PequenoExplorador.Application.Services
{
    public interface IPurchaseService : IApplicationService
    {
        ServiceAvailability Availability { get; }

        Task<ServiceOperationResult> TryPurchaseAsync(string productId, CancellationToken cancellationToken);
    }
}
