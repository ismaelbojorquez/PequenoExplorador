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
        private bool _allowed;

        public void Bind(ISafeAreaService service, bool visible)
        {
            Unbind();
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _allowed = visible;
            _service.Changed += Render;
            gameObject.SetActive(false);
            if (visible) Render(_service.Current);
        }

        public void SetVisible(bool visible)
        {
            bool show = _allowed && visible;
            gameObject.SetActive(show);
            if (show && _service != null) Render(_service.Current);
        }

        public void Unbind()
        {
            if (_service != null) _service.Changed -= Render;
            _service = null;
            _allowed = false;
        }

        private void Render(SafeAreaSnapshot snapshot)
        {
            if (_label != null)
                _label.text = $"VIEWPORT DEV · {snapshot.ScreenWidth}×{snapshot.ScreenHeight} · {snapshot.Orientation}";
        }

        private void OnDestroy() => Unbind();
    }
}
