using System;
using System.Collections;
using PequenoExplorador.Application.Accessibility;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Accessibility
{
    [DisallowMultipleComponent]
    public sealed class SurfaceLifecycleAdapter : MonoBehaviour
    {
        [SerializeField] private Camera _worldCamera;
        [SerializeField] private Canvas[] _canvases = Array.Empty<Canvas>();
        private ISafeAreaService _safeArea;
        private Coroutine _recovery;

        public int RecoveryCount { get; private set; }
        public SafeAreaSnapshot LastSnapshot { get; private set; }

        public void Bind(ISafeAreaService safeArea)
        {
            Unbind();
            _safeArea = safeArea ?? throw new ArgumentNullException(nameof(safeArea));
            _safeArea.Changed += HandleSafeAreaChanged;
            HandleSafeAreaChanged(_safeArea.Current);
        }

        public void NotifyApplicationResumed()
        {
            if (_safeArea != null) ScheduleRecovery(_safeArea.Current);
        }

        public void Unbind()
        {
            if (_safeArea != null) _safeArea.Changed -= HandleSafeAreaChanged;
            _safeArea = null;
            if (_recovery != null) StopCoroutine(_recovery);
            _recovery = null;
        }

#if UNITY_EDITOR
        public void ConfigureForEditorAndTests(Camera worldCamera, Canvas[] canvases)
        {
            _worldCamera = worldCamera;
            _canvases = canvases ?? Array.Empty<Canvas>();
        }
#endif

        private void HandleSafeAreaChanged(SafeAreaSnapshot snapshot) => ScheduleRecovery(snapshot);

        private void ScheduleRecovery(SafeAreaSnapshot snapshot)
        {
            LastSnapshot = snapshot;
            if (!isActiveAndEnabled) return;
            if (_recovery != null) StopCoroutine(_recovery);
            _recovery = StartCoroutine(RecoverSurface());
        }

        private IEnumerator RecoverSurface()
        {
            // A regular frame is deterministic in batchmode and lets Android finish recreating its surface.
            yield return null;
            Canvas.ForceUpdateCanvases();
            foreach (Canvas canvas in _canvases)
            {
                if (canvas == null) continue;
                if (canvas.transform is RectTransform rect) LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }

            if (_worldCamera != null && _worldCamera.enabled)
            {
                _worldCamera.ResetAspect();
                _worldCamera.ResetProjectionMatrix();
                _worldCamera.enabled = false;
                yield return null;
                _worldCamera.enabled = true;
            }
            Canvas.ForceUpdateCanvases();
            RecoveryCount++;
            _recovery = null;
        }

        private void OnDestroy() => Unbind();
    }
}
