using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Learning;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Missions;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Editor.BuildTools;
using PequenoExplorador.Infrastructure.Save;
using UnityEngine;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class LearningEngineTests
    {
        private static readonly ActivityId Activity = ActivityId.Parse("activity.fixture.visual-matching");
        private static readonly LearningConceptId Concept = LearningConceptId.Parse("concept.observation.visual-matching");
        private static readonly LearningOptionId Correct = LearningOptionId.Parse("activity-option.fixture.circle");
        private static readonly LearningOptionId Wrong = LearningOptionId.Parse("activity-option.fixture.triangle");

        [Test]
        public void IncorrectAttemptsProduceTryAgainThenGraduatedHintWithoutPunishment()
        {
            var repository = new MemoryRepository(); LearningCoordinator coordinator = Coordinator(repository);
            Assert.That(coordinator.Start(Activity).Outcome, Is.EqualTo(ActivityOutcome.Started));
            LearningActivityResult first = coordinator.Submit(Activity, Wrong);
            LearningActivityResult second = coordinator.Submit(Activity, Wrong);
            Assert.That(first.Outcome, Is.EqualTo(ActivityOutcome.TryAgain));
            Assert.That(second.Outcome, Is.EqualTo(ActivityOutcome.Hint));
            Assert.That(second.Session.Attempts, Is.EqualTo(2));
            Assert.That(second.Session.HintLevel, Is.EqualTo(1));
            Assert.That(repository.Current.Stars, Is.Zero);
        }

        [Test]
        public void ExplicitHintsCapAndCorrectCompletionRewardsAndEmitsFactOnce()
        {
            var repository = new MemoryRepository(); var facts = new FactSink(); LearningCoordinator coordinator = Coordinator(repository, facts);
            coordinator.Start(Activity);
            Assert.That(coordinator.RequestHint(Activity).Session.HintLevel, Is.EqualTo(1));
            Assert.That(coordinator.RequestHint(Activity).Session.HintLevel, Is.EqualTo(2));
            Assert.That(coordinator.RequestHint(Activity).Session.HintLevel, Is.EqualTo(3));
            Assert.That(coordinator.RequestHint(Activity).Session.HintLevel, Is.EqualTo(3));
            LearningActivityResult completed = coordinator.Submit(Activity, Correct);
            LearningActivityResult retry = coordinator.Start(Activity);
            Assert.That(completed.Outcome, Is.EqualTo(ActivityOutcome.Completed));
            Assert.That(completed.Reward.Outcome, Is.EqualTo(GrantRewardOutcome.Granted));
            Assert.That(retry.Outcome, Is.EqualTo(ActivityOutcome.AlreadyCompleted));
            Assert.That(retry.Reward.Outcome, Is.EqualTo(GrantRewardOutcome.AlreadyProcessed));
            Assert.That(repository.Current.Stars, Is.EqualTo(1));
            Assert.That(repository.Current.ProcessedEconomyTransactionIds.Count, Is.EqualTo(1));
            Assert.That(facts.Ids.Distinct().Count(), Is.EqualTo(1));
        }

        [Test]
        public void ExitResumesStateAndRestartClearsAttemptsWithoutPenalty()
        {
            var repository = new MemoryRepository(); LearningCoordinator coordinator = Coordinator(repository);
            coordinator.Start(Activity); coordinator.Submit(Activity, Wrong);
            Assert.That(coordinator.Exit(Activity).Outcome, Is.EqualTo(ActivityOutcome.Exited));
            LearningActivityResult resumed = coordinator.Start(Activity);
            Assert.That(resumed.Outcome, Is.EqualTo(ActivityOutcome.Resumed));
            Assert.That(resumed.Session.Attempts, Is.EqualTo(1));
            LearningActivityResult restarted = coordinator.Restart(Activity);
            Assert.That(restarted.Outcome, Is.EqualTo(ActivityOutcome.Restarted));
            Assert.That(restarted.Session.Attempts, Is.Zero);
        }

        [Test]
        public void UnknownOptionAndMissingActivityDoNotMutateProgress()
        {
            var repository = new MemoryRepository(); LearningCoordinator coordinator = Coordinator(repository);
            coordinator.Start(Activity); PlayerProgress before = repository.Current;
            Assert.That(coordinator.Submit(Activity, LearningOptionId.Parse("activity-option.fixture.unknown")).Outcome, Is.EqualTo(ActivityOutcome.InvalidOption));
            Assert.That(repository.Current, Is.SameAs(before));
            Assert.That(coordinator.Start(ActivityId.Parse("activity.missing.fixture")).Outcome, Is.EqualTo(ActivityOutcome.Missing));
        }

        [Test]
        public void DailyConceptStatsAreAggregatedWithoutRawAttemptLog()
        {
            var repository = new MemoryRepository(); LearningCoordinator coordinator = Coordinator(repository);
            coordinator.Start(Activity); coordinator.Submit(Activity, Wrong); coordinator.Submit(Activity, Correct);
            LearningConceptDailyProgress aggregate = repository.Current.LearningConcepts.Single();
            Assert.That(aggregate.ConceptId, Is.EqualTo(Concept)); Assert.That(aggregate.LocalDate, Is.EqualTo("2026-08-17"));
            Assert.That(aggregate.SeenCount, Is.EqualTo(1)); Assert.That(aggregate.CompletedCount, Is.EqualTo(1));
            Assert.That(repository.Current.LearningSessions.Single().Attempts, Is.EqualTo(1));
        }

        [Test]
        public void DraftActivityIsUnavailableWhenDevelopmentOverrideIsDisabled()
        {
            var repository = new MemoryRepository(); LearningCoordinator coordinator = Coordinator(repository, allowUnapproved: false);
            Assert.That(coordinator.Start(Activity).Outcome, Is.EqualTo(ActivityOutcome.Unavailable));
            Assert.That(repository.Current.LearningSessions, Is.Empty);
        }

        [Test]
        public void ReadOnlySaveAndReleaseReconciliationFailClosed()
        {
            var repository = new MemoryRepository();
            repository.Commit(repository.Current.WithLearningState(
                new[] { new LearningSession(Activity, LearningSessionStatus.Active, 1, 1) },
                Array.Empty<LearningConceptDailyProgress>()));
            repository.IsReadOnly = true;
            LearningCoordinator development = Coordinator(repository);

            Assert.That(development.RequestHint(Activity).Outcome, Is.EqualTo(ActivityOutcome.ReadOnly));
            Assert.That(development.Exit(Activity).Outcome, Is.EqualTo(ActivityOutcome.ReadOnly));
            Assert.That(development.Restart(Activity).Outcome, Is.EqualTo(ActivityOutcome.ReadOnly));
            Assert.That(development.ReconcileCompleted(), Is.Zero);

            repository.IsReadOnly = false;
            repository.Commit(repository.Current.WithLearningState(
                new[] { new LearningSession(Activity, LearningSessionStatus.Completed, 1, 1) },
                Array.Empty<LearningConceptDailyProgress>()));
            var facts = new FactSink();
            LearningCoordinator release = Coordinator(repository, facts, allowUnapproved: false);
            Assert.That(release.ReconcileCompleted(), Is.Zero);
            Assert.That(repository.Current.Stars, Is.Zero);
            Assert.That(facts.Ids, Is.Empty);
        }

        [Test]
        public void ReleaseValidatorRejectsTheDraftLearningFixture()
        {
            IReadOnlyList<string> errors = LearningValidationService.Validate(ContentValidationMode.Release);

            Assert.That(errors.Any(error => error.StartsWith("LEARN006", StringComparison.Ordinal)), Is.True,
                "Release must fail closed while the learning fixture remains Draft/placeholder.");
        }

        [Test]
        public void RegistryIsExplicitAndRejectsDuplicateTypeIds()
        {
            var registry = new LearningActivityStrategyRegistry(new ILearningActivityStrategy[] { new SingleChoiceActivityStrategy() });
            Assert.That(registry.TryGet(LearningActivityTypeIds.SingleChoice, out _), Is.True);
            Assert.Throws<ArgumentException>(() => new LearningActivityStrategyRegistry(new ILearningActivityStrategy[] { new SingleChoiceActivityStrategy(), new SingleChoiceActivityStrategy() }));
        }

        [Test]
        public void V8MigrationPreservesExistingStateAndStartsLearningEmpty()
        {
            PlayerProgressV8Dto source = PlayerProgressV8Dto.Create("0.1", 3, Array.Empty<string>(), Array.Empty<DiscoveryProgressV4Dto>(),
                Array.Empty<string>(), Array.Empty<PhotoProgressV6Dto>(), new[] { "mission.legacy.done" },
                PlayerPreferencesV3Dto.Create(0, "es", 1, 1, 1, 1, 1, true), Array.Empty<string>(),
                Array.Empty<EconomyLedgerEntryV7Dto>(), Array.Empty<MissionProgressV8Dto>(), Array.Empty<string>(), 0, SaveMetadataV1Dto.Create(4));
            string migrated = new V8ToV9LearningMigration().Migrate(JsonUtility.ToJson(source));
            PlayerProgressV9Dto dto = JsonUtility.FromJson<PlayerProgressV9Dto>(migrated);
            Assert.That(dto.Stars, Is.EqualTo(3)); Assert.That(dto.CompletedMissionIds, Is.EqualTo(new[] { "mission.legacy.done" }));
            Assert.That(dto.LearningSessions, Is.Empty); Assert.That(dto.LearningConcepts, Is.Empty);
        }

        private static LearningCoordinator Coordinator(MemoryRepository repository, FactSink facts = null, bool allowUnapproved = true)
        {
            LearningActivityDefinition definition = Definition();
            var rewards = new RewardCatalog(new[] { new RewardDefinition(definition.RewardId, new ExplorerStars(1), RewardSourceKind.Activity, Activity.Value) });
            return new LearningCoordinator(new LearningCatalog(new[] { definition }, new[] { ConceptDefinition() }),
                new LearningActivityStrategyRegistry(new ILearningActivityStrategy[] { new SingleChoiceActivityStrategy() }), repository,
                new GrantRewardUseCase(rewards, repository), facts ?? new FactSink(), new ManualClock(), allowUnapproved, TimeSpan.Zero);
        }
        private static LearningConceptDefinition ConceptDefinition() => new LearningConceptDefinition(Concept,
            new LocalizedKey("UI", "ui.learning.concept.visual_matching"), new EditorialMetadata(EditorialState.Draft, true, "Tests", "BORRADOR · PH_"));
        private static LearningActivityDefinition Definition() => new LearningActivityDefinition(Activity, LearningActivityTypeIds.SingleChoice,
            new LocalizedKey("UI", "ui.learning.fixture.title"), new LocalizedKey("UI", "ui.learning.fixture.instruction"),
            new LocalizedKey("UI", "ui.learning.fixture.success"), new LocalizedKey("UI", "ui.learning.fixture.try_again"), new[] { Concept },
            new[] { new LearningOptionDefinition(Correct, new LocalizedKey("UI", "ui.learning.option.circle")), new LearningOptionDefinition(Wrong, new LocalizedKey("UI", "ui.learning.option.triangle")), new LearningOptionDefinition(LearningOptionId.Parse("activity-option.fixture.square"), new LocalizedKey("UI", "ui.learning.option.square")) },
            Correct, new[] { new LocalizedKey("UI", "ui.learning.fixture.hint.1"), new LocalizedKey("UI", "ui.learning.fixture.hint.2"), new LocalizedKey("UI", "ui.learning.fixture.hint.3") },
            new HintPolicy(2, 3), true, RewardId.Parse("reward.activity.visual-matching.complete"), new EditorialMetadata(EditorialState.Draft, true, "Tests", "BORRADOR · PH_"));

        private sealed class MemoryRepository : ILearningRepository, IEconomyRepository
        { public bool IsReadOnly { get; set; } public PlayerProgress Current { get; private set; } = PlayerProgress.CreateDefault(); public event Action<PlayerProgress> Changed; public void Commit(PlayerProgress progress) { Current = progress; Changed?.Invoke(progress); } }
        private sealed class FactSink : IMissionFactSink
        { public readonly List<string> Ids = new List<string>(); public MissionFactResult Record(GameplayFact fact) { Ids.Add(fact.Id.Value); return default; } }
        private sealed class ManualClock : IClock { public DateTimeOffset UtcNow => new DateTimeOffset(2026, 8, 17, 12, 0, 0, TimeSpan.Zero); }
    }
}
