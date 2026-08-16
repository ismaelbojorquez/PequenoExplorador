using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Application.Worlds;
using PequenoExplorador.Content.Audio;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Content.Worlds;
using PequenoExplorador.Domain.Content;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Localization;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.SceneManagement;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class WorldCatalogValidationService
    {
        public static IReadOnlyList<string> Validate(ContentValidationMode mode, bool writeReports = true)
        {
            var violations = new List<string>();
            WorldCatalogAsset asset = AssetDatabase.LoadAssetAtPath<WorldCatalogAsset>(WorldFoundationSetup.CatalogPath);
            ContentCatalogAsset contentAsset = AssetDatabase.LoadAssetAtPath<ContentCatalogAsset>(ContentFoundationSetup.CatalogPath);
            ContentCatalog content = null;
            IReadOnlyList<string> contentErrors = Array.Empty<string>();
            if (contentAsset == null || !contentAsset.TryBuildRuntimeCatalog(ContentValidationMode.Development, out content, out contentErrors))
                violations.Add("WORLD100 canonical content catalog cannot be compiled: " + string.Join(" | ", contentErrors));
            WorldCatalog catalog = null;
            if (content != null)
            {
                WorldCatalogCompiler.TryCompile(asset, content, mode, new EditorResolver(), out catalog, out var compilerErrors);
                violations.AddRange(compilerErrors);
            }
            if (writeReports) WriteReport(mode, asset, catalog, violations);
            return violations;
        }

        private static void WriteReport(ContentValidationMode mode, WorldCatalogAsset asset, WorldCatalog catalog, IReadOnlyList<string> violations)
        {
            string reports = BuildArtifactPaths.RequireInsideArtifacts(Path.Combine(BuildArtifactPaths.ArtifactsRoot, "reports"));
            Directory.CreateDirectory(reports);
            string suffix = mode == ContentValidationMode.Release ? "release" : "development";
            string[] ids = catalog?.Worlds.Select(entry => entry.Manifest.Id.Value).ToArray() ??
                           asset?.Worlds.Where(world => world != null).Select(world => world.RawId).OrderBy(id => id, StringComparer.Ordinal).ToArray() ??
                           Array.Empty<string>();
            var report = new WorldCatalogReport
            {
                generatedUtc = DateTimeOffset.UtcNow.ToString("O"),
                mode = mode.ToString(),
                success = violations.Count == 0,
                worlds = ids.Length,
                worldIds = ids,
                remote = false,
                violations = violations.ToArray()
            };
            File.WriteAllText(Path.Combine(reports, $"world-catalog-{suffix}.json"), JsonUtility.ToJson(report, true) + Environment.NewLine);
            var markdown = new StringBuilder().AppendLine("# World catalog validation").AppendLine()
                .AppendLine($"- Mode: `{report.mode}`").AppendLine($"- Result: `{(report.success ? "PASS" : "FAIL")}`")
                .AppendLine($"- Worlds: {report.worlds}").AppendLine("- Remote: `false`").AppendLine().AppendLine("## IDs");
            foreach (string id in ids) markdown.AppendLine("- `" + id + "`");
            markdown.AppendLine().AppendLine("## Violations");
            if (violations.Count == 0) markdown.AppendLine("- None.");
            else foreach (string violation in violations) markdown.AppendLine("- " + violation);
            File.WriteAllText(Path.Combine(reports, $"world-catalog-{suffix}.md"), markdown.ToString());
        }

        private sealed class EditorResolver : IWorldReferenceResolver
        {
            public string Describe(WorldManifestAsset asset) => AssetDatabase.GetAssetPath(asset);

            public bool HasLocalization(LocalizedKey key)
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

            public bool HasAudioCue(AudioCueId cueId)
            {
                AudioCueCatalogAsset catalog = AssetDatabase.LoadAssetAtPath<AudioCueCatalogAsset>(AudioFoundationSetup.CatalogPath);
                return catalog != null && catalog.Cues.Any(cue => cue != null && cue.CueId == cueId);
            }

            public bool HasLocalScene(UnityEngine.AddressableAssets.AssetReference scene, SceneContentId address, WorldId worldId, IReadOnlyList<string> labels)
            {
                if (scene == null || string.IsNullOrEmpty(scene.AssetGUID)) return false;
                AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
                AddressableAssetEntry entry = settings?.FindAssetEntry(scene.AssetGUID);
                return entry != null && entry.address == address.Value && entry.AssetPath.EndsWith(".unity", StringComparison.OrdinalIgnoreCase) &&
                       entry.parentGroup != settings.DefaultGroup && labels.All(label => entry.labels.Contains(label));
            }

            public bool HasSpawnPoint(UnityEngine.AddressableAssets.AssetReference scene, SpawnPointId spawnPoint)
            {
                string path = AssetDatabase.GUIDToAssetPath(scene?.AssetGUID ?? string.Empty);
                if (string.IsNullOrEmpty(path)) return false;
                Scene loaded = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                try
                {
                    return loaded.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<WorldSpawnPointMarker>(true))
                        .Any(marker => marker.SpawnPointId == spawnPoint.Value);
                }
                finally
                {
                    EditorSceneManager.CloseScene(loaded, true);
                }
            }
        }

        [Serializable]
        private sealed class WorldCatalogReport
        {
            public string generatedUtc;
            public string mode;
            public bool success;
            public int worlds;
            public string[] worldIds;
            public bool remote;
            public string[] violations;
        }
    }
}
