using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Content.Economy;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Economy;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class EconomyValidationService
    {
        public static IReadOnlyList<string> Validate()
        {
            var errors = new List<string>();
            RewardCatalogAsset asset = AssetDatabase.LoadAssetAtPath<RewardCatalogAsset>(EconomyFoundationSetup.CatalogPath);
            RewardCatalog catalog = null;
            IReadOnlyList<string> catalogViolations = Array.Empty<string>();
            if (asset == null)
                errors.Add("ECONOMY100 canonical reward catalog is missing.");
            else if (!asset.TryBuild(out catalog, out catalogViolations))
                errors.AddRange(catalogViolations);
            if (catalog != null)
            {
                if (catalog.Definitions.Count != 2) errors.Add("ECONOMY101 Vertical Slice must contain discovery and mission reward definitions only.");
                RewardDefinition reward = catalog.Definitions.SingleOrDefault(item => item.SourceKind == RewardSourceKind.Discovery);
                if (reward == null || reward.Id.Value != "reward.discovery.keel-billed-toucan.first" || reward.Amount.Value != 1 ||
                    reward.SourceKind != RewardSourceKind.Discovery || reward.SourceId != PhotographyFoundationSetup.DiscoveryId)
                    errors.Add("ECONOMY102 canonical discovery reward must grant exactly one provisional Explorer Star.");
                RewardDefinition mission = catalog.Definitions.SingleOrDefault(item => item.SourceKind == RewardSourceKind.Mission);
                if (mission == null || mission.Id.Value != "reward.mission.photograph-toucan.complete" || mission.Amount.Value != 2 ||
                    mission.SourceId != "mission.vertical-slice.photograph-toucan")
                    errors.Add("ECONOMY109 canonical mission reward must grant exactly two provisional Explorer Stars.");
            }
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            EconomyView[] views = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<EconomyView>(true)).ToArray();
            if (views.Length != 1) errors.Add($"ECONOMY103 Bootstrap requires exactly one EconomyView; found {views.Length}.");
            else
            {
                if (views[0].GetComponentInChildren<SafeAreaFitter>(true) == null) errors.Add("ECONOMY104 EconomyView must use the central safe-area adapter.");
                foreach (Button button in views[0].GetComponentsInChildren<Button>(true))
                    if (((UnityEngine.RectTransform)button.transform).rect.width < 64f || ((UnityEngine.RectTransform)button.transform).rect.height < 64f)
                        errors.Add("ECONOMY105 economy touch targets must be at least 64x64 logical units.");
            }
            string root = Path.Combine(Directory.GetCurrentDirectory(), "Assets/_Game/Application/Economy");
            foreach (string file in Directory.GetFiles(root, "*.cs"))
            {
                string source = File.ReadAllText(file);
                foreach (string forbidden in new[] { "IAP", "Purchase", "Ads", "UnityEngine.Random", "System.Random", "premium", "streak", "timer" })
                    if (source.IndexOf(forbidden, StringComparison.OrdinalIgnoreCase) >= 0)
                        errors.Add($"ECONOMY106 forbidden coupling/mechanic '{forbidden}' in {Path.GetFileName(file)}.");
            }
            string viewSource = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),
                "Assets/_Game/Presentation/Economy/EconomyView.cs"));
            if (!viewSource.Contains("#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES") ||
                !viewSource.Contains("_debugGrant.gameObject.SetActive(false)"))
                errors.Add("ECONOMY107 debug grant must compile behind the Development define and be hidden in Release.");
            if (AssetDatabase.FindAssets("reward.debug", new[] { EconomyFoundationSetup.Root }).Length > 0)
                errors.Add("ECONOMY108 debug reward must not exist as a runtime Content asset.");
            return errors;
        }
    }
}
