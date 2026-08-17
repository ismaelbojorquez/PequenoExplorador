using UnityEngine;

namespace PequenoExplorador.DesignSystem
{
    [DisallowMultipleComponent]
    public sealed class UIDesignSystemRoot : MonoBehaviour
    {
        [SerializeField] private UIDesignTokens _tokens;
        [SerializeField, Range(1f, 1.25f)] private float _textScale = 1f;
        [SerializeField] private bool _reduceMotion;

        public UIDesignTokens Tokens => _tokens;
        public float TextScale => _textScale;
        public bool ReduceMotion => _reduceMotion;

        private void Awake() => Apply();

        public void Apply()
        {
            if (_tokens == null)
            {
                Debug.LogError("UI design tokens are not assigned.", this);
                return;
            }
            foreach (MonoBehaviour component in GetComponentsInChildren<MonoBehaviour>(true))
                if (component is IUIThemeElement element) element.ApplyTheme(_tokens, _textScale, _reduceMotion);
        }

        public void SetAccessibility(float textScale, bool reduceMotion)
        {
            _textScale = Mathf.Clamp(textScale, 1f, 1.25f);
            _reduceMotion = reduceMotion;
            Apply();
        }

#if UNITY_EDITOR
        public void ConfigureForEditor(UIDesignTokens tokens)
        {
            _tokens = tokens;
            Apply();
        }
#endif
    }
}
