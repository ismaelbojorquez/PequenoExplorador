using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Content.Audio;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Content.Interaction;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using Object = UnityEngine.Object;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class InteractionCatalogValidationService
    {
        public static IReadOnlyList<string> Validate(ContentValidationMode mode)
        {
            var violations = new List<string>();
            string[] paths = AssetDatabase.FindAssets(
                    "t:InteractionCatalogAsset",
                    new[] { InteractionFoundationSetup.Root })
                .Select(AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
            if (paths.Length != 1)
            {
                violations.Add($"INTERACTION100 expected one InteractionCatalogAsset; found {paths.Length}.");
                return violations;
            }
            InteractionCatalogAsset catalog = AssetDatabase.LoadAssetAtPath<InteractionCatalogAsset>(paths[0]);
            InteractionCatalogCompiler.TryCompile(
                catalog,
                mode,
                new EditorResolver(),
                out _,
                out IReadOnlyList<string> compilerViolations);
            violations.AddRange(compilerViolations);
            return violations;
        }

        private sealed class EditorResolver : IContentReferenceResolver
        {
            public string Describe(Object asset) => AssetDatabase.GetAssetPath(asset);

            public bool HasLocalization(LocalizedKey key)
            {
                StringTableCollection collection = LocalizationEditorSettings.GetStringTableCollection(key.Table);
                if (collection == null) return false;
                foreach (string locale in new[] { LocaleCode.Spanish, LocaleCode.English })
                {
                    StringTable table = collection.GetTable(new LocaleIdentifier(locale)) as StringTable;
                    if (table?.GetEntry(key.Entry) == null ||
                        string.IsNullOrWhiteSpace(table.GetEntry(key.Entry).Value)) return false;
                }
                return true;
            }

            public bool HasAudioCue(AudioCueId cueId)
            {
                AudioCueCatalogAsset catalog =
                    AssetDatabase.LoadAssetAtPath<AudioCueCatalogAsset>(AudioFoundationSetup.CatalogPath);
                return catalog != null && catalog.Cues.Any(cue => cue != null && cue.CueId == cueId);
            }

            public bool HasVisualAsset(PequenoExplorador.Domain.Content.VisualAssetId id, Object asset) => true;
        }
    }
}
