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
        public event Action<InputMapId> MapChanged;
        public InputMapId CurrentMap { get; private set; } = InputMapId.UI;
        public bool DebugMapEnabled => false;
        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }
        public void SetMap(InputMapId map)
        {
            if (CurrentMap == map) return;
            CurrentMap = map;
            MapChanged?.Invoke(map);
        }
        public void CancelActiveGestures() { }
        public void Shutdown() { }
    }
}
