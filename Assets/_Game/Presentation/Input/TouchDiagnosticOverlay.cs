using System;
using PequenoExplorador.Application.Accessibility;
using PequenoExplorador.Application.Input;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Input
{
    [DisallowMultipleComponent]
    public sealed class TouchDiagnosticOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private RectTransform _marker;
        [SerializeField] private Text _summary;
        private IInputService _input;
        private ISafeAreaService _safeArea;
        private bool _allowed;

        public void Bind(IInputService input, ISafeAreaService safeArea, bool allowed)
        {
            Unbind();
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _safeArea = safeArea ?? throw new ArgumentNullException(nameof(safeArea));
            _allowed = allowed;
            _input.IntentRaised += OnIntent;
            _safeArea.Changed += OnSafeAreaChanged;
            _root?.SetActive(false);
        }

        public void Unbind()
        {
            if (_input != null) _input.IntentRaised -= OnIntent;
            if (_safeArea != null) _safeArea.Changed -= OnSafeAreaChanged;
            _input = null;
            _safeArea = null;
            _allowed = false;
        }

        private void OnIntent(InputIntent intent)
        {
            if (!_allowed) return;
            if (intent.Kind == InputGestureKind.DebugToggle)
            {
                if (_root != null) _root.SetActive(!_root.activeSelf);
                return;
            }
            if (_root == null || !_root.activeSelf) return;
            if (_marker != null && intent.PointerId >= -1)
            {
                _marker.anchorMin = new Vector2(
                    Screen.width <= 0 ? 0.5f : intent.Position.X / Screen.width,
                    Screen.height <= 0 ? 0.5f : intent.Position.Y / Screen.height);
                _marker.anchorMax = _marker.anchorMin;
            }
            if (_summary != null)
                _summary.text = $"INPUT DEV · {intent.Map} · {intent.Kind} · pointers≤5";
        }

        private void OnSafeAreaChanged(SafeAreaSnapshot snapshot)
        {
            if (_summary != null && _root != null && _root.activeSelf)
                _summary.text = $"INPUT DEV · {snapshot.ScreenWidth}×{snapshot.ScreenHeight} · safe {snapshot.Width:0.000}×{snapshot.Height:0.000}";
        }

        private void OnDestroy() => Unbind();
    }
}
