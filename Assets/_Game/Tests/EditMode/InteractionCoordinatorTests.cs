using System;
using NUnit.Framework;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Explorer;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Tests.EditMode.Fixtures;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class InteractionCoordinatorTests
    {
        private static readonly DateTimeOffset Started =
            new DateTimeOffset(2026, 8, 16, 12, 0, 0, TimeSpan.Zero);

        [Test]
        public void SelectorUsesPriorityThenDistanceThenStableId()
        {
            var lowNear = new FakeInteractable(CreateDefinition("interaction.fixture.near", 10));
            var highFar = new FakeInteractable(CreateDefinition("interaction.fixture.far", 20));
            Assert.That(InteractionTargetSelector.TrySelect(
                new[]
                {
                    new InteractionCandidate(lowNear, 1f),
                    new InteractionCandidate(highFar, 5f)
                }, out IInteractable selected), Is.True);
            Assert.That(selected, Is.SameAs(highFar));

            var alpha = new FakeInteractable(CreateDefinition("interaction.fixture.alpha", 20));
            var beta = new FakeInteractable(CreateDefinition("interaction.fixture.beta", 20));
            InteractionTargetSelector.TrySelect(
                new[]
                {
                    new InteractionCandidate(beta, 2f),
                    new InteractionCandidate(alpha, 2f)
                }, out selected);
            Assert.That(selected, Is.SameAs(alpha));
        }

        [Test]
        public void FocusOutsideRangeApproachesAndBecomesReadyAtPoint()
        {
            var approach = new FakeApproach { Position = new WorldPosition(0f, 0f, 0f) };
            var coordinator = CreateCoordinator(approach, out _);
            var target = new FakeInteractable(CreateDefinition("interaction.fixture.plant", 50));
            WorldPosition point = new WorldPosition(4f, 0f, 0f);

            InteractionResult result = coordinator.Focus(target, point);
            Assert.That(result.Outcome, Is.EqualTo(InteractionOutcome.Approaching));
            Assert.That(approach.MoveCount, Is.EqualTo(1));
            approach.Position = new WorldPosition(3f, 0f, 0f);
            approach.State = ExplorerLocomotionState.Moving;
            coordinator.Tick();
            Assert.That(coordinator.Snapshot.State, Is.EqualTo(InteractionOutcome.Ready));
            Assert.That(approach.CancelCount, Is.EqualTo(2));
        }

        [Test]
        public void UnavailableTargetExplainsWithoutApproach()
        {
            var approach = new FakeApproach();
            var coordinator = CreateCoordinator(approach, out _);
            var target = new FakeInteractable(CreateDefinition("interaction.fixture.object", 50))
            {
                Available = false
            };
            InteractionResult result = coordinator.Focus(target, new WorldPosition(8f, 0f, 0f));
            Assert.That(result.Outcome, Is.EqualTo(InteractionOutcome.Unavailable));
            Assert.That(result.Feedback, Is.EqualTo(LocalizationKeys.InteractionUnavailable));
            Assert.That(approach.MoveCount, Is.Zero);
        }

        [Test]
        public void ActivateIsIdempotentAndCooldownUsesInjectedClock()
        {
            var approach = new FakeApproach();
            var coordinator = CreateCoordinator(approach, out ManualClock clock);
            var target = new FakeInteractable(CreateDefinition("interaction.fixture.animal", 50));
            coordinator.Focus(target, default);
            Assert.That(coordinator.Activate().Outcome, Is.EqualTo(InteractionOutcome.Completed));
            Assert.That(coordinator.Activate().Outcome, Is.EqualTo(InteractionOutcome.Completed));
            Assert.That(target.ActivationCount, Is.EqualTo(1));

            coordinator.Focus(target, default);
            Assert.That(coordinator.Activate().Outcome, Is.EqualTo(InteractionOutcome.CoolingDown));
            Assert.That(target.ActivationCount, Is.EqualTo(1));
            clock.UtcNow = clock.UtcNow.AddSeconds(2);
            coordinator.Focus(target, default);
            Assert.That(coordinator.Activate().Outcome, Is.EqualTo(InteractionOutcome.Completed));
            Assert.That(target.ActivationCount, Is.EqualTo(2));
        }

        [Test]
        public void CancelSuspendAndMissingTargetClearSingleFocus()
        {
            var approach = new FakeApproach();
            var coordinator = CreateCoordinator(approach, out _);
            var target = new FakeInteractable(CreateDefinition("interaction.fixture.animal", 50));
            coordinator.Focus(target, new WorldPosition(4f, 0f, 0f));
            coordinator.Cancel();
            Assert.That(coordinator.Snapshot.State, Is.EqualTo(InteractionOutcome.Cancelled));
            Assert.That(coordinator.Snapshot.HasFocus, Is.False);

            coordinator.Focus(target, new WorldPosition(4f, 0f, 0f));
            coordinator.SetSuspended(true);
            Assert.That(coordinator.Snapshot.State, Is.EqualTo(InteractionOutcome.Suspended));
            Assert.That(coordinator.Snapshot.HasFocus, Is.False);
            coordinator.SetSuspended(false);
            target.Alive = false;
            Assert.That(coordinator.Focus(target, default).Outcome, Is.EqualTo(InteractionOutcome.Missing));
            Assert.That(approach.CancelCount, Is.GreaterThanOrEqualTo(3));
        }

        private static InteractionCoordinator CreateCoordinator(
            FakeApproach approach,
            out ManualClock clock)
        {
            clock = new ManualClock(Started);
            return new InteractionCoordinator(approach, clock);
        }

        private static InteractionDefinition CreateDefinition(string id, int priority) =>
            new InteractionDefinition(
                InteractionId.Parse(id),
                LocalizationKeys.InteractionAnimalPlaceholderName,
                LocalizationKeys.InteractionAction,
                LocalizationKeys.InteractionUnavailable,
                AudioCueIds.ExploreInstruction,
                AudioCueIds.RetryFeedback,
                1.35f,
                1.5f,
                priority,
                default,
                new EditorialMetadata(EditorialState.Draft, true, "Tests", "PH_"));

        private sealed class FakeApproach : IInteractionApproach
        {
            public WorldPosition Position { get; set; }
            public ExplorerLocomotionState State { get; set; } = ExplorerLocomotionState.Idle;
            public int MoveCount { get; private set; }
            public int CancelCount { get; private set; }
            public bool AcceptMove { get; set; } = true;
            public bool TryMoveTo(WorldPosition destination)
            {
                MoveCount++;
                State = ExplorerLocomotionState.PathPending;
                return AcceptMove;
            }
            public void CancelMovement()
            {
                CancelCount++;
                State = ExplorerLocomotionState.Idle;
            }
        }

        private sealed class FakeInteractable : IInteractable
        {
            public FakeInteractable(InteractionDefinition definition) => Definition = definition;
            public InteractionDefinition Definition { get; }
            public bool Available { get; set; } = true;
            public bool Alive { get; set; } = true;
            public int ActivationCount { get; private set; }
            public bool IsAvailable => Available;
            public bool IsAlive => Alive;
            public InteractionResult Interact(InteractionContext context)
            {
                ActivationCount++;
                return new InteractionResult(
                    InteractionOutcome.Completed,
                    Definition.Id,
                    Definition.Prompt,
                    default);
            }
        }
    }
}
