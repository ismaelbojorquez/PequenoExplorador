using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Learning;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Economy;
using PequenoExplorador.Content.Learning;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Learning;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class LearningFoundationSetup
    {
        public const string Root = "Assets/_Game/Content/Learning";
        public const string ConceptPath = Root + "/PH_Concept_VisualMatching.asset";
        public const string ActivityPath = Root + "/PH_Activity_VisualMatching.asset";
        public const string CatalogPath = Root + "/LearningCatalog.asset";
        public const string RewardPath = "Assets/_Game/Content/Economy/Reward_ActivityVisualMatching.asset";

        [MenuItem("Pequeño Explorador/Development/Learning/Apply Foundation")]
        public static void Apply()
        {
            EnsureFolder("Assets/_Game/Content", "Learning");
            LearningConceptDefinitionAsset concept = EnsureAsset<LearningConceptDefinitionAsset>(ConceptPath);
            SerializedObject conceptObject = new SerializedObject(concept);
            conceptObject.FindProperty("_id").stringValue = "concept.observation.visual-matching";
            ConfigureEditorial(conceptObject.FindProperty("_editorial"));
            conceptObject.FindProperty("_labelTable").stringValue = "UI";
            conceptObject.FindProperty("_labelKey").stringValue = "ui.learning.concept.visual_matching";
            conceptObject.ApplyModifiedPropertiesWithoutUndo();

            LearningActivityDefinitionAsset activity = EnsureAsset<LearningActivityDefinitionAsset>(ActivityPath);
            SerializedObject activityObject = new SerializedObject(activity);
            activityObject.FindProperty("_id").stringValue = "activity.fixture.visual-matching";
            ConfigureEditorial(activityObject.FindProperty("_editorial"));
            activityObject.FindProperty("_typeId").stringValue = LearningActivityTypeIds.SingleChoice.Value;
            SetKey(activityObject, "_title", "ui.learning.fixture.title"); SetKey(activityObject, "_instruction", "ui.learning.fixture.instruction");
            SetKey(activityObject, "_success", "ui.learning.fixture.success"); SetKey(activityObject, "_tryAgain", "ui.learning.fixture.try_again");
            SerializedProperty concepts = activityObject.FindProperty("_conceptIds"); concepts.arraySize = 1; concepts.GetArrayElementAtIndex(0).stringValue = "concept.observation.visual-matching";
            SerializedProperty options = activityObject.FindProperty("_options"); options.arraySize = 3;
            ConfigureOption(options.GetArrayElementAtIndex(0), "activity-option.fixture.circle", "ui.learning.option.circle");
            ConfigureOption(options.GetArrayElementAtIndex(1), "activity-option.fixture.triangle", "ui.learning.option.triangle");
            ConfigureOption(options.GetArrayElementAtIndex(2), "activity-option.fixture.square", "ui.learning.option.square");
            activityObject.FindProperty("_correctOptionId").stringValue = "activity-option.fixture.circle";
            SerializedProperty hints = activityObject.FindProperty("_hintKeys"); hints.arraySize = 3;
            for (int index = 0; index < 3; index++) hints.GetArrayElementAtIndex(index).stringValue = "ui.learning.fixture.hint." + (index + 1);
            activityObject.FindProperty("_firstAutomaticHintAttempt").intValue = 2;
            activityObject.FindProperty("_maximumHintLevel").intValue = 3;
            activityObject.FindProperty("_resumable").boolValue = true;
            activityObject.FindProperty("_rewardId").stringValue = "reward.activity.visual-matching.complete";
            activityObject.ApplyModifiedPropertiesWithoutUndo();

            RewardDefinitionAsset activityReward = EnsureAsset<RewardDefinitionAsset>(RewardPath);
            SerializedObject rewardObject = new SerializedObject(activityReward);
            rewardObject.FindProperty("_id").stringValue = "reward.activity.visual-matching.complete";
            rewardObject.FindProperty("_stars").intValue = 1;
            rewardObject.FindProperty("_sourceKind").enumValueIndex = (int)RewardSourceKind.Activity - 1;
            rewardObject.FindProperty("_sourceId").stringValue = "activity.fixture.visual-matching";
            rewardObject.ApplyModifiedPropertiesWithoutUndo();
            RewardCatalogAsset rewards = AssetDatabase.LoadAssetAtPath<RewardCatalogAsset>(EconomyFoundationSetup.CatalogPath);
            RewardDefinitionAsset discovery = AssetDatabase.LoadAssetAtPath<RewardDefinitionAsset>(EconomyFoundationSetup.RewardPath);
            RewardDefinitionAsset mission = AssetDatabase.LoadAssetAtPath<RewardDefinitionAsset>(MissionFoundationSetup.RewardPath);
            if (rewards == null || discovery == null || mission == null) throw new InvalidOperationException("Economy and missions must exist before learning.");
            SerializedObject rewardsObject = new SerializedObject(rewards); SerializedProperty rewardDefinitions = rewardsObject.FindProperty("_definitions");
            rewardDefinitions.arraySize = 3; rewardDefinitions.GetArrayElementAtIndex(0).objectReferenceValue = discovery;
            rewardDefinitions.GetArrayElementAtIndex(1).objectReferenceValue = mission; rewardDefinitions.GetArrayElementAtIndex(2).objectReferenceValue = activityReward;
            rewardsObject.ApplyModifiedPropertiesWithoutUndo();

            LearningCatalogAsset catalog = EnsureAsset<LearningCatalogAsset>(CatalogPath); SerializedObject catalogObject = new SerializedObject(catalog);
            SerializedProperty catalogConcepts = catalogObject.FindProperty("_concepts"); catalogConcepts.arraySize = 1; catalogConcepts.GetArrayElementAtIndex(0).objectReferenceValue = concept;
            SerializedProperty activities = catalogObject.FindProperty("_activities"); activities.arraySize = 1; activities.GetArrayElementAtIndex(0).objectReferenceValue = activity;
            catalogObject.ApplyModifiedPropertiesWithoutUndo();
            LocalizationFoundationSetup.ApplyLearningEntries(); ConfigureBootstrap(catalog);
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            Debug.Log("PE_LEARNING_SETUP_OK activities=1 concepts=1 strategies=1 fixture=Draft rawEvents=0");
            if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
        }

        public static void ApplyCli() { try { Apply(); } catch (Exception exception) { Debug.LogException(exception); EditorApplication.Exit(2); } }

        private static void ConfigureBootstrap(LearningCatalogAsset catalog)
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            foreach (GameObject root in scene.GetRootGameObjects().Where(item => item.name == LearningActivityView.PlaceholderObjectName)) UnityEngine.Object.DestroyImmediate(root);
            var canvasObject = new GameObject(LearningActivityView.PlaceholderObjectName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(LearningActivityView));
            Canvas canvas = canvasObject.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 124;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = 0.5f;
            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter)); safe.transform.SetParent(canvasObject.transform, false); Stretch((RectTransform)safe.transform);
            var panel = new GameObject("PH_LEARNING_PANEL", typeof(RectTransform), typeof(Image)); panel.transform.SetParent(safe.transform, false);
            RectTransform panelRect = (RectTransform)panel.transform; panelRect.anchorMin = new Vector2(0.25f, 0.15f); panelRect.anchorMax = new Vector2(0.75f, 0.82f); panelRect.offsetMin = panelRect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.08f, 0.34f, 0.28f, 0.96f);
            Text title = CreateText(panel.transform, "Title", 38, new Vector2(0.05f, 0.82f), new Vector2(0.95f, 0.96f));
            Text instruction = CreateText(panel.transform, "Instruction", 30, new Vector2(0.05f, 0.67f), new Vector2(0.95f, 0.83f));
            Text feedback = CreateText(panel.transform, "Feedback", 28, new Vector2(0.05f, 0.20f), new Vector2(0.95f, 0.35f));
            Button[] optionButtons = new Button[3];
            for (int index = 0; index < 3; index++) optionButtons[index] = CreateButton(panel.transform, "Option " + (index + 1), new Vector2(0.18f + index * 0.32f, 0.53f), new Vector2(260, 130));
            Button hint = CreateButton(panel.transform, "Hint", new Vector2(0.25f, 0.10f), new Vector2(210, 80));
            Button replay = CreateButton(panel.transform, "Replay", new Vector2(0.50f, 0.10f), new Vector2(250, 80));
            Button exit = CreateButton(panel.transform, "Exit", new Vector2(0.76f, 0.10f), new Vector2(210, 80));
            SerializedObject viewObject = new SerializedObject(canvasObject.GetComponent<LearningActivityView>());
            viewObject.FindProperty("_title").objectReferenceValue = title; viewObject.FindProperty("_instruction").objectReferenceValue = instruction; viewObject.FindProperty("_feedback").objectReferenceValue = feedback;
            SerializedProperty serializedOptions = viewObject.FindProperty("_options"); serializedOptions.arraySize = 3; for (int index = 0; index < 3; index++) serializedOptions.GetArrayElementAtIndex(index).objectReferenceValue = optionButtons[index];
            viewObject.FindProperty("_hint").objectReferenceValue = hint; viewObject.FindProperty("_replay").objectReferenceValue = replay; viewObject.FindProperty("_exit").objectReferenceValue = exit; viewObject.ApplyModifiedPropertiesWithoutUndo();
            DiagnosticBootstrap bootstrap = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<DiagnosticBootstrap>(true)).Single();
            bootstrap.ConfigureLearningForEditorAndTests(catalog, canvasObject.GetComponent<LearningActivityView>());
            SerializedObject bootstrapObject = new SerializedObject(bootstrap); SerializedProperty fitters = bootstrapObject.FindProperty("_safeAreaFitters");
            var existing = new List<SafeAreaFitter>(); for (int index = 0; index < fitters.arraySize; index++) if (fitters.GetArrayElementAtIndex(index).objectReferenceValue is SafeAreaFitter fitter) existing.Add(fitter);
            existing.Add(safe.GetComponent<SafeAreaFitter>()); fitters.arraySize = existing.Count; for (int index = 0; index < existing.Count; index++) fitters.GetArrayElementAtIndex(index).objectReferenceValue = existing[index];
            bootstrapObject.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(bootstrap); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        }

        private static void SetKey(SerializedObject obj, string prefix, string key) { obj.FindProperty(prefix + "Table").stringValue = "UI"; obj.FindProperty(prefix + "Key").stringValue = key; }
        private static void ConfigureOption(SerializedProperty option, string id, string key) { option.FindPropertyRelative("_id").stringValue = id; option.FindPropertyRelative("_table").stringValue = "UI"; option.FindPropertyRelative("_key").stringValue = key; }
        private static void ConfigureEditorial(SerializedProperty editorial) { editorial.FindPropertyRelative("_state").enumValueIndex = (int)EditorialState.Draft; editorial.FindPropertyRelative("_isPlaceholder").boolValue = true; editorial.FindPropertyRelative("_owner").stringValue = "Learning Design"; editorial.FindPropertyRelative("_developmentWatermark").stringValue = "BORRADOR · PH_"; }
        private static T EnsureAsset<T>(string path) where T : ScriptableObject { T asset = AssetDatabase.LoadAssetAtPath<T>(path); if (asset != null) return asset; asset = ScriptableObject.CreateInstance<T>(); AssetDatabase.CreateAsset(asset, path); return asset; }
        private static Text CreateText(Transform parent, string name, int size, Vector2 min, Vector2 max) { var go = new GameObject(name, typeof(RectTransform), typeof(Text)); go.transform.SetParent(parent, false); Text text = go.GetComponent<Text>(); text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.fontSize = size; text.resizeTextForBestFit = true; text.resizeTextMinSize = 16; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white; text.raycastTarget = false; RectTransform rect = text.rectTransform; rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero; return text; }
        private static Button CreateButton(Transform parent, string name, Vector2 anchor, Vector2 size) { Button button = DefaultControls.CreateButton(new DefaultControls.Resources()).GetComponent<Button>(); button.name = name; button.transform.SetParent(parent, false); RectTransform rect = (RectTransform)button.transform; rect.anchorMin = rect.anchorMax = anchor; rect.sizeDelta = size; Text label = button.GetComponentInChildren<Text>(true); if (label != null) label.text = string.Empty; return button; }
        private static void Stretch(RectTransform rect) { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; }
        private static void EnsureFolder(string parent, string name) { string path = parent + "/" + name; if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, name); }
    }
}
