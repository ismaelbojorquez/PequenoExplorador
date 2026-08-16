using PequenoExplorador.Application.Input;
using UnityEngine;

namespace PequenoExplorador.Content.Input
{
    [CreateAssetMenu(fileName = "GestureThresholds", menuName = "Pequeño Explorador/Input/Gesture Thresholds")]
    public sealed class GestureThresholdsAsset : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float _tapMaximumSeconds = 0.35f;
        [SerializeField, Min(0.2f)] private float _holdMinimumSeconds = 0.65f;
        [SerializeField, Min(1f)] private float _tapMovementPixels = 24f;
        [SerializeField, Min(1f)] private float _dragStartPixels = 32f;
        [SerializeField, Min(1f)] private float _pinchDeltaPixels = 10f;

        public float TapMaximumSeconds => _tapMaximumSeconds;
        public float HoldMinimumSeconds => _holdMinimumSeconds;
        public float TapMovementPixels => _tapMovementPixels;
        public float DragStartPixels => _dragStartPixels;
        public float PinchDeltaPixels => _pinchDeltaPixels;

        public GestureThresholds ToRuntime() => new GestureThresholds(
            _tapMaximumSeconds,
            _holdMinimumSeconds,
            _tapMovementPixels,
            _dragStartPixels,
            _pinchDeltaPixels);
    }
}
