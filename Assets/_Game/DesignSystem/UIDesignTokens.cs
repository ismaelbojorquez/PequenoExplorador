using TMPro;
using UnityEngine;

namespace PequenoExplorador.DesignSystem
{
    public enum UIColorRole
    {
        Canvas,
        Surface,
        Paper,
        Primary,
        Secondary,
        Accent,
        Success,
        Warning,
        Error,
        Ink,
        MutedInk,
        OnDark,
        Outline
    }

    public enum UITypographyRole
    {
        Display,
        Headline,
        Title,
        Body,
        Label,
        Caption
    }

    public enum UIButtonStyle
    {
        Primary,
        Secondary,
        Quiet,
        Positive,
        Destructive
    }

    public enum UIStateKind
    {
        Loading,
        Empty,
        Error,
        Offline,
        Locked,
        Success
    }

    [CreateAssetMenu(fileName = "PH_UI_DesignTokens", menuName = "Pequeno Explorador/UI/Design Tokens")]
    public sealed class UIDesignTokens : ScriptableObject
    {
        [Header("Palette")]
        [SerializeField] private Color _canvas = new Color32(18, 67, 57, 255);
        [SerializeField] private Color _surface = new Color32(28, 91, 72, 255);
        [SerializeField] private Color _paper = new Color32(255, 248, 226, 255);
        [SerializeField] private Color _primary = new Color32(61, 168, 108, 255);
        [SerializeField] private Color _secondary = new Color32(73, 166, 211, 255);
        [SerializeField] private Color _accent = new Color32(255, 176, 57, 255);
        [SerializeField] private Color _success = new Color32(87, 184, 106, 255);
        [SerializeField] private Color _warning = new Color32(245, 151, 48, 255);
        [SerializeField] private Color _error = new Color32(210, 76, 75, 255);
        [SerializeField] private Color _ink = new Color32(28, 52, 47, 255);
        [SerializeField] private Color _mutedInk = new Color32(78, 103, 95, 255);
        [SerializeField] private Color _onDark = new Color32(255, 252, 239, 255);
        [SerializeField] private Color _outline = new Color32(10, 48, 41, 255);

        [Header("Typography")]
        [SerializeField] private TMP_FontAsset _font;
        [SerializeField] private float _displaySize = 52f;
        [SerializeField] private float _headlineSize = 42f;
        [SerializeField] private float _titleSize = 32f;
        [SerializeField] private float _bodySize = 26f;
        [SerializeField] private float _labelSize = 24f;
        [SerializeField] private float _captionSize = 18f;

        [Header("Geometry")]
        [SerializeField] private Sprite _roundedSprite;
        [SerializeField] private float _spacingUnit = 8f;
        [SerializeField] private float _minimumTouchTarget = 64f;
        [SerializeField] private float _recommendedTouchTarget = 72f;
        [SerializeField] private Vector2 _shadowDistance = new Vector2(0f, -5f);

        [Header("Motion")]
        [SerializeField] private float _motionFast = 0.12f;
        [SerializeField] private float _motionStandard = 0.22f;
        [SerializeField] private float _motionCelebrate = 0.36f;

        public TMP_FontAsset Font => _font;
        public Sprite RoundedSprite => _roundedSprite;
        public float SpacingUnit => _spacingUnit;
        public float MinimumTouchTarget => _minimumTouchTarget;
        public float RecommendedTouchTarget => _recommendedTouchTarget;
        public Vector2 ShadowDistance => _shadowDistance;
        public float MotionFast => _motionFast;
        public float MotionStandard => _motionStandard;
        public float MotionCelebrate => _motionCelebrate;

        public Color Color(UIColorRole role)
        {
            return role switch
            {
                UIColorRole.Canvas => _canvas,
                UIColorRole.Surface => _surface,
                UIColorRole.Paper => _paper,
                UIColorRole.Primary => _primary,
                UIColorRole.Secondary => _secondary,
                UIColorRole.Accent => _accent,
                UIColorRole.Success => _success,
                UIColorRole.Warning => _warning,
                UIColorRole.Error => _error,
                UIColorRole.Ink => _ink,
                UIColorRole.MutedInk => _mutedInk,
                UIColorRole.OnDark => _onDark,
                UIColorRole.Outline => _outline,
                _ => _onDark
            };
        }

        public float FontSize(UITypographyRole role)
        {
            return role switch
            {
                UITypographyRole.Display => _displaySize,
                UITypographyRole.Headline => _headlineSize,
                UITypographyRole.Title => _titleSize,
                UITypographyRole.Body => _bodySize,
                UITypographyRole.Label => _labelSize,
                UITypographyRole.Caption => _captionSize,
                _ => _bodySize
            };
        }

#if UNITY_EDITOR
        public void ConfigureForEditor(TMP_FontAsset font, Sprite roundedSprite)
        {
            _font = font;
            _roundedSprite = roundedSprite;
        }
#endif
    }
}
