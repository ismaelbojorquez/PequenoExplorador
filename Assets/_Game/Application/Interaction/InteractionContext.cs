using System;
using PequenoExplorador.Application.Explorer;

namespace PequenoExplorador.Application.Interaction
{
    public readonly struct InteractionContext
    {
        public InteractionContext(
            WorldPosition actorPosition,
            WorldPosition interactionPoint,
            DateTimeOffset timestamp)
        {
            ActorPosition = actorPosition;
            InteractionPoint = interactionPoint;
            Timestamp = timestamp;
        }

        public WorldPosition ActorPosition { get; }
        public WorldPosition InteractionPoint { get; }
        public DateTimeOffset Timestamp { get; }
    }
}
