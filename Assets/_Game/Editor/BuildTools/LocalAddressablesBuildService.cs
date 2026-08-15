using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace PequenoExplorador.Editor.BuildTools
{
    internal static class LocalAddressablesBuildService
    {
        public static void BuildDevelopment()
        {
            Build(SceneFlowFoundationSetup.DevelopmentProfileName);
        }

        public static void Build(string profileName)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                throw new InvalidOperationException("AddressableAssetSettings is missing.");
            }

            string profileId = settings.profileSettings.GetProfileId(profileName);
            if (string.IsNullOrEmpty(profileId))
            {
                throw new InvalidOperationException("Addressables profile is missing: " + profileName);
            }

            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
            {
                throw new InvalidOperationException("Unity could not switch Addressables content to Android.");
            }

            settings.activeProfileId = profileId;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
            if (result == null || !string.IsNullOrEmpty(result.Error))
            {
                throw new InvalidOperationException("Addressables content build failed: " + (result?.Error ?? "No result"));
            }

            if (string.IsNullOrEmpty(result.OutputPath) || !File.Exists(result.OutputPath))
            {
                throw new InvalidOperationException("Addressables build did not produce runtime settings.");
            }

            ArtifactReportWriter.WriteAddressablesManifest(
                profileName,
                result.OutputPath,
                result.Duration,
                result.LocationCount);
            Debug.Log(
                $"PE_ADDRESSABLES_BUILD_OK profile={profileName} target=Android locations={result.LocationCount} output={BuildArtifactPaths.RelativeToProject(result.OutputPath)}");
        }
    }
}
