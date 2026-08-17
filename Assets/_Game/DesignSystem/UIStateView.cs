using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.DesignSystem
{
    [DisallowMultipleComponent]
    public sealed class UIStateView : MonoBehaviour, IUIThemeElement
    {
        [SerializeField] private Image _symbol;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _body;
        [SerializeField] private Button _action;
        [SerializeField] private UIStateKind _kind = UIStateKind.Loading;

        public UIStateKind Kind => _kind;

        public void Present(UIStateKind kind, string title, string body, bool showAction)
        {
            _kind = kind;
            if (_title != null) _title.text = title ?? string.Empty;
            if (_body != null) _body.text = body ?? string.Empty;
            if (_action != null) _action.gameObject.SetActive(showAction);
        }

        public void ApplyTheme(UIDesignTokens tokens, float textScale, bool reduceMotion)
        {
            if (_symbol == null) return;
            _symbol.color = tokens.Color(_kind switch
            {
                UIStateKind.Error => UIColorRole.Error,
                UIStateKind.Offline => UIColorRole.Warning,
                UIStateKind.Locked => UIColorRole.MutedInk,
                UIStateKind.Success => UIColorRole.Success,
                _ => UIColorRole.Secondary
            });
        }

#if UNITY_EDITOR
        public void ConfigureForEditor(Image symbol, TMP_Text title, TMP_Text body, Button action, UIStateKind kind)
        {
            _symbol = symbol;
            _title = title;
            _body = body;
            _action = action;
            _kind = kind;
        }
#endif
    }
}
