using PequenoExplorador.Infrastructure.Accessibility;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Input
{
    [DisallowMultipleComponent]
    public sealed class MobileInputDriver : MonoBehaviour
    {
        private UnityInputService _input;
        private UnitySafeAreaService _safeArea;

        public void Bind(UnityInputService input, UnitySafeAreaService safeArea)
        {
            _input = input;
            _safeArea = safeArea;
        }

        private void Update()
        {
            _input?.Tick(UnityEngine.Time.unscaledTimeAsDouble);
            _safeArea?.Tick();
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused) _input?.CancelActiveGestures();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) _input?.CancelActiveGestures();
        }

        private void OnDestroy()
        {
            _input = null;
            _safeArea = null;
        }
    }
}
