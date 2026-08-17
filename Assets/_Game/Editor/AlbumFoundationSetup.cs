using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Album;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class AlbumFoundationSetup
    {
        [MenuItem("Pequeño Explorador/Development/Album/Apply Foundation")]
        public static void Apply()
        {
            try
            {
                LocalizationFoundationSetup.ApplyAlbumEntries();
                ContentFoundationSetup.ApplyAssetsAndBootstrap();
                ConfigureBootstrap();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("PE_ALBUM_SETUP_OK world=world.jungle cells=8 categories=4 safeArea=true");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        private static void ConfigureBootstrap()
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            RemoveRoot(scene, AlbumView.PlaceholderObjectName);
            var canvasObject = new GameObject(AlbumView.PlaceholderObjectName, typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(AlbumView));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 140;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var safeObject = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safeObject.transform.SetParent(canvasObject.transform, false);
            Stretch((RectTransform)safeObject.transform);

            Button open = CreateButton(safeObject.transform, "PH_ALBUM_OPEN", new Color(0.10f, 0.44f, 0.35f, 1f), 34);
            Place((RectTransform)open.transform, new Vector2(0.76f, 0.08f), new Vector2(270f, 110f));

            GameObject panel = CreatePanel(safeObject.transform, "PH_ALBUM_PANEL", new Color(0.035f, 0.105f, 0.12f, 0.985f));
            Button back = CreateButton(panel.transform, "PH_ALBUM_BACK", new Color(0.15f, 0.27f, 0.30f, 1f), 30);
            Place((RectTransform)back.transform, new Vector2(0.07f, 0.92f), new Vector2(190f, 90f));
            Text title = CreateText(panel.transform, "PH_ALBUM_TITLE", 48, TextAnchor.MiddleCenter);
            SetRect(title.rectTransform, new Vector2(0.25f, 0.88f), new Vector2(0.75f, 0.98f));
            Text progress = CreateText(panel.transform, "PH_ALBUM_PROGRESS", 30, TextAnchor.MiddleRight);
            SetRect(progress.rectTransform, new Vector2(0.72f, 0.88f), new Vector2(0.96f, 0.98f));

            GameObject categoryRoot = CreatePanel(panel.transform, "PH_ALBUM_CATEGORIES", new Color(0.055f, 0.15f, 0.17f, 1f));
            SetRect((RectTransform)categoryRoot.transform, new Vector2(0.04f, 0.73f), new Vector2(0.96f, 0.86f));
            var categoryLayout = categoryRoot.AddComponent<HorizontalLayoutGroup>();
            categoryLayout.padding = new RectOffset(20, 20, 16, 16);
            categoryLayout.spacing = 16f;
            categoryLayout.childAlignment = TextAnchor.MiddleLeft;
            categoryLayout.childForceExpandWidth = false;
            AlbumCategoryCell[] categoryCells = Enumerable.Range(0, 4)
                .Select(index => CreateCategoryCell(categoryRoot.transform, index)).ToArray();

            GameObject gridRoot = CreatePanel(panel.transform, "PH_ALBUM_GRID", new Color(0.025f, 0.085f, 0.095f, 1f));
            SetRect((RectTransform)gridRoot.transform, new Vector2(0.05f, 0.16f), new Vector2(0.95f, 0.71f));
            var grid = gridRoot.AddComponent<GridLayoutGroup>();
            grid.padding = new RectOffset(24, 24, 24, 24);
            grid.spacing = new Vector2(22f, 18f);
            grid.cellSize = new Vector2(380f, 230f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;
            grid.childAlignment = TextAnchor.MiddleCenter;
            AlbumEntryCell[] entries = Enumerable.Range(0, AlbumView.EntriesPerPage)
                .Select(index => CreateEntryCell(gridRoot.transform, index)).ToArray();

            Button previous = CreateButton(panel.transform, "PH_ALBUM_PREVIOUS", new Color(0.15f, 0.27f, 0.30f, 1f), 26);
            Place((RectTransform)previous.transform, new Vector2(0.36f, 0.08f), new Vector2(190f, 80f));
            Button next = CreateButton(panel.transform, "PH_ALBUM_NEXT", new Color(0.15f, 0.27f, 0.30f, 1f), 26);
            Place((RectTransform)next.transform, new Vector2(0.64f, 0.08f), new Vector2(190f, 80f));
            Text page = CreateText(panel.transform, "PH_ALBUM_PAGE", 26, TextAnchor.MiddleCenter);
            SetRect(page.rectTransform, new Vector2(0.43f, 0.035f), new Vector2(0.57f, 0.125f));

            GameObject loading = CreateState(panel.transform, "PH_ALBUM_LOADING");
            GameObject empty = CreateState(panel.transform, "PH_ALBUM_EMPTY");
            GameObject error = CreateState(panel.transform, "PH_ALBUM_ERROR");
            Text stateText = CreateText(panel.transform, "PH_ALBUM_STATE_TEXT", 34, TextAnchor.MiddleCenter);
            SetRect(stateText.rectTransform, new Vector2(0.22f, 0.38f), new Vector2(0.78f, 0.58f));
            stateText.gameObject.SetActive(true);

            GameObject detail = CreatePanel(panel.transform, "PH_ALBUM_DETAIL", new Color(0.035f, 0.105f, 0.12f, 1f));
            Button detailBack = CreateButton(detail.transform, "PH_ALBUM_DETAIL_BACK", new Color(0.15f, 0.27f, 0.30f, 1f), 30);
            Place((RectTransform)detailBack.transform, new Vector2(0.07f, 0.92f), new Vector2(190f, 90f));
            Text detailName = CreateText(detail.transform, "PH_ALBUM_DETAIL_NAME", 48, TextAnchor.MiddleCenter);
            SetRect(detailName.rectTransform, new Vector2(0.25f, 0.87f), new Vector2(0.75f, 0.98f));
            Image detailImage = CreateImage(detail.transform, "PH_ALBUM_DETAIL_IMAGE", new Color(0.22f, 0.55f, 0.43f, 1f));
            SetRect((RectTransform)detailImage.transform, new Vector2(0.055f, 0.22f), new Vector2(0.43f, 0.80f));
            Text photoState = CreateText(detail.transform, "PH_ALBUM_PHOTO_STATE", 26, TextAnchor.MiddleCenter);
            SetRect(photoState.rectTransform, new Vector2(0.08f, 0.10f), new Vector2(0.40f, 0.20f));

            var factLabels = new List<Text>();
            var factValues = new List<Text>();
            for (int index = 0; index < 5; index++)
            {
                float top = 0.80f - index * 0.125f;
                Text label = CreateText(detail.transform, "PH_ALBUM_FACT_LABEL_" + index, 25, TextAnchor.MiddleLeft);
                SetRect(label.rectTransform, new Vector2(0.48f, top - 0.045f), new Vector2(0.64f, top + 0.045f));
                label.color = new Color(0.61f, 0.91f, 0.76f, 1f);
                Text value = CreateText(detail.transform, "PH_ALBUM_FACT_VALUE_" + index, 27, TextAnchor.MiddleLeft);
                SetRect(value.rectTransform, new Vector2(0.64f, top - 0.055f), new Vector2(0.95f, top + 0.055f));
                factLabels.Add(label); factValues.Add(value);
            }
            Button replay = CreateButton(detail.transform, "PH_ALBUM_REPLAY", new Color(0.10f, 0.44f, 0.35f, 1f), 27);
            Place((RectTransform)replay.transform, new Vector2(0.74f, 0.12f), new Vector2(330f, 96f));
            Text replayLabel = replay.GetComponentInChildren<Text>(true);

            AlbumView view = canvasObject.GetComponent<AlbumView>();
            view.ConfigureForEditorAndTests(
                open, panel, back, title, progress, categoryCells, entries,
                previous, next, page, loading, empty, error, stateText,
                detail, detailBack, detailImage, detailName, factLabels.ToArray(), factValues.ToArray(),
                replay, replayLabel, photoState);
            panel.SetActive(false);
            detail.SetActive(false);
            loading.SetActive(false); empty.SetActive(false); error.SetActive(false);

            DiagnosticBootstrap bootstrap = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<DiagnosticBootstrap>(true)).Single();
            bootstrap.ConfigureAlbumForEditorAndTests(view);
            var bootstrapSerialized = new SerializedObject(bootstrap);
            SerializedProperty fitters = bootstrapSerialized.FindProperty("_safeAreaFitters");
            var existing = new List<SafeAreaFitter>();
            for (int index = 0; index < fitters.arraySize; index++)
            {
                var fitter = fitters.GetArrayElementAtIndex(index).objectReferenceValue as SafeAreaFitter;
                if (fitter != null) existing.Add(fitter);
            }
            existing.Add(safeObject.GetComponent<SafeAreaFitter>());
            fitters.arraySize = existing.Count;
            for (int index = 0; index < existing.Count; index++)
                fitters.GetArrayElementAtIndex(index).objectReferenceValue = existing[index];
            bootstrapSerialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(bootstrap);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static AlbumCategoryCell CreateCategoryCell(Transform parent, int index)
        {
            Button button = CreateButton(parent, "PH_ALBUM_CATEGORY_" + index, new Color(0.16f, 0.26f, 0.29f, 1f), 27);
            ((RectTransform)button.transform).sizeDelta = new Vector2(340f, 90f);
            var layout = button.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = 340f; layout.preferredHeight = 90f;
            AlbumCategoryCell cell = button.gameObject.AddComponent<AlbumCategoryCell>();
            cell.ConfigureForEditorAndTests(button, button.GetComponentInChildren<Text>(true));
            return cell;
        }

        private static AlbumEntryCell CreateEntryCell(Transform parent, int index)
        {
            GameObject root = CreatePanel(parent, "PH_ALBUM_ENTRY_" + index, new Color(0.08f, 0.18f, 0.20f, 1f));
            Button button = root.AddComponent<Button>();
            button.targetGraphic = root.GetComponent<Image>();
            Image image = CreateImage(root.transform, "PH_ALBUM_ENTRY_IMAGE", new Color(0.28f, 0.34f, 0.36f, 1f));
            SetRect((RectTransform)image.transform, new Vector2(0.04f, 0.28f), new Vector2(0.42f, 0.94f));
            Text name = CreateText(root.transform, "PH_ALBUM_ENTRY_NAME", 29, TextAnchor.MiddleLeft);
            SetRect(name.rectTransform, new Vector2(0.46f, 0.47f), new Vector2(0.96f, 0.91f));
            Text state = CreateText(root.transform, "PH_ALBUM_ENTRY_STATE", 22, TextAnchor.MiddleLeft);
            SetRect(state.rectTransform, new Vector2(0.46f, 0.12f), new Vector2(0.96f, 0.46f));
            state.color = new Color(0.70f, 0.90f, 0.83f, 1f);
            AlbumEntryCell cell = root.AddComponent<AlbumEntryCell>();
            cell.ConfigureForEditorAndTests(button, image, name, state);
            return cell;
        }

        private static GameObject CreateState(Transform parent, string name)
        {
            GameObject result = new GameObject(name, typeof(RectTransform));
            result.transform.SetParent(parent, false);
            SetRect((RectTransform)result.transform, new Vector2(0.20f, 0.32f), new Vector2(0.80f, 0.64f));
            return result;
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var result = new GameObject(name, typeof(RectTransform), typeof(Image));
            result.transform.SetParent(parent, false); Stretch((RectTransform)result.transform);
            result.GetComponent<Image>().color = color; return result;
        }

        private static Image CreateImage(Transform parent, string name, Color color)
        {
            var result = new GameObject(name, typeof(RectTransform), typeof(Image));
            result.transform.SetParent(parent, false);
            Image image = result.GetComponent<Image>(); image.color = color; return image;
        }

        private static Text CreateText(Transform parent, string name, int size, TextAnchor alignment)
        {
            var result = new GameObject(name, typeof(RectTransform), typeof(Text));
            result.transform.SetParent(parent, false);
            Text text = result.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size; text.resizeTextForBestFit = true; text.resizeTextMinSize = 17;
            text.alignment = alignment; text.color = Color.white; text.raycastTarget = false;
            return text;
        }

        private static Button CreateButton(Transform parent, string name, Color color, int fontSize)
        {
            GameObject result = DefaultControls.CreateButton(new DefaultControls.Resources());
            result.name = name; result.transform.SetParent(parent, false);
            Image image = result.GetComponent<Image>(); image.color = color;
            Button button = result.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.92f, 1f, 0.96f, 1f);
            colors.pressedColor = new Color(0.75f, 0.90f, 0.82f, 1f);
            colors.disabledColor = new Color(0.48f, 0.52f, 0.52f, 0.65f);
            button.colors = colors;
            Text label = result.GetComponentInChildren<Text>(true);
            label.text = string.Empty; label.fontSize = fontSize; label.resizeTextForBestFit = true;
            return button;
        }

        private static void Place(RectTransform rect, Vector2 anchor, Vector2 size)
        {
            rect.anchorMin = rect.anchorMax = anchor;
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
        }

        private static void Stretch(RectTransform rect) => SetRect(rect, Vector2.zero, Vector2.one);
        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
        }

        private static void RemoveRoot(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
                if (root.name == name) UnityEngine.Object.DestroyImmediate(root);
        }
    }
}
