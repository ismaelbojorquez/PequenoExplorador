using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.DesignSystem;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class UIDesignSystemValidationService
    {
        public static IReadOnlyList<string> Validate()
        {
            var violations = new List<string>();
            UIDesignTokens tokens = AssetDatabase.LoadAssetAtPath<UIDesignTokens>(UIDesignSystemSetup.TokenPath);
            if (tokens == null) violations.Add("UI001 design token asset is missing.");
            else
            {
                if (tokens.Font == null) violations.Add("UI002 canonical TMP font is missing.");
                if (tokens.RoundedSprite == null) violations.Add("UI003 rounded nine-slice sprite is missing.");
                if (tokens.MinimumTouchTarget < 64f || tokens.RecommendedTouchTarget < 72f) violations.Add("UI004 child touch target tokens must remain 64/72 or larger.");
                ValidateContrast(tokens, violations);
            }
            if (AssetDatabase.LoadAssetAtPath<GameObject>(UIDesignSystemSetup.GalleryPath) == null) violations.Add("UI005 component gallery prefab is missing.");

            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            UIDesignSystemRoot[] roots = scene.GetRootGameObjects().SelectMany(value => value.GetComponentsInChildren<UIDesignSystemRoot>(true)).ToArray();
            if (roots.Length != 9) violations.Add($"UI006 expected 9 themed critical roots; found {roots.Length}: {string.Join(",", roots.Select(value => value.gameObject.name))}.");
            Canvas.ForceUpdateCanvases();
            foreach (Button button in scene.GetRootGameObjects().SelectMany(value => value.GetComponentsInChildren<Button>(true)))
            {
                if (button.GetComponentInParent<UIDesignSystemRoot>() == null) continue;
                RectTransform rect = (RectTransform)button.transform;
                if (rect.rect.width < 64f || rect.rect.height < 64f)
                    violations.Add("UI007 touch target below 64 logical units: " + HierarchyPath(button.transform));
                if (button.GetComponent<UIThemedButton>() == null) violations.Add("UI008 unthemed critical button: " + HierarchyPath(button.transform));
            }
            foreach (CanvasScaler scaler in roots.SelectMany(value => value.GetComponentsInChildren<CanvasScaler>(true)))
            {
                if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize || scaler.referenceResolution != new Vector2(1280f, 720f))
                    violations.Add("UI009 critical canvas must use 1280x720 ScaleWithScreenSize: " + HierarchyPath(scaler.transform));
            }
            return violations.Distinct().ToArray();
        }

        private static void ValidateContrast(UIDesignTokens tokens, ICollection<string> violations)
        {
            if (Contrast(tokens.Color(UIColorRole.Ink), tokens.Color(UIColorRole.Paper)) < 4.5f) violations.Add("UI010 ink/paper contrast must be at least 4.5:1.");
            if (Contrast(tokens.Color(UIColorRole.OnDark), tokens.Color(UIColorRole.Surface)) < 4.5f) violations.Add("UI011 on-dark/surface contrast must be at least 4.5:1.");
        }

        private static float Contrast(Color first, Color second)
        {
            float a = Luminance(first); float b = Luminance(second);
            return (Mathf.Max(a, b) + 0.05f) / (Mathf.Min(a, b) + 0.05f);
        }

        private static float Luminance(Color color) => 0.2126f * Linear(color.r) + 0.7152f * Linear(color.g) + 0.0722f * Linear(color.b);
        private static float Linear(float value) => value <= 0.03928f ? value / 12.92f : Mathf.Pow((value + 0.055f) / 1.055f, 2.4f);

        private static string HierarchyPath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null) { transform = transform.parent; path = transform.name + "/" + path; }
            return path;
        }
    }
}
