using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Logging;

namespace PequenoExplorador.Application.SceneFlow
{
    public sealed class SceneFlowService : ISceneFlowService
    {
        private readonly object _gate = new object();
        private readonly ISceneContentLoader _loader;
        private readonly IAppLogger _logger;
        private readonly TimeSpan _transitionTimeout;
        private readonly CancellationTokenSource _lifetimeCancellation = new CancellationTokenSource();
        private ISceneContentHandle _activeHandle;
        private SceneFlowSnapshot _snapshot;
        private TaskCompletionSource<bool> _transitionCompletion;
        private bool _transitionClaimed;
        private bool _shutdown;

        public SceneFlowService(
            ISceneContentLoader loader,
            IAppLogger logger,
            TimeSpan transitionTimeout)
        {
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (transitionTimeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(transitionTimeout));
            }

            _transitionTimeout = transitionTimeout;
            _snapshot = CreateSnapshot(SceneFlowState.Boot, SceneFlowState.Boot, false, 0f, string.Empty, null);
        }

        public event Action<SceneFlowSnapshot> Changed;

        public SceneFlowSnapshot Snapshot => _snapshot;

        public Task<SceneTransitionResult> GoToCampAsync(CancellationToken cancellationToken)
        {
            return TransitionAsync(SceneFlowState.Camp, cancellationToken);
        }

        public Task<SceneTransitionResult> GoToExpeditionAsync(CancellationToken cancellationToken)
        {
            return TransitionAsync(SceneFlowState.Expedition, cancellationToken);
        }

        public Task<SceneTransitionResult> RetryAsync(CancellationToken cancellationToken)
        {
            SceneFlowState? retryTarget = _snapshot.RetryTarget;
            return retryTarget.HasValue
                ? TransitionAsync(retryTarget.Value, cancellationToken)
                : Task.FromResult(new SceneTransitionResult(SceneTransitionOutcome.Invalid, "NoRetryAvailable"));
        }

        public async Task ShutdownAsync()
        {
            Task transitionTask;
            lock (_gate)
            {
                if (_shutdown)
                {
                    return;
                }

                _shutdown = true;
                _lifetimeCancellation.Cancel();
                transitionTask = _transitionCompletion?.Task ?? Task.CompletedTask;
            }

            await transitionTask;
            ISceneContentHandle handle;
            lock (_gate)
            {
                handle = _activeHandle;
                _activeHandle = null;
            }

            if (handle != null)
            {
                await _loader.UnloadAsync(handle, CancellationToken.None);
            }

            Publish(CreateSnapshot(SceneFlowState.Boot, SceneFlowState.Boot, false, 0f, string.Empty, null));
            Changed = null;
            _lifetimeCancellation.Dispose();
        }

        private async Task<SceneTransitionResult> TransitionAsync(
            SceneFlowState target,
            CancellationToken cancellationToken)
        {
            SceneFlowState origin;
            lock (_gate)
            {
                if (_shutdown)
                {
                    return new SceneTransitionResult(SceneTransitionOutcome.Invalid, "SceneFlowShutdown");
                }

                origin = _snapshot.Current;
                if (_transitionClaimed)
                {
                    _logger.Write(new AppLogEntry(
                        AppLogLevel.Warning,
                        "SceneFlow",
                        "TransitionRejectedBusy",
                        origin + "To" + target));
                    return new SceneTransitionResult(SceneTransitionOutcome.Busy, "TransitionInProgress");
                }

                if (origin == target && string.IsNullOrEmpty(_snapshot.ErrorCode))
                {
                    return new SceneTransitionResult(SceneTransitionOutcome.AlreadyThere);
                }

                if (!IsAllowed(origin, target))
                {
                    return new SceneTransitionResult(SceneTransitionOutcome.Invalid, "TransitionNotAllowed");
                }

                _transitionClaimed = true;
                _transitionCompletion = new TaskCompletionSource<bool>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
            }

            _logger.Write(new AppLogEntry(
                AppLogLevel.Info,
                "SceneFlow",
                "TransitionStarted",
                origin + "To" + target));

            Publish(CreateSnapshot(origin, target, true, 0f, string.Empty, null));
            ISceneContentHandle pendingHandle = null;
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _lifetimeCancellation.Token);
            timeout.CancelAfter(_transitionTimeout);

            try
            {
                SceneContentId contentId = target == SceneFlowState.Camp
                    ? SceneContentId.Camp
                    : SceneContentId.Jungle;
                pendingHandle = await _loader.LoadAsync(
                    contentId,
                    new DirectProgress(value => PublishProgress(origin, target, value)),
                    timeout.Token);
                timeout.Token.ThrowIfCancellationRequested();

                ISceneContentHandle previous = _activeHandle;
                if (previous != null)
                {
                    await _loader.UnloadAsync(previous, CancellationToken.None);
                }

                _activeHandle = pendingHandle;
                pendingHandle = null;
                ReleaseTransitionClaim();
                Publish(CreateSnapshot(target, target, false, 1f, string.Empty, null));
                _logger.Write(new AppLogEntry(AppLogLevel.Info, "SceneFlow", "TransitionCompleted", target.ToString()));
                return new SceneTransitionResult(SceneTransitionOutcome.Succeeded);
            }
            catch (OperationCanceledException)
            {
                await CleanupPendingAsync(pendingHandle);
                bool callerCanceled = cancellationToken.IsCancellationRequested ||
                                      _lifetimeCancellation.IsCancellationRequested;
                string code = callerCanceled ? "TransitionCanceled" : "TransitionTimeout";
                ReleaseTransitionClaim();
                Publish(CreateSnapshot(origin, target, false, 0f, code, target));
                _logger.Write(new AppLogEntry(AppLogLevel.Warning, "SceneFlow", code, target.ToString()));
                return new SceneTransitionResult(
                    callerCanceled ? SceneTransitionOutcome.Canceled : SceneTransitionOutcome.TimedOut,
                    code);
            }
            catch (Exception exception)
            {
                await CleanupPendingAsync(pendingHandle);
                string code = "SceneLoad" + exception.GetType().Name;
                ReleaseTransitionClaim();
                Publish(CreateSnapshot(origin, target, false, 0f, code, target));
                _logger.Write(new AppLogEntry(AppLogLevel.Error, "SceneFlow", "TransitionFailed", code));
                return new SceneTransitionResult(SceneTransitionOutcome.Failed, code);
            }
            finally
            {
                ReleaseTransitionClaim();
                CompleteTransition();
            }
        }

        private static bool IsAllowed(SceneFlowState origin, SceneFlowState target)
        {
            return (origin == SceneFlowState.Boot && target == SceneFlowState.Camp) ||
                   (origin == SceneFlowState.Camp && target == SceneFlowState.Expedition) ||
                   (origin == SceneFlowState.Expedition && target == SceneFlowState.Camp) ||
                   (origin == target);
        }

        private async Task CleanupPendingAsync(ISceneContentHandle handle)
        {
            if (handle != null && !handle.IsReleased)
            {
                await _loader.UnloadAsync(handle, CancellationToken.None);
            }
        }

        private void PublishProgress(SceneFlowState origin, SceneFlowState target, float progress)
        {
            if (_transitionClaimed)
            {
                Publish(CreateSnapshot(origin, target, true, progress, string.Empty, null));
            }
        }

        private void ReleaseTransitionClaim()
        {
            lock (_gate)
            {
                _transitionClaimed = false;
            }
        }

        private void CompleteTransition()
        {
            TaskCompletionSource<bool> completion;
            lock (_gate)
            {
                completion = _transitionCompletion;
                _transitionCompletion = null;
            }

            completion?.TrySetResult(true);
        }

        private SceneFlowSnapshot CreateSnapshot(
            SceneFlowState current,
            SceneFlowState target,
            bool transitioning,
            float progress,
            string errorCode,
            SceneFlowState? retryTarget)
        {
            return new SceneFlowSnapshot(
                current,
                target,
                transitioning,
                progress,
                errorCode,
                retryTarget,
                _loader.ActiveHandleCount);
        }

        private void Publish(SceneFlowSnapshot snapshot)
        {
            _snapshot = snapshot;
            Changed?.Invoke(snapshot);
        }

        private sealed class DirectProgress : IProgress<float>
        {
            private readonly Action<float> _report;

            public DirectProgress(Action<float> report)
            {
                _report = report;
            }

            public void Report(float value)
            {
                _report(value);
            }
        }
    }
}
