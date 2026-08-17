using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Album
{
    public sealed class AlbumQueryService
    {
        private readonly IContentCatalog _catalog;
        private readonly IDiscoveryProgressRepository _discoveries;
        private readonly IPhotoProgressRepository _photos;

        public AlbumQueryService(
            IContentCatalog catalog,
            IDiscoveryProgressRepository discoveries,
            IPhotoProgressRepository photos)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _discoveries = discoveries ?? throw new ArgumentNullException(nameof(discoveries));
            _photos = photos ?? throw new ArgumentNullException(nameof(photos));
        }

        public AlbumSnapshot Query(WorldId worldId, CategoryId? selectedCategory = null)
        {
            if (!worldId.IsValid) throw new ArgumentException("Album world ID is invalid.", nameof(worldId));

            DiscoveryDefinition[] eligible = _catalog.Discoveries
                .Where(item => item.WorldId.Equals(worldId) && item.Editorial.IsReleaseApproved)
                .Where(item => _catalog.TryGetCategory(item.CategoryId, out CategoryDefinition category) &&
                               category.Editorial.IsReleaseApproved)
                .OrderBy(item => item.Id.Value, StringComparer.Ordinal)
                .ToArray();
            var progress = _discoveries.Current.Discoveries.ToDictionary(item => item.Id);
            var photos = _photos.Current.Photos.ToDictionary(item => item.DiscoveryId);

            AlbumCategoryViewModel[] categories = eligible
                .GroupBy(item => item.CategoryId)
                .Select(group =>
                {
                    _catalog.TryGetCategory(group.Key, out CategoryDefinition category);
                    return new AlbumCategoryViewModel(
                        group.Key,
                        category.DisplayName,
                        group.Count(item => progress.ContainsKey(item.Id)),
                        group.Count());
                })
                .OrderBy(item => item.Id.Value, StringComparer.Ordinal)
                .ToArray();

            IEnumerable<DiscoveryDefinition> filtered = selectedCategory.HasValue
                ? eligible.Where(item => item.CategoryId.Equals(selectedCategory.Value))
                : eligible;
            AlbumEntryViewModel[] entries = filtered.Select(item => MapEntry(item, progress, photos)).ToArray();
            return new AlbumSnapshot(worldId, selectedCategory, categories, entries);
        }

        private AlbumEntryViewModel MapEntry(
            DiscoveryDefinition definition,
            IReadOnlyDictionary<DiscoveryId, DiscoveryProgress> progress,
            IReadOnlyDictionary<DiscoveryId, PhotoProgress> photos)
        {
            if (!progress.TryGetValue(definition.Id, out DiscoveryProgress discovery))
            {
                return new AlbumEntryViewModel(
                    definition.Id,
                    definition.CategoryId,
                    AlbumEntryState.Locked,
                    default,
                    default,
                    default,
                    false,
                    string.Empty,
                    0,
                    Array.Empty<AlbumFactViewModel>());
            }

            photos.TryGetValue(definition.Id, out PhotoProgress photo);
            return new AlbumEntryViewModel(
                definition.Id,
                definition.CategoryId,
                AlbumEntryState.Discovered,
                definition.DisplayName,
                definition.VisualAssetId,
                definition.NameAudioCueId,
                definition.Album.HasPlayableAudio,
                photo?.FileReference,
                discovery.Count,
                BuildFacts(definition.Album));
        }

        private IReadOnlyList<AlbumFactViewModel> BuildFacts(AlbumEntryMetadata album)
        {
            return new[]
            {
                BuildFact(AlbumFactField.Habitat, album.HabitatFactId),
                BuildFact(AlbumFactField.Diet, album.DietFactId),
                BuildFact(AlbumFactField.Size, album.SizeFactId),
                BuildFact(AlbumFactField.Curiosity, album.CuriosityFactId),
                BuildFact(AlbumFactField.Sound, album.SoundFactId)
            };
        }

        private AlbumFactViewModel BuildFact(AlbumFactField field, EducationalFactId factId)
        {
            EducationalFactDefinition fact = null;
            bool valid = factId.IsValid &&
                         _catalog.TryGetFact(factId, out fact) &&
                         fact.Editorial.IsReleaseApproved;
            return new AlbumFactViewModel(field, valid ? fact.ChildCopy : default, valid);
        }
    }
}
