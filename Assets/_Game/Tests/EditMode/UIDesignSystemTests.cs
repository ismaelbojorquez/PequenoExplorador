using System.Linq;
using NUnit.Framework;
using PequenoExplorador.DesignSystem;
using PequenoExplorador.Editor;
using PequenoExplorador.Editor.BuildTools;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class UIDesignSystemTests
    {
        [Test]
        public void CanonicalTokensGalleryAndCriticalSceneAreValid()
        {
            Assert.That(UIDesignSystemValidationService.Validate(), Is.Empty);
            UIDesignTokens tokens = AssetDatabase.LoadAssetAtPath<UIDesignTokens>(UIDesignSystemSetup.TokenPath);
            Assert.That(tokens, Is.Not.Null);
            Assert.That(tokens.Font, Is.Not.Null);
            Assert.That(tokens.RoundedSprite, Is.Not.Null);
            Assert.That(tokens.MinimumTouchTarget, Is.GreaterThanOrEqualTo(64f));
            Assert.That(tokens.RecommendedTouchTarget, Is.GreaterThanOrEqualTo(72f));
            GameObject gallery = AssetDatabase.LoadAssetAtPath<GameObject>(UIDesignSystemSetup.GalleryPath);
            Assert.That(gallery.GetComponentsInChildren<UIStateView>(true).Select(value => value.Kind),
                Is.EquivalentTo(new[] { UIStateKind.Empty, UIStateKind.Success }));
            Assert.That(gallery.GetComponentsInChildren<UIIconGraphic>(true).Length, Is.GreaterThanOrEqualTo(3));
        }

        [Test]
        public void AccessibilityAppliesLargeTextAndReduceMotionWithoutGlobalState()
        {
            UIDesignTokens tokens = AssetDatabase.LoadAssetAtPath<UIDesignTokens>(UIDesignSystemSetup.TokenPath);
            var root = new GameObject("Test UI", typeof(RectTransform), typeof(UIDesignSystemRoot));
            var labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(UIThemedText));
            labelObject.transform.SetParent(root.transform, false);
            UIThemedText themed = labelObject.GetComponent<UIThemedText>();
            themed.ConfigureForEditor(UITypographyRole.Body, UIColorRole.OnDark, false);
            UIDesignSystemRoot designRoot = root.GetComponent<UIDesignSystemRoot>();
            designRoot.ConfigureForEditor(tokens);
            designRoot.SetAccessibility(1.25f, true);
            Assert.That(labelObject.GetComponent<TMP_Text>().fontSize, Is.EqualTo(tokens.FontSize(UITypographyRole.Body) * 1.25f));
            Assert.That(designRoot.ReduceMotion, Is.True);
            Object.DestroyImmediate(root);
        }

        [Test]
        public void ChildButtonCannotThemeBelowMinimumTarget()
        {
            UIDesignTokens tokens = AssetDatabase.LoadAssetAtPath<UIDesignTokens>(UIDesignSystemSetup.TokenPath);
            var value = new GameObject("Small Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(UIThemedButton));
            RectTransform rect = (RectTransform)value.transform; rect.sizeDelta = new Vector2(24f, 24f);
            UIThemedButton themed = value.GetComponent<UIThemedButton>(); themed.ConfigureForEditor(UIButtonStyle.Primary, false);
            themed.ApplyTheme(tokens, 1f, false);
            Assert.That(rect.sizeDelta.x, Is.GreaterThanOrEqualTo(64f));
            Assert.That(rect.sizeDelta.y, Is.GreaterThanOrEqualTo(64f));
            Object.DestroyImmediate(value);
        }
    }
}
