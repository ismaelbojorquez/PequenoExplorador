using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Interaction
{
    public readonly struct InteractionResult
    {
        public InteractionResult(
            InteractionOutcome outcome,
            InteractionId targetId,
            LocalizedKey feedback,
            AudioCueId audioCue)
        {
            Outcome = outcome;
            TargetId = targetId;
            Feedback = feedback;
            AudioCue = audioCue;
        }

        public InteractionOutcome Outcome { get; }
        public InteractionId TargetId { get; }
        public LocalizedKey Feedback { get; }
        public AudioCueId AudioCue { get; }
        public bool IsSuccess => Outcome == InteractionOutcome.Completed;
    }
}
