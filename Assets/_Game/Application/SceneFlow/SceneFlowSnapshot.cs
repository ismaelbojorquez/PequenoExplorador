using System;

namespace PequenoExplorador.Application.SceneFlow
{
    public sealed class SceneFlowSnapshot
    {
        public SceneFlowSnapshot(
            SceneFlowState current,
            SceneFlowState target,
            bool isTransitioning,
            float progress,
            string errorCode,
            SceneFlowState? retryTarget,
            int activeHandleCount,
            SceneContentId currentContent,
            SceneContentId targetContent)
        {
            Current = current;
            Target = target;
            IsTransitioning = isTransitioning;
            Progress = Math.Max(0f, Math.Min(1f, progress));
            ErrorCode = errorCode ?? string.Empty;
            RetryTarget = retryTarget;
            ActiveHandleCount = activeHandleCount;
            CurrentContent = currentContent;
            TargetContent = targetContent;
        }

        public SceneFlowState Current { get; }
        public SceneFlowState Target { get; }
        public bool IsTransitioning { get; }
        public float Progress { get; }
        public string ErrorCode { get; }
        public SceneFlowState? RetryTarget { get; }
        public int ActiveHandleCount { get; }
        public SceneContentId CurrentContent { get; }
        public SceneContentId TargetContent { get; }
        public bool HasRecoverableError => !string.IsNullOrEmpty(ErrorCode) && RetryTarget.HasValue;
    }
}
