#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Infrastructure.Lifecycle;

namespace PequenoExplorador.Infrastructure.Ads
{
    public sealed class MockAdsService : ImmediateApplicationService, IAdsService
    {
        public MockAdsService()
            : base("Ads")
        {
        }

        public ServiceAvailability Availability => ServiceAvailability.Available;

        public int SimulatedShowCount { get; private set; }

        public Task<ServiceOperationResult> TryShowAsync(
            string placementId,
            CancellationToken cancellationToken)
        {
            EnsureInitialized();
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(placementId))
            {
                throw new ArgumentException("A technical placement ID is required.", nameof(placementId));
            }

            SimulatedShowCount++;
            return Task.FromResult(ServiceOperationResult.Simulated("MockAdShown"));
        }
    }
}
#endif
