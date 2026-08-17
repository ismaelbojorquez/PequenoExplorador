using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Customization;
using PequenoExplorador.Content.Camp;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Camp;
using PequenoExplorador.Presentation.Customization;
using PequenoExplorador.Presentation.Explorer;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class CustomizationFoundationSetup
    {
        public const string Root = "Assets/_Game/Content/Customization";
        public const string SlotsRoot = Root + "/Slots";
        public const string CosmeticsRoot = Root + "/Cosmetics";
        public const string CatalogPath = Root + "/CustomizationCatalog.asset";
        private const string PreviewName = "PH_CustomizationPreviewExplorer";

        private static readonly SlotSeed[] Slots =
        {
            new SlotSeed("skin-tone", "skin.light", 0), new SlotSeed("hair", "hair.curls", 1),
            new SlotSeed("shirt", "shirt.jungle", 2), new SlotSeed("pants", "pants.sand", 3),
            new SlotSeed("shoes", "shoes.trail", 4), new SlotSeed("hat", "hat.none", 5),
            new SlotSeed("backpack", "backpack.field", 6), new SlotSeed("explorer-tool", "tool.camera", 7)
        };

        private static readonly CosmeticSeed[] Cosmetics =
        {
            new CosmeticSeed("skin.light", "skin-tone", 255, 210, 172, true),
            new CosmeticSeed("skin.medium", "skin-tone", 205, 145, 95, true),
            new CosmeticSeed("skin.deep", "skin-tone", 105, 65, 48, true),
            new CosmeticSeed("skin.warm", "skin-tone", 224, 165, 118, true),
            new CosmeticSeed("hair.curls", "hair", 72, 42, 26, true),
            new CosmeticSeed("hair.waves", "hair", 45, 28, 22, true),
            new CosmeticSeed("hair.puffs", "hair", 31, 22, 18, true, tags: new[] { "cosmetic-tag.hair.volume-wide" }),
            new CosmeticSeed("shirt.jungle", "shirt", 28, 158, 140, true),
            new CosmeticSeed("shirt.sun", "shirt", 244, 150, 48, true),
            new CosmeticSeed("shirt.river", "shirt", 52, 139, 210, false, 3),
            new CosmeticSeed("pants.sand", "pants", 180, 139, 78, true),
            new CosmeticSeed("pants.night", "pants", 45, 62, 103, true),
            new CosmeticSeed("shoes.trail", "shoes", 83, 62, 48, true),
            new CosmeticSeed("shoes.coral", "shoes", 214, 82, 79, true),
            new CosmeticSeed("hat.none", "hat", 255, 255, 255, true),
            new CosmeticSeed("hat.sun", "hat", 244, 190, 75, false, 2, blocked: new[] { "cosmetic-tag.hair.volume-wide" }),
            new CosmeticSeed("backpack.field", "backpack", 246, 173, 47, true),
            new CosmeticSeed("backpack.leaf", "backpack", 68, 153, 91, false, 2),
            new CosmeticSeed("tool.camera", "explorer-tool", 54, 69, 78, true),
            new CosmeticSeed("tool.binoculars", "explorer-tool", 69, 117, 71, false, requiredUpgrade: CampFoundationSetup.UpgradeId)
        };

        [MenuItem("Pequeño Explorador/Development/Customization/Apply Foundation")]
        public static void Apply()
        {
            try
            {
                EnsureFolders();
                LocalizationFoundationSetup.ApplyCustomizationEntries();
                CustomizationSlotDefinitionAsset[] slots = Slots.Select(EnsureSlot).ToArray();
                CosmeticDefinitionAsset[] cosmetics = Cosmetics.Select(EnsureCosmetic).ToArray();
                CustomizationCatalogAsset catalog = EnsureCatalog(slots, cosmetics);
                ConfigureCampStation();
                ConfigureExplorerPrefab();
                ConfigureCampPreview();
                ConfigureBootstrap(catalog);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("PE_CUSTOMIZATION_SETUP_OK slots=8 cosmetics=20 saveSchema=11 genderSelection=false debugUnlock=development-only");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        private static CustomizationSlotDefinitionAsset EnsureSlot(SlotSeed seed)
        {
            string path = SlotsRoot + "/PH_Slot_" + ToFileName(seed.Slug) + ".asset";
            CustomizationSlotDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<CustomizationSlotDefinitionAsset>(path);
            if (asset == null) { asset = ScriptableObject.CreateInstance<CustomizationSlotDefinitionAsset>(); AssetDatabase.CreateAsset(asset, path); }
            var serialized = new SerializedObject(asset);
            serialized.FindProperty("_id").stringValue = "customization-slot." + seed.Slug;
            serialized.FindProperty("_displayNameTable").stringValue = "UI";
            serialized.FindProperty("_displayNameKey").stringValue = "ui.customization.slot." + seed.Slug;
            serialized.FindProperty("_displayOrder").intValue = seed.Order;
            serialized.FindProperty("_defaultCosmeticId").stringValue = "cosmetic." + seed.DefaultSlug;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return asset;
        }

        private static CosmeticDefinitionAsset EnsureCosmetic(CosmeticSeed seed)
        {
            string path = CosmeticsRoot + "/PH_Cosmetic_" + ToFileName(seed.Slug) + ".asset";
            CosmeticDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<CosmeticDefinitionAsset>(path);
            if (asset == null) { asset = ScriptableObject.CreateInstance<CosmeticDefinitionAsset>(); AssetDatabase.CreateAsset(asset, path); }
            var serialized = new SerializedObject(asset);
            serialized.FindProperty("_id").stringValue = "cosmetic." + seed.Slug;
            serialized.FindProperty("_slotId").stringValue = "customization-slot." + seed.Slot;
            serialized.FindProperty("_displayNameTable").stringValue = "UI";
            serialized.FindProperty("_displayNameKey").stringValue = "ui.customization.cosmetic." + seed.Slug;
            serialized.FindProperty("_visualId").stringValue = "visual.customization." + seed.Slug;
            serialized.FindProperty("_color").colorValue = new Color32(seed.Red, seed.Green, seed.Blue, 255);
            serialized.FindProperty("_initiallyUnlocked").boolValue = seed.Initial;
            serialized.FindProperty("_starCost").intValue = seed.StarCost;
            serialized.FindProperty("_spendReasonId").stringValue = seed.StarCost > 0 ? "reward.cosmetic." + seed.Slug : string.Empty;
            serialized.FindProperty("_requiredCampUpgradeId").stringValue = seed.RequiredUpgrade ?? string.Empty;
            SetStringArray(serialized.FindProperty("_compatibilityTags"), seed.Tags);
            SetStringArray(serialized.FindProperty("_blockedTags"), seed.Blocked);
            SerializedProperty editorial = serialized.FindProperty("_editorial");
            editorial.FindPropertyRelative("_state").enumValueIndex = 0;
            editorial.FindPropertyRelative("_isPlaceholder").boolValue = true;
            editorial.FindPropertyRelative("_owner").stringValue = "Character Art / Inclusive Design";
            editorial.FindPropertyRelative("_developmentWatermark").stringValue = "BORRADOR · PH_CUSTOMIZATION";
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return asset;
        }

        private static CustomizationCatalogAsset EnsureCatalog(CustomizationSlotDefinitionAsset[] slots, CosmeticDefinitionAsset[] cosmetics)
        {
            CustomizationCatalogAsset catalog = AssetDatabase.LoadAssetAtPath<CustomizationCatalogAsset>(CatalogPath);
            if (catalog == null) { catalog = ScriptableObject.CreateInstance<CustomizationCatalogAsset>(); AssetDatabase.CreateAsset(catalog, CatalogPath); }
            var serialized = new SerializedObject(catalog);
            SetObjectArray(serialized.FindProperty("_slots"), slots.Cast<UnityEngine.Object>().ToArray());
            SetObjectArray(serialized.FindProperty("_cosmetics"), cosmetics.Cast<UnityEngine.Object>().ToArray());
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return catalog;
        }

        private static void ConfigureCampStation()
        {
            CampStationDefinitionAsset station = AssetDatabase.FindAssets("t:CampStationDefinitionAsset", new[] { CampFoundationSetup.StationsRoot })
                .Select(guid => AssetDatabase.LoadAssetAtPath<CampStationDefinitionAsset>(AssetDatabase.GUIDToAssetPath(guid)))
                .SingleOrDefault(value => value != null && value.RawId == "camp-station.customization") ??
                throw new InvalidOperationException("Customization Camp station is missing.");
            var serialized = new SerializedObject(station);
            serialized.FindProperty("_available").boolValue = true;
            serialized.FindProperty("_parentRestricted").boolValue = false;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureExplorerPrefab()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(ExplorerFoundationSetup.PrefabPath);
            try
            {
                ExplorerCustomizationRig oldRig = root.GetComponent<ExplorerCustomizationRig>();
                if (oldRig != null) UnityEngine.Object.DestroyImmediate(oldRig);
                Transform visual = root.transform.Find("PH_Visual") ?? throw new InvalidOperationException("Existing explorer PH_Visual is missing.");
                Transform old = visual.Find(ExplorerCustomizationRig.PlaceholderName);
                if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);
                var customization = new GameObject(ExplorerCustomizationRig.PlaceholderName).transform;
                customization.SetParent(visual, false);
                Material bodyMaterial = AssetDatabase.LoadAssetAtPath<Material>(ExplorerFoundationSetup.BodyMaterialPath);
                Material accentMaterial = AssetDatabase.LoadAssetAtPath<Material>(ExplorerFoundationSetup.AccentMaterialPath);
                if (bodyMaterial == null || accentMaterial == null) throw new InvalidOperationException("Explorer shared materials are missing.");

                Renderer head = RequireRenderer(visual, "PH_Head");
                Renderer body = RequireRenderer(visual, "PH_Body");
                Renderer backpack = RequireRenderer(visual, "PH_Backpack");
                GameObject pants = CreatePart(customization, "PH_Pants", PrimitiveType.Cube, new Vector3(0f, 0.47f, 0f), new Vector3(0.55f, 0.36f, 0.5f), bodyMaterial);
                GameObject shoes = new GameObject("PH_Shoes"); shoes.transform.SetParent(customization, false);
                GameObject shoeL = CreatePart(shoes.transform, "PH_Shoe_L", PrimitiveType.Cube, new Vector3(-0.19f, 0.16f, 0.08f), new Vector3(0.22f, 0.16f, 0.38f), accentMaterial);
                GameObject shoeR = CreatePart(shoes.transform, "PH_Shoe_R", PrimitiveType.Cube, new Vector3(0.19f, 0.16f, 0.08f), new Vector3(0.22f, 0.16f, 0.38f), accentMaterial);
                GameObject hairCurls = CreateHair(customization, "PH_Hair_Curls", PrimitiveType.Sphere, bodyMaterial, new Vector3(0f, 1.70f, 0f), new Vector3(0.50f, 0.20f, 0.47f));
                GameObject hairWaves = CreateHair(customization, "PH_Hair_Waves", PrimitiveType.Capsule, bodyMaterial, new Vector3(0f, 1.66f, -0.05f), new Vector3(0.47f, 0.20f, 0.45f));
                GameObject hairPuffs = new GameObject("PH_Hair_Puffs"); hairPuffs.transform.SetParent(customization, false);
                GameObject puffL = CreatePart(hairPuffs.transform, "PH_Puff_L", PrimitiveType.Sphere, new Vector3(-0.29f, 1.68f, 0f), Vector3.one * 0.27f, bodyMaterial);
                GameObject puffR = CreatePart(hairPuffs.transform, "PH_Puff_R", PrimitiveType.Sphere, new Vector3(0.29f, 1.68f, 0f), Vector3.one * 0.27f, bodyMaterial);
                GameObject sunHat = new GameObject("PH_Hat_Sun"); sunHat.transform.SetParent(customization, false);
                GameObject brim = CreatePart(sunHat.transform, "PH_Hat_Brim", PrimitiveType.Cylinder, new Vector3(0f, 1.77f, 0f), new Vector3(0.62f, 0.035f, 0.62f), accentMaterial);
                GameObject crown = CreatePart(sunHat.transform, "PH_Hat_Crown", PrimitiveType.Cylinder, new Vector3(0f, 1.88f, 0f), new Vector3(0.34f, 0.11f, 0.34f), accentMaterial);
                GameObject camera = CreateTool(customization, "PH_Tool_Camera", accentMaterial, new Vector3(0.40f, 0.82f, 0.16f), false);
                GameObject binoculars = CreateTool(customization, "PH_Tool_Binoculars", bodyMaterial, new Vector3(0.40f, 0.82f, 0.16f), true);

                ExplorerCustomizationRig rig = root.AddComponent<ExplorerCustomizationRig>();
                var serialized = new SerializedObject(rig);
                SerializedProperty bindings = serialized.FindProperty("_bindings"); bindings.arraySize = 8;
                SetBinding(bindings.GetArrayElementAtIndex(0), "customization-slot.skin-tone", head.gameObject, new[] { head });
                SetVariantBinding(bindings.GetArrayElementAtIndex(1), "customization-slot.hair", new[]
                {
                    Variant("visual.customization.hair.curls", hairCurls), Variant("visual.customization.hair.waves", hairWaves),
                    Variant("visual.customization.hair.puffs", hairPuffs)
                });
                SetBinding(bindings.GetArrayElementAtIndex(2), "customization-slot.shirt", body.gameObject, new[] { body });
                SetBinding(bindings.GetArrayElementAtIndex(3), "customization-slot.pants", pants, pants.GetComponentsInChildren<Renderer>(true));
                SetBinding(bindings.GetArrayElementAtIndex(4), "customization-slot.shoes", shoes, new[] { shoeL.GetComponent<Renderer>(), shoeR.GetComponent<Renderer>() });
                SetVariantBinding(bindings.GetArrayElementAtIndex(5), "customization-slot.hat", new[] { Variant("visual.customization.hat.sun", sunHat) });
                SetBinding(bindings.GetArrayElementAtIndex(6), "customization-slot.backpack", backpack.gameObject, new[] { backpack });
                SetVariantBinding(bindings.GetArrayElementAtIndex(7), "customization-slot.explorer-tool", new[]
                {
                    Variant("visual.customization.tool.camera", camera), Variant("visual.customization.tool.binoculars", binoculars)
                });
                serialized.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(root, ExplorerFoundationSetup.PrefabPath);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }

        private static void ConfigureCampPreview()
        {
            Scene scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.CampScenePath, OpenSceneMode.Single);
            foreach (GameObject value in scene.GetRootGameObjects().SelectMany(value => value.GetComponentsInChildren<Transform>(true))
                         .Where(value => value.name == PreviewName).Select(value => value.gameObject).ToArray()) UnityEngine.Object.DestroyImmediate(value);
            CampSceneRoot camp = scene.GetRootGameObjects().SelectMany(value => value.GetComponentsInChildren<CampSceneRoot>(true)).Single();
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ExplorerFoundationSetup.PrefabPath);
            GameObject preview = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene); preview.name = PreviewName;
            preview.transform.SetParent(camp.transform, false); preview.transform.localPosition = new Vector3(2.4f, 0f, -0.6f);
            preview.transform.localRotation = Quaternion.Euler(0f, -25f, 0f);
            ExplorerLocomotionRoot locomotion = preview.GetComponent<ExplorerLocomotionRoot>(); if (locomotion != null) UnityEngine.Object.DestroyImmediate(locomotion);
            NavMeshAgent agent = preview.GetComponent<NavMeshAgent>(); if (agent != null) UnityEngine.Object.DestroyImmediate(agent);
            var serialized = new SerializedObject(camp); serialized.FindProperty("_customizationPreviewRig").objectReferenceValue = preview.GetComponent<ExplorerCustomizationRig>();
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureBootstrap(CustomizationCatalogAsset catalog)
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            foreach (GameObject value in scene.GetRootGameObjects().Where(value => value.name == CustomizationView.PlaceholderObjectName)) UnityEngine.Object.DestroyImmediate(value);
            var canvasObject = new GameObject(CustomizationView.PlaceholderObjectName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CustomizationView));
            Canvas canvas = canvasObject.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 118;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f); scaler.matchWidthOrHeight = 0.5f;
            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter)); safe.transform.SetParent(canvasObject.transform, false); Stretch((RectTransform)safe.transform);
            GameObject panel = CreatePanel(safe.transform, "PH_Customization Panel", new Color(0.035f, 0.13f, 0.16f, 0.98f)); Stretch((RectTransform)panel.transform);
            Text title = CreateText(panel.transform, "Title", 46); SetRect(title.rectTransform, new Vector2(0.04f, 0.89f), new Vector2(0.44f, 0.98f));
            Text balance = CreateText(panel.transform, "Balance", 28); SetRect(balance.rectTransform, new Vector2(0.57f, 0.90f), new Vector2(0.94f, 0.98f));
            var slotViews = new List<CustomizationSlotButtonView>();
            for (int index = 0; index < 8; index++)
            {
                Button button = CreateButton(panel.transform, "Slot " + index, new Color(0.12f, 0.40f, 0.43f, 1f));
                SetRect((RectTransform)button.transform, new Vector2(0.03f, 0.79f - index * 0.092f), new Vector2(0.26f, 0.865f - index * 0.092f));
                CustomizationSlotButtonView view = button.gameObject.AddComponent<CustomizationSlotButtonView>();
                var serialized = new SerializedObject(view); serialized.FindProperty("_button").objectReferenceValue = button;
                serialized.FindProperty("_label").objectReferenceValue = button.GetComponentInChildren<Text>(true); serialized.ApplyModifiedPropertiesWithoutUndo(); slotViews.Add(view);
            }
            Text selectedName = CreateText(panel.transform, "Selected Name", 38); SetRect(selectedName.rectTransform, new Vector2(0.30f, 0.73f), new Vector2(0.70f, 0.84f));
            Text selectedState = CreateText(panel.transform, "Selected State", 27); SetRect(selectedState.rectTransform, new Vector2(0.30f, 0.63f), new Vector2(0.70f, 0.72f));
            var optionViews = new List<CustomizationOptionButtonView>();
            for (int index = 0; index < 4; index++)
            {
                Button button = CreateButton(panel.transform, "Option " + index, new Color(0.19f, 0.50f, 0.37f, 1f));
                float x = 0.30f + index * 0.165f; SetRect((RectTransform)button.transform, new Vector2(x, 0.43f), new Vector2(x + 0.145f, 0.61f));
                Image swatch = CreatePanel(button.transform, "Swatch", Color.white).GetComponent<Image>(); SetRect(swatch.rectTransform, new Vector2(0.10f, 0.50f), new Vector2(0.90f, 0.90f));
                GameObject locked = CreatePanel(button.transform, "Locked", new Color(0.12f, 0.12f, 0.15f, 0.72f)); Stretch((RectTransform)locked.transform); locked.SetActive(false);
                GameObject equipped = CreatePanel(button.transform, "Equipped", new Color(1f, 0.78f, 0.22f, 0.45f)); Stretch((RectTransform)equipped.transform); equipped.SetActive(false);
                CustomizationOptionButtonView view = button.gameObject.AddComponent<CustomizationOptionButtonView>();
                var serialized = new SerializedObject(view); serialized.FindProperty("_button").objectReferenceValue = button;
                serialized.FindProperty("_label").objectReferenceValue = button.GetComponentInChildren<Text>(true); serialized.FindProperty("_swatch").objectReferenceValue = swatch;
                serialized.FindProperty("_lockedBadge").objectReferenceValue = locked; serialized.FindProperty("_equippedBadge").objectReferenceValue = equipped;
                serialized.ApplyModifiedPropertiesWithoutUndo(); optionViews.Add(view);
            }
            Button unlock = CreateButton(panel.transform, "Unlock", new Color(0.93f, 0.55f, 0.16f, 1f)); SetRect((RectTransform)unlock.transform, new Vector2(0.31f, 0.26f), new Vector2(0.51f, 0.39f));
            Button equip = CreateButton(panel.transform, "Equip", new Color(0.14f, 0.62f, 0.36f, 1f)); SetRect((RectTransform)equip.transform, new Vector2(0.53f, 0.26f), new Vector2(0.73f, 0.39f));
            Button close = CreateButton(panel.transform, "Close", new Color(0.26f, 0.34f, 0.40f, 1f)); SetRect((RectTransform)close.transform, new Vector2(0.78f, 0.07f), new Vector2(0.95f, 0.18f));
            Text feedback = CreateText(panel.transform, "Feedback", 26); SetRect(feedback.rectTransform, new Vector2(0.29f, 0.08f), new Vector2(0.73f, 0.20f));
            Button debug = CreateButton(panel.transform, "PH_DEBUG Unlock All", new Color(0.55f, 0.22f, 0.58f, 1f)); SetRect((RectTransform)debug.transform, new Vector2(0.76f, 0.76f), new Vector2(0.96f, 0.86f));
            CustomizationView viewRoot = canvasObject.GetComponent<CustomizationView>();
            var viewSerialized = new SerializedObject(viewRoot);
            viewSerialized.FindProperty("_panel").objectReferenceValue = panel; viewSerialized.FindProperty("_title").objectReferenceValue = title;
            viewSerialized.FindProperty("_balance").objectReferenceValue = balance; SetObjectArray(viewSerialized.FindProperty("_slotButtons"), slotViews.Cast<UnityEngine.Object>().ToArray());
            SetObjectArray(viewSerialized.FindProperty("_optionButtons"), optionViews.Cast<UnityEngine.Object>().ToArray());
            viewSerialized.FindProperty("_selectedName").objectReferenceValue = selectedName; viewSerialized.FindProperty("_selectedState").objectReferenceValue = selectedState;
            viewSerialized.FindProperty("_unlockButton").objectReferenceValue = unlock; viewSerialized.FindProperty("_unlockLabel").objectReferenceValue = unlock.GetComponentInChildren<Text>(true);
            viewSerialized.FindProperty("_equipButton").objectReferenceValue = equip; viewSerialized.FindProperty("_equipLabel").objectReferenceValue = equip.GetComponentInChildren<Text>(true);
            viewSerialized.FindProperty("_closeButton").objectReferenceValue = close; viewSerialized.FindProperty("_closeLabel").objectReferenceValue = close.GetComponentInChildren<Text>(true);
            viewSerialized.FindProperty("_feedback").objectReferenceValue = feedback; viewSerialized.FindProperty("_debugUnlockAllButton").objectReferenceValue = debug;
            viewSerialized.ApplyModifiedPropertiesWithoutUndo(); panel.SetActive(false);
            DiagnosticBootstrap bootstrap = scene.GetRootGameObjects().SelectMany(value => value.GetComponentsInChildren<DiagnosticBootstrap>(true)).Single();
            bootstrap.ConfigureCustomizationForEditorAndTests(catalog, viewRoot);
            var bootstrapSerialized = new SerializedObject(bootstrap); SerializedProperty fitters = bootstrapSerialized.FindProperty("_safeAreaFitters");
            var values = new List<UnityEngine.Object>(); for (int index = 0; index < fitters.arraySize; index++)
            { UnityEngine.Object value = fitters.GetArrayElementAtIndex(index).objectReferenceValue; if (value != null) values.Add(value); }
            if (!values.Contains(safe.GetComponent<SafeAreaFitter>())) values.Add(safe.GetComponent<SafeAreaFitter>()); SetObjectArray(fitters, values.ToArray());
            bootstrapSerialized.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(bootstrap);
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        }

        private static GameObject CreateHair(Transform parent, string name, PrimitiveType type, Material material, Vector3 position, Vector3 scale) =>
            CreatePart(parent, name, type, position, scale, material);
        private static GameObject CreateTool(Transform parent, string name, Material material, Vector3 position, bool binoculars)
        {
            GameObject root = new GameObject(name); root.transform.SetParent(parent, false);
            if (binoculars)
            {
                CreatePart(root.transform, "PH_Lens_L", PrimitiveType.Cylinder, position + new Vector3(-0.11f, 0f, 0f), new Vector3(0.09f, 0.13f, 0.09f), material);
                CreatePart(root.transform, "PH_Lens_R", PrimitiveType.Cylinder, position + new Vector3(0.11f, 0f, 0f), new Vector3(0.09f, 0.13f, 0.09f), material);
            }
            else CreatePart(root.transform, "PH_CameraBody", PrimitiveType.Cube, position, new Vector3(0.30f, 0.22f, 0.14f), material);
            return root;
        }
        private static GameObject CreatePart(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material)
        {
            GameObject result = GameObject.CreatePrimitive(type); result.name = name; result.transform.SetParent(parent, false);
            result.transform.localPosition = position; result.transform.localScale = scale; result.GetComponent<Renderer>().sharedMaterial = material;
            Collider collider = result.GetComponent<Collider>(); if (collider != null) UnityEngine.Object.DestroyImmediate(collider); return result;
        }
        private static Renderer RequireRenderer(Transform parent, string name) => parent.Find(name)?.GetComponent<Renderer>() ?? throw new InvalidOperationException(name + " renderer is missing.");
        private static VariantSeed Variant(string id, GameObject root) => new VariantSeed(id, root);
        private static void SetBinding(SerializedProperty property, string slot, GameObject root, Renderer[] renderers)
        {
            property.FindPropertyRelative("_slotId").stringValue = slot; property.FindPropertyRelative("_defaultRoot").objectReferenceValue = root;
            SetObjectArray(property.FindPropertyRelative("_defaultRenderers"), renderers.Cast<UnityEngine.Object>().ToArray()); property.FindPropertyRelative("_variants").arraySize = 0;
        }
        private static void SetVariantBinding(SerializedProperty property, string slot, VariantSeed[] variants)
        {
            property.FindPropertyRelative("_slotId").stringValue = slot; property.FindPropertyRelative("_defaultRoot").objectReferenceValue = null;
            property.FindPropertyRelative("_defaultRenderers").arraySize = 0; SerializedProperty values = property.FindPropertyRelative("_variants"); values.arraySize = variants.Length;
            for (int index = 0; index < variants.Length; index++)
            {
                SerializedProperty item = values.GetArrayElementAtIndex(index); item.FindPropertyRelative("_visualId").stringValue = variants[index].Id;
                item.FindPropertyRelative("_root").objectReferenceValue = variants[index].Root;
                SetObjectArray(item.FindPropertyRelative("_renderers"), variants[index].Root.GetComponentsInChildren<Renderer>(true).Cast<UnityEngine.Object>().ToArray());
            }
        }
        private static Button CreateButton(Transform parent, string name, Color color)
        {
            Button button = DefaultControls.CreateButton(new DefaultControls.Resources()).GetComponent<Button>(); button.name = name; button.transform.SetParent(parent, false);
            button.GetComponent<Image>().color = color; Text label = button.GetComponentInChildren<Text>(true); label.text = string.Empty; label.color = Color.white;
            label.fontSize = 24; label.resizeTextForBestFit = true; return button;
        }
        private static GameObject CreatePanel(Transform parent, string name, Color color)
        { var result = new GameObject(name, typeof(RectTransform), typeof(Image)); result.transform.SetParent(parent, false); result.GetComponent<Image>().color = color; return result; }
        private static Text CreateText(Transform parent, string name, int size)
        {
            var result = new GameObject(name, typeof(RectTransform), typeof(Text)); result.transform.SetParent(parent, false); Text text = result.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.fontSize = size; text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 15; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white; text.raycastTarget = false; return text;
        }
        private static void EnsureFolders() { EnsureFolder(Root); EnsureFolder(SlotsRoot); EnsureFolder(CosmeticsRoot); }
        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return; string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent); AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
        private static string ToFileName(string value) => string.Join("_", value.Split('.', '-').Select(part => char.ToUpperInvariant(part[0]) + part.Substring(1)));
        private static void SetStringArray(SerializedProperty property, string[] values)
        { values ??= Array.Empty<string>(); property.arraySize = values.Length; for (int index = 0; index < values.Length; index++) property.GetArrayElementAtIndex(index).stringValue = values[index]; }
        private static void SetObjectArray(SerializedProperty property, UnityEngine.Object[] values)
        { property.arraySize = values.Length; for (int index = 0; index < values.Length; index++) property.GetArrayElementAtIndex(index).objectReferenceValue = values[index]; }
        private static void Stretch(RectTransform rect) => SetRect(rect, Vector2.zero, Vector2.one);
        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max) { rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero; }

        private readonly struct SlotSeed
        { public SlotSeed(string slug, string defaultSlug, int order) { Slug = slug; DefaultSlug = defaultSlug; Order = order; } public string Slug { get; } public string DefaultSlug { get; } public int Order { get; } }
        private readonly struct CosmeticSeed
        {
            public CosmeticSeed(string slug, string slot, byte red, byte green, byte blue, bool initial, int starCost = 0,
                string[] tags = null, string[] blocked = null, string requiredUpgrade = null)
            { Slug = slug; Slot = slot; Red = red; Green = green; Blue = blue; Initial = initial; StarCost = starCost; Tags = tags ?? Array.Empty<string>(); Blocked = blocked ?? Array.Empty<string>(); RequiredUpgrade = requiredUpgrade; }
            public string Slug { get; } public string Slot { get; } public byte Red { get; } public byte Green { get; } public byte Blue { get; }
            public bool Initial { get; } public int StarCost { get; } public string[] Tags { get; } public string[] Blocked { get; } public string RequiredUpgrade { get; }
        }
        private readonly struct VariantSeed { public VariantSeed(string id, GameObject root) { Id = id; Root = root; } public string Id { get; } public GameObject Root { get; } }
    }
}
