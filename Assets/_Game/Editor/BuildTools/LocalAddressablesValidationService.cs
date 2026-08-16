using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Infrastructure.SceneFlow;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class LocalAddressablesValidationService
    {
        public static IReadOnlyList<string> Validate()
        {
            var violations = new List<string>();
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                return new[] { "ADDR001 AddressableAssetSettings is missing" };
            }

            RequireProfile(settings, SceneFlowFoundationSetup.DevelopmentProfileName, violations);
            RequireProfile(settings, SceneFlowFoundationSetup.ReleaseProfileName, violations);
            string activeProfile = settings.profileSettings.GetProfileName(settings.activeProfileId);
            if (activeProfile != SceneFlowFoundationSetup.DevelopmentProfileName &&
                activeProfile != SceneFlowFoundationSetup.ReleaseProfileName)
            {
                violations.Add("ADDR002 active profile must be LocalDevelopment or LocalRelease");
            }

            if (settings.BuildRemoteCatalog)
            {
                violations.Add("ADDR003 remote catalog must remain disabled");
            }

            if (!settings.DisableCatalogUpdateOnStartup)
            {
                violations.Add("ADDR004 catalog update on startup must remain disabled");
            }

            if (!settings.ContentStateBuildPath.StartsWith("Library/", StringComparison.Ordinal))
            {
                violations.Add("ADDR014 generated content state must remain under Library");
            }

            AddressableAssetGroup shared = settings.FindGroup(SceneFlowFoundationSetup.SharedGroupName);
            AddressableAssetGroup jungle = settings.FindGroup(SceneFlowFoundationSetup.JungleGroupName);
            ValidateGroup(settings, shared, SceneFlowFoundationSetup.SharedGroupName, violations);
            ValidateGroup(settings, jungle, SceneFlowFoundationSetup.JungleGroupName, violations);
            foreach (AddressableAssetGroup localizationGroup in settings.groups
                         .Where(group => group != null && group.Name.StartsWith("Localization-", StringComparison.Ordinal)))
            {
                ValidateGroup(settings, localizationGroup, localizationGroup.Name, violations);
            }
            ValidateEntry(shared, LocalSceneAddresses.Camp, SceneFlowFoundationSetup.CampScenePath,
                new[] { SceneFlowFoundationSetup.SceneLabel, SceneFlowFoundationSetup.SharedLabel }, violations);
            ValidateEntry(jungle, LocalSceneAddresses.Jungle, SceneFlowFoundationSetup.JungleScenePath,
                new[] { SceneFlowFoundationSetup.SceneLabel, SceneFlowFoundationSetup.JungleLabel }, violations);
            ValidateNoSharedToJungleDependency(shared, jungle, violations);

            string settingsText = System.IO.File.ReadAllText(AssetDatabase.GetAssetPath(settings));
            if (settingsText.IndexOf("http://", StringComparison.OrdinalIgnoreCase) >= 0 ||
                settingsText.IndexOf("https://", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                violations.Add("ADDR005 no HTTP/CDN endpoint is allowed in local settings");
            }

            return violations;
        }

        private static void RequireProfile(
            AddressableAssetSettings settings,
            string profileName,
            ICollection<string> violations)
        {
            if (string.IsNullOrEmpty(settings.profileSettings.GetProfileId(profileName)))
            {
                violations.Add("ADDR006 missing profile: " + profileName);
            }
        }

        private static void ValidateGroup(
            AddressableAssetSettings settings,
            AddressableAssetGroup group,
            string expectedName,
            ICollection<string> violations)
        {
            if (group == null)
            {
                violations.Add("ADDR007 missing local group: " + expectedName);
                return;
            }

            BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
            if (schema == null || !schema.IncludeInBuild)
            {
                violations.Add("ADDR008 group must use an included AssetBundle schema: " + expectedName);
                return;
            }

            if (schema.BuildPath.GetName(settings) != AddressableAssetSettings.kLocalBuildPath ||
                schema.LoadPath.GetName(settings) != AddressableAssetSettings.kLocalLoadPath)
            {
                violations.Add("ADDR009 group paths must be Local.BuildPath/Local.LoadPath: " + expectedName);
            }

            if (schema.UseUnityWebRequestForLocalBundles)
            {
                violations.Add("ADDR010 local groups must not force UnityWebRequest: " + expectedName);
            }
        }

        private static void ValidateEntry(
            AddressableAssetGroup group,
            string address,
            string expectedPath,
            IEnumerable<string> expectedLabels,
            ICollection<string> violations)
        {
            AddressableAssetEntry entry = group?.entries.FirstOrDefault(item => item.address == address);
            if (entry == null || !string.Equals(entry.AssetPath, expectedPath, StringComparison.Ordinal))
            {
                violations.Add("ADDR011 missing or misplaced scene address: " + address);
                return;
            }

            foreach (string label in expectedLabels)
            {
                if (!entry.labels.Contains(label))
                {
                    violations.Add("ADDR012 missing label " + label + " on " + address);
                }
            }
        }

        private static void ValidateNoSharedToJungleDependency(
            AddressableAssetGroup shared,
            AddressableAssetGroup jungle,
            ICollection<string> violations)
        {
            if (shared == null || jungle == null)
            {
                return;
            }

            var junglePaths = new HashSet<string>(
                jungle.entries.Select(entry => entry.AssetPath),
                StringComparer.Ordinal);
            foreach (AddressableAssetEntry entry in shared.entries)
            {
                string forbidden = AssetDatabase.GetDependencies(entry.AssetPath, true)
                    .FirstOrDefault(junglePaths.Contains);
                if (!string.IsNullOrEmpty(forbidden))
                {
                    violations.Add("ADDR013 SharedLocal references JungleLocal: " + entry.AssetPath + " -> " + forbidden);
                }
            }
        }
    }
}
