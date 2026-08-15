using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Infrastructure.Lifecycle;

namespace PequenoExplorador.Infrastructure.Ads
{
    public sealed class NoAdsService : ImmediateApplicationService, IAdsService
    {
        public NoAdsService()
            : base("Ads")
        {
        }

        public ServiceAvailability Availability => ServiceAvailability.Disabled;

        public Task<ServiceOperationResult> TryShowAsync(
            string placementId,
            CancellationToken cancellationToken)
        {
            EnsureInitialized();
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(ServiceOperationResult.Disabled("AdsDisabled"));
        }
    }
}
