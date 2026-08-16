using System;
using System.Collections.Generic;
using NUnit.Framework;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Tests.EditMode.Fixtures;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class DiscoveryProgressTests
    {
        private static readonly WorldId Jungle = WorldId.Parse("world.jungle");
        private static readonly CategoryId Animals = CategoryId.Parse("category.animals");
        private static readonly DateTimeOffset Started =
            new DateTimeOffset(2026, 8, 16, 2, 30, 0, TimeSpan.Zero);

        [Test]
        public void FirstRepeatAndDuplicateGrantHaveExplicitNonDuplicatingOutcomes()
        {
            DiscoveryDefinition approved = Definition("discovery.jungle.toucan", approved: true);
            var repository = new MemoryRepository(PlayerProgress.CreateDefault());
            var useCase = UseCase(new ContentCatalog(new[] { approved }, Array.Empty<DiscoveryIdAlias>()), repository);

            DiscoverResult first = useCase.Execute(approved.Id, Grant("grant.interaction.first"));
            DiscoverResult repeated = useCase.Execute(approved.Id, Grant("grant.interaction.second"));
            DiscoverResult duplicate = useCase.Execute(approved.Id, Grant("grant.interaction.second"));

            Assert.That(first.Outcome, Is.EqualTo(DiscoverOutcome.First));
            Assert.That(first.GrantsUniqueReward, Is.True);
            Assert.That(first.Count, Is.EqualTo(1));
            Assert.That(first.Progress.FirstObservedLocalDate, Is.EqualTo("2026-08-15"));
            Assert.That(repeated.Outcome, Is.EqualTo(DiscoverOutcome.Repeated));
            Assert.That(repeated.GrantsUniqueReward, Is.False);
            Assert.That(repeated.Count, Is.EqualTo(2));
            Assert.That(duplicate.Outcome, Is.EqualTo(DiscoverOutcome.AlreadyProcessed));
            Assert.That(duplicate.GrantsUniqueReward, Is.False);
            Assert.That(duplicate.Count, Is.EqualTo(2));
            Assert.That(repository.CommitCount, Is.EqualTo(2));
            Assert.That(repository.Current.ProcessedDiscoveryGrantIds.Count, Is.EqualTo(2));
        }

        [Test]
        public void MissingAndUnapprovedContentDoNotMutateProgress()
        {
            DiscoveryDefinition draft = Definition("discovery.jungle.draft", approved: false);
            var repository = new MemoryRepository(PlayerProgress.CreateDefault());
            var useCase = UseCase(
                new ContentCatalog(new[] { draft }, Array.Empty<DiscoveryIdAlias>()),
                repository,
                allowUnapproved: false);

            DiscoverResult missing = useCase.Execute(
                DiscoveryId.Parse("discovery.jungle.missing"),
                Grant("grant.test.missing"));
            DiscoverResult unapproved = useCase.Execute(draft.Id, Grant("grant.test.draft"));

            Assert.That(missing.Outcome, Is.EqualTo(DiscoverOutcome.MissingContent));
            Assert.That(unapproved.Outcome, Is.EqualTo(DiscoverOutcome.UnapprovedContent));
            Assert.That(repository.CommitCount, Is.Zero);
            Assert.That(repository.Current.Discoveries, Is.Empty);
        }

        [Test]
        public void ApprovedCatalogDefinesWorldAndCategoryDenominatorsAndIgnoresRemovedProgress()
        {
            DiscoveryDefinition first = Definition("discovery.jungle.toucan", approved: true);
            DiscoveryDefinition second = Definition("discovery.jungle.jaguar", approved: true);
            DiscoveryDefinition draft = Definition("discovery.jungle.draft", approved: false);
            var stored = new[]
            {
                new DiscoveryProgress(first.Id, 1, "2026-08-15"),
                new DiscoveryProgress(DiscoveryId.Parse("discovery.jungle.retired"), 4, "2026-08-01")
            };
            var repository = new MemoryRepository(new PlayerProgress(
                0,
                Array.Empty<string>(),
                stored,
                Array.Empty<string>(),
                Array.Empty<string>(),
                PlayerPreferences.CreateDefault()));
            var catalog = new ContentCatalog(
                new[] { first, second, draft },
                Array.Empty<DiscoveryIdAlias>());
            var queries = new DiscoveryProgressQueries(catalog, repository);

            DiscoveryProgressSummary world = queries.ForWorld(Jungle);
            DiscoveryProgressSummary category = queries.ForCategory(Animals);

            Assert.That(world.Discovered, Is.EqualTo(1));
            Assert.That(world.Total, Is.EqualTo(2));
            Assert.That(world.Ratio, Is.EqualTo(.5f));
            Assert.That(category.Discovered, Is.EqualTo(1));
            Assert.That(category.Total, Is.EqualTo(2));
            Assert.That(repository.Current.Discoveries.Count, Is.EqualTo(2),
                "Retired records remain preserved for backward compatibility.");
        }

        private static DiscoverUseCase UseCase(
            IContentCatalog catalog,
            MemoryRepository repository,
            bool allowUnapproved = true) =>
            new DiscoverUseCase(
                catalog,
                repository,
                new ManualClock(Started),
                allowUnapproved,
                TimeSpan.FromHours(-6));

        private static DiscoveryDefinition Definition(string id, bool approved) =>
            new DiscoveryDefinition(
                DiscoveryId.Parse(id),
                Jungle,
                Animals,
                Array.Empty<TagId>(),
                Array.Empty<EducationalFactId>(),
                LocalizationKeys.DiscoveryPlaceholderName,
                AudioCueIds.ConfirmFeedback,
                VisualAssetId.Parse("visual.discovery.test"),
                approved
                    ? new EditorialMetadata(EditorialState.Approved, false, "Tests", string.Empty)
                    : new EditorialMetadata(EditorialState.Draft, true, "Tests", "PH_"));

        private static DiscoveryGrantId Grant(string value) => DiscoveryGrantId.Parse(value);

        private sealed class MemoryRepository : IDiscoveryProgressRepository
        {
            public MemoryRepository(PlayerProgress current) => Current = current;
            public bool IsReadOnly { get; set; }
            public PlayerProgress Current { get; private set; }
            public int CommitCount { get; private set; }
            public void Commit(PlayerProgress progress)
            {
                Current = progress;
                CommitCount++;
            }
        }
    }
}
