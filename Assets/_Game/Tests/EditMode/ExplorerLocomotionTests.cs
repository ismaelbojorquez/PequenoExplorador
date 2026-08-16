using System;
using NUnit.Framework;
using PequenoExplorador.Application.Explorer;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class ExplorerLocomotionTests
    {
        [Test]
        public void MoveCommandTransitionsPendingMovingAndArrived()
        {
            var navigator = new FakeNavigator();
            var controller = CreateController(navigator);
            Assert.That(controller.MoveTo(new WorldPosition(2f, 0f, 3f)), Is.True);
            Assert.That(controller.State, Is.EqualTo(ExplorerLocomotionState.PathPending));
            Assert.That(navigator.LastDestination, Is.EqualTo(new WorldPosition(2f, 0f, 3f)));

            navigator.Pending = false;
            navigator.HasActivePath = true;
            navigator.Remaining = 2f;
            navigator.CurrentSpeed = 1f;
            controller.Tick();
            Assert.That(controller.State, Is.EqualTo(ExplorerLocomotionState.Moving));

            navigator.Remaining = 0.1f;
            navigator.CurrentSpeed = 0.01f;
            controller.Tick();
            Assert.That(controller.State, Is.EqualTo(ExplorerLocomotionState.Arrived));
            Assert.That(navigator.StopCount, Is.EqualTo(1));
        }

        [Test]
        public void InvalidDestinationIsNonPunitiveAndNextTapCanRecover()
        {
            var navigator = new FakeNavigator { AcceptDestination = false };
            var controller = CreateController(navigator);
            controller.RejectDestination();
            Assert.That(controller.State, Is.EqualTo(ExplorerLocomotionState.InvalidDestination));
            Assert.That(controller.MoveTo(default), Is.False);
            Assert.That(controller.State, Is.EqualTo(ExplorerLocomotionState.InvalidDestination));

            navigator.AcceptDestination = true;
            Assert.That(controller.MoveTo(new WorldPosition(1f, 0f, 1f)), Is.True);
            Assert.That(controller.State, Is.EqualTo(ExplorerLocomotionState.PathPending));
        }

        [Test]
        public void SuspensionAndCancelStopCurrentPathAndBlockCommands()
        {
            var navigator = new FakeNavigator();
            var controller = CreateController(navigator);
            controller.MoveTo(new WorldPosition(1f, 0f, 1f));
            controller.SetSuspended(true);
            Assert.That(controller.State, Is.EqualTo(ExplorerLocomotionState.Suspended));
            Assert.That(controller.MoveTo(new WorldPosition(2f, 0f, 2f)), Is.False);
            Assert.That(navigator.SetCount, Is.EqualTo(1));

            controller.SetSuspended(false);
            Assert.That(controller.State, Is.EqualTo(ExplorerLocomotionState.Idle));
            controller.MoveTo(new WorldPosition(3f, 0f, 3f));
            controller.Cancel();
            Assert.That(controller.State, Is.EqualTo(ExplorerLocomotionState.Idle));
            Assert.That(navigator.StopCount, Is.EqualTo(3));
        }

        [Test]
        public void RepeatedTapsReplaceDestinationWithoutGrowingState()
        {
            var navigator = new FakeNavigator();
            var controller = CreateController(navigator);
            for (int index = 0; index < 100; index++)
                Assert.That(controller.MoveTo(new WorldPosition(index, 0f, -index)), Is.True);
            Assert.That(navigator.SetCount, Is.EqualTo(100));
            Assert.That(navigator.LastDestination, Is.EqualTo(new WorldPosition(99f, 0f, -99f)));
            Assert.That(controller.State, Is.EqualTo(ExplorerLocomotionState.PathPending));
        }

        [Test]
        public void CommandHotPathIsAllocationStableAfterWarmup()
        {
            var navigator = new FakeNavigator();
            var controller = CreateController(navigator);
            controller.MoveTo(default);
            long before = GC.GetAllocatedBytesForCurrentThread();
            for (int index = 0; index < 1000; index++)
                controller.MoveTo(new WorldPosition(index, 0f, index));
            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
            Assert.That(allocated, Is.LessThanOrEqualTo(256));
        }

        private static ExplorerLocomotionController CreateController(FakeNavigator navigator) =>
            new ExplorerLocomotionController(navigator, new ExplorerLocomotionSettings(0.18f, 0.08f));

        private sealed class FakeNavigator : IPathNavigator
        {
            public bool IsAvailable { get; set; } = true;
            public bool Pending { get; set; } = true;
            public bool HasActivePath { get; set; } = true;
            public float Remaining { get; set; } = 10f;
            public float CurrentSpeed { get; set; }
            public bool AcceptDestination { get; set; } = true;
            public int SetCount { get; private set; }
            public int StopCount { get; private set; }
            public WorldPosition LastDestination { get; private set; }
            public bool IsPathPending => Pending;
            public bool HasPath => HasActivePath;
            public float RemainingDistance => Remaining;
            public float Speed => CurrentSpeed;
            public bool TrySetDestination(WorldPosition destination)
            {
                SetCount++;
                LastDestination = destination;
                return AcceptDestination;
            }
            public void Stop() => StopCount++;
        }
    }
}
