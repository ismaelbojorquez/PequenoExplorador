using System;
using PequenoExplorador.Application.Accessibility;
using UnityEngine;

namespace PequenoExplorador.Presentation.Accessibility
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        private ISafeAreaService _service;
        private RectTransform _rectTransform;

        public SafeAreaSnapshot Applied { get; private set; }

        private void Awake() => _rectTransform = (RectTransform)transform;

        public void Bind(ISafeAreaService service)
        {
            Unbind();
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _service.Changed += Apply;
            Apply(_service.Current);
        }

        public void Apply(SafeAreaSnapshot snapshot)
        {
            if (_rectTransform == null) _rectTransform = (RectTransform)transform;
            _rectTransform.anchorMin = new Vector2(snapshot.Left, snapshot.Bottom);
            _rectTransform.anchorMax = new Vector2(1f - snapshot.Right, 1f - snapshot.Top);
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
            Applied = snapshot;
        }

        public void Unbind()
        {
            if (_service != null) _service.Changed -= Apply;
            _service = null;
        }

        private void OnDestroy() => Unbind();
    }
}
