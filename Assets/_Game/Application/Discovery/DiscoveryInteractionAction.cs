using System;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Discovery
{
    public sealed class DiscoveryInteractionAction : IInteractionAction
    {
        private readonly DiscoverUseCase _discover;

        public DiscoveryInteractionAction(DiscoverUseCase discover)
        {
            _discover = discover ?? throw new ArgumentNullException(nameof(discover));
        }

        public event Action<DiscoverResult> Completed;
        public DiscoverResult LastResult { get; private set; }

        public InteractionResult Execute(InteractionDefinition definition, InteractionContext context)
        {
            if (definition == null || !definition.HasDirectDiscovery)
                return new InteractionResult(
                    InteractionOutcome.Unavailable,
                    definition?.Id ?? default,
                    LocalizationKeys.InteractionUnavailable,
                    AudioCueIds.RetryFeedback);

            string grantValue = string.Concat(
                "grant.interaction.",
                context.Timestamp.UtcTicks.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ".",
                definition.DirectDiscoveryId.Value);
            LastResult = _discover.Execute(
                definition.DirectDiscoveryId,
                DiscoveryGrantId.Parse(grantValue));
            Completed?.Invoke(LastResult);

            LocalizedKey feedback = LastResult.Outcome == DiscoverOutcome.First
                ? LocalizationKeys.DiscoveryNew
                : LastResult.Outcome == DiscoverOutcome.Repeated ||
                  LastResult.Outcome == DiscoverOutcome.AlreadyProcessed
                    ? LocalizationKeys.DiscoveryRepeated
                    : LocalizationKeys.InteractionUnavailable;
            return new InteractionResult(
                LastResult.ChangedProgress || LastResult.Outcome == DiscoverOutcome.AlreadyProcessed
                    ? InteractionOutcome.Completed
                    : InteractionOutcome.Unavailable,
                definition.Id,
                feedback,
                LastResult.ChangedProgress ? AudioCueIds.ConfirmFeedback : AudioCueIds.RetryFeedback);
        }
    }
}
