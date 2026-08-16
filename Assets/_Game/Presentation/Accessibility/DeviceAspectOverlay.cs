using System;
using PequenoExplorador.Application.Accessibility;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Accessibility
{
    [DisallowMultipleComponent]
    public sealed class DeviceAspectOverlay : MonoBehaviour
    {
        [SerializeField] private Text _label;
        private ISafeAreaService _service;

        public void Bind(ISafeAreaService service, bool visible)
        {
            Unbind();
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _service.Changed += Render;
            gameObject.SetActive(visible);
            if (visible) Render(_service.Current);
        }

        public void Unbind()
        {
            if (_service != null) _service.Changed -= Render;
            _service = null;
        }

        private void Render(SafeAreaSnapshot snapshot)
        {
            if (_label != null)
                _label.text = $"VIEWPORT DEV · {snapshot.ScreenWidth}×{snapshot.ScreenHeight} · {snapshot.Orientation}";
        }

        private void OnDestroy() => Unbind();
    }
}
