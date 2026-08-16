using System;
using PequenoExplorador.Application.Lifecycle;

namespace PequenoExplorador.Application.Input
{
    public interface IInputService : IApplicationService
    {
        event Action<InputIntent> IntentRaised;
        event Action BackRequested;
        InputMapId CurrentMap { get; }
        bool DebugMapEnabled { get; }
        void SetMap(InputMapId map);
        void CancelActiveGestures();
    }
}
