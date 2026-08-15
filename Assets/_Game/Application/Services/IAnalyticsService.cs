using PequenoExplorador.Application.Lifecycle;

namespace PequenoExplorador.Application.Services
{
    public interface IAnalyticsService : IApplicationService
    {
        void Record(string eventId);
    }
}
