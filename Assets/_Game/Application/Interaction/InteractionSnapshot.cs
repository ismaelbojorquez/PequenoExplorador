namespace PequenoExplorador.Application.Interaction
{
    public sealed class InteractionSnapshot
    {
        public InteractionSnapshot(
            InteractionOutcome state,
            InteractionDefinition definition,
            InteractionResult result)
        {
            State = state;
            Definition = definition;
            Result = result;
        }

        public InteractionOutcome State { get; }
        public InteractionDefinition Definition { get; }
        public InteractionResult Result { get; }
        public bool HasFocus => Definition != null;
        public bool CanActivate => State == InteractionOutcome.Ready;

        public static InteractionSnapshot Idle { get; } =
            new InteractionSnapshot(InteractionOutcome.None, null, default);
    }
}
