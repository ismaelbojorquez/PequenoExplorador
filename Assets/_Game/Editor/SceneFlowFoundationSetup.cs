using System;
using System.IO;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Infrastructure.SceneFlow;
using PequenoExplorador.Presentation.SceneFlow;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class SceneFlowFoundationSetup
    {
        public const string CampScenePath = "Assets/_Game/Worlds/Camp/Camp.unity";
        public const string JungleScenePath = "Assets/_Game/Worlds/Jungle/Jungle.unity";
        public const string SharedGroupName = "SharedLocal";
        public const string JungleGroupName = "JungleLocal";
        public const string DevelopmentProfileName = "LocalDevelopment";
        public const string ReleaseProfileName = "LocalRelease";
        public const string SceneLabel = "scene";
        public const string SharedLabel = "shared-local";
        public const string JungleLabel = "world-jungle";

        [MenuItem("Pequeño Explorador/Setup Local Scene Flow")]
        public static void Apply()
        {
            try
            {
                EnsureFolder("Assets/_Game/Worlds/Camp");
                EnsureFolder("Assets/_Game/Worlds/Jungle");
                CreateWorldScene(CampScenePath, "PH_WORLD_CAMP", "Campamento · PLACEHOLDER", new Color(0.12f, 0.34f, 0.25f));
                CreateWorldScene(JungleScenePath, "PH_WORLD_JUNGLE", "Expedición Selva · PLACEHOLDER", new Color(0.05f, 0.23f, 0.12f));
                ConfigureBootstrapScene();
                ConfigureAddressables();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("PE_SCENE_FLOW_SETUP_OK profiles=LocalDevelopment,LocalRelease groups=SharedLocal,JungleLocal remote=false");
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(0);
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(2);
                }

                throw;
            }
        }

        private static void ConfigureBootstrapScene()
        {
            Scene scene = EditorSceneManager.OpenScene(
                "Assets/_Game/Bootstrap/Bootstrap.unity",
                OpenSceneMode.Single);
            DiagnosticBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<DiagnosticBootstrap>();
            if (bootstrap == null)
            {
                throw new InvalidOperationException("Bootstrap scene has no DiagnosticBootstrap.");
            }

            Canvas existingCanvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
            if (existingCanvas != null)
            {
                existingCanvas.sortingOrder = -10;
            }

            GameObject oldFlow = GameObject.Find("PH_UI_SCENE_FLOW");
            if (oldFlow != null)
            {
                UnityEngine.Object.DestroyImmediate(oldFlow);
            }

            GameObject flowRoot = CreateCanvas("PH_UI_SCENE_FLOW", 100);
            SceneTransitionView view = flowRoot.AddComponent<SceneTransitionView>();
            GameObject transitionPanel = CreatePanel(flowRoot.transform, "Transition Panel", new Color(0.02f, 0.06f, 0.08f, 0.93f));
            Text status = CreateText(transitionPanel.transform, "Transition Status", 34, TextAnchor.MiddleCenter);
            SetRect(status.rectTransform, new Vector2(0.2f, 0.55f), new Vector2(0.8f, 0.72f), Vector2.zero, Vector2.zero);
            Slider progress = DefaultControls.CreateSlider(new DefaultControls.Resources()).GetComponent<Slider>();
            progress.name = "Transition Progress";
            progress.transform.SetParent(transitionPanel.transform, false);
            SetRect((RectTransform)progress.transform, new Vector2(0.3f, 0.45f), new Vector2(0.7f, 0.51f), Vector2.zero, Vector2.zero);
            Button retry = CreateButton(transitionPanel.transform, "Retry", "Intentar otra vez");
            SetRect((RectTransform)retry.transform, new Vector2(0.38f, 0.29f), new Vector2(0.62f, 0.4f), Vector2.zero, Vector2.zero);

            GameObject controls = new GameObject("Development Controls", typeof(RectTransform));
            controls.transform.SetParent(flowRoot.transform, false);
            SetRect((RectTransform)controls.transform, new Vector2(0.66f, 0.02f), new Vector2(0.98f, 0.28f), Vector2.zero, Vector2.zero);
            Button enter = CreateButton(controls.transform, "Enter Jungle", "Ir a Selva");
            Button back = CreateButton(controls.transform, "Return Camp", "Volver al campamento");
            Button fail = CreateButton(controls.transform, "Simulate Failure", "Simular fallo");
            SetRect((RectTransform)enter.transform, new Vector2(0f, 0.68f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            SetRect((RectTransform)back.transform, new Vector2(0f, 0.34f), new Vector2(1f, 0.66f), Vector2.zero, Vector2.zero);
            SetRect((RectTransform)fail.transform, new Vector2(0f, 0f), new Vector2(1f, 0.32f), Vector2.zero, Vector2.zero);

            var serializedView = new SerializedObject(view);
            serializedView.FindProperty("_transitionPanel").objectReferenceValue = transitionPanel;
            serializedView.FindProperty("_statusText").objectReferenceValue = status;
            serializedView.FindProperty("_progress").objectReferenceValue = progress;
            serializedView.FindProperty("_enterJungleButton").objectReferenceValue = enter;
            serializedView.FindProperty("_returnCampButton").objectReferenceValue = back;
            serializedView.FindProperty("_retryButton").objectReferenceValue = retry;
            serializedView.FindProperty("_simulateFailureButton").objectReferenceValue = fail;
            serializedView.FindProperty("_developmentControls").objectReferenceValue = controls;
            serializedView.ApplyModifiedPropertiesWithoutUndo();

            var serializedBootstrap = new SerializedObject(bootstrap);
            serializedBootstrap.FindProperty("_sceneFlowView").objectReferenceValue = view;
            serializedBootstrap.ApplyModifiedPropertiesWithoutUndo();

            if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            }

            transitionPanel.SetActive(true);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureAddressables()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                settings = AddressableAssetSettings.Create(
                    AddressableAssetSettingsDefaultObject.kDefaultConfigFolder,
                    AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName,
                    false,
                    true);
                AddressableAssetSettingsDefaultObject.Settings = settings;
            }

            string defaultProfile = settings.activeProfileId;
            string developmentProfile = settings.profileSettings.GetProfileId(DevelopmentProfileName);
            if (string.IsNullOrEmpty(developmentProfile))
            {
                developmentProfile = settings.profileSettings.AddProfile(DevelopmentProfileName, defaultProfile);
            }

            string releaseProfile = settings.profileSettings.GetProfileId(ReleaseProfileName);
            if (string.IsNullOrEmpty(releaseProfile))
            {
                releaseProfile = settings.profileSettings.AddProfile(ReleaseProfileName, developmentProfile);
            }

            settings.activeProfileId = developmentProfile;
            settings.BuildRemoteCatalog = false;
            settings.DisableCatalogUpdateOnStartup = true;
            settings.BuildAddressablesWithPlayerBuild = AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer;
            settings.ContentStateBuildPath = "Library/com.unity.addressables/ContentState";
            settings.AddLabel(SceneLabel);
            settings.AddLabel(SharedLabel);
            settings.AddLabel(JungleLabel);

            AddressableAssetGroup shared = EnsureLocalGroup(settings, SharedGroupName, true);
            AddressableAssetGroup jungle = EnsureLocalGroup(settings, JungleGroupName, false);
            ConfigureEntry(settings, shared, CampScenePath, LocalSceneAddresses.Camp, SceneLabel, SharedLabel);
            ConfigureEntry(settings, jungle, JungleScenePath, LocalSceneAddresses.Jungle, SceneLabel, JungleLabel);
            EditorUtility.SetDirty(settings);
        }

        private static AddressableAssetGroup EnsureLocalGroup(
            AddressableAssetSettings settings,
            string name,
            bool defaultGroup)
        {
            AddressableAssetGroup group = settings.FindGroup(name);
            if (group == null)
            {
                group = settings.CreateGroup(
                    name,
                    defaultGroup,
                    false,
                    false,
                    null,
                    typeof(ContentUpdateGroupSchema),
                    typeof(BundledAssetGroupSchema));
            }

            settings.DefaultGroup = defaultGroup ? group : settings.DefaultGroup;
            BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
            schema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kLocalBuildPath);
            schema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kLocalLoadPath);
            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            schema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
            schema.IncludeInBuild = true;
            schema.UseUnityWebRequestForLocalBundles = false;
            EditorUtility.SetDirty(schema);
            return group;
        }

        private static void ConfigureEntry(
            AddressableAssetSettings settings,
            AddressableAssetGroup group,
            string assetPath,
            string address,
            params string[] labels)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid))
            {
                throw new InvalidOperationException("Addressable scene is missing: " + assetPath);
            }

            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group, false, false);
            entry.address = address;
            foreach (string label in labels)
            {
                entry.SetLabel(label, true, true, false);
            }
        }

        private static void CreateWorldScene(string path, string rootName, string title, Color background)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject(rootName);
            GameObject canvasObject = CreateCanvas("World Canvas", 0);
            canvasObject.transform.SetParent(root.transform, false);
            GameObject panel = CreatePanel(canvasObject.transform, "World Background", background);
            Text label = CreateText(panel.transform, "World Label", 48, TextAnchor.MiddleCenter);
            label.text = title;
            SetRect(label.rectTransform, new Vector2(0.1f, 0.35f), new Vector2(0.9f, 0.65f), Vector2.zero, Vector2.zero);
            EditorSceneManager.SaveScene(scene, path);
        }

        private static GameObject CreateCanvas(string name, int sortingOrder)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            return root;
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = color;
            SetRect((RectTransform)panel.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return panel;
        }

        private static Text CreateText(Transform parent, string name, int size, TextAnchor alignment)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(parent, false);
            Text text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.alignment = alignment;
            text.color = Color.white;
            text.text = name;
            return text;
        }

        private static Button CreateButton(Transform parent, string name, string label)
        {
            GameObject buttonObject = DefaultControls.CreateButton(new DefaultControls.Resources());
            buttonObject.name = name;
            buttonObject.transform.SetParent(parent, false);
            Text text = buttonObject.GetComponentInChildren<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 24;
            text.text = label;
            return buttonObject.GetComponent<Button>();
        }

        private static void SetRect(
            RectTransform rect,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 offsetMin,
            Vector2 offsetMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void EnsureFolder(string path)
        {
            string[] segments = path.Split('/');
            string current = segments[0];
            for (int index = 1; index < segments.Length; index++)
            {
                string next = current + "/" + segments[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[index]);
                }

                current = next;
            }
        }
    }
}
