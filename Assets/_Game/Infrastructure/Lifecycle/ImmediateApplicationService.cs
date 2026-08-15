using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Lifecycle;

namespace PequenoExplorador.Infrastructure.Lifecycle
{
    public abstract class ImmediateApplicationService : IApplicationService
    {
        protected ImmediateApplicationService(string serviceId)
        {
            ServiceId = string.IsNullOrWhiteSpace(serviceId)
                ? throw new ArgumentException("Service ID is required.", nameof(serviceId))
                : serviceId;
        }

        public string ServiceId { get; }

        public bool IsInitialized { get; private set; }

        public virtual Task InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IsInitialized = true;
            return Task.CompletedTask;
        }

        public virtual void Shutdown()
        {
            IsInitialized = false;
        }

        protected void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException($"Service '{ServiceId}' is not initialized.");
            }
        }
    }
}
