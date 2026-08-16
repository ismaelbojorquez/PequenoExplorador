using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Accessibility;

namespace PequenoExplorador.Infrastructure.Accessibility
{
    public sealed class StaticSafeAreaService : ISafeAreaService
    {
        public string ServiceId => "SafeArea";
        public event Action<SafeAreaSnapshot> Changed;
        public SafeAreaSnapshot Current { get; } =
            new SafeAreaSnapshot(1, 1, 0f, 0f, 0f, 0f, DisplayOrientation.Unknown);
        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }
        public void Shutdown() => Changed = null;
    }
}
