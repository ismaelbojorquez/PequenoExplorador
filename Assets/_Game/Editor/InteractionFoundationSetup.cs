using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Interaction;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Interaction;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class InteractionFoundationSetup
    {
        public const string Root = "Assets/_Game/Content/Interaction";
        public const string DefinitionsRoot = Root + "/Definitions";
        public const string CatalogPath = Root + "/InteractionCatalog.asset";
        public const string AnimalPath = DefinitionsRoot + "/PH_Interaction_Animal.asset";
        public const string PlantPath = DefinitionsRoot + "/PH_Interaction_Plant.asset";
        public const string ObjectPath = DefinitionsRoot + "/PH_Interaction_Object.asset";
        public const string CanvasName = "PH_UI_INTERACTION_CANVAS";

        [MenuItem("Pequeño Explorador/Development/Interaction/Apply Foundation")]
        public static void Apply()
        {
            try
            {
                EnsureFolder(DefinitionsRoot);
                InteractionDefinitionAsset animal = EnsureDefinition(
                    AnimalPath,
                    "interaction.fixture.animal",
                    "content.interaction.fixture.animal.name",
                    70);
                SetDirectDiscovery(animal, "discovery.jungle.placeholder");
                InteractionDefinitionAsset plant = EnsureDefinition(
                    PlantPath,
                    "interaction.fixture.plant",
                    "content.interaction.fixture.plant.name",
                    60);
                InteractionDefinitionAsset genericObject = EnsureDefinition(
                    ObjectPath,
                    "interaction.fixture.object",
                    "content.interaction.fixture.object.name",
                    50);
                InteractionCatalogAsset catalog = EnsureCatalog(animal, plant, genericObject);
                ConfigureJungle();
                ConfigureBootstrap(catalog);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("PE_INTERACTION_SETUP_OK definitions=3 fixtures=3 promptSafeArea=true colliderIndex=true");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        private static InteractionDefinitionAsset EnsureDefinition(
            string path,
            string id,
            string displayNameKey,
            int priority)
        {
            InteractionDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<InteractionDefinitionAsset>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<InteractionDefinitionAsset>();
                asset.name = Path.GetFileNameWithoutExtension(path);
                AssetDatabase.CreateAsset(asset, path);
            }
            asset.ConfigureIdentityForEditorAndTests(id);
            var serialized = new SerializedObject(asset);
            serialized.FindProperty("_displayNameTable").stringValue = "Content";
            serialized.FindProperty("_displayNameKey").stringValue = displayNameKey;
            serialized.FindProperty("_promptTable").stringValue = "UI";
            serialized.FindProperty("_promptKey").stringValue = "ui.interaction.action";
            serialized.FindProperty("_unavailableTable").stringValue = "UI";
            serialized.FindProperty("_unavailableKey").stringValue = "ui.interaction.unavailable";
            serialized.FindProperty("_promptAudioCueId").stringValue = "audio.voice.instruction.explore";
            serialized.FindProperty("_unavailableAudioCueId").stringValue = "audio.feedback.retry";
            serialized.FindProperty("_interactionRange").floatValue = 1.35f;
            serialized.FindProperty("_cooldownSeconds").floatValue = 1.5f;
            serialized.FindProperty("_priority").intValue = priority;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static void SetDirectDiscovery(InteractionDefinitionAsset asset, string discoveryId)
        {
            var serialized = new SerializedObject(asset);
            serialized.FindProperty("_directDiscoveryId").stringValue = discoveryId;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static InteractionCatalogAsset EnsureCatalog(params InteractionDefinitionAsset[] definitions)
        {
            InteractionCatalogAsset catalog = AssetDatabase.LoadAssetAtPath<InteractionCatalogAsset>(CatalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<InteractionCatalogAsset>();
                catalog.name = "InteractionCatalog";
                AssetDatabase.CreateAsset(catalog, CatalogPath);
            }
            var serialized = new SerializedObject(catalog);
            SerializedProperty property = serialized.FindProperty("_definitions");
            property.arraySize = definitions.Length;
            for (int index = 0; index < definitions.Length; index++)
                property.GetArrayElementAtIndex(index).objectReferenceValue = definitions[index];
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        private static void ConfigureJungle()
        {
            Scene scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.JungleScenePath, OpenSceneMode.Single);
            RemoveRoot(scene, InteractionSceneRoot.PlaceholderRootName);
            var root = new GameObject(InteractionSceneRoot.PlaceholderRootName);
            InteractionDetector detector = root.AddComponent<InteractionDetector>();
            WorldInteractableView animal = CreateFixture(
                root.transform,
                "PH_FIXTURE_ANIMAL",
                "interaction.fixture.animal",
                PrimitiveType.Capsule,
                new Vector3(-4.2f, 0f, 2.6f),
                new Vector3(1.25f, 0f, 0f),
                new Color(0.94f, 0.56f, 0.22f),
                true);
            WorldInteractableView plant = CreateFixture(
                root.transform,
                "PH_FIXTURE_PLANT",
                "interaction.fixture.plant",
                PrimitiveType.Cylinder,
                new Vector3(4.2f, 0f, 2.8f),
                new Vector3(-1.25f, 0f, 0f),
                new Color(0.23f, 0.78f, 0.42f),
                true);
            WorldInteractableView genericObject = CreateFixture(
                root.transform,
                "PH_FIXTURE_OBJECT",
                "interaction.fixture.object",
                PrimitiveType.Cube,
                new Vector3(0f, 0f, 4.7f),
                new Vector3(0f, 0f, -1.25f),
                new Color(0.35f, 0.58f, 0.92f),
                false);
            InteractionSceneRoot sceneRoot = root.AddComponent<InteractionSceneRoot>();
            var serialized = new SerializedObject(sceneRoot);
            serialized.FindProperty("_detector").objectReferenceValue = detector;
            SerializedProperty targets = serialized.FindProperty("_targets");
            targets.arraySize = 3;
            targets.GetArrayElementAtIndex(0).objectReferenceValue = animal;
            targets.GetArrayElementAtIndex(1).objectReferenceValue = plant;
            targets.GetArrayElementAtIndex(2).objectReferenceValue = genericObject;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static WorldInteractableView CreateFixture(
            Transform parent,
            string name,
            string id,
            PrimitiveType primitive,
            Vector3 position,
            Vector3 pointOffset,
            Color color,
            bool available)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            GameObject visual = GameObject.CreatePrimitive(primitive);
            visual.name = name + "_VISUAL";
            visual.transform.SetParent(root.transform, false);
            visual.transform.localPosition = new Vector3(0f, primitive == PrimitiveType.Cube ? 0.55f : 0.7f, 0f);
            visual.transform.localScale = primitive == PrimitiveType.Capsule
                ? new Vector3(1.1f, 0.9f, 1.1f)
                : primitive == PrimitiveType.Cylinder
                    ? new Vector3(1.25f, 0.65f, 1.25f)
                    : new Vector3(1.2f, 1.1f, 1.2f);
            Renderer renderer = visual.GetComponent<Renderer>();
            renderer.sharedMaterial = CreateFixtureMaterial(name, color);
            Collider collider = visual.GetComponent<Collider>();
            collider.isTrigger = true;

            var point = new GameObject("PH_INTERACTION_POINT");
            point.transform.SetParent(root.transform, false);
            point.transform.localPosition = pointOffset;

            GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            indicator.name = "PH_FOCUS_INDICATOR";
            indicator.transform.SetParent(root.transform, false);
            indicator.transform.localPosition = new Vector3(0f, 0.04f, 0f);
            indicator.transform.localScale = new Vector3(1.5f, 0.025f, 1.5f);
            UnityEngine.Object.DestroyImmediate(indicator.GetComponent<Collider>());
            indicator.GetComponent<Renderer>().sharedMaterial =
                AssetDatabase.LoadAssetAtPath<Material>(ExplorerFoundationSetup.DestinationMaterialPath);
            indicator.SetActive(false);

            WorldInteractableView view = root.AddComponent<WorldInteractableView>();
            var serialized = new SerializedObject(view);
            serialized.FindProperty("_interactionId").stringValue = id;
            serialized.FindProperty("_interactionPoint").objectReferenceValue = point.transform;
            SerializedProperty colliders = serialized.FindProperty("_targetColliders");
            colliders.arraySize = 1;
            colliders.GetArrayElementAtIndex(0).objectReferenceValue = collider;
            serialized.FindProperty("_focusIndicator").objectReferenceValue = indicator;
            serialized.FindProperty("_available").boolValue = available;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return view;
        }

        private static Material CreateFixtureMaterial(string name, Color color)
        {
            string path = Root + "/" + name + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) throw new InvalidOperationException("URP Lit shader is unavailable.");
                material = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(material, path);
            }
            material.color = color;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void ConfigureBootstrap(InteractionCatalogAsset catalog)
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            RemoveRoot(scene, CanvasName);
            GameObject canvasObject = new GameObject(
                CanvasName,
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster),
                typeof(InteractionPromptView));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 70;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var safeObject = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safeObject.transform.SetParent(canvasObject.transform, false);
            RectTransform safe = (RectTransform)safeObject.transform;
            SetRect(safe, Vector2.zero, Vector2.one);

            var panel = new GameObject("PH_INTERACTION_PANEL", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(safe, false);
            RectTransform panelRect = (RectTransform)panel.transform;
            SetRect(panelRect, new Vector2(0.27f, 0.04f), new Vector2(0.73f, 0.34f));
            panel.GetComponent<Image>().color = new Color(0.025f, 0.12f, 0.13f, 0.94f);

            var iconObject = new GameObject("PH_INTERACTION_ICON", typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(panel.transform, false);
            RectTransform iconRect = (RectTransform)iconObject.transform;
            SetRect(iconRect, new Vector2(0.04f, 0.42f), new Vector2(0.18f, 0.86f));
            Image icon = iconObject.GetComponent<Image>();
            icon.color = new Color(0.98f, 0.78f, 0.25f, 1f);
            icon.raycastTarget = false;

            Text nameText = CreateText(panel.transform, "PH_INTERACTION_NAME", 36, TextAnchor.MiddleLeft);
            SetRect(nameText.rectTransform, new Vector2(0.21f, 0.62f), new Vector2(0.94f, 0.9f));
            Text statusText = CreateText(panel.transform, "PH_INTERACTION_STATUS", 28, TextAnchor.MiddleLeft);
            SetRect(statusText.rectTransform, new Vector2(0.21f, 0.37f), new Vector2(0.94f, 0.63f));
            Button action = CreateButton(panel.transform, "PH_INTERACTION_ACTION");
            SetRect((RectTransform)action.transform, new Vector2(0.22f, 0.05f), new Vector2(0.70f, 0.35f));
            Button cancel = CreateButton(panel.transform, "PH_INTERACTION_CANCEL");
            SetRect((RectTransform)cancel.transform, new Vector2(0.75f, 0.05f), new Vector2(0.96f, 0.35f));

            InteractionPromptView prompt = canvasObject.GetComponent<InteractionPromptView>();
            var promptSerialized = new SerializedObject(prompt);
            promptSerialized.FindProperty("_panel").objectReferenceValue = panel;
            promptSerialized.FindProperty("_nameText").objectReferenceValue = nameText;
            promptSerialized.FindProperty("_statusText").objectReferenceValue = statusText;
            promptSerialized.FindProperty("_actionButton").objectReferenceValue = action;
            promptSerialized.FindProperty("_cancelButton").objectReferenceValue = cancel;
            promptSerialized.FindProperty("_icon").objectReferenceValue = icon;
            promptSerialized.ApplyModifiedPropertiesWithoutUndo();
            panel.SetActive(false);

            DiagnosticBootstrap bootstrap = scene.GetRootGameObjects()
                .SelectMany(item => item.GetComponentsInChildren<DiagnosticBootstrap>(true)).Single();
            bootstrap.ConfigureInteractionsForEditorAndTests(catalog, prompt);
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

        private static Text CreateText(Transform parent, string name, int size, TextAnchor anchor)
        {
            var value = new GameObject(name, typeof(RectTransform), typeof(Text));
            value.transform.SetParent(parent, false);
            Text text = value.GetComponent<Text>();
            text.text = string.Empty;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 18;
            text.alignment = anchor;
            text.color = Color.white;
            text.raycastTarget = false;
            return text;
        }

        private static Button CreateButton(Transform parent, string name)
        {
            GameObject value = DefaultControls.CreateButton(new DefaultControls.Resources());
            value.name = name;
            value.transform.SetParent(parent, false);
            Text label = value.GetComponentInChildren<Text>();
            label.text = string.Empty;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 28;
            label.resizeTextForBestFit = true;
            return value.GetComponent<Button>();
        }

        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void RemoveRoot(Scene scene, string name)
        {
            GameObject existing = scene.GetRootGameObjects().FirstOrDefault(item => item.name == name);
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}
