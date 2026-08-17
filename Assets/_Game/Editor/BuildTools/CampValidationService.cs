using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PequenoExplorador.Application.Camp;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Content.Camp;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Camp;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.Localization;
using UnityEditor.SceneManagement;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class CampValidationService
    {
        public static IReadOnlyList<string> Validate(ContentValidationMode mode)
        {
            var errors = new List<string>();
            CampCatalogAsset asset = AssetDatabase.LoadAssetAtPath<CampCatalogAsset>(CampFoundationSetup.CatalogPath);
            CampCatalog catalog = null;
            if (asset == null) errors.Add("CAMP100 canonical Camp catalog is missing.");
            else if (!asset.TryBuild(mode, out catalog, out IReadOnlyList<string> violations)) errors.AddRange(violations);
            if (mode == ContentValidationMode.Release) return errors;
            if (catalog != null)
            {
                if (catalog.Stations.Count != 4) errors.Add("CAMP101 Camp baseline requires exactly four stations.");
                if (catalog.Upgrades.Count != 1) errors.Add("CAMP102 Vertical Slice requires exactly one Camp upgrade.");
                CampUpgradeDefinition upgrade = catalog.Upgrades.SingleOrDefault();
                if (upgrade == null || upgrade.Id.Value != CampFoundationSetup.UpgradeId || upgrade.StarCost.Value != 3 ||
                    upgrade.Prerequisites.Count != 0 || !upgrade.IsPlaceholder)
                    errors.Add("CAMP103 observation corner must cost three provisional stars, have no prerequisite and remain PH_.");
                CampStationDefinition parents = catalog.Stations.SingleOrDefault(value => value.Id.Value == "camp-station.parents");
                CampStationDefinition customization = catalog.Stations.SingleOrDefault(value => value.Id.Value == "camp-station.customization");
                if (parents == null || parents.IsAvailable || !parents.IsParentRestricted)
                    errors.Add("CAMP104 parent station must remain unavailable and explicitly parent-restricted until a gate exists.");
                if (customization == null || !customization.IsAvailable || customization.IsParentRestricted)
                    errors.Add("CAMP105 customization station must be available to children and remain separate from the parent-restricted area.");
                foreach (CampStationDefinition station in catalog.Stations)
                    if (!HasLocalization(station.DisplayName) || !HasLocalization(station.Description))
                        errors.Add($"CAMP116 station '{station.Id}' requires non-empty ES/EN localization.");
                foreach (CampUpgradeDefinition definition in catalog.Upgrades)
                    if (!HasLocalization(definition.DisplayName) || !HasLocalization(definition.Description) ||
                        !HasLocalization(definition.PreviewCopy))
                        errors.Add($"CAMP117 upgrade '{definition.Id}' requires non-empty ES/EN localization.");
            }
            ValidateVisualReferences(asset, errors);
            ValidateCampScene(catalog, errors);
            ValidateBootstrap(errors);
            ValidateSourceBoundaries(errors);
            return errors;
        }

        private static void ValidateVisualReferences(CampCatalogAsset asset, ICollection<string> errors)
        {
            CampUpgradeDefinitionAsset upgrade = asset?.Upgrades.FirstOrDefault();
            if (upgrade == null) return;
            foreach (var reference in new[] { upgrade.BeforeVariant, upgrade.AfterVariant })
            {
                string guid = reference?.AssetGUID;
                string path = string.IsNullOrEmpty(guid) ? string.Empty : AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path) || AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(path) == null)
                    errors.Add("CAMP106 upgrade visual AssetReference is missing or invalid.");
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings == null || string.IsNullOrEmpty(guid) || settings.FindAssetEntry(guid) == null)
                    errors.Add("CAMP107 upgrade visual must be local Addressable content.");
            }
        }

        private static void ValidateCampScene(ICampCatalog catalog, ICollection<string> errors)
        {
            Scene scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.CampScenePath, OpenSceneMode.Single);
            CampSceneRoot[] roots = scene.GetRootGameObjects().SelectMany(value => value.GetComponentsInChildren<CampSceneRoot>(true)).ToArray();
            if (roots.Length != 1) { errors.Add($"CAMP108 Camp scene requires exactly one CampSceneRoot; found {roots.Length}."); return; }
            CampSceneRoot root = roots[0];
            string[] anchors = root.Anchors.Where(value => value != null).Select(value => value.RawStationId).OrderBy(value => value, StringComparer.Ordinal).ToArray();
            string[] expected = catalog?.Stations.Select(value => value.Id.Value).OrderBy(value => value, StringComparer.Ordinal).ToArray() ?? Array.Empty<string>();
            if (!anchors.SequenceEqual(expected)) errors.Add("CAMP109 scene anchors must match the data-driven station catalog exactly.");
            if (root.UpgradeVisuals.Length != 1 || root.UpgradeVisuals[0] == null || root.UpgradeVisuals[0].RawUpgradeId != CampFoundationSetup.UpgradeId)
                errors.Add("CAMP110 Camp scene must contain one before/after visual mapping for the observation upgrade.");
        }

        private static void ValidateBootstrap(ICollection<string> errors)
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            CampHubView[] views = scene.GetRootGameObjects().SelectMany(value => value.GetComponentsInChildren<CampHubView>(true)).ToArray();
            if (views.Length != 1) { errors.Add($"CAMP111 Bootstrap requires exactly one CampHubView; found {views.Length}."); return; }
            CampHubView view = views[0];
            if (view.GetComponentInChildren<SafeAreaFitter>(true) == null) errors.Add("CAMP112 Camp hub must use the central safe-area adapter.");
            CampStationButtonView[] stations = view.GetComponentsInChildren<CampStationButtonView>(true);
            if (stations.Length != 4) errors.Add("CAMP113 Camp hub requires exactly four station buttons.");
            foreach (Button button in view.GetComponentsInChildren<Button>(true))
            {
                UnityEngine.RectTransform rect = button.transform as UnityEngine.RectTransform;
                if (rect != null && (rect.rect.width < 64f || rect.rect.height < 64f))
                    errors.Add($"CAMP114 touch target '{button.name}' must be at least 64x64 logical units.");
            }
        }

        private static void ValidateSourceBoundaries(ICollection<string> errors)
        {
            string root = Path.Combine(Directory.GetCurrentDirectory(), "Assets/_Game/Application/Camp");
            foreach (string path in Directory.GetFiles(root, "*.cs"))
            {
                string source = File.ReadAllText(path);
                foreach (string forbidden in new[] { "IAP", "Ads", "AssetDatabase", "PlayerPrefs", "UnityEngine" })
                    if (source.IndexOf(forbidden, StringComparison.OrdinalIgnoreCase) >= 0)
                        errors.Add($"CAMP115 forbidden Camp Application coupling '{forbidden}' in {Path.GetFileName(path)}.");
            }
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
    }
}
