using PequenoExplorador.Application.Services;
using PequenoExplorador.Infrastructure.Lifecycle;

namespace PequenoExplorador.Infrastructure.Analytics
{
    public sealed class NullAnalyticsService : ImmediateApplicationService, IAnalyticsService
    {
        public NullAnalyticsService()
            : base("Analytics")
        {
        }

        public void Record(string eventId)
        {
            EnsureInitialized();
        }
    }
}
