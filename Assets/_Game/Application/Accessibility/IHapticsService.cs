using PequenoExplorador.Application.Lifecycle;

namespace PequenoExplorador.Application.Accessibility
{
    public interface IHapticsService : IApplicationService
    {
        bool Enabled { get; }
        void SetEnabled(bool enabled);
        void Pulse(HapticFeedbackKind kind);
    }
}
