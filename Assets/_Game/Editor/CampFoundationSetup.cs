using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Camp;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Camp;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class CampFoundationSetup
    {
        public const string Root = "Assets/_Game/Content/Camp";
        public const string StationsRoot = Root + "/Stations";
        public const string UpgradesRoot = Root + "/Upgrades";
        public const string VisualsRoot = Root + "/Visuals";
        public const string CatalogPath = Root + "/CampCatalog.asset";
        public const string UpgradePath = UpgradesRoot + "/PH_Upgrade_ObservationCorner.asset";
        public const string BeforePrefabPath = VisualsRoot + "/PH_CampObservationTable_Before.prefab";
        public const string AfterPrefabPath = VisualsRoot + "/PH_CampObservationCorner_After.prefab";
        public const string UpgradeId = "camp-upgrade.observation-corner";

        private static readonly StationSeed[] StationSeeds =
        {
            new StationSeed("camp-station.expedition", "camp-action.expedition", "expedition", 0, true, false),
            new StationSeed("camp-station.album", "camp-action.album", "album", 1, true, false),
            new StationSeed("camp-station.customization", "camp-action.customization", "customization", 2, true, false),
            new StationSeed("camp-station.parents", "camp-action.parents", "parents", 3, false, true)
        };

        [MenuItem("Pequeño Explorador/Development/Camp/Apply Foundation")]
        public static void Apply()
        {
            EnsureFolders();
            LocalizationFoundationSetup.ApplyCampEntries();
            Material wood = EnsureMaterial(VisualsRoot + "/PH_Camp_Wood.mat", new Color(0.48f, 0.27f, 0.13f));
            Material accent = EnsureMaterial(VisualsRoot + "/PH_Camp_Accent.mat", new Color(0.16f, 0.55f, 0.39f));
            Material paper = EnsureMaterial(VisualsRoot + "/PH_Camp_Paper.mat", new Color(0.94f, 0.85f, 0.57f));
            GameObject beforePrefab = CreateObservationPrefab(BeforePrefabPath, false, wood, accent, paper);
            GameObject afterPrefab = CreateObservationPrefab(AfterPrefabPath, true, wood, accent, paper);
            CampStationDefinitionAsset[] stations = EnsureStationAssets();
            CampUpgradeDefinitionAsset upgrade = EnsureUpgradeAsset(beforePrefab, afterPrefab);
            CampCatalogAsset catalog = EnsureCatalog(stations, upgrade);
            ConfigureAddressables();
            ConfigureCampScene(beforePrefab, afterPrefab, stations);
            ConfigureBootstrap(catalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("PE_CAMP_SETUP_OK stations=4 upgrades=1 cost=3 parentGate=false purchases=0");
            if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
        }

        public static void ApplyCli()
        {
            try { Apply(); }
            catch (Exception exception) { Debug.LogException(exception); EditorApplication.Exit(2); }
        }

        private static CampStationDefinitionAsset[] EnsureStationAssets()
        {
            var result = new List<CampStationDefinitionAsset>();
            foreach (StationSeed seed in StationSeeds)
            {
                string path = StationsRoot + "/PH_Station_" + char.ToUpperInvariant(seed.Slug[0]) + seed.Slug.Substring(1) + ".asset";
                CampStationDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<CampStationDefinitionAsset>(path);
                if (asset == null) { asset = ScriptableObject.CreateInstance<CampStationDefinitionAsset>(); AssetDatabase.CreateAsset(asset, path); }
                var serialized = new SerializedObject(asset);
                serialized.FindProperty("_id").stringValue = seed.Id;
                serialized.FindProperty("_actionId").stringValue = seed.ActionId;
                serialized.FindProperty("_displayNameTable").stringValue = "UI";
                serialized.FindProperty("_displayNameKey").stringValue = "ui.camp.station." + seed.Slug + ".name";
                serialized.FindProperty("_descriptionTable").stringValue = "UI";
                serialized.FindProperty("_descriptionKey").stringValue = "ui.camp.station." + seed.Slug + ".description";
                serialized.FindProperty("_displayOrder").intValue = seed.Order;
                serialized.FindProperty("_available").boolValue = seed.Available;
                serialized.FindProperty("_parentRestricted").boolValue = seed.ParentRestricted;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                result.Add(asset);
            }
            return result.ToArray();
        }

        private static CampUpgradeDefinitionAsset EnsureUpgradeAsset(GameObject beforePrefab, GameObject afterPrefab)
        {
            CampUpgradeDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<CampUpgradeDefinitionAsset>(UpgradePath);
            if (asset == null) { asset = ScriptableObject.CreateInstance<CampUpgradeDefinitionAsset>(); AssetDatabase.CreateAsset(asset, UpgradePath); }
            var serialized = new SerializedObject(asset);
            serialized.FindProperty("_id").stringValue = UpgradeId;
            serialized.FindProperty("_stationId").stringValue = "camp-station.album";
            serialized.FindProperty("_displayNameTable").stringValue = "UI";
            serialized.FindProperty("_displayNameKey").stringValue = "ui.camp.upgrade.observation.name";
            serialized.FindProperty("_descriptionTable").stringValue = "UI";
            serialized.FindProperty("_descriptionKey").stringValue = "ui.camp.upgrade.observation.description";
            serialized.FindProperty("_previewTable").stringValue = "UI";
            serialized.FindProperty("_previewKey").stringValue = "ui.camp.upgrade.observation.preview";
            serialized.FindProperty("_starCost").intValue = 3;
            serialized.FindProperty("_spendReasonId").stringValue = "reward.camp-upgrade.observation-corner";
            serialized.FindProperty("_beforeVisualId").stringValue = "visual.camp.observation-table.before";
            serialized.FindProperty("_afterVisualId").stringValue = "visual.camp.observation-corner.after";
            SetAssetReference(serialized.FindProperty("_beforeVariant"), AssetDatabase.AssetPathToGUID(BeforePrefabPath));
            SetAssetReference(serialized.FindProperty("_afterVariant"), AssetDatabase.AssetPathToGUID(AfterPrefabPath));
            serialized.FindProperty("_prerequisiteIds").arraySize = 0;
            SerializedProperty editorial = serialized.FindProperty("_editorial");
            editorial.FindPropertyRelative("_state").enumValueIndex = 0;
            editorial.FindPropertyRelative("_isPlaceholder").boolValue = true;
            editorial.FindPropertyRelative("_owner").stringValue = "Camp Design";
            editorial.FindPropertyRelative("_developmentWatermark").stringValue = "BORRADOR · PH_CAMP_UPGRADE";
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return asset;
        }

        private static CampCatalogAsset EnsureCatalog(CampStationDefinitionAsset[] stations, CampUpgradeDefinitionAsset upgrade)
        {
            CampCatalogAsset catalog = AssetDatabase.LoadAssetAtPath<CampCatalogAsset>(CatalogPath);
            if (catalog == null) { catalog = ScriptableObject.CreateInstance<CampCatalogAsset>(); AssetDatabase.CreateAsset(catalog, CatalogPath); }
            var serialized = new SerializedObject(catalog);
            SerializedProperty stationArray = serialized.FindProperty("_stations");
            stationArray.arraySize = stations.Length;
            for (int index = 0; index < stations.Length; index++) stationArray.GetArrayElementAtIndex(index).objectReferenceValue = stations[index];
            SerializedProperty upgradeArray = serialized.FindProperty("_upgrades");
            upgradeArray.arraySize = 1; upgradeArray.GetArrayElementAtIndex(0).objectReferenceValue = upgrade;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return catalog;
        }

        private static GameObject CreateObservationPrefab(string path, bool upgraded, Material wood, Material accent, Material paper)
        {
            GameObject root = new GameObject(upgraded ? "PH_ObservationCorner_After" : "PH_ObservationTable_Before");
            CreatePart(root.transform, "Table Top", PrimitiveType.Cube, new Vector3(0f, 0.82f, 0f), new Vector3(2.8f, 0.18f, 1.35f), wood);
            CreatePart(root.transform, "Leg L", PrimitiveType.Cube, new Vector3(-1.05f, 0.4f, 0f), new Vector3(0.18f, 0.8f, 1.05f), wood);
            CreatePart(root.transform, "Leg R", PrimitiveType.Cube, new Vector3(1.05f, 0.4f, 0f), new Vector3(0.18f, 0.8f, 1.05f), wood);
            CreatePart(root.transform, "Field Notes", PrimitiveType.Cube, new Vector3(-0.55f, 0.96f, 0f), new Vector3(0.8f, 0.04f, 0.62f), paper);
            if (upgraded)
            {
                CreatePart(root.transform, "Photo Stand", PrimitiveType.Cube, new Vector3(0.62f, 1.25f, 0.16f), new Vector3(0.85f, 0.68f, 0.08f), accent);
                CreatePart(root.transform, "Magnifier", PrimitiveType.Cylinder, new Vector3(0.6f, 1.0f, -0.32f), new Vector3(0.24f, 0.04f, 0.24f), accent);
                CreatePart(root.transform, "Plant Pot", PrimitiveType.Cylinder, new Vector3(-1.05f, 1.12f, 0.15f), new Vector3(0.28f, 0.28f, 0.28f), accent);
            }
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static void ConfigureCampScene(GameObject beforePrefab, GameObject afterPrefab, CampStationDefinitionAsset[] stations)
        {
            Scene scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.CampScenePath, OpenSceneMode.Single);
            foreach (GameObject existing in scene.GetRootGameObjects().Where(value => value.name == "PH_CAMP_LAYOUT"))
                UnityEngine.Object.DestroyImmediate(existing);
            var root = new GameObject("PH_CAMP_LAYOUT", typeof(CampSceneRoot));
            CreatePart(root.transform, "Ground", PrimitiveType.Cube, new Vector3(0f, -0.15f, 0f), new Vector3(12f, 0.3f, 8f),
                EnsureMaterial(VisualsRoot + "/PH_Camp_Ground.mat", new Color(0.18f, 0.39f, 0.25f)));
            Vector3[] positions = { new Vector3(-4f, 0.3f, 2.4f), new Vector3(4f, 0.3f, 2.4f), new Vector3(-4f, 0.3f, -2.4f), new Vector3(4f, 0.3f, -2.4f) };
            var anchors = new List<CampStationAnchorView>();
            for (int index = 0; index < StationSeeds.Length; index++)
            {
                GameObject anchor = CreatePart(root.transform, "PH_Anchor_" + StationSeeds[index].Slug, PrimitiveType.Cylinder,
                    positions[index], new Vector3(0.7f, 0.12f, 0.7f),
                    EnsureMaterial(VisualsRoot + "/PH_Camp_Anchor.mat", new Color(0.93f, 0.66f, 0.24f)));
                CampStationAnchorView view = anchor.AddComponent<CampStationAnchorView>();
                var serialized = new SerializedObject(view); serialized.FindProperty("_stationId").stringValue = StationSeeds[index].Id;
                serialized.ApplyModifiedPropertiesWithoutUndo(); anchors.Add(view);
            }
            var visualRoot = new GameObject("PH_ObservationUpgrade", typeof(CampUpgradeVisualView));
            visualRoot.transform.SetParent(root.transform, false); visualRoot.transform.localPosition = new Vector3(0f, 0f, 0f);
            GameObject before = (GameObject)PrefabUtility.InstantiatePrefab(beforePrefab, scene); before.transform.SetParent(visualRoot.transform, false);
            GameObject after = (GameObject)PrefabUtility.InstantiatePrefab(afterPrefab, scene); after.transform.SetParent(visualRoot.transform, false); after.SetActive(false);
            CampUpgradeVisualView visual = visualRoot.GetComponent<CampUpgradeVisualView>();
            var visualSerialized = new SerializedObject(visual);
            visualSerialized.FindProperty("_upgradeId").stringValue = UpgradeId;
            visualSerialized.FindProperty("_beforeVariant").objectReferenceValue = before;
            visualSerialized.FindProperty("_afterVariant").objectReferenceValue = after;
            visualSerialized.ApplyModifiedPropertiesWithoutUndo();
            var rootSerialized = new SerializedObject(root.GetComponent<CampSceneRoot>());
            SetObjectArray(rootSerialized.FindProperty("_anchors"), anchors.Cast<UnityEngine.Object>().ToArray());
            SetObjectArray(rootSerialized.FindProperty("_upgradeVisuals"), new UnityEngine.Object[] { visual });
            rootSerialized.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureBootstrap(CampCatalogAsset catalog)
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            foreach (GameObject root in scene.GetRootGameObjects().Where(value => value.name == CampHubView.PlaceholderObjectName))
                UnityEngine.Object.DestroyImmediate(root);
            var canvasObject = new GameObject(CampHubView.PlaceholderObjectName, typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CampHubView));
            Canvas canvas = canvasObject.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 112;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f); scaler.matchWidthOrHeight = 0.5f;
            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter)); safe.transform.SetParent(canvasObject.transform, false); Stretch((RectTransform)safe.transform);
            GameObject panel = CreateUiPanel(safe.transform, "PH_Camp Hub Panel", new Color(0.04f, 0.18f, 0.14f, 0.93f));
            SetRect((RectTransform)panel.transform, new Vector2(0.02f, 0.08f), new Vector2(0.67f, 0.88f));
            Text title = CreateText(panel.transform, "Title", 44); SetRect(title.rectTransform, new Vector2(0.05f, 0.84f), new Vector2(0.95f, 0.97f));
            var stationViews = new List<CampStationButtonView>();
            Vector2[] mins = { new Vector2(0.05f, 0.53f), new Vector2(0.52f, 0.53f), new Vector2(0.05f, 0.20f), new Vector2(0.52f, 0.08f) };
            Vector2[] maxs = { new Vector2(0.48f, 0.80f), new Vector2(0.95f, 0.80f), new Vector2(0.48f, 0.47f), new Vector2(0.95f, 0.35f) };
            for (int index = 0; index < StationSeeds.Length; index++)
            {
                CampStationButtonView view = CreateStationButton(panel.transform, StationSeeds[index].Slug,
                    StationSeeds[index].ParentRestricted ? new Color(0.22f, 0.28f, 0.35f, 0.96f) : new Color(0.16f, 0.48f, 0.34f, 0.96f));
                SetRect((RectTransform)view.transform, mins[index], maxs[index]); stationViews.Add(view);
            }
            Button upgradeButton = CreateButton(safe.transform, "PH_Observation Upgrade", new Color(0.82f, 0.45f, 0.15f, 0.98f));
            SetRect((RectTransform)upgradeButton.transform, new Vector2(0.70f, 0.56f), new Vector2(0.97f, 0.76f));
            Text upgradeLabel = upgradeButton.GetComponentInChildren<Text>(true);
            Text feedback = CreateText(safe.transform, "Camp Feedback", 28); SetRect(feedback.rectTransform, new Vector2(0.70f, 0.40f), new Vector2(0.97f, 0.54f));
            GameObject preview = CreateUiPanel(safe.transform, "PH_Upgrade Preview", new Color(0.04f, 0.12f, 0.11f, 0.98f));
            SetRect((RectTransform)preview.transform, new Vector2(0.22f, 0.17f), new Vector2(0.78f, 0.83f));
            Text previewTitle = CreateText(preview.transform, "Preview Title", 42); SetRect(previewTitle.rectTransform, new Vector2(0.08f, 0.72f), new Vector2(0.92f, 0.91f));
            Text previewDescription = CreateText(preview.transform, "Preview Description", 30); SetRect(previewDescription.rectTransform, new Vector2(0.08f, 0.42f), new Vector2(0.92f, 0.70f));
            Text previewCost = CreateText(preview.transform, "Preview Cost", 34); SetRect(previewCost.rectTransform, new Vector2(0.08f, 0.28f), new Vector2(0.92f, 0.42f));
            Button confirm = CreateButton(preview.transform, "Confirm Upgrade", new Color(0.16f, 0.58f, 0.34f, 1f));
            SetRect((RectTransform)confirm.transform, new Vector2(0.12f, 0.08f), new Vector2(0.48f, 0.24f));
            Button cancel = CreateButton(preview.transform, "Cancel Upgrade", new Color(0.35f, 0.39f, 0.41f, 1f));
            SetRect((RectTransform)cancel.transform, new Vector2(0.52f, 0.08f), new Vector2(0.88f, 0.24f));
            Text confirmLabel = confirm.GetComponentInChildren<Text>(true); Text cancelLabel = cancel.GetComponentInChildren<Text>(true);
            preview.SetActive(false);
            var viewSerialized = new SerializedObject(canvasObject.GetComponent<CampHubView>());
            viewSerialized.FindProperty("_panel").objectReferenceValue = panel;
            viewSerialized.FindProperty("_title").objectReferenceValue = title;
            SetObjectArray(viewSerialized.FindProperty("_stationButtons"), stationViews.Cast<UnityEngine.Object>().ToArray());
            viewSerialized.FindProperty("_upgradeButton").objectReferenceValue = upgradeButton;
            viewSerialized.FindProperty("_upgradeButtonLabel").objectReferenceValue = upgradeLabel;
            viewSerialized.FindProperty("_previewPanel").objectReferenceValue = preview;
            viewSerialized.FindProperty("_previewTitle").objectReferenceValue = previewTitle;
            viewSerialized.FindProperty("_previewDescription").objectReferenceValue = previewDescription;
            viewSerialized.FindProperty("_previewCost").objectReferenceValue = previewCost;
            viewSerialized.FindProperty("_confirmButton").objectReferenceValue = confirm;
            viewSerialized.FindProperty("_confirmLabel").objectReferenceValue = confirmLabel;
            viewSerialized.FindProperty("_cancelButton").objectReferenceValue = cancel;
            viewSerialized.FindProperty("_cancelLabel").objectReferenceValue = cancelLabel;
            viewSerialized.FindProperty("_feedback").objectReferenceValue = feedback;
            viewSerialized.ApplyModifiedPropertiesWithoutUndo();
            DiagnosticBootstrap bootstrap = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<DiagnosticBootstrap>(true)).Single();
            bootstrap.ConfigureCampForEditorAndTests(catalog, canvasObject.GetComponent<CampHubView>());
            var bootstrapSerialized = new SerializedObject(bootstrap);
            SerializedProperty fitters = bootstrapSerialized.FindProperty("_safeAreaFitters");
            var existing = new List<SafeAreaFitter>();
            for (int index = 0; index < fitters.arraySize; index++)
            {
                SafeAreaFitter value = fitters.GetArrayElementAtIndex(index).objectReferenceValue as SafeAreaFitter;
                if (value != null && !existing.Contains(value)) existing.Add(value);
            }
            existing.Add(safe.GetComponent<SafeAreaFitter>());
            SetObjectArray(fitters, existing.Cast<UnityEngine.Object>().ToArray());
            bootstrapSerialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(bootstrap); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        }

        private static CampStationButtonView CreateStationButton(Transform parent, string slug, Color color)
        {
            Button button = CreateButton(parent, "Station " + slug, color);
            Text title = button.GetComponentInChildren<Text>(true); title.name = "Station Title"; title.fontSize = 31;
            SetRect(title.rectTransform, new Vector2(0.06f, 0.46f), new Vector2(0.94f, 0.92f));
            Text description = CreateText(button.transform, "Station Description", 21);
            SetRect(description.rectTransform, new Vector2(0.06f, 0.08f), new Vector2(0.94f, 0.46f));
            GameObject parentBadge = new GameObject("Parent Badge", typeof(RectTransform), typeof(Image)); parentBadge.transform.SetParent(button.transform, false);
            parentBadge.GetComponent<Image>().color = new Color(0.95f, 0.72f, 0.23f, 0.9f);
            SetRect((RectTransform)parentBadge.transform, new Vector2(0.80f, 0.78f), new Vector2(0.96f, 0.94f));
            parentBadge.SetActive(false);
            CampStationButtonView view = button.gameObject.AddComponent<CampStationButtonView>();
            var serialized = new SerializedObject(view);
            serialized.FindProperty("_button").objectReferenceValue = button;
            serialized.FindProperty("_title").objectReferenceValue = title;
            serialized.FindProperty("_description").objectReferenceValue = description;
            serialized.FindProperty("_parentBadge").objectReferenceValue = parentBadge;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return view;
        }

        private static Button CreateButton(Transform parent, string name, Color color)
        {
            Button button = DefaultControls.CreateButton(new DefaultControls.Resources()).GetComponent<Button>();
            button.name = name; button.transform.SetParent(parent, false); button.GetComponent<Image>().color = color;
            Text label = button.GetComponentInChildren<Text>(true); if (label != null) { label.text = string.Empty; label.color = Color.white; label.resizeTextForBestFit = true; }
            return button;
        }

        private static GameObject CreateUiPanel(Transform parent, string name, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image)); panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = color; return panel;
        }

        private static Text CreateText(Transform parent, string name, int size)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Text)); root.transform.SetParent(parent, false);
            Text text = root.GetComponent<Text>(); text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.fontSize = size;
            text.resizeTextForBestFit = true; text.resizeTextMinSize = 15; text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white; text.raycastTarget = false; return text;
        }

        private static GameObject CreatePart(Transform parent, string name, PrimitiveType primitive, Vector3 position, Vector3 scale, Material material)
        {
            GameObject part = GameObject.CreatePrimitive(primitive); part.name = name; part.transform.SetParent(parent, false);
            part.transform.localPosition = position; part.transform.localScale = scale;
            Collider collider = part.GetComponent<Collider>(); if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
            Renderer renderer = part.GetComponent<Renderer>(); if (renderer != null) renderer.sharedMaterial = material;
            return part;
        }

        private static Material EnsureMaterial(string path, Color color)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                material = new Material(shader); AssetDatabase.CreateAsset(material, path);
            }
            material.color = color; EditorUtility.SetDirty(material); return material;
        }

        private static void ConfigureAddressables()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings ??
                throw new InvalidOperationException("Addressables settings must exist before Camp setup.");
            AddressableAssetGroup group = settings.FindGroup(SceneFlowFoundationSetup.SharedGroupName) ??
                throw new InvalidOperationException("SharedLocal Addressables group is missing.");
            AddAddressable(settings, group, BeforePrefabPath, "camp/observation-table-before");
            AddAddressable(settings, group, AfterPrefabPath, "camp/observation-corner-after");
            EditorUtility.SetDirty(settings);
        }

        private static void AddAddressable(AddressableAssetSettings settings, AddressableAssetGroup group, string path, string address)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid)) throw new InvalidOperationException("Camp visual is missing: " + path);
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group, false, false);
            entry.address = address; entry.SetLabel(SceneFlowFoundationSetup.SharedLabel, true, true, false);
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets/_Game/Content", "Camp"); EnsureFolder(Root, "Stations");
            EnsureFolder(Root, "Upgrades"); EnsureFolder(Root, "Visuals");
        }
        private static void EnsureFolder(string parent, string name)
        { string path = parent + "/" + name; if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, name); }
        private static void SetAssetReference(SerializedProperty property, string guid)
        {
            SerializedProperty assetGuid = property.FindPropertyRelative("m_AssetGUID");
            if (assetGuid == null) throw new InvalidOperationException("AssetReference serialized GUID field was not found.");
            assetGuid.stringValue = guid;
        }
        private static void SetObjectArray(SerializedProperty property, UnityEngine.Object[] values)
        {
            property.arraySize = values.Length;
            for (int index = 0; index < values.Length; index++) property.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
        }
        private static void Stretch(RectTransform rect)
        { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; }
        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max)
        { rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero; }

        private readonly struct StationSeed
        {
            public StationSeed(string id, string actionId, string slug, int order, bool available, bool parentRestricted)
            { Id = id; ActionId = actionId; Slug = slug; Order = order; Available = available; ParentRestricted = parentRestricted; }
            public string Id { get; }
            public string ActionId { get; }
            public string Slug { get; }
            public int Order { get; }
            public bool Available { get; }
            public bool ParentRestricted { get; }
        }
    }
}
