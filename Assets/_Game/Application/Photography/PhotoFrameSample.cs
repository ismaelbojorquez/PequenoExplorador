using System;

namespace PequenoExplorador.Application.Photography
{
    public readonly struct PhotoFrameSample
    {
        public PhotoFrameSample(float viewportCoverage, float distance, bool hasLineOfSight, float centerOffset, float orientationAlignment)
        {
            if (!IsUnit(viewportCoverage) || float.IsNaN(distance) || float.IsInfinity(distance) || distance < 0f ||
                !IsUnit(centerOffset) || !IsUnit(orientationAlignment)) throw new ArgumentOutOfRangeException(nameof(viewportCoverage));
            ViewportCoverage = viewportCoverage;
            Distance = distance;
            HasLineOfSight = hasLineOfSight;
            CenterOffset = centerOffset;
            OrientationAlignment = orientationAlignment;
        }

        public float ViewportCoverage { get; }
        public float Distance { get; }
        public bool HasLineOfSight { get; }
        public float CenterOffset { get; }
        public float OrientationAlignment { get; }
        private static bool IsUnit(float value) => !float.IsNaN(value) && !float.IsInfinity(value) && value >= 0f && value <= 1f;
    }
}
