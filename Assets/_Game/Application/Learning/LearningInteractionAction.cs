using System;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Missions;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Learning
{
    public sealed class LearningInteractionAction : IInteractionAction
    {
        private readonly IMissionFactSink _missionFacts;
        private readonly IContentCatalog _content;

        public LearningInteractionAction(IMissionFactSink missionFacts = null, IContentCatalog content = null)
        {
            _missionFacts = missionFacts;
            _content = content;
        }

        public event Action<ActivityId, DiscoveryId> Requested;

        public void Request(ActivityId activityId, DiscoveryId discoveryId)
        {
            if (activityId.IsValid) Requested?.Invoke(activityId, discoveryId);
        }

        public InteractionResult Execute(InteractionDefinition definition, InteractionContext context)
        {
            if (definition == null || !definition.HasLearningActivity)
                return new InteractionResult(InteractionOutcome.Unavailable, definition?.Id ?? default,
                    LocalizationKeys.InteractionUnavailable, default);

            Request(definition.LearningActivityId, definition.DirectDiscoveryId);
            if (_missionFacts != null)
            {
                TagId[] tags = _content != null && definition.HasDirectDiscovery &&
                               _content.TryResolveDiscovery(definition.DirectDiscoveryId, out DiscoveryDefinition discovery)
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
                LocalizationKeys.LearningActivityOpen, default);
        }
    }
}
