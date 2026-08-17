using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Learning;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Content.Economy;
using PequenoExplorador.Content.Learning;
using PequenoExplorador.Content.Audio;
using PequenoExplorador.Content.Interaction;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Learning;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.SceneManagement;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class LearningValidationService
    {
        public static IReadOnlyList<string> Validate(ContentValidationMode mode = ContentValidationMode.Development)
        {
            var errors = new List<string>();
            LearningCatalogAsset asset = AssetDatabase.LoadAssetAtPath<LearningCatalogAsset>(LearningFoundationSetup.CatalogPath);
            RewardCatalogAsset rewardAsset = AssetDatabase.LoadAssetAtPath<RewardCatalogAsset>(EconomyFoundationSetup.CatalogPath);
            RewardCatalog rewards = null;
            ContentCatalog content = null;
            ContentCatalogAsset contentAsset = AssetDatabase.LoadAssetAtPath<ContentCatalogAsset>(ContentFoundationSetup.CatalogPath);
            AudioCueCatalogAsset audio = AssetDatabase.LoadAssetAtPath<AudioCueCatalogAsset>(AudioFoundationSetup.CatalogPath);
            if (rewardAsset == null) errors.Add("LEARN100 reward catalog is missing.");
            else if (!rewardAsset.TryBuild(out rewards, out IReadOnlyList<string> rewardErrors)) errors.AddRange(rewardErrors);
            if (contentAsset == null) errors.Add("LEARN108 content catalog is missing.");
            else if (!contentAsset.TryBuildRuntimeCatalog(ContentValidationMode.Development, out content, out IReadOnlyList<string> contentErrors))
                errors.AddRange(contentErrors);
            if (audio == null) errors.Add("LEARN109 audio catalog is missing.");
            InteractionDefinitionAsset toucanInteraction = AssetDatabase.LoadAssetAtPath<InteractionDefinitionAsset>(InteractionFoundationSetup.AnimalPath);
            if (toucanInteraction == null || toucanInteraction.LearningActivityId != "activity.jungle.keel-billed-toucan.choose-food")
                errors.Add("LEARN110 approved toucan interaction must reference the integrated activity ID.");
            if (asset == null) errors.Add("LEARN101 learning catalog is missing.");
            else if (rewards != null && content != null && audio != null)
            {
                bool built = asset.TryBuild(mode, rewards, content, HasLocalization,
                    cue => audio.Cues.Any(item => item != null && item.CueId == cue),
                    out LearningCatalog catalog, out IReadOnlyList<string> buildErrors);
                if (!built) errors.AddRange(buildErrors);
                else if (catalog.Activities.Count != 2 || catalog.Concepts.Count != 2 || catalog.Activities.Any(item => item.Options.Count != 3))
                    errors.Add("LEARN102 Prompt 24 requires two concepts, two activities and three options each.");
            }
            var registry = new LearningActivityStrategyRegistry(new ILearningActivityStrategy[] { new SingleChoiceActivityStrategy() });
            if (registry.TypeIds.Count != 1) errors.Add("LEARN103 exactly one baseline activity strategy must be registered.");
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            LearningActivityView[] views = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<LearningActivityView>(true)).ToArray();
            if (views.Length != 1) errors.Add($"LEARN104 Bootstrap requires exactly one LearningActivityView; found {views.Length}.");
            else
            {
                if (views[0].GetComponentInChildren<SafeAreaFitter>(true) == null) errors.Add("LEARN105 LearningActivityView requires SafeAreaFitter.");
                foreach (Button button in views[0].GetComponentsInChildren<Button>(true))
                    if (((UnityEngine.RectTransform)button.transform).rect.width < 64f || ((UnityEngine.RectTransform)button.transform).rect.height < 64f)
                        errors.Add("LEARN106 learning touch targets must be at least 64x64 logical units.");
            }
            string applicationRoot = Path.Combine(Directory.GetCurrentDirectory(), "Assets/_Game/Application/Learning");
            foreach (string path in Directory.GetFiles(applicationRoot, "*.cs"))
            {
                string source = File.ReadAllText(path);
                foreach (string forbidden in new[] { "UnityEngine", "GameObject", "Analytics", "Activator.CreateInstance", "System.Reflection", "raw event", "failed" })
                    if (source.IndexOf(forbidden, StringComparison.OrdinalIgnoreCase) >= 0)
                        errors.Add($"LEARN107 forbidden coupling/copy '{forbidden}' in {Path.GetFileName(path)}.");
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
