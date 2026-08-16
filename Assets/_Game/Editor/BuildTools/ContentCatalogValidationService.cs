using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Content.Audio;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Domain.Content;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using Object = UnityEngine.Object;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class ContentCatalogValidationService
    {
        public static IReadOnlyList<string> Validate(ContentValidationMode mode, bool writeReports = true)
        {
            var violations = new List<string>();
            string[] catalogPaths = AssetDatabase.FindAssets("t:ContentCatalogAsset", new[] { ContentFoundationSetup.Root })
                .Select(AssetDatabase.GUIDToAssetPath).OrderBy(path => path, StringComparer.Ordinal).ToArray();
            ContentCatalogAsset catalogAsset = catalogPaths.Length == 1
                ? AssetDatabase.LoadAssetAtPath<ContentCatalogAsset>(catalogPaths[0])
                : null;
            if (catalogPaths.Length != 1)
                violations.Add($"DATA100 expected exactly one ContentCatalogAsset under {ContentFoundationSetup.Root}; found {catalogPaths.Length}.");

            ContentCatalog catalog = null;
            if (catalogAsset != null)
            {
                var resolver = new EditorContentReferenceResolver();
                ContentCatalogCompiler.TryCompile(catalogAsset, mode, resolver, out catalog, out IReadOnlyList<string> compilerViolations);
                violations.AddRange(compilerViolations);
            }

            if (writeReports) WriteReports(mode, catalogAsset, catalog, violations);
            return violations;
        }

        private static void WriteReports(ContentValidationMode mode, ContentCatalogAsset asset, ContentCatalog catalog, IReadOnlyList<string> violations)
        {
            string reports = BuildArtifactPaths.RequireInsideArtifacts(Path.Combine(BuildArtifactPaths.ArtifactsRoot, "reports"));
            Directory.CreateDirectory(reports);
            string suffix = mode == ContentValidationMode.Release ? "release" : "development";
            string[] discoveryIds = catalog?.Discoveries.Select(item => item.Id.Value).OrderBy(value => value, StringComparer.Ordinal).ToArray()
                                    ?? asset?.Discoveries.Where(item => item != null).Select(item => item.RawId).OrderBy(value => value, StringComparer.Ordinal).ToArray()
                                    ?? Array.Empty<string>();
            var report = new ContentCatalogReport
            {
                generatedUtc = DateTimeOffset.UtcNow.ToString("O"),
                mode = mode.ToString(),
                success = violations.Count == 0,
                categories = asset?.Categories.Count ?? 0,
                tags = asset?.Tags.Count ?? 0,
                sources = asset?.Sources.Count ?? 0,
                facts = asset?.Facts.Count ?? 0,
                discoveries = asset?.Discoveries.Count ?? 0,
                aliases = asset?.DiscoveryAliases.Count ?? 0,
                discoveryIds = discoveryIds,
                violations = violations.ToArray()
            };
            File.WriteAllText(Path.Combine(reports, $"content-catalog-{suffix}.json"), JsonUtility.ToJson(report, true) + Environment.NewLine);
            var markdown = new StringBuilder()
                .AppendLine("# Content catalog validation")
                .AppendLine()
                .AppendLine($"- Mode: `{report.mode}`")
                .AppendLine($"- Result: `{(report.success ? "PASS" : "FAIL")}`")
                .AppendLine($"- Definitions: categories {report.categories}, tags {report.tags}, sources {report.sources}, facts {report.facts}, discoveries {report.discoveries}, aliases {report.aliases}")
                .AppendLine()
                .AppendLine("## Discoveries");
            foreach (string id in report.discoveryIds) markdown.AppendLine("- `" + id + "`");
            markdown.AppendLine().AppendLine("## Violations");
            if (report.violations.Length == 0) markdown.AppendLine("- None.");
            else foreach (string violation in report.violations) markdown.AppendLine("- " + violation);
            File.WriteAllText(Path.Combine(reports, $"content-catalog-{suffix}.md"), markdown.ToString());
        }

        private sealed class EditorContentReferenceResolver : IContentReferenceResolver
        {
            public string Describe(Object asset)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                return string.IsNullOrWhiteSpace(path) ? asset?.name ?? "<missing asset>" : path;
            }

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

            public bool HasVisualAsset(VisualAssetId id, Object asset)
            {
                if (!id.IsValid || asset == null) return false;
                string path = AssetDatabase.GetAssetPath(asset);
                return !string.IsNullOrWhiteSpace(path) && path.StartsWith("Assets/", StringComparison.Ordinal) && AssetDatabase.LoadMainAssetAtPath(path) != null;
            }

            public bool HasDiscovery(DiscoveryId id) => true;
        }

        [Serializable]
        private sealed class ContentCatalogReport
        {
            public string generatedUtc;
            public string mode;
            public bool success;
            public int categories;
            public int tags;
            public int sources;
            public int facts;
            public int discoveries;
            public int aliases;
            public string[] discoveryIds;
            public string[] violations;
        }
    }
}
