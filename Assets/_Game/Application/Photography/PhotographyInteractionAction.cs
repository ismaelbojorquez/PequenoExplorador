using System;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Photography
{
    public sealed class PhotographyInteractionAction : IInteractionAction
    {
        public event Action<DiscoveryId> Requested;
        public InteractionResult Execute(InteractionDefinition definition, InteractionContext context)
        {
            if (definition == null || !definition.HasDirectDiscovery)
                return new InteractionResult(InteractionOutcome.Unavailable, definition?.Id ?? default,
                    LocalizationKeys.InteractionUnavailable, default);
            Requested?.Invoke(definition.DirectDiscoveryId);
            return new InteractionResult(InteractionOutcome.Completed, definition.Id,
                LocalizationKeys.PhotographyOpen, default);
        }
    }
}
