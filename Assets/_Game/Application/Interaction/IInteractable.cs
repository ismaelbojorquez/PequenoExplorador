namespace PequenoExplorador.Application.Interaction
{
    public interface IInteractable
    {
        InteractionDefinition Definition { get; }
        bool IsAvailable { get; }
        bool IsAlive { get; }
        InteractionResult Interact(InteractionContext context);
    }
}
