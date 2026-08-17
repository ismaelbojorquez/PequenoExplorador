namespace PequenoExplorador.DesignSystem
{
    public interface IUIThemeElement
    {
        void ApplyTheme(UIDesignTokens tokens, float textScale, bool reduceMotion);
    }
}
