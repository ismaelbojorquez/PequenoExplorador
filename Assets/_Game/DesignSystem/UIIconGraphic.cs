using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.DesignSystem
{
    public enum UIIconKind
    {
        None,
        Back,
        Explore,
        Album,
        Camera,
        Replay,
        Hint,
        Lock,
        Check,
        Star,
        Customize,
        Parents
        ,GestureTap
        ,Arrow
    }

    [DisallowMultipleComponent]
    public sealed class UIIconGraphic : MaskableGraphic, IUIThemeElement
    {
        [SerializeField] private UIIconKind _kind;
        [SerializeField] private UIColorRole _colorRole = UIColorRole.Ink;
        [SerializeField, Range(2f, 8f)] private float _stroke = 4f;

        public UIIconKind Kind => _kind;

        public void ApplyTheme(UIDesignTokens tokens, float textScale, bool reduceMotion)
        {
            color = tokens.Color(_colorRole);
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper helper)
        {
            helper.Clear();
            if (_kind == UIIconKind.None) return;
            float scale = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) * 0.42f;
            Vector2 center = rectTransform.rect.center;
            switch (_kind)
            {
                case UIIconKind.Back:
                    Line(helper, center + new Vector2(scale * 0.7f, 0f), center - new Vector2(scale * 0.7f, 0f));
                    Line(helper, center - new Vector2(scale * 0.7f, 0f), center + new Vector2(-scale * 0.15f, scale * 0.55f));
                    Line(helper, center - new Vector2(scale * 0.7f, 0f), center + new Vector2(-scale * 0.15f, -scale * 0.55f));
                    break;
                case UIIconKind.Album:
                    Box(helper, center + new Vector2(-scale * 0.45f, 0f), new Vector2(scale * 0.75f, scale * 1.25f));
                    Box(helper, center + new Vector2(scale * 0.45f, 0f), new Vector2(scale * 0.75f, scale * 1.25f));
                    Line(helper, center + new Vector2(0f, -scale * 0.62f), center + new Vector2(0f, scale * 0.62f));
                    break;
                case UIIconKind.Camera:
                    Box(helper, center, new Vector2(scale * 1.7f, scale * 1.05f));
                    Box(helper, center + new Vector2(-scale * 0.45f, scale * 0.68f), new Vector2(scale * 0.55f, scale * 0.22f));
                    Diamond(helper, center, scale * 0.38f);
                    break;
                case UIIconKind.Check:
                    Line(helper, center + new Vector2(-scale * 0.7f, 0f), center + new Vector2(-scale * 0.18f, -scale * 0.48f));
                    Line(helper, center + new Vector2(-scale * 0.18f, -scale * 0.48f), center + new Vector2(scale * 0.75f, scale * 0.55f));
                    break;
                case UIIconKind.Lock:
                    Box(helper, center + new Vector2(0f, -scale * 0.25f), new Vector2(scale * 1.2f, scale * 0.9f));
                    Line(helper, center + new Vector2(-scale * 0.42f, scale * 0.2f), center + new Vector2(-scale * 0.42f, scale * 0.72f));
                    Line(helper, center + new Vector2(scale * 0.42f, scale * 0.2f), center + new Vector2(scale * 0.42f, scale * 0.72f));
                    Line(helper, center + new Vector2(-scale * 0.42f, scale * 0.72f), center + new Vector2(scale * 0.42f, scale * 0.72f));
                    break;
                case UIIconKind.Hint:
                    Diamond(helper, center + new Vector2(0f, scale * 0.2f), scale * 0.65f);
                    Line(helper, center + new Vector2(-scale * 0.28f, -scale * 0.58f), center + new Vector2(scale * 0.28f, -scale * 0.58f));
                    break;
                case UIIconKind.Parents:
                    Diamond(helper, center + new Vector2(-scale * 0.38f, scale * 0.35f), scale * 0.3f);
                    Diamond(helper, center + new Vector2(scale * 0.38f, scale * 0.35f), scale * 0.3f);
                    Line(helper, center + new Vector2(-scale * 0.78f, -scale * 0.55f), center + new Vector2(0f, scale * 0.05f));
                    Line(helper, center + new Vector2(0f, scale * 0.05f), center + new Vector2(scale * 0.78f, -scale * 0.55f));
                    break;
                case UIIconKind.Customize:
                    Line(helper, center + new Vector2(-scale * 0.75f, -scale * 0.45f), center + new Vector2(scale * 0.75f, scale * 0.45f));
                    Diamond(helper, center + new Vector2(-scale * 0.75f, -scale * 0.45f), scale * 0.18f);
                    break;
                case UIIconKind.Star:
                    Diamond(helper, center, scale * 0.72f);
                    Line(helper, center + new Vector2(-scale * 0.72f, 0f), center + new Vector2(scale * 0.72f, 0f));
                    break;
                case UIIconKind.Replay:
                    Diamond(helper, center, scale * 0.58f);
                    Line(helper, center + new Vector2(-scale * 0.78f, scale * 0.55f), center + new Vector2(-scale * 0.78f, -scale * 0.2f));
                    Line(helper, center + new Vector2(-scale * 0.78f, scale * 0.55f), center + new Vector2(-scale * 0.2f, scale * 0.55f));
                    break;
                case UIIconKind.GestureTap:
                    Diamond(helper, center + new Vector2(0f, scale * 0.28f), scale * 0.28f);
                    Line(helper, center + new Vector2(0f, scale * 0.02f), center + new Vector2(0f, -scale * 0.72f));
                    Line(helper, center + new Vector2(0f, -scale * 0.4f), center + new Vector2(scale * 0.45f, -scale * 0.7f));
                    break;
                case UIIconKind.Arrow:
                    Line(helper, center + new Vector2(-scale * 0.75f, 0f), center + new Vector2(scale * 0.72f, 0f));
                    Line(helper, center + new Vector2(scale * 0.72f, 0f), center + new Vector2(scale * 0.18f, scale * 0.5f));
                    Line(helper, center + new Vector2(scale * 0.72f, 0f), center + new Vector2(scale * 0.18f, -scale * 0.5f));
                    break;
                default:
                    Diamond(helper, center, scale * 0.75f);
                    Line(helper, center, center + new Vector2(0f, scale * 0.75f));
                    break;
            }
        }

        public void SetKind(UIIconKind kind) { _kind = kind; SetVerticesDirty(); }

        private void Box(VertexHelper helper, Vector2 center, Vector2 size)
        {
            Vector2 half = size * 0.5f;
            Vector2 a = center + new Vector2(-half.x, -half.y); Vector2 b = center + new Vector2(-half.x, half.y);
            Vector2 c = center + new Vector2(half.x, half.y); Vector2 d = center + new Vector2(half.x, -half.y);
            Line(helper, a, b); Line(helper, b, c); Line(helper, c, d); Line(helper, d, a);
        }

        private void Diamond(VertexHelper helper, Vector2 center, float radius)
        {
            Vector2 top = center + Vector2.up * radius; Vector2 right = center + Vector2.right * radius;
            Vector2 bottom = center + Vector2.down * radius; Vector2 left = center + Vector2.left * radius;
            Line(helper, top, right); Line(helper, right, bottom); Line(helper, bottom, left); Line(helper, left, top);
        }

        private void Line(VertexHelper helper, Vector2 start, Vector2 end)
        {
            Vector2 direction = (end - start).normalized; Vector2 normal = new Vector2(-direction.y, direction.x) * (_stroke * 0.5f);
            int index = helper.currentVertCount;
            helper.AddVert(start - normal, color, Vector2.zero); helper.AddVert(start + normal, color, Vector2.zero);
            helper.AddVert(end + normal, color, Vector2.zero); helper.AddVert(end - normal, color, Vector2.zero);
            helper.AddTriangle(index, index + 1, index + 2); helper.AddTriangle(index, index + 2, index + 3);
        }

#if UNITY_EDITOR
        public void ConfigureForEditor(UIIconKind kind, UIColorRole colorRole)
        {
            _kind = kind; _colorRole = colorRole; SetVerticesDirty();
        }
#endif
    }
}
