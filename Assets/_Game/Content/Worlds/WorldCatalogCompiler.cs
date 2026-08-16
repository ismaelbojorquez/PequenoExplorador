using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Application.Worlds;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Content.Worlds
{
    public static class WorldCatalogCompiler
    {
        public static bool TryCompile(
            WorldCatalogAsset source,
            IContentCatalog loadedContent,
            ContentValidationMode mode,
            IWorldReferenceResolver resolver,
            out WorldCatalog catalog,
            out IReadOnlyList<string> violations)
        {
            var errors = new List<string>();
            var entries = new List<WorldCatalogEntry>();
            var ids = new HashSet<WorldId>();
            catalog = null;
            if (source == null)
            {
                violations = new[] { "WORLD001 missing WorldCatalogAsset; create and wire the canonical local catalog." };
                return false;
            }
            if (loadedContent == null) errors.Add("WORLD002 loaded content catalog is missing; worlds cannot resolve content catalogs.");

            int slot = 0;
            foreach (WorldManifestAsset asset in source.Worlds)
            {
                if (asset == null) { errors.Add($"WORLD003 missing world manifest in catalog slot {slot}; assign or remove the slot."); slot++; continue; }
                slot++;
                string path = resolver?.Describe(asset) ?? asset.name;
                try
                {
                    WorldId id = WorldId.Parse(asset.RawId);
                    if (!ids.Add(id)) { errors.Add($"WORLD004 duplicate world ID '{id}' at {path}; keep one manifest."); continue; }
                    if (asset.ManifestVersion < 1) errors.Add($"WORLD005 manifestVersion at {path} must be >= 1.");
                    if (string.IsNullOrWhiteSpace(asset.ContentVersion)) errors.Add($"WORLD006 contentVersion at {path} is required.");
                    if (asset.EstimatedInstalledBytes < 0) errors.Add($"WORLD007 estimated size at {path} cannot be negative.");
                    SceneContentId scene = SceneContentId.Parse(asset.SceneAddress);
                    SpawnPointId spawn = SpawnPointId.Parse(asset.SpawnPointId);
                    CheckpointId[] checkpoints = asset.CheckpointIds.Select(CheckpointId.Parse).Distinct()
                        .OrderBy(item => item.Value, StringComparer.Ordinal).ToArray();
                    if (checkpoints.Length == 0 || checkpoints.Length != asset.CheckpointIds.Count)
                        errors.Add($"WORLD021 checkpoints at {path} must contain at least one valid unique ID.");
                    string[] labels = asset.Labels.Where(label => !string.IsNullOrWhiteSpace(label)).Distinct(StringComparer.Ordinal).OrderBy(label => label, StringComparer.Ordinal).ToArray();
                    if (labels.Length != asset.Labels.Count || !labels.Contains("scene") || !labels.Contains("world-" + id.Value.Substring("world.".Length)))
                        errors.Add($"WORLD008 labels at {path} must be unique/non-empty and include 'scene' plus 'world-<id>'.");
                    var displayName = new LocalizedKey(asset.DisplayNameTable, asset.DisplayNameKey);
                    var music = new AudioCueId(asset.MusicCueId);
                    var ambience = new AudioCueId(asset.AmbienceCueId);
                    if (resolver != null && !resolver.HasLocalization(displayName)) errors.Add($"WORLD009 missing ES/EN display name '{displayName}' at {path}.");
                    if (resolver != null && !resolver.HasAudioCue(music)) errors.Add($"WORLD010 missing music cue '{music}' at {path}.");
                    if (resolver != null && !resolver.HasAudioCue(ambience)) errors.Add($"WORLD011 missing ambience cue '{ambience}' at {path}.");
                    if (asset.Scene == null || string.IsNullOrEmpty(asset.Scene.AssetGUID) ||
                        resolver != null && !resolver.HasLocalScene(asset.Scene, scene, id, labels))
                        errors.Add($"WORLD012 scene AssetReference/address/group/labels are invalid at {path}; keep the scene local in its world group.");
                    if (resolver != null && !resolver.HasSpawnPoint(asset.Scene, spawn))
                        errors.Add($"WORLD013 spawn '{spawn}' is absent from the referenced scene at {path}.");

                    var contentIds = new List<ContentCatalogId>();
                    foreach (ContentCatalogAsset content in asset.ContentCatalogs)
                    {
                        if (content == null) { errors.Add($"WORLD014 missing content catalog reference at {path}."); continue; }
                        ContentCatalogId contentId = content.Id;
                        if (loadedContent != null && contentId != loadedContent.Id)
                            errors.Add($"WORLD015 content catalog '{contentId}' at {path} is not loaded by Bootstrap.");
                        contentIds.Add(contentId);
                    }
                    if (contentIds.Count == 0) errors.Add($"WORLD016 world at {path} requires at least one content catalog.");
                    WorldRequirementId[] requirements = asset.Requirements.Select(WorldRequirementId.Parse).Distinct().OrderBy(item => item.Value, StringComparer.Ordinal).ToArray();
                    EditorialMetadata editorial = asset.Editorial?.ToRuntime();
                    if (editorial == null) errors.Add($"WORLD017 editorial metadata is missing at {path}.");
                    else if (mode == ContentValidationMode.Release && !editorial.IsReleaseApproved)
                        errors.Add($"WORLD018 Release rejects {editorial.State} or placeholder world '{id}' at {path}; approve and replace PH_ assets.");

                    if (editorial != null)
                    {
                        var manifest = new WorldManifest(id, asset.ManifestVersion, asset.ContentVersion, displayName, scene,
                            labels, spawn, checkpoints, contentIds, music, ambience, requirements, asset.EstimatedInstalledBytes, editorial);
                        entries.Add(new WorldCatalogEntry(manifest, asset.Availability));
                    }
                }
                catch (Exception exception) when (exception is FormatException || exception is ArgumentException)
                {
                    errors.Add($"WORLD019 invalid world manifest at {path}: {exception.Message}");
                }
            }

            if (errors.Count == 0)
            {
                try { catalog = new WorldCatalog(entries); }
                catch (ArgumentException exception) { errors.Add("WORLD020 world catalog index failed: " + exception.Message); }
            }
            violations = new ReadOnlyCollection<string>(errors);
            return errors.Count == 0;
        }
    }
}
