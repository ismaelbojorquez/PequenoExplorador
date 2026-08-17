using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.DesignSystem
{
    [DisallowMultipleComponent]
    public sealed class UIThemedText : MonoBehaviour, IUIThemeElement
    {
        [SerializeField] private UITypographyRole _role = UITypographyRole.Body;
        [SerializeField] private UIColorRole _colorRole = UIColorRole.OnDark;
        [SerializeField] private bool _bold;

        public void ApplyTheme(UIDesignTokens tokens, float textScale, bool reduceMotion)
        {
            TMP_Text tmp = GetComponent<TMP_Text>();
            if (tmp != null)
            {
                if (tokens.Font != null) tmp.font = tokens.Font;
                tmp.fontSize = tokens.FontSize(_role) * textScale;
                tmp.color = tokens.Color(_colorRole);
                tmp.fontStyle = _bold ? FontStyles.Bold : FontStyles.Normal;
                tmp.textWrappingMode = TextWrappingModes.Normal;
                tmp.overflowMode = TextOverflowModes.Ellipsis;
                tmp.raycastTarget = false;
                return;
            }
            Text legacy = GetComponent<Text>();
            if (legacy == null) return;
            int resolvedSize = Mathf.RoundToInt(tokens.FontSize(_role) * textScale);
            legacy.fontSize = resolvedSize;
            if (legacy.resizeTextForBestFit)
            {
                legacy.resizeTextMinSize = Mathf.Min(16, resolvedSize);
                legacy.resizeTextMaxSize = resolvedSize;
            }
            legacy.color = tokens.Color(_colorRole);
            legacy.fontStyle = _bold ? FontStyle.Bold : FontStyle.Normal;
            legacy.raycastTarget = false;
        }

#if UNITY_EDITOR
        public void ConfigureForEditor(UITypographyRole role, UIColorRole colorRole, bool bold)
        {
            _role = role; _colorRole = colorRole; _bold = bold;
        }
#endif
    }
}
