using System;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Missions;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Photography
{
    public sealed class PhotographyInteractionAction : IInteractionAction
    {
        private readonly IMissionFactSink _missionFacts;
        private readonly IContentCatalog _content;
        public PhotographyInteractionAction(IMissionFactSink missionFacts = null, IContentCatalog content = null)
        {
            _missionFacts = missionFacts;
            _content = content;
        }
        public event Action<DiscoveryId> Requested;
        public void Request(DiscoveryId discoveryId)
        {
            if (discoveryId.IsValid) Requested?.Invoke(discoveryId);
        }
        public InteractionResult Execute(InteractionDefinition definition, InteractionContext context)
        {
            if (definition == null || !definition.HasDirectDiscovery)
                return new InteractionResult(InteractionOutcome.Unavailable, definition?.Id ?? default,
                    LocalizationKeys.InteractionUnavailable, default);
            Request(definition.DirectDiscoveryId);
            if (_missionFacts != null)
            {
                TagId[] tags = _content != null && _content.TryResolveDiscovery(definition.DirectDiscoveryId, out DiscoveryDefinition discovery)
                    ? new System.Collections.Generic.List<TagId>(discovery.TagIds).ToArray()
                    : Array.Empty<TagId>();
                _missionFacts.Record(new GameplayFact(
                    GameplayFactId.Parse("gameplay-fact.interaction." + context.Timestamp.UtcTicks.ToString(System.Globalization.CultureInfo.InvariantCulture) + "." + definition.Id.Value),
                    GameplayFactTypes.Interaction,
                    definition.Id.Value,
                    tags,
                    GameplayFactScope.World));
            }
            return new InteractionResult(InteractionOutcome.Completed, definition.Id,
                LocalizationKeys.PhotographyOpen, default);
        }
    }
}
