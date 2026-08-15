using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Tests.EditMode.Fixtures;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class SceneFlowServiceTests
    {
        [Test]
        public async Task BootCampExpeditionCampOwnsOnlyTheCurrentScene()
        {
            var loader = new FakeSceneContentLoader();
            var flow = Create(loader);

            Assert.That((await flow.GoToCampAsync(CancellationToken.None)).Outcome,
                Is.EqualTo(SceneTransitionOutcome.Succeeded));
            Assert.That((await flow.GoToExpeditionAsync(CancellationToken.None)).Outcome,
                Is.EqualTo(SceneTransitionOutcome.Succeeded));
            Assert.That((await flow.GoToCampAsync(CancellationToken.None)).Outcome,
                Is.EqualTo(SceneTransitionOutcome.Succeeded));

            Assert.That(flow.Snapshot.Current, Is.EqualTo(SceneFlowState.Camp));
            Assert.That(flow.Snapshot.ActiveHandleCount, Is.EqualTo(1));
            Assert.That(loader.LoadCount, Is.EqualTo(3));
            Assert.That(loader.UnloadCount, Is.EqualTo(2));
            await flow.ShutdownAsync();
            Assert.That(loader.ActiveHandleCount, Is.Zero);
        }

        [Test]
        public async Task ConcurrentActivationIsRejectedAsBusy()
        {
            var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var loader = new FakeSceneContentLoader
            {
                BeforeLoad = async (content, token) => await gate.Task
            };
            var flow = Create(loader);

            Task<SceneTransitionResult> first = flow.GoToCampAsync(CancellationToken.None);
            SceneTransitionResult second = await flow.GoToCampAsync(CancellationToken.None);
            gate.SetResult(true);

            Assert.That(second.Outcome, Is.EqualTo(SceneTransitionOutcome.Busy));
            Assert.That((await first).Outcome, Is.EqualTo(SceneTransitionOutcome.Succeeded));
            Assert.That(loader.ActiveHandleCount, Is.EqualTo(1));
        }

        [Test]
        public async Task FailureIsRecoverableAndRetryDoesNotLeak()
        {
            var loader = new FakeSceneContentLoader();
            var flow = Create(loader);
            await flow.GoToCampAsync(CancellationToken.None);
            loader.FailNextLoad = true;

            SceneTransitionResult failed = await flow.GoToExpeditionAsync(CancellationToken.None);
            SceneTransitionResult retried = await flow.RetryAsync(CancellationToken.None);

            Assert.That(failed.Outcome, Is.EqualTo(SceneTransitionOutcome.Failed));
            Assert.That(retried.Outcome, Is.EqualTo(SceneTransitionOutcome.Succeeded));
            Assert.That(flow.Snapshot.Current, Is.EqualTo(SceneFlowState.Expedition));
            Assert.That(loader.ActiveHandleCount, Is.EqualTo(1));
        }

        [Test]
        public async Task CallerCancellationLeavesNoOrphan()
        {
            var loader = new FakeSceneContentLoader
            {
                BeforeLoad = async (content, token) => await Task.Delay(Timeout.Infinite, token)
            };
            var flow = Create(loader);
            using var cancellation = new CancellationTokenSource();

            Task<SceneTransitionResult> transition = flow.GoToCampAsync(cancellation.Token);
            cancellation.Cancel();
            SceneTransitionResult result = await transition;

            Assert.That(result.Outcome, Is.EqualTo(SceneTransitionOutcome.Canceled));
            Assert.That(flow.Snapshot.Current, Is.EqualTo(SceneFlowState.Boot));
            Assert.That(loader.ActiveHandleCount, Is.Zero);
        }

        [Test]
        public async Task TimeoutLeavesStateRecoverableAndNoOrphan()
        {
            var loader = new FakeSceneContentLoader
            {
                BeforeLoad = async (content, token) => await Task.Delay(Timeout.Infinite, token)
            };
            var flow = new SceneFlowService(loader, new RecordingLogger(), TimeSpan.FromMilliseconds(25));

            SceneTransitionResult result = await flow.GoToCampAsync(CancellationToken.None);

            Assert.That(result.Outcome, Is.EqualTo(SceneTransitionOutcome.TimedOut));
            Assert.That(flow.Snapshot.RetryTarget, Is.EqualTo(SceneFlowState.Camp));
            Assert.That(loader.ActiveHandleCount, Is.Zero);
        }

        [Test]
        public async Task ShutdownCancelsInFlightLoadAndLeavesBootWithoutOrphans()
        {
            var loadStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var loader = new FakeSceneContentLoader
            {
                BeforeLoad = async (content, token) =>
                {
                    loadStarted.TrySetResult(true);
                    await Task.Delay(Timeout.Infinite, token);
                }
            };
            var flow = Create(loader);

            Task<SceneTransitionResult> transition = flow.GoToCampAsync(CancellationToken.None);
            await loadStarted.Task;
            await flow.ShutdownAsync();
            SceneTransitionResult result = await transition;

            Assert.That(result.Outcome, Is.EqualTo(SceneTransitionOutcome.Canceled));
            Assert.That(flow.Snapshot.Current, Is.EqualTo(SceneFlowState.Boot));
            Assert.That(flow.Snapshot.IsTransitioning, Is.False);
            Assert.That(loader.ActiveHandleCount, Is.Zero);
            Assert.That(
                (await flow.GoToCampAsync(CancellationToken.None)).Outcome,
                Is.EqualTo(SceneTransitionOutcome.Invalid));
        }

        private static SceneFlowService Create(FakeSceneContentLoader loader)
        {
            return new SceneFlowService(loader, new RecordingLogger(), TimeSpan.FromSeconds(5));
        }
    }
}
