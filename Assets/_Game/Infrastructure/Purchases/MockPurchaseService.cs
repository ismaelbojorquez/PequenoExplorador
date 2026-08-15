#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Infrastructure.Lifecycle;

namespace PequenoExplorador.Infrastructure.Purchases
{
    public sealed class MockPurchaseService : ImmediateApplicationService, IPurchaseService
    {
        public MockPurchaseService()
            : base("Purchases")
        {
        }

        public ServiceAvailability Availability => ServiceAvailability.Available;

        public int SimulatedPurchaseCount { get; private set; }

        public Task<ServiceOperationResult> TryPurchaseAsync(
            string productId,
            CancellationToken cancellationToken)
        {
            EnsureInitialized();
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(productId))
            {
                throw new ArgumentException("A technical product ID is required.", nameof(productId));
            }

            SimulatedPurchaseCount++;
            return Task.FromResult(ServiceOperationResult.Simulated("MockPurchaseCompleted"));
        }
    }
}
#endif
