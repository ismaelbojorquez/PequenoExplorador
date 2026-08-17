using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Learning;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Economy;
using PequenoExplorador.Content.Learning;
using PequenoExplorador.Content.Interaction;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Learning;
using PequenoExplorador.Presentation.Interaction;
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
        public const string ToucanConceptPath = Root + "/PH_Concept_ToucanDiet.asset";
        public const string ToucanActivityPath = Root + "/PH_Activity_ToucanChooseFood.asset";
        public const string ToucanRewardPath = "Assets/_Game/Content/Economy/Reward_ActivityToucanChooseFood.asset";

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

            LearningConceptDefinitionAsset toucanConcept = EnsureAsset<LearningConceptDefinitionAsset>(ToucanConceptPath);
            SerializedObject toucanConceptObject = new SerializedObject(toucanConcept);
            toucanConceptObject.FindProperty("_id").stringValue = "concept.nature.diet.fruit-primary";
            ConfigureSourcedPlaceholder(toucanConceptObject.FindProperty("_editorial"));
            toucanConceptObject.FindProperty("_labelTable").stringValue = "UI";
            toucanConceptObject.FindProperty("_labelKey").stringValue = "ui.learning.concept.toucan_diet";
            toucanConceptObject.ApplyModifiedPropertiesWithoutUndo();

            LearningActivityDefinitionAsset toucanActivity = EnsureAsset<LearningActivityDefinitionAsset>(ToucanActivityPath);
            SerializedObject toucanActivityObject = new SerializedObject(toucanActivity);
            toucanActivityObject.FindProperty("_id").stringValue = "activity.jungle.keel-billed-toucan.choose-food";
            ConfigureSourcedPlaceholder(toucanActivityObject.FindProperty("_editorial"));
            toucanActivityObject.FindProperty("_typeId").stringValue = LearningActivityTypeIds.SingleChoice.Value;
            SetKey(toucanActivityObject, "_title", "ui.learning.toucan_food.title");
            SetKey(toucanActivityObject, "_instruction", "ui.learning.toucan_food.instruction");
            SetKey(toucanActivityObject, "_success", "ui.learning.toucan_food.success");
            SetKey(toucanActivityObject, "_tryAgain", "ui.learning.toucan_food.try_again");
            SerializedProperty toucanConcepts = toucanActivityObject.FindProperty("_conceptIds");
            toucanConcepts.arraySize = 1;
            toucanConcepts.GetArrayElementAtIndex(0).stringValue = "concept.nature.diet.fruit-primary";
            SerializedProperty toucanOptions = toucanActivityObject.FindProperty("_options");
            toucanOptions.arraySize = 3;
            ConfigureOption(toucanOptions.GetArrayElementAtIndex(0), "activity-option.jungle.toucan.fruit", "ui.learning.toucan_food.option.fruit", "tag.food.fruit", new Color32(239, 121, 76, 255));
            ConfigureOption(toucanOptions.GetArrayElementAtIndex(1), "activity-option.jungle.toucan.rock", "ui.learning.toucan_food.option.rock", "tag.object.rock", new Color32(137, 154, 164, 255));
            ConfigureOption(toucanOptions.GetArrayElementAtIndex(2), "activity-option.jungle.toucan.hat", "ui.learning.toucan_food.option.hat", "tag.object.hat", new Color32(246, 196, 74, 255));
            toucanActivityObject.FindProperty("_correctOptionId").stringValue = "activity-option.jungle.toucan.fruit";
            toucanActivityObject.FindProperty("_correctTagId").stringValue = "tag.food.fruit";
            SerializedProperty toucanHints = toucanActivityObject.FindProperty("_hintKeys");
            toucanHints.arraySize = 3;
            for (int index = 0; index < 3; index++) toucanHints.GetArrayElementAtIndex(index).stringValue = "ui.learning.toucan_food.hint." + (index + 1);
            toucanActivityObject.FindProperty("_firstAutomaticHintAttempt").intValue = 2;
            toucanActivityObject.FindProperty("_maximumHintLevel").intValue = 3;
            toucanActivityObject.FindProperty("_resumable").boolValue = true;
            toucanActivityObject.FindProperty("_rewardId").stringValue = "reward.activity.toucan-choose-food.complete";
            toucanActivityObject.FindProperty("_factId").stringValue = "fact.jungle.keel-billed-toucan.diet";
            toucanActivityObject.FindProperty("_factTable").stringValue = "Content";
            toucanActivityObject.FindProperty("_factKey").stringValue = "content.fact.keel-billed-toucan.diet";
            toucanActivityObject.FindProperty("_instructionCueId").stringValue = "audio.voice.instruction.toucan-food";
            toucanActivityObject.FindProperty("_factCueId").stringValue = "audio.voice.fact.toucan-fruit";
            toucanActivityObject.FindProperty("_retryCueId").stringValue = "audio.feedback.retry";
            toucanActivityObject.FindProperty("_positiveReactionId").stringValue = "learning-reaction.toucan.positive";
            toucanActivityObject.FindProperty("_neutralReactionId").stringValue = "learning-reaction.toucan.neutral";
            toucanActivityObject.ApplyModifiedPropertiesWithoutUndo();

            RewardDefinitionAsset activityReward = EnsureAsset<RewardDefinitionAsset>(RewardPath);
            SerializedObject rewardObject = new SerializedObject(activityReward);
            rewardObject.FindProperty("_id").stringValue = "reward.activity.visual-matching.complete";
            rewardObject.FindProperty("_stars").intValue = 1;
            rewardObject.FindProperty("_sourceKind").enumValueIndex = (int)RewardSourceKind.Activity - 1;
            rewardObject.FindProperty("_sourceId").stringValue = "activity.fixture.visual-matching";
            rewardObject.ApplyModifiedPropertiesWithoutUndo();

            RewardDefinitionAsset toucanReward = EnsureAsset<RewardDefinitionAsset>(ToucanRewardPath);
            SerializedObject toucanRewardObject = new SerializedObject(toucanReward);
            toucanRewardObject.FindProperty("_id").stringValue = "reward.activity.toucan-choose-food.complete";
            toucanRewardObject.FindProperty("_stars").intValue = 1;
            toucanRewardObject.FindProperty("_sourceKind").enumValueIndex = (int)RewardSourceKind.Activity - 1;
            toucanRewardObject.FindProperty("_sourceId").stringValue = "activity.jungle.keel-billed-toucan.choose-food";
            toucanRewardObject.ApplyModifiedPropertiesWithoutUndo();
            RewardCatalogAsset rewards = AssetDatabase.LoadAssetAtPath<RewardCatalogAsset>(EconomyFoundationSetup.CatalogPath);
            RewardDefinitionAsset discovery = AssetDatabase.LoadAssetAtPath<RewardDefinitionAsset>(EconomyFoundationSetup.RewardPath);
            RewardDefinitionAsset mission = AssetDatabase.LoadAssetAtPath<RewardDefinitionAsset>(MissionFoundationSetup.RewardPath);
            if (rewards == null || discovery == null || mission == null) throw new InvalidOperationException("Economy and missions must exist before learning.");
            SerializedObject rewardsObject = new SerializedObject(rewards); SerializedProperty rewardDefinitions = rewardsObject.FindProperty("_definitions");
            rewardDefinitions.arraySize = 4; rewardDefinitions.GetArrayElementAtIndex(0).objectReferenceValue = discovery;
            rewardDefinitions.GetArrayElementAtIndex(1).objectReferenceValue = mission; rewardDefinitions.GetArrayElementAtIndex(2).objectReferenceValue = activityReward;
            rewardDefinitions.GetArrayElementAtIndex(3).objectReferenceValue = toucanReward;
            rewardsObject.ApplyModifiedPropertiesWithoutUndo();

            LearningCatalogAsset catalog = EnsureAsset<LearningCatalogAsset>(CatalogPath); SerializedObject catalogObject = new SerializedObject(catalog);
            SerializedProperty catalogConcepts = catalogObject.FindProperty("_concepts"); catalogConcepts.arraySize = 2; catalogConcepts.GetArrayElementAtIndex(0).objectReferenceValue = concept; catalogConcepts.GetArrayElementAtIndex(1).objectReferenceValue = toucanConcept;
            SerializedProperty activities = catalogObject.FindProperty("_activities"); activities.arraySize = 2; activities.GetArrayElementAtIndex(0).objectReferenceValue = activity; activities.GetArrayElementAtIndex(1).objectReferenceValue = toucanActivity;
            catalogObject.ApplyModifiedPropertiesWithoutUndo();
            ConfigureInteractionEntry(); LocalizationFoundationSetup.ApplyLearningEntries(); ConfigureBootstrap(catalog);
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            ConfigureToucanReaction();
            Debug.Log("PE_LEARNING_SETUP_OK activities=2 concepts=2 strategies=1 sourcedActivity=1 releaseBlocked=1 rawEvents=0");
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
            Text watermark = CreateText(panel.transform, "Watermark", 18, new Vector2(0.02f, 0.95f), new Vector2(0.38f, 0.995f));
            Button[] optionButtons = new Button[3];
            for (int index = 0; index < 3; index++) optionButtons[index] = CreateButton(panel.transform, "Option " + (index + 1), new Vector2(0.18f + index * 0.32f, 0.53f), new Vector2(260, 130));
            Button hint = CreateButton(panel.transform, "Hint", new Vector2(0.25f, 0.10f), new Vector2(210, 80));
            Button replay = CreateButton(panel.transform, "Replay", new Vector2(0.50f, 0.10f), new Vector2(250, 80));
            Button exit = CreateButton(panel.transform, "Exit", new Vector2(0.76f, 0.10f), new Vector2(210, 80));
            SerializedObject viewObject = new SerializedObject(canvasObject.GetComponent<LearningActivityView>());
            viewObject.FindProperty("_title").objectReferenceValue = title; viewObject.FindProperty("_instruction").objectReferenceValue = instruction; viewObject.FindProperty("_feedback").objectReferenceValue = feedback;
            viewObject.FindProperty("_watermark").objectReferenceValue = watermark;
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
        private static void ConfigureOption(SerializedProperty option, string id, string key) { ConfigureOption(option, id, key, string.Empty, new Color32(255, 255, 255, 255)); }
        private static void ConfigureOption(SerializedProperty option, string id, string key, string tagId, Color32 color) { option.FindPropertyRelative("_id").stringValue = id; option.FindPropertyRelative("_table").stringValue = "UI"; option.FindPropertyRelative("_key").stringValue = key; option.FindPropertyRelative("_tagId").stringValue = tagId; option.FindPropertyRelative("_color").colorValue = color; }
        private static void ConfigureEditorial(SerializedProperty editorial) { editorial.FindPropertyRelative("_state").enumValueIndex = (int)EditorialState.Draft; editorial.FindPropertyRelative("_isPlaceholder").boolValue = true; editorial.FindPropertyRelative("_owner").stringValue = "Learning Design"; editorial.FindPropertyRelative("_developmentWatermark").stringValue = "BORRADOR · PH_"; }
        private static void ConfigureSourcedPlaceholder(SerializedProperty editorial) { editorial.FindPropertyRelative("_state").enumValueIndex = (int)EditorialState.Sourced; editorial.FindPropertyRelative("_isPlaceholder").boolValue = true; editorial.FindPropertyRelative("_owner").stringValue = "Learning Design — revisión humana pendiente"; editorial.FindPropertyRelative("_developmentWatermark").stringValue = "FUENTE VERIFICADA · REPRESENTACIÓN PH_ PENDIENTE"; }

        private static void ConfigureToucanReaction()
        {
            Scene scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.JungleScenePath, OpenSceneMode.Single);
            WorldInteractableView toucan = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<WorldInteractableView>(true))
                .Single(item => item.RawInteractionId == InteractionFoundationSetup.AnimalId);
            AnimalLearningReactionView reaction = toucan.GetComponent<AnimalLearningReactionView>();
            if (reaction == null) reaction = toucan.gameObject.AddComponent<AnimalLearningReactionView>();
            Transform visual = toucan.GetComponentsInChildren<Renderer>(true).Select(item => item.transform).FirstOrDefault();
            if (visual == null) throw new InvalidOperationException("Toucan interaction fixture has no visual for learning reaction.");
            reaction.ConfigureForEditorAndTests(visual);
            EditorUtility.SetDirty(reaction);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureInteractionEntry()
        {
            InteractionDefinitionAsset interaction = AssetDatabase.LoadAssetAtPath<InteractionDefinitionAsset>(InteractionFoundationSetup.AnimalPath);
            if (interaction == null) throw new InvalidOperationException("Approved toucan interaction must exist before the integrated activity.");
            SerializedObject serialized = new SerializedObject(interaction);
            serialized.FindProperty("_learningActivityId").stringValue = "activity.jungle.keel-billed-toucan.choose-food";
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(interaction);
        }
        private static T EnsureAsset<T>(string path) where T : ScriptableObject { T asset = AssetDatabase.LoadAssetAtPath<T>(path); if (asset != null) return asset; asset = ScriptableObject.CreateInstance<T>(); AssetDatabase.CreateAsset(asset, path); return asset; }
        private static Text CreateText(Transform parent, string name, int size, Vector2 min, Vector2 max) { var go = new GameObject(name, typeof(RectTransform), typeof(Text)); go.transform.SetParent(parent, false); Text text = go.GetComponent<Text>(); text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.fontSize = size; text.resizeTextForBestFit = true; text.resizeTextMinSize = 16; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white; text.raycastTarget = false; RectTransform rect = text.rectTransform; rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero; return text; }
        private static Button CreateButton(Transform parent, string name, Vector2 anchor, Vector2 size) { Button button = DefaultControls.CreateButton(new DefaultControls.Resources()).GetComponent<Button>(); button.name = name; button.transform.SetParent(parent, false); RectTransform rect = (RectTransform)button.transform; rect.anchorMin = rect.anchorMax = anchor; rect.sizeDelta = size; Text label = button.GetComponentInChildren<Text>(true); if (label != null) label.text = string.Empty; return button; }
        private static void Stretch(RectTransform rect) { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; }
        private static void EnsureFolder(string parent, string name) { string path = parent + "/" + name; if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, name); }
    }
}
