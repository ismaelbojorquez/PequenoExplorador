using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PequenoExplorador.Application.Customization;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Content.Customization;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Camp;
using PequenoExplorador.Presentation.Customization;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class CustomizationValidationService
    {
        public static IReadOnlyList<string> Validate(ContentValidationMode mode)
        {
            var errors = new List<string>();
            CustomizationCatalogAsset asset = AssetDatabase.LoadAssetAtPath<CustomizationCatalogAsset>(CustomizationFoundationSetup.CatalogPath);
            CustomizationCatalog catalog = null;
            if (asset == null) errors.Add("CUSTOM100 canonical customization catalog is missing.");
            else if (!asset.TryBuild(mode, out catalog, out IReadOnlyList<string> violations)) errors.AddRange(violations);
            if (mode == ContentValidationMode.Release) return errors;
            ValidateCatalog(catalog, errors);
            ValidatePrefab(catalog, errors);
            ValidateCamp(errors);
            ValidateBootstrap(errors);
            ValidateSourceBoundaries(errors);
            return errors;
        }

        private static void ValidateCatalog(CustomizationCatalog catalog, ICollection<string> errors)
        {
            if (catalog == null) return;
            if (catalog.Slots.Count != 8) errors.Add($"CUSTOM101 expected 8 inclusive slots; found {catalog.Slots.Count}.");
            if (catalog.Cosmetics.Count != 20) errors.Add($"CUSTOM102 expected 20 minimal placeholder options; found {catalog.Cosmetics.Count}.");
            if (catalog.Cosmetics.Any(value => !value.IsPlaceholder)) errors.Add("CUSTOM103 all provisional visuals must remain explicitly PH_ until final art review.");
            foreach (CustomizationSlotDefinition slot in catalog.Slots)
            {
                if (!HasLocalization(slot.DisplayName)) errors.Add($"CUSTOM104 missing ES/EN slot localization for '{slot.Id}'.");
                IReadOnlyList<CosmeticDefinition> options = catalog.GetForSlot(slot.Id);
                if (options.Count < 2) errors.Add($"CUSTOM105 slot '{slot.Id}' needs at least two representative options.");
                if (!catalog.TryGetCosmetic(slot.DefaultCosmeticId, out CosmeticDefinition fallback) || !fallback.IsInitiallyUnlocked)
                    errors.Add($"CUSTOM106 slot '{slot.Id}' needs a free safe fallback.");
                foreach (CosmeticDefinition cosmetic in options)
                    if (!HasLocalization(cosmetic.DisplayName)) errors.Add($"CUSTOM107 missing ES/EN cosmetic localization for '{cosmetic.Id}'.");
            }
            if (catalog.Cosmetics.Any(value => value.StarCost.Value > 0 && !value.SpendReasonId.IsValid))
                errors.Add("CUSTOM108 every star-priced cosmetic requires a stable spend reason.");
            if (catalog.Cosmetics.Any(value => value.StarCost.Value < 0)) errors.Add("CUSTOM109 cosmetic costs cannot be negative.");
        }

        private static void ValidatePrefab(ICustomizationCatalog catalog, ICollection<string> errors)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ExplorerFoundationSetup.PrefabPath);
            if (prefab == null) { errors.Add("CUSTOM110 explorer prefab is missing."); return; }
            ValidateMissingScripts(prefab, ExplorerFoundationSetup.PrefabPath, errors);
            ExplorerCustomizationRig[] rigs = prefab.GetComponentsInChildren<ExplorerCustomizationRig>(true);
            if (rigs.Length != 1) { errors.Add($"CUSTOM111 explorer prefab requires one customization rig; found {rigs.Length}."); return; }
            ExplorerCustomizationRig rig = rigs[0];
            string[] actual = rig.Bindings.Where(value => value != null).Select(value => value.RawSlotId).OrderBy(value => value, StringComparer.Ordinal).ToArray();
            string[] expected = catalog?.Slots.Select(value => value.Id.Value).OrderBy(value => value, StringComparer.Ordinal).ToArray() ?? Array.Empty<string>();
            if (!actual.SequenceEqual(expected)) errors.Add("CUSTOM112 rig bindings must match the catalog slots exactly.");
            foreach (CustomizationSlotVisualBinding binding in rig.Bindings)
            {
                if (binding == null) { errors.Add("CUSTOM113 rig contains a null slot binding."); continue; }
                foreach (Renderer renderer in binding.DefaultRenderers.Concat(binding.Variants.SelectMany(value => value?.Renderers ?? Array.Empty<Renderer>())))
                    if (renderer == null || renderer.sharedMaterial == null) errors.Add($"CUSTOM114 slot '{binding.RawSlotId}' has a missing shared material renderer.");
            }
            if (prefab.GetComponent<PequenoExplorador.Presentation.Explorer.ExplorerLocomotionRoot>() == null)
                errors.Add("CUSTOM115 customization must preserve the existing locomotion root.");
        }

        private static void ValidateCamp(ICollection<string> errors)
        {
            Scene scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.CampScenePath, OpenSceneMode.Single);
            foreach (GameObject root in scene.GetRootGameObjects())
                ValidateMissingScripts(root, SceneFlowFoundationSetup.CampScenePath, errors);
            CampSceneRoot[] roots = scene.GetRootGameObjects().SelectMany(value => value.GetComponentsInChildren<CampSceneRoot>(true)).ToArray();
            if (roots.Length != 1 || roots[0].CustomizationPreviewRig == null)
                errors.Add("CUSTOM116 Camp requires one wired explorer preview rig.");
        }

        private static void ValidateBootstrap(ICollection<string> errors)
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            foreach (GameObject root in scene.GetRootGameObjects())
                ValidateMissingScripts(root, ProjectFoundationSetup.BootstrapScenePath, errors);
            CustomizationView[] views = scene.GetRootGameObjects().SelectMany(value => value.GetComponentsInChildren<CustomizationView>(true)).ToArray();
            if (views.Length != 1) { errors.Add($"CUSTOM117 Bootstrap requires one CustomizationView; found {views.Length}."); return; }
            CustomizationView view = views[0];
            if (view.GetComponentInChildren<SafeAreaFitter>(true) == null) errors.Add("CUSTOM118 customization UI must use SafeAreaFitter.");
            if (view.SlotButtons.Count != 8 || view.OptionButtons.Count < 4) errors.Add("CUSTOM119 customization UI needs 8 slot controls and at least 4 reusable option controls.");
            foreach (Button button in view.GetComponentsInChildren<Button>(true))
            {
                RectTransform rect = button.transform as RectTransform;
                if (rect != null && (rect.rect.width < 64f || rect.rect.height < 64f))
                    errors.Add($"CUSTOM120 touch target '{button.name}' must be at least 64x64 logical units.");
            }
            string combined = string.Join(" ", view.GetComponentsInChildren<Text>(true).Select(value => value.text)).ToLowerInvariant();
            foreach (string forbidden in new[] { "niño", "niña", "boy", "girl" })
                if (combined.Contains(forbidden)) errors.Add($"CUSTOM121 gendered label '{forbidden}' is forbidden in customization UI.");
        }

        private static void ValidateSourceBoundaries(ICollection<string> errors)
        {
            string root = Path.Combine(Directory.GetCurrentDirectory(), "Assets/_Game/Application/Customization");
            foreach (string path in Directory.GetFiles(root, "*.cs"))
            {
                string source = File.ReadAllText(path);
                foreach (string forbidden in new[] { "IAP", "Ads", "AssetDatabase", "PlayerPrefs", "UnityEngine" })
                    if (source.IndexOf(forbidden, StringComparison.OrdinalIgnoreCase) >= 0)
                        errors.Add($"CUSTOM122 forbidden Application coupling '{forbidden}' in {Path.GetFileName(path)}.");
            }
            string presentation = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Assets/_Game/Presentation/Customization/ExplorerCustomizationRig.cs"));
            if (presentation.Contains(".material", StringComparison.Ordinal)) errors.Add("CUSTOM123 rig must not instantiate renderer materials; use MaterialPropertyBlock/sharedMaterial.");
        }

        private static bool HasLocalization(LocalizedKey key)
        {
            StringTableCollection collection = LocalizationEditorSettings.GetStringTableCollection(key.Table);
            if (collection == null) return false;
            foreach (string locale in new[] { LocaleCode.Spanish, LocaleCode.English })
            {
                StringTable table = collection.GetTable(new LocaleIdentifier(locale)) as StringTable;
                StringTableEntry entry = table?.GetEntry(key.Entry);
                if (entry == null || string.IsNullOrWhiteSpace(entry.Value)) return false;
            }
            return true;
        }

        private static void ValidateMissingScripts(GameObject root, string assetPath, ICollection<string> errors)
        {
            foreach (Transform value in root.GetComponentsInChildren<Transform>(true))
            {
                int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(value.gameObject);
                if (missing > 0)
                    errors.Add($"CUSTOM124 '{assetPath}' contains {missing} missing script(s) on '{GetHierarchyPath(value)}'.");
            }
        }

        private static string GetHierarchyPath(Transform value)
        {
            string result = value.name;
            for (Transform parent = value.parent; parent != null; parent = parent.parent)
                result = parent.name + "/" + result;
            return result;
        }
    }
}
