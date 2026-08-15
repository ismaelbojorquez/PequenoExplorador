using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Infrastructure.Lifecycle;

namespace PequenoExplorador.Infrastructure.Purchases
{
    public sealed class UnavailablePurchaseService : ImmediateApplicationService, IPurchaseService
    {
        public UnavailablePurchaseService()
            : base("Purchases")
        {
        }

        public ServiceAvailability Availability => ServiceAvailability.Unavailable;

        public Task<ServiceOperationResult> TryPurchaseAsync(
            string productId,
            CancellationToken cancellationToken)
        {
            EnsureInitialized();
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(ServiceOperationResult.Unavailable("PurchasesUnavailable"));
        }
    }
}
