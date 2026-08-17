using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Economy;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Economy;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class EconomyFoundationSetup
    {
        public const string Root = "Assets/_Game/Content/Economy";
        public const string RewardPath = Root + "/Reward_ToucanFirstDiscovery.asset";
        public const string CatalogPath = Root + "/RewardCatalog.asset";

        [MenuItem("Pequeño Explorador/Development/Economy/Apply Foundation")]
        public static void Apply()
        {
            EnsureFolder("Assets/_Game/Content", "Economy");
            RewardDefinitionAsset reward = AssetDatabase.LoadAssetAtPath<RewardDefinitionAsset>(RewardPath);
            if (reward == null) { reward = ScriptableObject.CreateInstance<RewardDefinitionAsset>(); AssetDatabase.CreateAsset(reward, RewardPath); }
            var rewardSerialized = new SerializedObject(reward);
            rewardSerialized.FindProperty("_id").stringValue = "reward.discovery.keel-billed-toucan.first";
            rewardSerialized.FindProperty("_stars").intValue = 1;
            rewardSerialized.FindProperty("_sourceKind").enumValueIndex = (int)RewardSourceKind.Discovery - 1;
            rewardSerialized.FindProperty("_sourceId").stringValue = PhotographyFoundationSetup.DiscoveryId;
            rewardSerialized.ApplyModifiedPropertiesWithoutUndo();

            RewardCatalogAsset catalog = AssetDatabase.LoadAssetAtPath<RewardCatalogAsset>(CatalogPath);
            if (catalog == null) { catalog = ScriptableObject.CreateInstance<RewardCatalogAsset>(); AssetDatabase.CreateAsset(catalog, CatalogPath); }
            var catalogSerialized = new SerializedObject(catalog);
            SerializedProperty definitions = catalogSerialized.FindProperty("_definitions");
            RewardDefinitionAsset[] retained = catalog.Definitions.Where(item => item != null && item != reward).ToArray();
            definitions.arraySize = 1 + retained.Length; definitions.GetArrayElementAtIndex(0).objectReferenceValue = reward;
            for (int index = 0; index < retained.Length; index++) definitions.GetArrayElementAtIndex(index + 1).objectReferenceValue = retained[index];
            catalogSerialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            catalog = AssetDatabase.LoadAssetAtPath<RewardCatalogAsset>(CatalogPath);
            if (catalog == null) throw new InvalidOperationException("RewardCatalog asset could not be reloaded after creation.");
            LocalizationFoundationSetup.ApplyPhotographyEntries();
            ConfigureBootstrap(catalog);
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            Debug.Log($"PE_ECONOMY_SETUP_OK currency=explorer-stars rewards={catalog.Definitions.Count} premium=0 purchases=0 ledger=32");
            if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
        }

        public static void ApplyCli()
        {
            try { Apply(); }
            catch (Exception exception) { Debug.LogException(exception); EditorApplication.Exit(2); }
        }

        private static void ConfigureBootstrap(RewardCatalogAsset catalog)
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            foreach (GameObject root in scene.GetRootGameObjects().Where(item => item.name == EconomyView.PlaceholderObjectName))
                UnityEngine.Object.DestroyImmediate(root);
            var canvasObject = new GameObject(EconomyView.PlaceholderObjectName, typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(EconomyView));
            Canvas canvas = canvasObject.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 125;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f); scaler.matchWidthOrHeight = 0.5f;
            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter)); safe.transform.SetParent(canvasObject.transform, false); Stretch((RectTransform)safe.transform);
            var panel = new GameObject("PH_ECONOMY_BALANCE", typeof(RectTransform), typeof(Image)); panel.transform.SetParent(safe.transform, false);
            RectTransform panelRect = (RectTransform)panel.transform; panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.84f, 0.91f);
            panelRect.sizeDelta = new Vector2(470f, 125f); panel.GetComponent<Image>().color = new Color(0.09f, 0.31f, 0.27f, 0.94f);
            Text balance = CreateText(panel.transform, "Balance", 38); SetRect(balance.rectTransform, new Vector2(0.03f, 0.43f), new Vector2(0.97f, 0.94f));
            Text notice = CreateText(panel.transform, "Virtual Notice", 18); SetRect(notice.rectTransform, new Vector2(0.03f, 0.05f), new Vector2(0.97f, 0.43f));
            Button debug = DefaultControls.CreateButton(new DefaultControls.Resources()).GetComponent<Button>(); debug.name = "PH_DEBUG_GRANT_STAR";
            debug.transform.SetParent(safe.transform, false); RectTransform debugRect = (RectTransform)debug.transform;
            debugRect.anchorMin = debugRect.anchorMax = new Vector2(0.87f, 0.79f); debugRect.sizeDelta = new Vector2(300f, 75f);
            Text debugLabel = debug.GetComponentInChildren<Text>(true); if (debugLabel != null) debugLabel.text = string.Empty;
            var viewSerialized = new SerializedObject(canvasObject.GetComponent<EconomyView>());
            viewSerialized.FindProperty("_animatedRoot").objectReferenceValue = panelRect;
            viewSerialized.FindProperty("_balance").objectReferenceValue = balance;
            viewSerialized.FindProperty("_notice").objectReferenceValue = notice;
            viewSerialized.FindProperty("_debugGrant").objectReferenceValue = debug;
            viewSerialized.ApplyModifiedPropertiesWithoutUndo();

            DiagnosticBootstrap bootstrap = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<DiagnosticBootstrap>(true)).Single();
            bootstrap.ConfigureRewardsForEditorAndTests(catalog);
            bootstrap.ConfigureEconomyForEditorAndTests(canvasObject.GetComponent<EconomyView>());
            var bootstrapSerialized = new SerializedObject(bootstrap);
            SerializedProperty fitters = bootstrapSerialized.FindProperty("_safeAreaFitters");
            var existing = new List<SafeAreaFitter>();
            for (int i = 0; i < fitters.arraySize; i++) { var value = fitters.GetArrayElementAtIndex(i).objectReferenceValue as SafeAreaFitter; if (value != null) existing.Add(value); }
            existing.Add(safe.GetComponent<SafeAreaFitter>()); fitters.arraySize = existing.Count;
            for (int i = 0; i < existing.Count; i++) fitters.GetArrayElementAtIndex(i).objectReferenceValue = existing[i];
            bootstrapSerialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(bootstrap);
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        }

        private static Text CreateText(Transform parent, string name, int size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text)); go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>(); text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.fontSize = size;
            text.resizeTextForBestFit = true; text.resizeTextMinSize = 14; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white; text.raycastTarget = false; return text;
        }
        private static void Stretch(RectTransform rect) { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; }
        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max) { rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero; }
        private static void EnsureFolder(string parent, string name) { string path = parent + "/" + name; if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, name); }
    }
}
