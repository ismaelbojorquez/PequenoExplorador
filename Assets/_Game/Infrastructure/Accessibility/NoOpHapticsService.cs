using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Accessibility;

namespace PequenoExplorador.Infrastructure.Accessibility
{
    public sealed class NoOpHapticsService : IHapticsService
    {
        public string ServiceId => "Haptics";
        public bool Enabled { get; private set; }

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Enabled = false;
            return Task.CompletedTask;
        }

        public void SetEnabled(bool enabled) => Enabled = enabled;
        public void Pulse(HapticFeedbackKind kind) { }
        public void Shutdown() => Enabled = false;
    }
}
