using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.DesignSystem
{
    [DisallowMultipleComponent, RequireComponent(typeof(Button), typeof(Image))]
    public sealed class UIThemedButton : MonoBehaviour, IUIThemeElement
    {
        [SerializeField] private UIButtonStyle _style = UIButtonStyle.Primary;
        [SerializeField] private bool _recommendedChildTarget = true;

        public void ApplyTheme(UIDesignTokens tokens, float textScale, bool reduceMotion)
        {
            RectTransform rect = (RectTransform)transform;
            float target = _recommendedChildTarget ? tokens.RecommendedTouchTarget : tokens.MinimumTouchTarget;
            bool stretchesX = !Mathf.Approximately(rect.anchorMin.x, rect.anchorMax.x);
            bool stretchesY = !Mathf.Approximately(rect.anchorMin.y, rect.anchorMax.y);
            float baseWidth = rect.rect.width - rect.sizeDelta.x;
            float baseHeight = rect.rect.height - rect.sizeDelta.y;
            rect.sizeDelta = new Vector2(
                stretchesX ? Mathf.Max(0f, target - baseWidth) : Mathf.Max(rect.sizeDelta.x, target),
                stretchesY ? Mathf.Max(0f, target - baseHeight) : Mathf.Max(rect.sizeDelta.y, target));
            UIColorRole role = _style switch
            {
                UIButtonStyle.Secondary => UIColorRole.Secondary,
                UIButtonStyle.Quiet => UIColorRole.Paper,
                UIButtonStyle.Positive => UIColorRole.Success,
                UIButtonStyle.Destructive => UIColorRole.Error,
                _ => UIColorRole.Accent
            };
            Color normal = tokens.Color(role);
            Image image = GetComponent<Image>(); image.color = normal;
            if (tokens.RoundedSprite != null) { image.sprite = tokens.RoundedSprite; image.type = Image.Type.Sliced; }
            Button button = GetComponent<Button>(); ColorBlock colors = button.colors;
            colors.normalColor = normal; colors.highlightedColor = Color.Lerp(normal, Color.white, 0.14f);
            colors.pressedColor = Color.Lerp(normal, Color.black, 0.16f); colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(normal.r, normal.g, normal.b, 0.38f); colors.fadeDuration = reduceMotion ? 0f : tokens.MotionFast;
            button.colors = colors;
            UIColorRole labelRole = _style == UIButtonStyle.Destructive ? UIColorRole.OnDark : UIColorRole.Ink;
            foreach (UIThemedText text in GetComponentsInChildren<UIThemedText>(true)) text.ApplyTheme(tokens, textScale, reduceMotion);
            foreach (TMPro.TMP_Text text in GetComponentsInChildren<TMPro.TMP_Text>(true)) text.color = tokens.Color(labelRole);
            foreach (Text text in GetComponentsInChildren<Text>(true)) text.color = tokens.Color(labelRole);
        }

#if UNITY_EDITOR
        public void ConfigureForEditor(UIButtonStyle style, bool recommendedChildTarget = true)
        {
            _style = style; _recommendedChildTarget = recommendedChildTarget;
        }
#endif
    }
}
