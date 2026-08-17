using System;
using System.Collections.Generic;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Album
{
    public sealed class AlbumEntryViewModel
    {
        private readonly IReadOnlyList<AlbumFactViewModel> _facts;

        public AlbumEntryViewModel(
            DiscoveryId id,
            CategoryId categoryId,
            AlbumEntryState state,
            LocalizedKey displayName,
            VisualAssetId visualAssetId,
            AudioCueId audioCueId,
            bool hasPlayableAudio,
            string photoFileReference,
            int observationCount,
            IEnumerable<AlbumFactViewModel> facts)
        {
            if (!id.IsValid || !categoryId.IsValid) throw new ArgumentException("Album entry IDs are invalid.");
            Id = id;
            CategoryId = categoryId;
            State = state;
            DisplayName = displayName;
            VisualAssetId = visualAssetId;
            AudioCueId = audioCueId;
            HasPlayableAudio = state == AlbumEntryState.Discovered && hasPlayableAudio;
            PhotoFileReference = photoFileReference ?? string.Empty;
            ObservationCount = state == AlbumEntryState.Discovered ? Math.Max(1, observationCount) : 0;
            _facts = Array.AsReadOnly(facts == null ? Array.Empty<AlbumFactViewModel>() : new List<AlbumFactViewModel>(facts).ToArray());
        }

        public DiscoveryId Id { get; }
        public CategoryId CategoryId { get; }
        public AlbumEntryState State { get; }
        public bool IsDiscovered => State == AlbumEntryState.Discovered;
        public LocalizedKey DisplayName { get; }
        public VisualAssetId VisualAssetId { get; }
        public AudioCueId AudioCueId { get; }
        public bool HasPlayableAudio { get; }
        public string PhotoFileReference { get; }
        public bool HasPhotoReference => !string.IsNullOrWhiteSpace(PhotoFileReference);
        public int ObservationCount { get; }
        public IReadOnlyList<AlbumFactViewModel> Facts => _facts;
    }
}
