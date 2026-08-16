namespace PequenoExplorador.Application.Interaction
{
    public interface IInteractionAction
    {
        InteractionResult Execute(InteractionDefinition definition, InteractionContext context);
    }
}
