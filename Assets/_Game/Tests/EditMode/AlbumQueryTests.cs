using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PequenoExplorador.Application.Album;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class AlbumQueryTests
    {
        private static readonly WorldId Jungle = WorldId.Parse("world.jungle");
        private static readonly WorldId OtherWorld = WorldId.Parse("world.test-other");
        private static readonly CategoryId Animals = CategoryId.Parse("category.discovery.animals");
        private static readonly CategoryId Plants = CategoryId.Parse("category.discovery.plants");
        private static readonly DiscoveryId Toucan = DiscoveryId.Parse("discovery.jungle.keel-billed-toucan");
        private static readonly DiscoveryId Frog = DiscoveryId.Parse("discovery.jungle.frog-fixture");
        private static readonly DiscoveryId Fern = DiscoveryId.Parse("discovery.jungle.fern-fixture");
        private static readonly EducationalFactId Habitat = EducationalFactId.Parse("fact.jungle.keel-billed-toucan.habitat");
        private static readonly EducationalFactId Diet = EducationalFactId.Parse("fact.jungle.keel-billed-toucan.diet");

        [Test]
        public void CountsAndFiltersUseOnlyApprovedCatalogEntries()
        {
            var progress = new MemoryProgress(BuildProgress(Toucan));
            ContentCatalog catalog = CreateCatalog(
                CreateDiscovery(Toucan, Animals, EditorialState.Approved),
                CreateDiscovery(Frog, Animals, EditorialState.Draft),
                CreateDiscovery(Fern, Plants, EditorialState.Approved),
                CreateDiscovery(DiscoveryId.Parse("discovery.test-other.bird"), Animals, EditorialState.Approved, OtherWorld));
            var query = new AlbumQueryService(catalog, progress, progress);

            AlbumSnapshot all = query.Query(Jungle);
            AlbumSnapshot animals = query.Query(Jungle, Animals);

            Assert.That(all.Total, Is.EqualTo(2));
            Assert.That(all.Discovered, Is.EqualTo(1));
            Assert.That(all.Categories.Select(item => item.Id), Is.EquivalentTo(new[] { Animals, Plants }));
            Assert.That(animals.Entries.Count, Is.EqualTo(1));
            Assert.That(animals.Entries[0].Id, Is.EqualTo(Toucan));
            Assert.That(all.Entries.Any(item => item.Id == Frog), Is.False, "Draft must never be revealed by album queries.");
        }

        [Test]
        public void LockedEntryDoesNotExposeNameFactsAudioVisualOrPhoto()
        {
            var progress = new MemoryProgress(PlayerProgress.CreateDefault());
            var query = new AlbumQueryService(CreateCatalog(CreateDiscovery(Toucan, Animals, EditorialState.Approved)), progress, progress);

            AlbumEntryViewModel entry = query.Query(Jungle).Entries.Single();

            Assert.That(entry.State, Is.EqualTo(AlbumEntryState.Locked));
            Assert.That(entry.DisplayName.Entry, Is.Null);
            Assert.That(entry.VisualAssetId.IsValid, Is.False);
            Assert.That(entry.AudioCueId.Value, Is.Null);
            Assert.That(entry.Facts, Is.Empty);
            Assert.That(entry.HasPhotoReference, Is.False);
            Assert.That(entry.ObservationCount, Is.Zero);
        }

        [Test]
        public void DiscoveredEntryMapsApprovedFactsAndBestPhotoMetadata()
        {
            var progress = new MemoryProgress(BuildProgress(Toucan, withPhoto: true));
            var query = new AlbumQueryService(CreateCatalog(CreateDiscovery(Toucan, Animals, EditorialState.Approved)), progress, progress);

            AlbumEntryViewModel entry = query.Query(Jungle).Entries.Single();

            Assert.That(entry.IsDiscovered, Is.True);
            Assert.That(entry.DisplayName.Entry, Is.EqualTo("content.discovery.fixture.name"));
            Assert.That(entry.PhotoFileReference, Is.EqualTo("discovery_jungle_keel-billed-toucan-900.png"));
            Assert.That(entry.Facts.Single(item => item.Field == AlbumFactField.Habitat).HasApprovedValue, Is.True);
            Assert.That(entry.Facts.Single(item => item.Field == AlbumFactField.Diet).HasApprovedValue, Is.True);
            Assert.That(entry.Facts.Single(item => item.Field == AlbumFactField.Size).HasApprovedValue, Is.False,
                "Absent factual claims degrade safely instead of being invented.");
            Assert.That(entry.HasPlayableAudio, Is.False, "Placeholder confirm cue must not be presented as animal audio.");
        }

        [Test]
        public void RemovedProgressAndMissingFactAreIgnoredSafely()
        {
            DiscoveryId removed = DiscoveryId.Parse("discovery.jungle.removed");
            var progress = new MemoryProgress(BuildProgress(removed));
            DiscoveryDefinition toucan = CreateDiscovery(Toucan, Animals, EditorialState.Approved, albumFactOverride: EducationalFactId.Parse("fact.jungle.missing"));
            var query = new AlbumQueryService(CreateCatalog(toucan), progress, progress);

            AlbumSnapshot snapshot = query.Query(Jungle);

            Assert.That(snapshot.Total, Is.EqualTo(1));
            Assert.That(snapshot.Discovered, Is.Zero);
            Assert.That(snapshot.Entries.Single().IsDiscovered, Is.False);
        }

        [Test]
        public void MissingCategoryAndUnknownFilterReturnNoEntriesWithoutReadingSaveRaw()
        {
            var progress = new MemoryProgress(BuildProgress(Toucan));
            ContentCatalog missingCategory = new ContentCatalog(
                ContentCatalogId.Parse("catalog.album.test"),
                Array.Empty<CategoryDefinition>(),
                Array.Empty<TagDefinition>(),
                Array.Empty<ContentSourceRecord>(),
                CreateFacts(),
                new[] { CreateDiscovery(Toucan, Animals, EditorialState.Approved) },
                Array.Empty<DiscoveryIdAlias>());
            var query = new AlbumQueryService(missingCategory, progress, progress);

            Assert.That(query.Query(Jungle).Entries, Is.Empty);
            Assert.That(query.Query(Jungle, Plants).Entries, Is.Empty);
        }

        private static ContentCatalog CreateCatalog(params DiscoveryDefinition[] discoveries)
        {
            var categories = new[]
            {
                new CategoryDefinition(Animals, new LocalizedKey("Content", "content.category.animals"), Approved()),
                new CategoryDefinition(Plants, new LocalizedKey("Content", "content.category.plants"), Approved())
            };
            return new ContentCatalog(
                ContentCatalogId.Parse("catalog.album.test"),
                categories,
                Array.Empty<TagDefinition>(),
                Array.Empty<ContentSourceRecord>(),
                CreateFacts(),
                discoveries,
                Array.Empty<DiscoveryIdAlias>());
        }

        private static EducationalFactDefinition[] CreateFacts() => new[]
        {
            new EducationalFactDefinition(Habitat, new LocalizedKey("Content", "content.fact.fixture.habitat"), "habitat", Array.Empty<ContentSourceId>(), Approved()),
            new EducationalFactDefinition(Diet, new LocalizedKey("Content", "content.fact.fixture.diet"), "diet", Array.Empty<ContentSourceId>(), Approved())
        };

        private static DiscoveryDefinition CreateDiscovery(
            DiscoveryId id,
            CategoryId category,
            EditorialState state,
            WorldId? world = null,
            EducationalFactId? albumFactOverride = null)
        {
            EditorialMetadata editorial = state == EditorialState.Approved
                ? Approved()
                : new EditorialMetadata(state, true, "test", "BORRADOR · PH_");
            EducationalFactId habitat = albumFactOverride ?? Habitat;
            return new DiscoveryDefinition(
                id,
                world ?? Jungle,
                category,
                Array.Empty<TagId>(),
                new[] { Habitat, Diet },
                new LocalizedKey("Content", "content.discovery.fixture.name"),
                new AudioCueId("audio.feedback.confirm"),
                VisualAssetId.Parse("visual.album.fixture"),
                editorial,
                new AlbumEntryMetadata(habitat, Diet, default, Habitat, default, false));
        }

        private static EditorialMetadata Approved() =>
            new EditorialMetadata(EditorialState.Approved, false, "test", string.Empty);

        private static PlayerProgress BuildProgress(DiscoveryId id, bool withPhoto = false)
        {
            var discoveries = new[] { new DiscoveryProgress(id, 2, "2026-08-17") };
            PhotoProgress[] photos = withPhoto
                ? new[] { new PhotoProgress(id, "discovery_jungle_keel-billed-toucan-900.png", 900, 384, 216, 1234) }
                : Array.Empty<PhotoProgress>();
            return new PlayerProgress(0, Array.Empty<string>(), discoveries, Array.Empty<string>(), photos,
                Array.Empty<string>(), PlayerPreferences.CreateDefault());
        }

        private sealed class MemoryProgress : IDiscoveryProgressRepository, IPhotoProgressRepository
        {
            public MemoryProgress(PlayerProgress current) => Current = current;
            public bool IsReadOnly => false;
            public PlayerProgress Current { get; private set; }
            public void Commit(PlayerProgress progress) => Current = progress;
        }
    }
}
