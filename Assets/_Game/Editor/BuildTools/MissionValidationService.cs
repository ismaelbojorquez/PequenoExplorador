using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Missions;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Content.Economy;
using PequenoExplorador.Content.Missions;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Missions;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.SceneManagement;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class MissionValidationService
    {
        public static IReadOnlyList<string> Validate(ContentValidationMode mode = ContentValidationMode.Development)
        {
            var errors = new List<string>();
            MissionCatalogAsset asset = AssetDatabase.LoadAssetAtPath<MissionCatalogAsset>(MissionFoundationSetup.CatalogPath);
            RewardCatalogAsset rewardAsset = AssetDatabase.LoadAssetAtPath<RewardCatalogAsset>(EconomyFoundationSetup.CatalogPath);
            ContentCatalogAsset contentAsset = AssetDatabase.LoadAssetAtPath<ContentCatalogAsset>(ContentFoundationSetup.CatalogPath);
            RewardCatalog rewards = null; ContentCatalog content = null;
            if (rewardAsset == null) errors.Add("MISSION100 reward catalog is missing.");
            else if (!rewardAsset.TryBuild(out rewards, out IReadOnlyList<string> rewardErrors)) errors.AddRange(rewardErrors);
            if (contentAsset == null) errors.Add("MISSION101 content catalog is missing.");
            else if (!contentAsset.TryBuildRuntimeCatalog(mode, out content, out IReadOnlyList<string> contentErrors)) errors.AddRange(contentErrors);
            if (asset == null) errors.Add("MISSION102 canonical MissionCatalogAsset is missing.");
            else if (rewards != null && content != null)
            {
                if (!asset.TryBuild(mode, rewards, content, HasLocalization, out MissionCatalog catalog, out IReadOnlyList<string> buildErrors))
                    errors.AddRange(buildErrors);
                else
                {
                    if (catalog.Missions.Count != 1) errors.Add("MISSION103 Vertical Slice must contain exactly one runtime mission.");
                    MissionDefinition fixture = catalog.Missions.SingleOrDefault();
                    if (fixture == null || fixture.Id.Value != "mission.vertical-slice.photograph-toucan" || fixture.Objectives.Count != 1 ||
                        !fixture.Objectives[0].TypeId.Equals(MissionObjectiveTypeIds.PhotographSpecific) || fixture.Expires || !fixture.AutoClaimReward)
                        errors.Add("MISSION104 canonical photo mission must be single-objective, non-expiring and auto-claim.");
                }
            }
            var registry = new MissionObjectiveStrategyRegistry(new IMissionObjectiveStrategy[]
                { new DiscoverCountObjectiveStrategy(), new PhotographSpecificObjectiveStrategy(), new InteractTagObjectiveStrategy() });
            if (registry.TypeIds.Count != 3) errors.Add("MISSION105 exactly three baseline objective strategies must be registered.");
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            MissionView[] views = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<MissionView>(true)).ToArray();
            if (views.Length != 1) errors.Add($"MISSION106 Bootstrap requires exactly one MissionView; found {views.Length}.");
            else
            {
                if (views[0].GetComponentInChildren<SafeAreaFitter>(true) == null) errors.Add("MISSION107 MissionView requires SafeAreaFitter.");
                foreach (Button button in views[0].GetComponentsInChildren<Button>(true))
                    if (((UnityEngine.RectTransform)button.transform).rect.width < 64f || ((UnityEngine.RectTransform)button.transform).rect.height < 64f)
                        errors.Add("MISSION108 mission touch targets must be at least 64x64 logical units.");
            }
            string applicationRoot = Path.Combine(Directory.GetCurrentDirectory(), "Assets/_Game/Application/Missions");
            foreach (string path in Directory.GetFiles(applicationRoot, "*.cs"))
            {
                string source = File.ReadAllText(path);
                if (source.Contains("switch (objective", StringComparison.OrdinalIgnoreCase))
                    errors.Add("MISSION109 objective evaluation must use the strategy registry, not a central switch.");
            }
            return errors;
        }

        private static bool HasLocalization(LocalizedKey key)
        {
            StringTableCollection collection = LocalizationEditorSettings.GetStringTableCollection(key.Table);
            if (collection == null) return false;
            foreach (string locale in new[] { LocaleCode.Spanish, LocaleCode.English })
            {
                StringTable table = collection.GetTable(new LocaleIdentifier(locale)) as StringTable;
                if (table?.GetEntry(key.Entry) == null || string.IsNullOrWhiteSpace(table.GetEntry(key.Entry).Value)) return false;
            }
            return true;
        }
    }
}
