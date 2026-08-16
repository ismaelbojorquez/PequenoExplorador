using PequenoExplorador.Application.Input;

namespace PequenoExplorador.Application.Interaction
{
    public interface IExplorerTapHandler
    {
        bool TryHandleTap(ScreenPoint screenPoint);
    }
}
