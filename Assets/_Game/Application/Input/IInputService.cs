using System;
using PequenoExplorador.Application.Lifecycle;

namespace PequenoExplorador.Application.Input
{
    public interface IInputService : IApplicationService
    {
        event Action<InputIntent> IntentRaised;
        event Action BackRequested;
        event Action<InputMapId> MapChanged;
        InputMapId CurrentMap { get; }
        bool DebugMapEnabled { get; }
        void SetMap(InputMapId map);
        void CancelActiveGestures();
    }
}
