using System;

namespace PequenoExplorador.Application.Input
{
    public sealed class GestureThresholds
    {
        public GestureThresholds(
            double tapMaximumSeconds,
            double holdMinimumSeconds,
            float tapMovementPixels,
            float dragStartPixels,
            float pinchDeltaPixels)
        {
            if (tapMaximumSeconds <= 0d || holdMinimumSeconds <= tapMaximumSeconds)
                throw new ArgumentOutOfRangeException(nameof(holdMinimumSeconds));
            if (tapMovementPixels <= 0f || dragStartPixels < tapMovementPixels || pinchDeltaPixels <= 0f)
                throw new ArgumentOutOfRangeException(nameof(dragStartPixels));

            TapMaximumSeconds = tapMaximumSeconds;
            HoldMinimumSeconds = holdMinimumSeconds;
            TapMovementPixels = tapMovementPixels;
            DragStartPixels = dragStartPixels;
            PinchDeltaPixels = pinchDeltaPixels;
        }

        public double TapMaximumSeconds { get; }
        public double HoldMinimumSeconds { get; }
        public float TapMovementPixels { get; }
        public float DragStartPixels { get; }
        public float PinchDeltaPixels { get; }

        public static GestureThresholds ChildFriendlyDefault { get; } =
            new GestureThresholds(0.35d, 0.65d, 24f, 32f, 10f);
    }
}
