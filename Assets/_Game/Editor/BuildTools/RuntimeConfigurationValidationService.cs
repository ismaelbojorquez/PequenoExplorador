using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Content.Configuration;
using UnityEditor;

namespace PequenoExplorador.Editor.BuildTools
{
    internal static class RuntimeConfigurationValidationService
    {
        private const string ConfigurationRoot = "Assets/_Game/Content/Configuration";
        private const string RuntimeResourceSegment = "/Resources/Configuration/";

        public static IReadOnlyList<string> Validate()
        {
            string[] guids = AssetDatabase.FindAssets("t:AppConfigAsset", new[] { ConfigurationRoot });
            AppConfigAsset[] assets = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(AssetDatabase.LoadAssetAtPath<AppConfigAsset>)
                .Where(asset => asset != null)
                .ToArray();
            var violations = new List<string>();

            foreach (AppConfigAsset asset in assets)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                if (!path.Contains(RuntimeResourceSegment, StringComparison.Ordinal))
                {
                    violations.Add(
                        $"CONFIG201 {path} must be under Content/Configuration/Resources/Configuration for local runtime loading.");
                }
            }

            if (!AppConfigCatalog.TryCreate(assets, out _, out IReadOnlyList<string> catalogViolations))
            {
                violations.AddRange(catalogViolations);
            }

            return violations;
        }
    }
}
