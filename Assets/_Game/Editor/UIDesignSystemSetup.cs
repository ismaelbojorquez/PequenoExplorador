using System;
using System.IO;
using System.Linq;
using PequenoExplorador.DesignSystem;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class UIDesignSystemSetup
    {
        public const string TokenPath = "Assets/_Game/Content/UI/PH_UI_DesignTokens.asset";
        public const string FontPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";
        public const string RoundedTexturePath = "Assets/_Game/Content/UI/Sprites/PH_RoundedRect.png";
        public const string GalleryPath = "Assets/_Game/Content/UI/PH_UI_ComponentGallery.prefab";

        private static readonly string[] CriticalRoots =
        {
            "Diagnostic Canvas",
            "PH_UI_SCENE_FLOW",
            "PH_UI_CAMP_HUB",
            "PH_UI_PHOTOGRAPHY",
            "PH_UI_ALBUM",
            "PH_UI_LEARNING",
            "PH_UI_MISSIONS",
            "PH_UI_CUSTOMIZATION",
            "PH_UI_TUTORIAL"
        };

        [MenuItem("Pequeño Explorador/Setup/27 Apply UI Design System")]
        public static void Apply()
        {
            try
            {
                EnsureFolders();
                TMP_FontAsset font = EnsureFont();
                Sprite rounded = EnsureRoundedSprite();
                UIDesignTokens tokens = EnsureTokens(font, rounded);
                CreateGallery(tokens);
                Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
                ApplyToScene(scene, tokens);
                EditorSceneManager.SaveScene(scene);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("PE_UI_DESIGN_SYSTEM_SETUP_OK roots=9 gallery=1 tmp=canonical legacyBridge=true");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        public static void ApplyToScene(Scene scene, UIDesignTokens tokens)
        {
            foreach (string rootName in CriticalRoots)
            {
                GameObject root = scene.GetRootGameObjects().FirstOrDefault(value => value.name == rootName);
                if (root == null) throw new InvalidOperationException("Critical UI root is missing: " + rootName);
                ApplyRoot(root, tokens);
            }
            foreach (string rootName in CriticalRoots)
            {
                GameObject root = scene.GetRootGameObjects().First(value => value.name == rootName);
                UIDesignSystemRoot designRoot = root.GetComponent<UIDesignSystemRoot>() ?? root.AddComponent<UIDesignSystemRoot>();
                designRoot.ConfigureForEditor(tokens);
                foreach (Button button in root.GetComponentsInChildren<Button>(true))
                {
                    UIThemedButton themed = button.GetComponent<UIThemedButton>() ?? button.gameObject.AddComponent<UIThemedButton>();
                    UIButtonStyle style = ButtonStyleFor(button.name);
                    themed.ConfigureForEditor(style);
                    foreach (UIThemedText label in button.GetComponentsInChildren<UIThemedText>(true))
                        label.ConfigureForEditor(UITypographyRole.Label, style == UIButtonStyle.Destructive ? UIColorRole.OnDark : UIColorRole.Ink, true);
                    if (button.GetComponent<UICancelableMotion>() == null) button.gameObject.AddComponent<UICancelableMotion>();
                }
                designRoot.Apply();
            }
            EditorSceneManager.MarkSceneDirty(scene);
        }

        private static void ApplyRoot(GameObject root, UIDesignTokens tokens)
        {
            foreach (Transform value in root.GetComponentsInChildren<Transform>(true))
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(value.gameObject);
                RemoveDuplicates<UIDesignSystemRoot>(value.gameObject);
                RemoveDuplicates<UIThemedText>(value.gameObject);
                RemoveDuplicates<UIThemedPanel>(value.gameObject);
                RemoveDuplicates<UIThemedButton>(value.gameObject);
                RemoveDuplicates<UICancelableMotion>(value.gameObject);
                RemoveDuplicates<UIIconGraphic>(value.gameObject);
            }
            UIDesignSystemRoot systemRoot = root.GetComponent<UIDesignSystemRoot>() ?? root.AddComponent<UIDesignSystemRoot>();
            systemRoot.ConfigureForEditor(tokens);

            CanvasScaler scaler = root.GetComponentInChildren<CanvasScaler>(true);
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1280f, 720f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            foreach (Text legacy in root.GetComponentsInChildren<Text>(true))
            {
                UIThemedText themed = legacy.GetComponent<UIThemedText>() ?? legacy.gameObject.AddComponent<UIThemedText>();
                ConfigureTypography(themed, legacy.name, legacy.fontSize, IsOnPaper(legacy.transform));
            }
            foreach (TMP_Text tmp in root.GetComponentsInChildren<TMP_Text>(true))
            {
                UIThemedText themed = tmp.GetComponent<UIThemedText>() ?? tmp.gameObject.AddComponent<UIThemedText>();
                ConfigureTypography(themed, tmp.name, Mathf.RoundToInt(tmp.fontSize), IsOnPaper(tmp.transform));
            }

            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                UIThemedButton themed = button.GetComponent<UIThemedButton>() ?? button.gameObject.AddComponent<UIThemedButton>();
                UIButtonStyle style = ButtonStyleFor(button.name);
                themed.ConfigureForEditor(style);
                foreach (UIThemedText label in button.GetComponentsInChildren<UIThemedText>(true))
                    label.ConfigureForEditor(UITypographyRole.Label, style == UIButtonStyle.Destructive ? UIColorRole.OnDark : UIColorRole.Ink, true);
                if (button.GetComponent<UICancelableMotion>() == null) button.gameObject.AddComponent<UICancelableMotion>();
                EnsureIcon(button, IconFor(button.name), style);
            }

            foreach (Image image in root.GetComponentsInChildren<Image>(true))
            {
                if (image.GetComponent<Button>() != null || image.type == Image.Type.Filled) continue;
                string normalized = image.name.ToLowerInvariant();
                bool panel = normalized.Contains("panel") || normalized.Contains("card") || normalized.Contains("preview") ||
                    normalized.Contains("hud") || normalized.Contains("state") || normalized.Contains("cell");
                if (!panel) continue;
                UIThemedPanel themed = image.GetComponent<UIThemedPanel>() ?? image.gameObject.AddComponent<UIThemedPanel>();
                bool paper = normalized.Contains("card") || normalized.Contains("preview") || normalized.Contains("detail");
                themed.ConfigureForEditor(paper ? UIColorRole.Paper : UIColorRole.Surface, paper, paper);
            }

            ApplyCriticalLayout(root);
            systemRoot.Apply();
        }

        private static void ApplyCriticalLayout(GameObject root)
        {
            if (root.name == "PH_UI_LEARNING")
            {
                SetAnchors(Find(root.transform, "PH_LEARNING_PANEL") as RectTransform,
                    new Vector2(0.15f, 0.10f), new Vector2(0.85f, 0.88f));
                SetButton(Find(root.transform, "Option 1") as RectTransform, new Vector2(0.18f, 0.53f), new Vector2(210f, 120f));
                SetButton(Find(root.transform, "Option 2") as RectTransform, new Vector2(0.50f, 0.53f), new Vector2(210f, 120f));
                SetButton(Find(root.transform, "Option 3") as RectTransform, new Vector2(0.82f, 0.53f), new Vector2(210f, 120f));
                SetButton(Find(root.transform, "Hint") as RectTransform, new Vector2(0.20f, 0.10f), new Vector2(180f, 76f));
                SetButton(Find(root.transform, "Replay") as RectTransform, new Vector2(0.50f, 0.10f), new Vector2(200f, 76f));
                SetButton(Find(root.transform, "Exit") as RectTransform, new Vector2(0.80f, 0.10f), new Vector2(180f, 76f));
            }
            else if (root.name == "PH_UI_CUSTOMIZATION")
            {
                for (int index = 0; index < 4; index++)
                {
                    Transform option = Find(root.transform, "Option " + index);
                    Text label = option == null ? null : option.GetComponentInChildren<Text>(true);
                    if (label == null) continue;
                    SetAnchors(label.rectTransform, new Vector2(0.06f, 0.03f), new Vector2(0.94f, 0.44f));
                    label.resizeTextForBestFit = true;
                    label.resizeTextMinSize = 16;
                    label.resizeTextMaxSize = 24;
                }
                SetButtonAnchoredLabel(root.transform, "Unlock");
                SetButtonAnchoredLabel(root.transform, "Equip");
                SetButtonAnchoredLabel(root.transform, "Close");
            }
        }

        private static void SetButtonAnchoredLabel(Transform root, string name)
        {
            Transform button = Find(root, name);
            Text label = button == null ? null : button.GetComponentInChildren<Text>(true);
            if (label == null) return;
            SetAnchors(label.rectTransform, new Vector2(0.18f, 0.08f), new Vector2(0.96f, 0.92f));
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 16;
            label.resizeTextMaxSize = 24;
        }

        private static void SetButton(RectTransform rect, Vector2 anchor, Vector2 size)
        {
            if (rect == null) return;
            rect.anchorMin = rect.anchorMax = anchor;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
        }

        private static void SetAnchors(RectTransform rect, Vector2 min, Vector2 max)
        {
            if (rect == null) return;
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        private static Transform Find(Transform root, string name)
        {
            foreach (Transform value in root.GetComponentsInChildren<Transform>(true))
                if (value.name == name) return value;
            return null;
        }

        private static void RemoveDuplicates<T>(GameObject value) where T : Component
        {
            T[] components = value.GetComponents<T>();
            for (int index = components.Length - 1; index > 0; index--)
                UnityEngine.Object.DestroyImmediate(components[index]);
        }

        private static bool IsOnPaper(Transform value)
        {
            for (Transform current = value.parent; current != null; current = current.parent)
            {
                string name = current.name.ToLowerInvariant();
                if (name.Contains("card") || name.Contains("preview") || name.Contains("detail")) return true;
            }
            return false;
        }

        private static void ConfigureTypography(UIThemedText themed, string name, int size, bool onPaper)
        {
            string normalized = name.ToLowerInvariant();
            UITypographyRole role = size >= 44 || normalized.Contains("product") ? UITypographyRole.Display :
                size >= 36 || normalized.Contains("title") ? UITypographyRole.Headline :
                size >= 29 || normalized.Contains("name") ? UITypographyRole.Title :
                size <= 20 ? UITypographyRole.Caption : normalized.Contains("button") || normalized.Contains("label") ? UITypographyRole.Label : UITypographyRole.Body;
            bool bold = role == UITypographyRole.Display || role == UITypographyRole.Headline || role == UITypographyRole.Title || role == UITypographyRole.Label;
            themed.ConfigureForEditor(role, onPaper ? UIColorRole.Ink : UIColorRole.OnDark, bold);
        }

        private static UIButtonStyle ButtonStyleFor(string name)
        {
            string normalized = name.ToLowerInvariant();
            if (normalized.Contains("station expedition")) return UIButtonStyle.Primary;
            if (normalized.Contains("station customization") || normalized.StartsWith("option")) return UIButtonStyle.Positive;
            if (normalized.Contains("station parents") || normalized.StartsWith("slot")) return UIButtonStyle.Quiet;
            if (normalized.Contains("back") || normalized.Contains("close") || normalized.Contains("cancel") || normalized.Contains("exit")) return UIButtonStyle.Quiet;
            if (normalized.Contains("confirm") || normalized.Contains("shutter") || normalized.Contains("equip") || normalized.Contains("upgrade")) return UIButtonStyle.Positive;
            if (normalized.Contains("delete") || normalized.Contains("reset")) return UIButtonStyle.Destructive;
            if (normalized.Contains("next") || normalized.Contains("enter") || normalized.Contains("open")) return UIButtonStyle.Primary;
            return UIButtonStyle.Secondary;
        }

        private static UIIconKind IconFor(string name)
        {
            string normalized = name.ToLowerInvariant();
            if (normalized.Contains("back") || normalized.Contains("close") || normalized.Contains("exit") || normalized.Contains("cancel")) return UIIconKind.Back;
            if (normalized.Contains("expedition") || normalized.Contains("enter")) return UIIconKind.Explore;
            if (normalized.Contains("album")) return UIIconKind.Album;
            if (normalized.Contains("shutter") || normalized.Contains("photo")) return UIIconKind.Camera;
            if (normalized.Contains("replay")) return UIIconKind.Replay;
            if (normalized.Contains("hint")) return UIIconKind.Hint;
            if (normalized.Contains("parents")) return UIIconKind.Parents;
            if (normalized.Contains("customization") || normalized.Contains("equip")) return UIIconKind.Customize;
            if (normalized.Contains("confirm") || normalized.Contains("upgrade") || normalized.Contains("unlock")) return UIIconKind.Check;
            return UIIconKind.None;
        }

        private static void EnsureIcon(Button button, UIIconKind kind, UIButtonStyle style)
        {
            Transform existing = button.transform.Find("PH_UI_ICON");
            if (kind == UIIconKind.None)
            {
                if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);
                return;
            }
            GameObject iconObject = existing == null ? new GameObject("PH_UI_ICON", typeof(RectTransform), typeof(UIIconGraphic)) : existing.gameObject;
            if (existing == null) iconObject.transform.SetParent(button.transform, false);
            RectTransform rect = (RectTransform)iconObject.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.12f, 0.5f); rect.anchoredPosition = Vector2.zero; rect.sizeDelta = new Vector2(34f, 34f);
            UIIconGraphic icon = iconObject.GetComponent<UIIconGraphic>();
            icon.ConfigureForEditor(kind, style == UIButtonStyle.Destructive ? UIColorRole.OnDark : UIColorRole.Ink);
            icon.raycastTarget = false;
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets/_Game/Content", "UI");
            EnsureFolder("Assets/_Game/Content/UI", "Fonts");
            EnsureFolder("Assets/_Game/Content/UI", "Sprites");
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, child);
        }

        private static TMP_FontAsset EnsureFont()
        {
            TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
            return existing != null ? existing : throw new InvalidOperationException(
                "Official versioned TMP Essential Resources are missing; restore Assets/TextMesh Pro before setup.");
        }

        private static Sprite EnsureRoundedSprite()
        {
            Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(RoundedTexturePath);
            if (existing != null) return existing;
            const int size = 64;
            const int radius = 18;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false, false);
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                int dx = x < radius ? radius - x : x >= size - radius ? x - (size - radius - 1) : 0;
                int dy = y < radius ? radius - y : y >= size - radius ? y - (size - radius - 1) : 0;
                bool inside = dx == 0 || dy == 0 || dx * dx + dy * dy <= radius * radius;
                texture.SetPixel(x, y, inside ? Color.white : Color.clear);
            }
            texture.Apply(false, false);
            File.WriteAllBytes(RoundedTexturePath, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(RoundedTexturePath, ImportAssetOptions.ForceSynchronousImport);
            var importer = (TextureImporter)AssetImporter.GetAtPath(RoundedTexturePath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.spriteBorder = new Vector4(radius, radius, radius, radius);
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(RoundedTexturePath);
        }

        private static UIDesignTokens EnsureTokens(TMP_FontAsset font, Sprite rounded)
        {
            UIDesignTokens tokens = AssetDatabase.LoadAssetAtPath<UIDesignTokens>(TokenPath);
            if (tokens == null)
            {
                tokens = ScriptableObject.CreateInstance<UIDesignTokens>();
                AssetDatabase.CreateAsset(tokens, TokenPath);
            }
            tokens.ConfigureForEditor(font, rounded);
            EditorUtility.SetDirty(tokens);
            return tokens;
        }

        private static void CreateGallery(UIDesignTokens tokens)
        {
            var root = new GameObject("PH_UI_ComponentGallery", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(UIDesignSystemRoot));
            Canvas canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = root.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = 0.5f;
            UIDesignSystemRoot designRoot = root.GetComponent<UIDesignSystemRoot>(); designRoot.ConfigureForEditor(tokens);

            CreatePanel(root.transform, "PH_GallerySurface", new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.94f), false, tokens);
            CreateText(root.transform, "PH_GalleryTitle", "Kit de expedición", 44, new Vector2(0.08f, 0.79f), new Vector2(0.92f, 0.91f), UITypographyRole.Display, UIColorRole.OnDark, tokens);
            CreateButton(root.transform, "PH_PrimaryButton", "Explorar", new Vector2(0.23f, 0.61f), UIButtonStyle.Primary, tokens);
            CreateButton(root.transform, "PH_SecondaryButton", "Mi álbum", new Vector2(0.50f, 0.61f), UIButtonStyle.Secondary, tokens);
            CreateButton(root.transform, "PH_QuietButton", "Volver", new Vector2(0.77f, 0.61f), UIButtonStyle.Quiet, tokens);
            CreateStateCard(root.transform, "PH_EmptyState", "Aún no hay fotos", "Explora la Selva para encontrar una.", new Vector2(0.28f, 0.29f), UIStateKind.Empty, tokens);
            CreateStateCard(root.transform, "PH_SuccessState", "¡Descubrimiento!", "Tu álbum tiene una nueva ficha.", new Vector2(0.72f, 0.29f), UIStateKind.Success, tokens);
            designRoot.Apply();
            PrefabUtility.SaveAsPrefabAsset(root, GalleryPath);
            UnityEngine.Object.DestroyImmediate(root);
        }

        private static void CreatePanel(Transform parent, string name, Vector2 min, Vector2 max, bool paper, UIDesignTokens tokens)
        {
            var value = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(UIThemedPanel)); value.transform.SetParent(parent, false);
            RectTransform rect = (RectTransform)value.transform; rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero;
            value.GetComponent<UIThemedPanel>().ConfigureForEditor(paper ? UIColorRole.Paper : UIColorRole.Surface, paper, paper);
        }

        private static TMP_Text CreateText(Transform parent, string name, string copy, float size, Vector2 min, Vector2 max, UITypographyRole role, UIColorRole color, UIDesignTokens tokens)
        {
            var value = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI), typeof(UIThemedText)); value.transform.SetParent(parent, false);
            RectTransform rect = (RectTransform)value.transform; rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero;
            TMP_Text text = value.GetComponent<TMP_Text>(); text.text = copy; text.fontSize = size; text.alignment = TextAlignmentOptions.Center; text.enableWordWrapping = true;
            value.GetComponent<UIThemedText>().ConfigureForEditor(role, color, role != UITypographyRole.Body && role != UITypographyRole.Caption);
            return text;
        }

        private static void CreateButton(Transform parent, string name, string copy, Vector2 anchor, UIButtonStyle style, UIDesignTokens tokens)
        {
            var value = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(UIThemedButton), typeof(UICancelableMotion)); value.transform.SetParent(parent, false);
            RectTransform rect = (RectTransform)value.transform; rect.anchorMin = rect.anchorMax = anchor; rect.sizeDelta = new Vector2(260, 76);
            value.GetComponent<UIThemedButton>().ConfigureForEditor(style);
            CreateText(value.transform, "Label", copy, 24, Vector2.zero, Vector2.one, UITypographyRole.Label, UIColorRole.Ink, tokens);
            EnsureIcon(value.GetComponent<Button>(), style == UIButtonStyle.Primary ? UIIconKind.Explore : style == UIButtonStyle.Secondary ? UIIconKind.Album : UIIconKind.Back, style);
        }

        private static void CreateStateCard(Transform parent, string name, string titleCopy, string bodyCopy, Vector2 anchor, UIStateKind kind, UIDesignTokens tokens)
        {
            var value = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(UIThemedPanel), typeof(UIStateView)); value.transform.SetParent(parent, false);
            RectTransform rect = (RectTransform)value.transform; rect.anchorMin = rect.anchorMax = anchor; rect.sizeDelta = new Vector2(470, 230);
            value.GetComponent<UIThemedPanel>().ConfigureForEditor(UIColorRole.Paper, true, true);
            var symbol = new GameObject("Symbol", typeof(RectTransform), typeof(Image)); symbol.transform.SetParent(value.transform, false); RectTransform symbolRect = (RectTransform)symbol.transform; symbolRect.anchorMin = symbolRect.anchorMax = new Vector2(0.12f, 0.72f); symbolRect.sizeDelta = new Vector2(48, 48); symbol.GetComponent<Image>().sprite = tokens.RoundedSprite;
            TMP_Text title = CreateText(value.transform, "Title", titleCopy, 30, new Vector2(0.20f, 0.61f), new Vector2(0.94f, 0.86f), UITypographyRole.Title, UIColorRole.Ink, tokens);
            TMP_Text body = CreateText(value.transform, "Body", bodyCopy, 23, new Vector2(0.08f, 0.13f), new Vector2(0.92f, 0.58f), UITypographyRole.Body, UIColorRole.MutedInk, tokens);
            value.GetComponent<UIStateView>().ConfigureForEditor(symbol.GetComponent<Image>(), title, body, null, kind);
        }
    }
}
