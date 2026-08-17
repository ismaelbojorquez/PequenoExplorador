using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using PequenoExplorador.Content.Data;

namespace PequenoExplorador.Editor.BuildTools
{
    internal static class ContentValidationService
    {
        public static IReadOnlyList<string> Validate(ContentValidationMode mode = ContentValidationMode.Development)
        {
            var violations = new List<string>();
            violations.AddRange(ContentCatalogValidationService.Validate(mode));
            violations.AddRange(InteractionCatalogValidationService.Validate(mode));
            violations.AddRange(WorldCatalogValidationService.Validate(mode));
            violations.AddRange(RuntimeConfigurationValidationService.Validate());
            violations.AddRange(LocalizationValidationService.Validate());
            violations.AddRange(AudioValidationService.Validate());
            violations.AddRange(ToucanFixtureValidationService.Validate(mode));
            violations.AddRange(EconomyValidationService.Validate());
            violations.AddRange(MissionValidationService.Validate(mode));
            violations.AddRange(LearningValidationService.Validate(mode));
            string directory = Path.Combine(UnityEngine.Application.dataPath, "_Game", "Content", "Placeholders");
            if (!Directory.Exists(directory))
            {
                return violations;
            }

            foreach (string path in Directory.GetFiles(directory, "*.placeholder.json", SearchOption.AllDirectories)
                         .OrderBy(value => value, StringComparer.Ordinal))
            {
                PlaceholderMetadata metadata;
                try
                {
                    metadata = JsonUtility.FromJson<PlaceholderMetadata>(File.ReadAllText(path));
                }
                catch (Exception exception)
                {
                    violations.Add($"CONTENT001 invalid JSON in {AssetPath(path)}: {exception.Message}");
                    continue;
                }

                if (metadata == null || string.IsNullOrWhiteSpace(metadata.id) ||
                    !metadata.id.StartsWith("PH_", StringComparison.Ordinal))
                {
                    violations.Add($"CONTENT002 placeholder id must use PH_ prefix: {AssetPath(path)}");
                }

                if (metadata == null || !metadata.isPlaceholder)
                {
                    violations.Add($"CONTENT003 isPlaceholder must be true: {AssetPath(path)}");
                }

                if (metadata == null || string.IsNullOrWhiteSpace(metadata.owner) ||
                    string.IsNullOrWhiteSpace(metadata.purpose) || string.IsNullOrWhiteSpace(metadata.replaceByPhase))
                {
                    violations.Add($"CONTENT004 owner, purpose and replaceByPhase are required: {AssetPath(path)}");
                }

                if (metadata == null || metadata.releaseApproved ||
                    !string.Equals(metadata.releaseStatus, "Blocked", StringComparison.OrdinalIgnoreCase))
                {
                    violations.Add($"CONTENT005 placeholders must remain blocked and unapproved for Release: {AssetPath(path)}");
                }
            }

            return violations;
        }

        private static string AssetPath(string path)
        {
            return "Assets" + path.Substring(UnityEngine.Application.dataPath.Length).Replace(Path.DirectorySeparatorChar, '/');
        }

        [Serializable]
        private sealed class PlaceholderMetadata
        {
            public string id;
            public bool isPlaceholder;
            public string owner;
            public string purpose;
            public string replaceByPhase;
            public string releaseStatus;
            public bool releaseApproved;
        }
    }
}
