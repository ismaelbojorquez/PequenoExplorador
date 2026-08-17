using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PequenoExplorador.DesignSystem
{
    [DisallowMultipleComponent]
    public sealed class UICancelableMotion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ICancelHandler, IUIThemeElement
    {
        private Coroutine _animation;
        private float _duration = 0.12f;
        private bool _reduceMotion;

        public void ApplyTheme(UIDesignTokens tokens, float textScale, bool reduceMotion)
        {
            _duration = tokens.MotionFast; _reduceMotion = reduceMotion;
            if (_reduceMotion) transform.localScale = Vector3.one;
        }
        public void OnPointerDown(PointerEventData eventData) => AnimateTo(new Vector3(0.96f, 0.96f, 1f));
        public void OnPointerUp(PointerEventData eventData) => AnimateTo(Vector3.one);
        public void OnCancel(BaseEventData eventData) => AnimateTo(Vector3.one);
        private void OnDisable() { if (_animation != null) StopCoroutine(_animation); _animation = null; transform.localScale = Vector3.one; }
        private void AnimateTo(Vector3 target)
        {
            if (_animation != null) StopCoroutine(_animation);
            if (_reduceMotion || !isActiveAndEnabled) { transform.localScale = target; return; }
            _animation = StartCoroutine(Animate(target));
        }
        private IEnumerator Animate(Vector3 target)
        {
            Vector3 start = transform.localScale; float elapsed = 0f;
            while (elapsed < _duration) { elapsed += Time.unscaledDeltaTime; transform.localScale = Vector3.Lerp(start, target, Mathf.Clamp01(elapsed / _duration)); yield return null; }
            transform.localScale = target; _animation = null;
        }
    }
}
