using PequenoExplorador.Application.Explorer;

namespace PequenoExplorador.Application.Interaction
{
    public interface IInteractionApproach
    {
        WorldPosition Position { get; }
        ExplorerLocomotionState State { get; }
        bool TryMoveTo(WorldPosition destination);
        void CancelMovement();
    }
}
