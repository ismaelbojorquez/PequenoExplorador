using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Missions;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Economy;
using PequenoExplorador.Content.Missions;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Missions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class MissionFoundationSetup
    {
        public const string Root = "Assets/_Game/Content/Missions";
        public const string MissionPath = Root + "/Mission_PhotographToucan.asset";
        public const string CatalogPath = Root + "/MissionCatalog.asset";
        public const string RewardPath = "Assets/_Game/Content/Economy/Reward_MissionPhotographToucan.asset";

        [MenuItem("Pequeño Explorador/Development/Missions/Apply Foundation")]
        public static void Apply()
        {
            EnsureFolder("Assets/_Game/Content", "Missions");
            RewardDefinitionAsset reward = EnsureAsset<RewardDefinitionAsset>(RewardPath);
            var rewardObject = new SerializedObject(reward);
            rewardObject.FindProperty("_id").stringValue = "reward.mission.photograph-toucan.complete";
            rewardObject.FindProperty("_stars").intValue = 2;
            rewardObject.FindProperty("_sourceKind").enumValueIndex = (int)RewardSourceKind.Mission - 1;
            rewardObject.FindProperty("_sourceId").stringValue = "mission.vertical-slice.photograph-toucan";
            rewardObject.ApplyModifiedPropertiesWithoutUndo();

            RewardCatalogAsset rewards = AssetDatabase.LoadAssetAtPath<RewardCatalogAsset>(EconomyFoundationSetup.CatalogPath);
            RewardDefinitionAsset discoveryReward = AssetDatabase.LoadAssetAtPath<RewardDefinitionAsset>(EconomyFoundationSetup.RewardPath);
            if (rewards == null || discoveryReward == null) throw new InvalidOperationException("Economy foundation must exist before missions.");
            var rewardsObject = new SerializedObject(rewards);
            SerializedProperty rewardDefinitions = rewardsObject.FindProperty("_definitions");
            rewardDefinitions.arraySize = 2;
            rewardDefinitions.GetArrayElementAtIndex(0).objectReferenceValue = discoveryReward;
            rewardDefinitions.GetArrayElementAtIndex(1).objectReferenceValue = reward;
            rewardsObject.ApplyModifiedPropertiesWithoutUndo();

            MissionDefinitionAsset mission = EnsureAsset<MissionDefinitionAsset>(MissionPath);
            var missionObject = new SerializedObject(mission);
            missionObject.FindProperty("_id").stringValue = "mission.vertical-slice.photograph-toucan";
            ConfigureEditorial(missionObject.FindProperty("_editorial"));
            missionObject.FindProperty("_titleTable").stringValue = "UI";
            missionObject.FindProperty("_titleKey").stringValue = "ui.mission.photograph_toucan.title";
            missionObject.FindProperty("_summaryTable").stringValue = "UI";
            missionObject.FindProperty("_summaryKey").stringValue = "ui.mission.photograph_toucan.summary";
            missionObject.FindProperty("_completionTable").stringValue = "UI";
            missionObject.FindProperty("_completionKey").stringValue = "ui.mission.photograph_toucan.completion";
            missionObject.FindProperty("_rewardId").stringValue = "reward.mission.photograph-toucan.complete";
            missionObject.FindProperty("_prerequisiteIds").arraySize = 0;
            SerializedProperty objectives = missionObject.FindProperty("_objectives");
            objectives.arraySize = 1;
            SerializedProperty objective = objectives.GetArrayElementAtIndex(0);
            objective.FindPropertyRelative("_id").stringValue = "mission-objective.photograph-toucan";
            objective.FindPropertyRelative("_typeId").stringValue = MissionObjectiveTypeIds.PhotographSpecific.Value;
            objective.FindPropertyRelative("_labelTable").stringValue = "UI";
            objective.FindPropertyRelative("_labelKey").stringValue = "ui.mission.photograph_toucan.objective";
            objective.FindPropertyRelative("_targetCount").intValue = 1;
            objective.FindPropertyRelative("_subjectId").stringValue = PhotographyFoundationSetup.DiscoveryId;
            objective.FindPropertyRelative("_requiredTagId").stringValue = string.Empty;
            missionObject.ApplyModifiedPropertiesWithoutUndo();

            MissionCatalogAsset catalog = EnsureAsset<MissionCatalogAsset>(CatalogPath);
            var catalogObject = new SerializedObject(catalog);
            SerializedProperty definitions = catalogObject.FindProperty("_definitions");
            definitions.arraySize = 1;
            definitions.GetArrayElementAtIndex(0).objectReferenceValue = mission;
            catalogObject.ApplyModifiedPropertiesWithoutUndo();
            LocalizationFoundationSetup.ApplyMissionEntries();
            ConfigureBootstrap(catalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("PE_MISSION_SETUP_OK missions=1 strategies=3 missionRewards=1 expiry=0 manualClaim=0");
            if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
        }

        public static void ApplyCli()
        {
            try { Apply(); }
            catch (Exception exception) { Debug.LogException(exception); EditorApplication.Exit(2); }
        }

        private static void ConfigureBootstrap(MissionCatalogAsset catalog)
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            foreach (GameObject root in scene.GetRootGameObjects().Where(item => item.name == MissionView.PlaceholderObjectName))
                UnityEngine.Object.DestroyImmediate(root);
            var canvasObject = new GameObject(MissionView.PlaceholderObjectName, typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(MissionView));
            Canvas canvas = canvasObject.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 123;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f); scaler.matchWidthOrHeight = 0.5f;
            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvasObject.transform, false); Stretch((RectTransform)safe.transform);
            var panel = new GameObject("PH_MISSION_PANEL", typeof(RectTransform), typeof(Image)); panel.transform.SetParent(safe.transform, false);
            RectTransform panelRect = (RectTransform)panel.transform; panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.18f, 0.84f);
            panelRect.sizeDelta = new Vector2(600f, 210f); panel.GetComponent<Image>().color = new Color(0.12f, 0.25f, 0.42f, 0.94f);
            Text title = CreateText(panel.transform, "Title", 34); SetRect(title.rectTransform, new Vector2(0.04f, 0.66f), new Vector2(0.96f, 0.96f));
            Text body = CreateText(panel.transform, "Body", 24); SetRect(body.rectTransform, new Vector2(0.04f, 0.28f), new Vector2(0.96f, 0.68f));
            Button activate = DefaultControls.CreateButton(new DefaultControls.Resources()).GetComponent<Button>();
            activate.name = "Mission Activate"; activate.transform.SetParent(panel.transform, false);
            RectTransform activateRect = (RectTransform)activate.transform; activateRect.anchorMin = activateRect.anchorMax = new Vector2(0.5f, 0.14f);
            activateRect.sizeDelta = new Vector2(320f, 72f);
            Text buttonText = activate.GetComponentInChildren<Text>(true); if (buttonText != null) buttonText.text = string.Empty;
            var viewObject = new SerializedObject(canvasObject.GetComponent<MissionView>());
            viewObject.FindProperty("_title").objectReferenceValue = title;
            viewObject.FindProperty("_body").objectReferenceValue = body;
            viewObject.FindProperty("_activate").objectReferenceValue = activate;
            viewObject.ApplyModifiedPropertiesWithoutUndo();

            DiagnosticBootstrap bootstrap = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<DiagnosticBootstrap>(true)).Single();
            bootstrap.ConfigureMissionsForEditorAndTests(catalog, canvasObject.GetComponent<MissionView>());
            var bootstrapObject = new SerializedObject(bootstrap);
            SerializedProperty fitters = bootstrapObject.FindProperty("_safeAreaFitters");
            var existing = new List<SafeAreaFitter>();
            for (int index = 0; index < fitters.arraySize; index++)
            {
                var fitter = fitters.GetArrayElementAtIndex(index).objectReferenceValue as SafeAreaFitter;
                if (fitter != null) existing.Add(fitter);
            }
            existing.Add(safe.GetComponent<SafeAreaFitter>());
            fitters.arraySize = existing.Count;
            for (int index = 0; index < existing.Count; index++) fitters.GetArrayElementAtIndex(index).objectReferenceValue = existing[index];
            bootstrapObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(bootstrap);
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureEditorial(SerializedProperty editorial)
        {
            editorial.FindPropertyRelative("_state").enumValueIndex = (int)EditorialState.Approved;
            editorial.FindPropertyRelative("_isPlaceholder").boolValue = false;
            editorial.FindPropertyRelative("_owner").stringValue = "Mission Design";
            editorial.FindPropertyRelative("_developmentWatermark").stringValue = string.Empty;
        }
        private static T EnsureAsset<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>(); AssetDatabase.CreateAsset(asset, path); return asset;
        }
        private static Text CreateText(Transform parent, string name, int size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text)); go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>(); text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.fontSize = size;
            text.resizeTextForBestFit = true; text.resizeTextMinSize = 14; text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white; text.raycastTarget = false; return text;
        }
        private static void Stretch(RectTransform rect) { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; }
        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max) { rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero; }
        private static void EnsureFolder(string parent, string name) { string path = parent + "/" + name; if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, name); }
    }
}
