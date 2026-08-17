using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.DesignSystem
{
    [DisallowMultipleComponent, RequireComponent(typeof(Image))]
    public sealed class UIThemedPanel : MonoBehaviour, IUIThemeElement
    {
        [SerializeField] private UIColorRole _colorRole = UIColorRole.Surface;
        [SerializeField] private bool _paperCard;
        [SerializeField] private bool _shadow;

        public void ApplyTheme(UIDesignTokens tokens, float textScale, bool reduceMotion)
        {
            Image image = GetComponent<Image>();
            image.color = tokens.Color(_paperCard ? UIColorRole.Paper : _colorRole);
            if (tokens.RoundedSprite != null) { image.sprite = tokens.RoundedSprite; image.type = Image.Type.Sliced; }
            Shadow existing = GetComponent<Shadow>();
            if (_shadow && existing == null) existing = gameObject.AddComponent<Shadow>();
            if (existing == null) return;
            existing.enabled = _shadow;
            existing.effectColor = new Color(0f, 0f, 0f, 0.22f);
            existing.effectDistance = tokens.ShadowDistance;
            existing.useGraphicAlpha = true;
        }

#if UNITY_EDITOR
        public void ConfigureForEditor(UIColorRole role, bool paperCard, bool shadow)
        {
            _colorRole = role; _paperCard = paperCard; _shadow = shadow;
        }
#endif
    }
}
