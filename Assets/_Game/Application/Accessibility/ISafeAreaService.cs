using System;
using PequenoExplorador.Application.Lifecycle;

namespace PequenoExplorador.Application.Accessibility
{
    public interface ISafeAreaService : IApplicationService
    {
        event Action<SafeAreaSnapshot> Changed;
        SafeAreaSnapshot Current { get; }
    }
}
