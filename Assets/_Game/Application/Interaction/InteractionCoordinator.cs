using System;
using System.Collections.Generic;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Explorer;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Interaction
{
    public sealed class InteractionCoordinator
    {
        private readonly IInteractionApproach _approach;
        private readonly IClock _clock;
        private readonly Dictionary<InteractionId, DateTimeOffset> _cooldowns =
            new Dictionary<InteractionId, DateTimeOffset>();
        private IInteractable _active;
        private WorldPosition _interactionPoint;
        private bool _suspended;

        public InteractionCoordinator(IInteractionApproach approach, IClock clock)
        {
            _approach = approach ?? throw new ArgumentNullException(nameof(approach));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            Snapshot = InteractionSnapshot.Idle;
        }

        public event Action<InteractionSnapshot> Changed;
        public InteractionSnapshot Snapshot { get; private set; }

        public InteractionResult Focus(IInteractable target, WorldPosition interactionPoint)
        {
            if (_suspended) return PublishSystemOutcome(InteractionOutcome.Suspended);
            if (target == null || !target.IsAlive || target.Definition == null)
                return PublishSystemOutcome(InteractionOutcome.Missing);

            if (ReferenceEquals(_active, target) &&
                (Snapshot.State == InteractionOutcome.Approaching ||
                 Snapshot.State == InteractionOutcome.Ready ||
                 Snapshot.State == InteractionOutcome.Unavailable))
                return Snapshot.Result;

            _approach.CancelMovement();
            _active = target;
            _interactionPoint = interactionPoint;
            if (!target.IsAvailable)
                return PublishUnavailable(target.Definition);

            if (IsInRange(target.Definition.InteractionRange))
                return PublishReady(target.Definition);

            if (!_approach.TryMoveTo(interactionPoint))
                return PublishUnavailable(target.Definition);

            return Publish(
                InteractionOutcome.Approaching,
                target.Definition,
                new InteractionResult(
                    InteractionOutcome.Approaching,
                    target.Definition.Id,
                    target.Definition.Prompt,
                    default));
        }

        public void Tick()
        {
            if (_suspended || _active == null) return;
            if (!_active.IsAlive)
            {
                CancelInternal(InteractionOutcome.Missing);
                return;
            }
            if (!_active.IsAvailable)
            {
                _approach.CancelMovement();
                PublishUnavailable(_active.Definition);
                return;
            }
            if (Snapshot.State != InteractionOutcome.Approaching) return;
            if (IsInRange(_active.Definition.InteractionRange))
            {
                _approach.CancelMovement();
                PublishReady(_active.Definition);
                return;
            }
            if (_approach.State == ExplorerLocomotionState.InvalidDestination ||
                _approach.State == ExplorerLocomotionState.Arrived ||
                _approach.State == ExplorerLocomotionState.Idle)
            {
                _approach.CancelMovement();
                PublishUnavailable(_active.Definition);
            }
        }

        public InteractionResult Activate()
        {
            if (_suspended) return PublishSystemOutcome(InteractionOutcome.Suspended);
            if (_active == null || !_active.IsAlive)
                return PublishSystemOutcome(InteractionOutcome.Missing);
            if (Snapshot.State != InteractionOutcome.Ready) return Snapshot.Result;

            InteractionDefinition definition = _active.Definition;
            if (_cooldowns.TryGetValue(definition.Id, out DateTimeOffset until) && _clock.UtcNow < until)
            {
                return Publish(
                    InteractionOutcome.CoolingDown,
                    definition,
                    new InteractionResult(
                        InteractionOutcome.CoolingDown,
                        definition.Id,
                        definition.Prompt,
                        default));
            }

            var context = new InteractionContext(_approach.Position, _interactionPoint, _clock.UtcNow);
            InteractionResult result = _active.Interact(context);
            if (result.IsSuccess)
            {
                _cooldowns[definition.Id] = _clock.UtcNow.AddSeconds(definition.CooldownSeconds);
                return Publish(
                    InteractionOutcome.Completed,
                    definition,
                    new InteractionResult(
                        InteractionOutcome.Completed,
                        definition.Id,
                        string.IsNullOrWhiteSpace(result.Feedback.Entry)
                            ? definition.Prompt
                            : result.Feedback,
                        string.IsNullOrWhiteSpace(result.AudioCue.Value)
                            ? AudioCueIds.ConfirmFeedback
                            : result.AudioCue));
            }

            return PublishUnavailable(definition);
        }

        public void Cancel() => CancelInternal(InteractionOutcome.Cancelled);

        public void SetSuspended(bool suspended)
        {
            if (_suspended == suspended) return;
            _suspended = suspended;
            _approach.CancelMovement();
            _active = null;
            PublishSystemOutcome(suspended ? InteractionOutcome.Suspended : InteractionOutcome.None);
        }

        private void CancelInternal(InteractionOutcome outcome)
        {
            _approach.CancelMovement();
            _active = null;
            PublishSystemOutcome(outcome);
        }

        private InteractionResult PublishSystemOutcome(InteractionOutcome outcome) => Publish(
            outcome,
            null,
            new InteractionResult(outcome, default, default, default));

        private bool IsInRange(float range)
        {
            WorldPosition position = _approach.Position;
            float x = position.X - _interactionPoint.X;
            float y = position.Y - _interactionPoint.Y;
            float z = position.Z - _interactionPoint.Z;
            return x * x + y * y + z * z <= range * range;
        }

        private InteractionResult PublishReady(InteractionDefinition definition) => Publish(
            InteractionOutcome.Ready,
            definition,
            new InteractionResult(
                InteractionOutcome.Ready,
                definition.Id,
                definition.Prompt,
                definition.PromptAudioCue));

        private InteractionResult PublishUnavailable(InteractionDefinition definition) => Publish(
            InteractionOutcome.Unavailable,
            definition,
            new InteractionResult(
                InteractionOutcome.Unavailable,
                definition.Id,
                definition.Unavailable,
                definition.UnavailableAudioCue));

        private InteractionResult Publish(
            InteractionOutcome state,
            InteractionDefinition definition,
            InteractionResult result)
        {
            if (Snapshot.State == state && ReferenceEquals(Snapshot.Definition, definition) &&
                Snapshot.Result.Outcome == result.Outcome)
                return Snapshot.Result;
            Snapshot = new InteractionSnapshot(state, definition, result);
            Changed?.Invoke(Snapshot);
            return result;
        }
    }
}
