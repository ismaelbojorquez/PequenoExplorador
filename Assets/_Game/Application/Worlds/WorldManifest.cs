using System;
using System.Collections.Generic;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Worlds
{
    public sealed class WorldManifest : IWorldDefinition
    {
        public WorldManifest(
            WorldId id,
            int manifestVersion,
            string contentVersion,
            LocalizedKey displayName,
            SceneContentId scene,
            IEnumerable<string> labels,
            SpawnPointId spawnPoint,
            IEnumerable<CheckpointId> checkpoints,
            IEnumerable<ContentCatalogId> contentCatalogIds,
            AudioCueId musicCue,
            AudioCueId ambienceCue,
            IEnumerable<WorldRequirementId> requirements,
            long estimatedInstalledBytes,
            EditorialMetadata editorial)
        {
            if (!id.IsValid || !scene.IsValid || !spawnPoint.IsValid) throw new ArgumentException("World manifest IDs are invalid.");
            if (manifestVersion < 1) throw new ArgumentOutOfRangeException(nameof(manifestVersion));
            if (string.IsNullOrWhiteSpace(contentVersion)) throw new ArgumentException("Content version is required.", nameof(contentVersion));
            if (estimatedInstalledBytes < 0) throw new ArgumentOutOfRangeException(nameof(estimatedInstalledBytes));
            Id = id;
            ManifestVersion = manifestVersion;
            ContentVersion = contentVersion;
            DisplayName = displayName;
            Scene = scene;
            Labels = Freeze(labels);
            SpawnPoint = spawnPoint;
            Checkpoints = Freeze(checkpoints);
            ContentCatalogIds = Freeze(contentCatalogIds);
            MusicCue = musicCue;
            AmbienceCue = ambienceCue;
            Requirements = Freeze(requirements);
            EstimatedInstalledBytes = estimatedInstalledBytes;
            Editorial = editorial ?? throw new ArgumentNullException(nameof(editorial));
        }

        public WorldId Id { get; }
        public int ManifestVersion { get; }
        public string ContentVersion { get; }
        public LocalizedKey DisplayName { get; }
        public SceneContentId Scene { get; }
        public IReadOnlyList<string> Labels { get; }
        public SpawnPointId SpawnPoint { get; }
        public IReadOnlyList<CheckpointId> Checkpoints { get; }
        public IReadOnlyList<ContentCatalogId> ContentCatalogIds { get; }
        public AudioCueId MusicCue { get; }
        public AudioCueId AmbienceCue { get; }
        public IReadOnlyList<WorldRequirementId> Requirements { get; }
        public long EstimatedInstalledBytes { get; }
        public EditorialMetadata Editorial { get; }

        private static IReadOnlyList<T> Freeze<T>(IEnumerable<T> values) =>
            Array.AsReadOnly(values == null ? Array.Empty<T>() : new List<T>(values).ToArray());
    }
}
