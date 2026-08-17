using System;
using System.Linq;
using PequenoExplorador.Application.Tutorial;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Tutorial;
using PequenoExplorador.DesignSystem;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Tutorial;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class TutorialFoundationSetup
    {
        public const string RootPath = "Assets/_Game/Content/Tutorial";
        public const string DefinitionPath = RootPath + "/PH_Tutorial_VerticalSlice.asset";

        [MenuItem("Pequeño Explorador/Setup/28 Apply FTUE Tutorial")]
        public static void Apply()
        {
            try
            {
                EnsureFolder(RootPath);
                LocalizationFoundationSetup.ApplyTutorialEntries();
                TutorialDefinitionAsset definition = EnsureDefinition();
                Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
                DiagnosticBootstrap bootstrap = scene.GetRootGameObjects().SelectMany(value => value.GetComponentsInChildren<DiagnosticBootstrap>(true)).Single();
                foreach (GameObject old in scene.GetRootGameObjects().Where(value => value.name == TutorialView.PlaceholderObjectName).ToArray())
                    UnityEngine.Object.DestroyImmediate(old);
                TutorialView view = CreateView();
                bootstrap.ConfigureTutorialForEditorAndTests(definition, view);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("PE_TUTORIAL_SETUP_OK id=tutorial.vertical-slice version=1 steps=7 choice=2 help=6/12s remote=false");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        private static TutorialDefinitionAsset EnsureDefinition()
        {
            TutorialDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<TutorialDefinitionAsset>(DefinitionPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<TutorialDefinitionAsset>();
                AssetDatabase.CreateAsset(asset, DefinitionPath);
            }
            var serialized = new SerializedObject(asset);
            serialized.FindProperty("_tutorialId").stringValue = "tutorial.vertical-slice";
            serialized.FindProperty("_contentVersion").intValue = 1;
            serialized.FindProperty("_placeholderId").stringValue = "PH_TUTORIAL_VERTICAL_SLICE";
            serialized.FindProperty("_releaseState").stringValue = "ReleaseBlockedPendingNarration";
            SerializedProperty steps = serialized.FindProperty("_steps");
            steps.arraySize = 7;
            Configure(0, "tutorial-step.enter-expedition", TutorialTrigger.ExpeditionEntered, TutorialAction.EnterExpedition,
                TutorialSpotlight.Expedition, "ui.tutorial.step.enter-expedition", "audio.voice.tutorial.enter-expedition");
            Configure(1, "tutorial-step.move", TutorialTrigger.MovementAccepted, TutorialAction.Move,
                TutorialSpotlight.Ground, "ui.tutorial.step.move", "audio.voice.tutorial.move");
            Configure(2, "tutorial-step.interact", TutorialTrigger.InteractionCompleted, TutorialAction.Move | TutorialAction.Interact,
                TutorialSpotlight.Interactable, "ui.tutorial.step.interact", "audio.voice.tutorial.interact");
            Configure(3, "tutorial-step.photograph", TutorialTrigger.PhotoCaptured, TutorialAction.Photograph,
                TutorialSpotlight.Shutter, "ui.tutorial.step.photograph", "audio.voice.tutorial.photograph");
            Configure(4, "tutorial-step.discovery-reward", TutorialTrigger.Continue, TutorialAction.Continue,
                TutorialSpotlight.DiscoveryReward, "ui.tutorial.step.discovery-reward", "audio.voice.tutorial.discovery-reward");
            Configure(5, "tutorial-step.return-camp", TutorialTrigger.CampReturned, TutorialAction.ReturnCamp,
                TutorialSpotlight.ReturnCamp, "ui.tutorial.step.return-camp", "audio.voice.tutorial.return-camp");
            Configure(6, "tutorial-step.open-album", TutorialTrigger.AlbumOpened, TutorialAction.OpenAlbum,
                TutorialSpotlight.Album, "ui.tutorial.step.open-album", "audio.voice.tutorial.open-album");
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            return asset;

            void Configure(int index, string id, TutorialTrigger trigger, TutorialAction actions, TutorialSpotlight spotlight,
                string instructionKey, string voiceCue)
            {
                SerializedProperty step = steps.GetArrayElementAtIndex(index);
                step.FindPropertyRelative("_id").stringValue = id;
                step.FindPropertyRelative("_trigger").intValue = (int)trigger;
                step.FindPropertyRelative("_allowedActions").intValue = (int)actions;
                step.FindPropertyRelative("_spotlight").intValue = (int)spotlight;
                step.FindPropertyRelative("_instructionKey").stringValue = instructionKey;
                step.FindPropertyRelative("_voiceCueId").stringValue = voiceCue;
                step.FindPropertyRelative("_standardHelpSeconds").floatValue = 12f;
                step.FindPropertyRelative("_moreGuidanceHelpSeconds").floatValue = 6f;
            }
        }

        private static TutorialView CreateView()
        {
            UIDesignTokens tokens = AssetDatabase.LoadAssetAtPath<UIDesignTokens>(UIDesignSystemSetup.TokenPath) ??
                throw new InvalidOperationException("Design tokens must exist before tutorial setup.");
            var root = new GameObject(TutorialView.PlaceholderObjectName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler),
                typeof(GraphicRaycaster), typeof(SafeAreaFitter), typeof(TutorialView));
            Canvas canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 210;
            CanvasScaler scaler = root.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = .5f;

            GameObject instructionPanel = Panel(root.transform, "PH_TUTORIAL_INSTRUCTION_PANEL", new Vector2(.18f, .70f), new Vector2(.82f, .97f), new Color(1f, .97f, .86f, .97f), false);
            TMP_Text instruction = Text(instructionPanel.transform, "Instruction", tokens.Font, 31, new Vector2(.14f, .30f), new Vector2(.86f, .90f), new Color32(28, 52, 47, 255));
            TMP_Text progress = Text(instructionPanel.transform, "Progress", tokens.Font, 18, new Vector2(.40f, .05f), new Vector2(.60f, .28f), new Color32(78, 103, 95, 255));
            GameObject gesture = new GameObject("PH_GESTURE_HAND_ARROW", typeof(RectTransform), typeof(UIIconGraphic));
            gesture.transform.SetParent(instructionPanel.transform, false); SetRect((RectTransform)gesture.transform, new Vector2(.04f, .34f), new Vector2(.13f, .82f));
            UIIconGraphic gestureIcon = gesture.GetComponent<UIIconGraphic>(); gestureIcon.ConfigureForEditor(UIIconKind.GestureTap, UIColorRole.Accent); gestureIcon.raycastTarget = false;
            Button replay = Button(instructionPanel.transform, "Replay Instruction", tokens.Font, new Vector2(.03f, .02f), new Vector2(.26f, .40f));
            Button skip = Button(instructionPanel.transform, "Skip Tutorial", tokens.Font, new Vector2(.74f, .02f), new Vector2(.97f, .40f));
            Button next = Button(instructionPanel.transform, "Continue Tutorial", tokens.Font, new Vector2(.73f, .44f), new Vector2(.97f, .96f));

            GameObject choiceBlocker = Panel(root.transform, "PH_TUTORIAL_GUIDE_CHOICE", Vector2.zero, Vector2.one,
                new Color(0.04f, 0.12f, 0.10f, 0.72f), true);
            GameObject choice = Panel(choiceBlocker.transform, "Guide Choice Card", new Vector2(.25f, .18f),
                new Vector2(.75f, .82f), new Color(1f, .97f, .86f, 1f), true);
            TMP_Text choiceTitle = Text(choice.transform, "Guide Choice Title", tokens.Font, 38, new Vector2(.08f, .68f), new Vector2(.92f, .92f), new Color32(28, 52, 47, 255));
            Button more = Button(choice.transform, "More Guidance", tokens.Font, new Vector2(.10f, .36f), new Vector2(.90f, .62f));
            Button standard = Button(choice.transform, "Standard Guidance", tokens.Font, new Vector2(.10f, .08f), new Vector2(.90f, .34f));
            Button replayTutorial = Button(root.transform, "Replay Tutorial From Settings", tokens.Font, new Vector2(.76f, .02f), new Vector2(.98f, .13f));

            var serialized = new SerializedObject(root.GetComponent<TutorialView>());
            Set(serialized, "_instructionPanel", instructionPanel); Set(serialized, "_instruction", instruction); Set(serialized, "_progress", progress);
            Set(serialized, "_gesture", gesture); Set(serialized, "_gestureIcon", gestureIcon); Set(serialized, "_continueButton", next);
            Set(serialized, "_replayButton", replay); Set(serialized, "_skipButton", skip); Set(serialized, "_guideChoicePanel", choiceBlocker);
            Set(serialized, "_guideChoiceTitle", choiceTitle); Set(serialized, "_moreGuidanceButton", more);
            Set(serialized, "_standardGuidanceButton", standard); Set(serialized, "_replayTutorialButton", replayTutorial);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            instructionPanel.SetActive(false); choiceBlocker.SetActive(false); gesture.SetActive(false); next.gameObject.SetActive(false); replayTutorial.gameObject.SetActive(false);
            return root.GetComponent<TutorialView>();
        }

        private static GameObject Panel(Transform parent, string name, Vector2 min, Vector2 max, Color color, bool raycast)
        {
            var value = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            value.transform.SetParent(parent, false); SetRect((RectTransform)value.transform, min, max);
            Image image = value.GetComponent<Image>(); image.color = color; image.raycastTarget = raycast; return value;
        }
        private static TMP_Text Text(Transform parent, string name, TMP_FontAsset font, float size, Vector2 min, Vector2 max, Color color)
        {
            var value = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            value.transform.SetParent(parent, false); SetRect((RectTransform)value.transform, min, max);
            TMP_Text text = value.GetComponent<TMP_Text>(); text.font = font; text.fontSize = size; text.color = color;
            text.alignment = TextAlignmentOptions.Center; text.enableWordWrapping = true; text.raycastTarget = false; return text;
        }
        private static Button Button(Transform parent, string name, TMP_FontAsset font, Vector2 min, Vector2 max)
        {
            var value = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            value.transform.SetParent(parent, false); SetRect((RectTransform)value.transform, min, max); value.GetComponent<Image>().color = new Color32(255, 190, 77, 255);
            Text(value.transform, "Label", font, 22, new Vector2(.10f, .08f), new Vector2(.90f, .92f), new Color32(28, 52, 47, 255));
            return value.GetComponent<Button>();
        }
        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max)
        { rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero; }
        private static void Set(SerializedObject target, string field, UnityEngine.Object value) => target.FindProperty(field).objectReferenceValue = value;
        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/');
            string leaf = System.IO.Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, leaf);
        }
    }
}
