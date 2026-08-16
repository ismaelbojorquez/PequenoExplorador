using System;
using System.Collections.Generic;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Content
{
    public sealed class DiscoveryDefinition
    {
        private readonly IReadOnlyList<TagId> _tagIds;
        private readonly IReadOnlyList<EducationalFactId> _factIds;
        public DiscoveryDefinition(
            DiscoveryId id,
            WorldId worldId,
            CategoryId categoryId,
            IEnumerable<TagId> tagIds,
            IEnumerable<EducationalFactId> factIds,
            LocalizedKey displayName,
            AudioCueId nameAudioCueId,
            VisualAssetId visualAssetId,
            EditorialMetadata editorial)
        {
            if (!id.IsValid || !worldId.IsValid || !categoryId.IsValid || !visualAssetId.IsValid)
                throw new ArgumentException("Discovery references contain an invalid typed ID.");
            Id = id;
            WorldId = worldId;
            CategoryId = categoryId;
            _tagIds = Array.AsReadOnly(tagIds == null ? Array.Empty<TagId>() : new List<TagId>(tagIds).ToArray());
            _factIds = Array.AsReadOnly(factIds == null ? Array.Empty<EducationalFactId>() : new List<EducationalFactId>(factIds).ToArray());
            DisplayName = displayName;
            NameAudioCueId = nameAudioCueId;
            VisualAssetId = visualAssetId;
            Editorial = editorial ?? throw new ArgumentNullException(nameof(editorial));
        }
        public DiscoveryId Id { get; }
        public WorldId WorldId { get; }
        public CategoryId CategoryId { get; }
        public IReadOnlyList<TagId> TagIds => _tagIds;
        public IReadOnlyList<EducationalFactId> FactIds => _factIds;
        public LocalizedKey DisplayName { get; }
        public AudioCueId NameAudioCueId { get; }
        public VisualAssetId VisualAssetId { get; }
        public EditorialMetadata Editorial { get; }
        public string DevelopmentWatermark => Editorial.DevelopmentWatermark;
    }
}
