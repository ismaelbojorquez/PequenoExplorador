using System;

namespace PequenoExplorador.Application.Photography
{
    public sealed class PhotoEvaluationSettings
    {
        public PhotoEvaluationSettings(float minimumCoverage, float maximumDistance, float maximumCenterOffset, float minimumOrientationAlignment)
        {
            if (!IsUnit(minimumCoverage) || maximumDistance < 1f || maximumDistance > 50f ||
                !IsUnit(maximumCenterOffset) || !IsUnit(minimumOrientationAlignment)) throw new ArgumentOutOfRangeException(nameof(minimumCoverage));
            MinimumCoverage = minimumCoverage;
            MaximumDistance = maximumDistance;
            MaximumCenterOffset = maximumCenterOffset;
            MinimumOrientationAlignment = minimumOrientationAlignment;
        }

        public float MinimumCoverage { get; }
        public float MaximumDistance { get; }
        public float MaximumCenterOffset { get; }
        public float MinimumOrientationAlignment { get; }
        public static PhotoEvaluationSettings ChildFriendlyDefault { get; } = new PhotoEvaluationSettings(0.08f, 10f, 0.36f, 0.35f);
        private static bool IsUnit(float value) => !float.IsNaN(value) && !float.IsInfinity(value) && value >= 0f && value <= 1f;
    }
}
