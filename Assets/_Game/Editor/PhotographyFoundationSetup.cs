using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Visuals;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Photography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class PhotographyFoundationSetup
    {
        public const string DiscoveryId = "discovery.jungle.keel-billed-toucan";
        [MenuItem("Pequeño Explorador/Development/Photography/Apply Foundation")]
        public static void Apply()
        {
            try
            {
                LocalizationFoundationSetup.ApplyPhotographyEntries();
                ConfigureToucanPrefab();
                ConfigureJungle();
                ConfigureBootstrap();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("PE_PHOTOGRAPHY_SETUP_OK target=toucan thumbnail=384x216 cameraPermission=false");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        private static void ConfigureToucanPrefab()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(ToucanFixtureSetup.PrefabPath);
            try
            {
                ToucanReviewFixtureMetadata metadata = root.GetComponent<ToucanReviewFixtureMetadata>();
                if (metadata == null) throw new InvalidOperationException("Toucan reviewed metadata is missing.");
                PhotographableView view = root.GetComponent<PhotographableView>() ?? root.AddComponent<PhotographableView>();
                Transform anchor = root.transform.Find("VS_PhotoAnchor");
                if (anchor == null) throw new InvalidOperationException("Toucan photo anchor is missing.");
                var serialized = new SerializedObject(view);
                serialized.FindProperty("_discoveryId").stringValue = DiscoveryId;
                serialized.FindProperty("_photoAnchor").objectReferenceValue = anchor;
                serialized.FindProperty("_facingTransform").objectReferenceValue = metadata.VisualRoot;
                serialized.FindProperty("_candidateLocalBounds").boundsValue = metadata.CandidatePhotoBounds;
                serialized.FindProperty("_minimumCoverage").floatValue = 0.08f;
                serialized.FindProperty("_maximumDistance").floatValue = 10f;
                serialized.FindProperty("_maximumCenterOffset").floatValue = 0.36f;
                serialized.FindProperty("_minimumOrientationAlignment").floatValue = 0.35f;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(root, ToucanFixtureSetup.PrefabPath);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }

        private static void ConfigureJungle()
        {
            Scene scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.JungleScenePath, OpenSceneMode.Single);
            RemoveRoot(scene, PhotographySceneRoot.RuntimeRootName);
            PhotographableView target = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<PhotographableView>(true))
                .Single(item => item.RawDiscoveryId == DiscoveryId);
            var rootObject = new GameObject(PhotographySceneRoot.RuntimeRootName);
            PhotographySceneRoot root = rootObject.AddComponent<PhotographySceneRoot>();
            var serialized = new SerializedObject(root);
            SerializedProperty targets = serialized.FindProperty("_targets");
            targets.arraySize = 0;
            serialized.FindProperty("_thumbnailWidth").intValue = 384;
            serialized.FindProperty("_thumbnailHeight").intValue = 216;
            serialized.FindProperty("_thumbnailFormat").intValue = (int)RenderTextureFormat.ARGB32;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureBootstrap()
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            RemoveRoot(scene, PhotographyView.PlaceholderObjectName);
            var canvasObject = new GameObject(PhotographyView.PlaceholderObjectName, typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(PhotographyView));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 180;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var safeObject = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safeObject.transform.SetParent(canvasObject.transform, false);
            Stretch((RectTransform)safeObject.transform);
            GameObject panel = CreatePanel(safeObject.transform, "PH_PHOTOGRAPHY_PANEL", new Color(0f, 0f, 0f, 0.10f));

            Image reticle = CreateImage(panel.transform, "PH_ASSIST_RETICLE", new Color(1f, 0.78f, 0.22f, 0.32f));
            RectTransform reticleRect = (RectTransform)reticle.transform;
            reticleRect.anchorMin = reticleRect.anchorMax = new Vector2(0.5f, 0.53f);
            reticleRect.sizeDelta = new Vector2(620f, 390f);
            reticleRect.anchoredPosition = Vector2.zero;
            reticle.raycastTarget = false;

            Text guidance = CreateText(panel.transform, "PH_PHOTOGRAPHY_GUIDANCE", 42, TextAnchor.MiddleCenter);
            SetRect(guidance.rectTransform, new Vector2(0.25f, 0.86f), new Vector2(0.75f, 0.97f));
            Button shutter = CreateButton(panel.transform, "PH_PHOTOGRAPHY_SHUTTER");
            RectTransform shutterRect = (RectTransform)shutter.transform;
            shutterRect.anchorMin = shutterRect.anchorMax = new Vector2(0.88f, 0.14f);
            shutterRect.sizeDelta = new Vector2(170f, 170f);
            shutterRect.anchoredPosition = Vector2.zero;
            Button exit = CreateButton(panel.transform, "PH_PHOTOGRAPHY_EXIT");
            RectTransform exitRect = (RectTransform)exit.transform;
            exitRect.anchorMin = exitRect.anchorMax = new Vector2(0.09f, 0.91f);
            exitRect.sizeDelta = new Vector2(180f, 110f);
            exitRect.anchoredPosition = Vector2.zero;

            GameObject card = CreatePanel(panel.transform, "PH_DISCOVERY_CARD", new Color(0.03f, 0.18f, 0.18f, 0.94f));
            SetRect((RectTransform)card.transform, new Vector2(0.27f, 0.04f), new Vector2(0.73f, 0.18f));
            Text cardText = CreateText(card.transform, "PH_DISCOVERY_CARD_TEXT", 36, TextAnchor.MiddleCenter);
            Stretch(cardText.rectTransform);
            Image flash = CreateImage(panel.transform, "PH_PHOTOGRAPHY_FLASH", new Color(1f, 1f, 1f, 0.18f));
            Stretch((RectTransform)flash.transform);
            flash.raycastTarget = false;
            flash.gameObject.SetActive(false);

            PhotographyView view = canvasObject.GetComponent<PhotographyView>();
            var viewSerialized = new SerializedObject(view);
            viewSerialized.FindProperty("_panel").objectReferenceValue = panel;
            viewSerialized.FindProperty("_reticle").objectReferenceValue = reticle;
            viewSerialized.FindProperty("_guidance").objectReferenceValue = guidance;
            viewSerialized.FindProperty("_shutter").objectReferenceValue = shutter;
            viewSerialized.FindProperty("_exit").objectReferenceValue = exit;
            viewSerialized.FindProperty("_card").objectReferenceValue = card;
            viewSerialized.FindProperty("_cardText").objectReferenceValue = cardText;
            viewSerialized.FindProperty("_flash").objectReferenceValue = flash;
            viewSerialized.ApplyModifiedPropertiesWithoutUndo();
            panel.SetActive(false);

            DiagnosticBootstrap bootstrap = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<DiagnosticBootstrap>(true)).Single();
            bootstrap.ConfigurePhotographyForEditorAndTests(view);
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
            for (int index = 0; index < existing.Count; index++) fitters.GetArrayElementAtIndex(index).objectReferenceValue = existing[index];
            bootstrapSerialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(bootstrap);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
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
            result.transform.SetParent(parent, false); Image image = result.GetComponent<Image>(); image.color = color; return image;
        }
        private static Text CreateText(Transform parent, string name, int size, TextAnchor alignment)
        {
            var result = new GameObject(name, typeof(RectTransform), typeof(Text));
            result.transform.SetParent(parent, false); Text text = result.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.fontSize = size;
            text.resizeTextForBestFit = true; text.resizeTextMinSize = 18; text.alignment = alignment;
            text.color = Color.white; text.raycastTarget = false; return text;
        }
        private static Button CreateButton(Transform parent, string name)
        {
            GameObject result = DefaultControls.CreateButton(new DefaultControls.Resources());
            result.name = name; result.transform.SetParent(parent, false);
            Text label = result.GetComponentInChildren<Text>(); label.text = string.Empty; label.fontSize = 30;
            label.resizeTextForBestFit = true; return result.GetComponent<Button>();
        }
        private static void Stretch(RectTransform rect) => SetRect(rect, Vector2.zero, Vector2.one);
        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max)
        { rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero; }
        private static void RemoveRoot(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects()) if (root.name == name) UnityEngine.Object.DestroyImmediate(root);
        }
    }
}
