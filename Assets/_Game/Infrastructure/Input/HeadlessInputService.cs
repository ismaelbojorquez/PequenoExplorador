using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Input;

namespace PequenoExplorador.Infrastructure.Input
{
    public sealed class HeadlessInputService : IInputService
    {
        public string ServiceId => "Input";
        public event Action<InputIntent> IntentRaised;
        public event Action BackRequested;
        public InputMapId CurrentMap { get; private set; } = InputMapId.UI;
        public bool DebugMapEnabled => false;
        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }
        public void SetMap(InputMapId map) => CurrentMap = map;
        public void CancelActiveGestures() { }
        public void Shutdown() { }
    }
}
