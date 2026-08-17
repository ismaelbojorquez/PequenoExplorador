using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Missions;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Infrastructure.Save;
using UnityEngine;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class MissionTests
    {
        private static readonly MissionId PhotoMission = MissionId.Parse("mission.vertical-slice.photograph-toucan");
        private static readonly DiscoveryId Toucan = DiscoveryId.Parse("discovery.jungle.keel-billed-toucan");
        private static readonly TagId Jungle = TagId.Parse("tag.world.jungle");
        private static readonly RewardId MissionReward = RewardId.Parse("reward.mission.photograph-toucan.complete");

        [Test]
        public void ThreeStrategiesEvaluateTypedFactsWithoutCentralSwitch()
        {
            GameplayFact discovery = Fact("gameplay-fact.test.discovery", GameplayFactTypes.Discovery, Toucan.Value, Jungle);
            GameplayFact photo = Fact("gameplay-fact.test.photo", GameplayFactTypes.Photograph, Toucan.Value);
            GameplayFact interaction = Fact("gameplay-fact.test.interaction", GameplayFactTypes.Interaction, "interaction.jungle.toucan", Jungle);
            Assert.That(new DiscoverCountObjectiveStrategy().Evaluate(Objective(MissionObjectiveTypeIds.DiscoverCount, string.Empty, Jungle, 2), 0, discovery).Count, Is.EqualTo(1));
            Assert.That(new PhotographSpecificObjectiveStrategy().Evaluate(Objective(MissionObjectiveTypeIds.PhotographSpecific, Toucan.Value, default, 1), 0, photo).Count, Is.EqualTo(1));
            Assert.That(new InteractTagObjectiveStrategy().Evaluate(Objective(MissionObjectiveTypeIds.InteractTag, string.Empty, Jungle, 1), 0, interaction).Count, Is.EqualTo(1));
        }

        [Test]
        public void PreActivationFactDoesNotCountAndCompletionAutoRewardsExactlyOnce()
        {
            var repository = new MemoryRepository();
            MissionCoordinator coordinator = Coordinator(repository, new[] { Definition(PhotoMission, MissionObjectiveTypeIds.PhotographSpecific, Toucan.Value) });
            GameplayFact before = Fact("gameplay-fact.photo.before", GameplayFactTypes.Photograph, Toucan.Value);
            Assert.That(coordinator.Record(before).Outcome, Is.EqualTo(MissionFactOutcome.Ignored));
            Assert.That(coordinator.Activate(PhotoMission).Outcome, Is.EqualTo(MissionActivationOutcome.Activated));
            Assert.That(repository.Current.Missions.Single().Objectives.Single().Count, Is.Zero);
            GameplayFact after = Fact("gameplay-fact.photo.after", GameplayFactTypes.Photograph, Toucan.Value);
            MissionFactResult completed = coordinator.Record(after);
            MissionFactResult duplicate = coordinator.Record(after);
            Assert.That(completed.Outcome, Is.EqualTo(MissionFactOutcome.Completed));
            Assert.That(completed.Reward.Outcome, Is.EqualTo(GrantRewardOutcome.Granted));
            Assert.That(duplicate.Outcome, Is.EqualTo(MissionFactOutcome.Duplicate));
            Assert.That(repository.Current.CompletedMissionIds, Does.Contain(PhotoMission.Value));
            Assert.That(repository.Current.Stars, Is.EqualTo(2));
            Assert.That(repository.Current.ProcessedEconomyTransactionIds.Count, Is.EqualTo(1));
        }

        [Test]
        public void PrerequisitesMissingAndCyclesAreRejected()
        {
            MissionId first = MissionId.Parse("mission.test.first");
            MissionId second = MissionId.Parse("mission.test.second");
            MissionDefinition firstDefinition = Definition(first, MissionObjectiveTypeIds.DiscoverCount, string.Empty, new[] { second });
            MissionDefinition secondDefinition = Definition(second, MissionObjectiveTypeIds.InteractTag, string.Empty, new[] { first });
            Assert.Throws<ArgumentException>(() => new MissionCatalog(new[] { firstDefinition, secondDefinition }));
            var repository = new MemoryRepository();
            MissionDefinition locked = Definition(second, MissionObjectiveTypeIds.PhotographSpecific, Toucan.Value, new[] { first });
            MissionCoordinator coordinator = Coordinator(repository, new[] { Definition(first), locked });
            Assert.That(coordinator.Activate(second).Outcome, Is.EqualTo(MissionActivationOutcome.PrerequisitesMissing));
        }

        [Test]
        public void OneFactCompletesMultipleActiveMissionsAndGrantsEachRewardOnce()
        {
            MissionId first = MissionId.Parse("mission.test.photo-first");
            MissionId second = MissionId.Parse("mission.test.photo-second");
            var repository = new MemoryRepository();
            MissionCoordinator coordinator = Coordinator(repository, new[]
            {
                Definition(first, MissionObjectiveTypeIds.PhotographSpecific, Toucan.Value),
                Definition(second, MissionObjectiveTypeIds.PhotographSpecific, Toucan.Value)
            });
            Assert.That(coordinator.Activate(first).Outcome, Is.EqualTo(MissionActivationOutcome.Activated));
            Assert.That(coordinator.Activate(second).Outcome, Is.EqualTo(MissionActivationOutcome.Activated));

            GameplayFact fact = Fact("gameplay-fact.photo.shared", GameplayFactTypes.Photograph, Toucan.Value);
            Assert.That(coordinator.Record(fact).Outcome, Is.EqualTo(MissionFactOutcome.Completed));
            Assert.That(coordinator.Record(fact).Outcome, Is.EqualTo(MissionFactOutcome.Duplicate));
            Assert.That(repository.Current.Missions.All(item => item.IsCompleted), Is.True);
            Assert.That(repository.Current.Stars, Is.EqualTo(4));
            Assert.That(repository.Current.ProcessedEconomyTransactionIds.Count, Is.EqualTo(2));
        }

        [Test]
        public void StartupReconciliationGrantsRewardMissingAfterCompletionExactlyOnce()
        {
            MissionDefinition definition = Definition(PhotoMission, MissionObjectiveTypeIds.PhotographSpecific, Toucan.Value);
            MissionObjectiveDefinition objective = definition.Objectives.Single();
            var repository = new MemoryRepository
            {
                Current = PlayerProgress.CreateDefault().WithMissionState(
                    new[] { new MissionProgress(PhotoMission, MissionProgressStatus.Completed, 0,
                        new[] { new MissionObjectiveProgress(objective.Id, objective.TargetCount) }) },
                    new[] { PhotoMission.Value }, Array.Empty<string>(), 0)
            };
            MissionCoordinator coordinator = Coordinator(repository, new[] { definition });

            Assert.That(coordinator.ReconcileCompletedRewards(), Is.EqualTo(1));
            Assert.That(coordinator.ReconcileCompletedRewards(), Is.Zero);
            Assert.That(repository.Current.Stars, Is.EqualTo(2));
            Assert.That(repository.Current.ProcessedEconomyTransactionIds.Count, Is.EqualTo(1));
        }

        [Test]
        public void RemovedMissionStateSurvivesAndFactIsSafelyIgnored()
        {
            MissionId removed = MissionId.Parse("mission.removed.fixture");
            MissionObjectiveId objective = MissionObjectiveId.Parse("mission-objective.removed.fixture");
            var repository = new MemoryRepository
            {
                Current = PlayerProgress.CreateDefault().WithMissionState(
                    new[] { new MissionProgress(removed, MissionProgressStatus.Active, 0, new[] { new MissionObjectiveProgress(objective, 0) }) },
                    Array.Empty<string>(), Array.Empty<string>(), 0)
            };
            MissionCoordinator coordinator = Coordinator(repository, new[] { Definition(PhotoMission) });
            Assert.That(coordinator.Record(Fact("gameplay-fact.removed.safe", GameplayFactTypes.Photograph, Toucan.Value)).Outcome,
                Is.EqualTo(MissionFactOutcome.Ignored));
            Assert.That(repository.Current.Missions.Single().Id, Is.EqualTo(removed));
        }

        [Test]
        public void V7MigrationPreservesLegacyCompletionAndStartsMissionRuntimeEmpty()
        {
            PlayerProgressV7Dto source = PlayerProgressV7Dto.Create("0.1", 3, Array.Empty<string>(),
                Array.Empty<DiscoveryProgressV4Dto>(), Array.Empty<string>(), Array.Empty<PhotoProgressV6Dto>(),
                new[] { "mission.legacy.done" }, PlayerPreferencesV3Dto.Create(0, "es", 1, 1, 1, 1, 1, true),
                Array.Empty<string>(), Array.Empty<EconomyLedgerEntryV7Dto>(), SaveMetadataV1Dto.Create(4));
            string migrated = new V7ToV8MissionMigration().Migrate(JsonUtility.ToJson(source));
            PlayerProgressV8Dto dto = JsonUtility.FromJson<PlayerProgressV8Dto>(migrated);
            Assert.That(dto.CompletedMissionIds, Is.EqualTo(new[] { "mission.legacy.done" }));
            Assert.That(dto.Missions, Is.Empty);
            Assert.That(dto.ProcessedMissionFactIds, Is.Empty);
            Assert.That(dto.LastMissionFactSequence, Is.Zero);
        }

        private static MissionCoordinator Coordinator(MemoryRepository repository, IEnumerable<MissionDefinition> definitions)
        {
            MissionDefinition[] missions = definitions.ToArray();
            RewardDefinition[] rewards = missions.Select(item => new RewardDefinition(item.RewardId, new ExplorerStars(2), RewardSourceKind.Mission, item.Id.Value)).ToArray();
            var catalog = new RewardCatalog(rewards);
            return new MissionCoordinator(new MissionCatalog(missions), Registry(), repository,
                new GrantRewardUseCase(catalog, repository));
        }
        private static MissionObjectiveStrategyRegistry Registry() => new MissionObjectiveStrategyRegistry(new IMissionObjectiveStrategy[]
            { new DiscoverCountObjectiveStrategy(), new PhotographSpecificObjectiveStrategy(), new InteractTagObjectiveStrategy() });
        private static MissionDefinition Definition(MissionId id, MissionObjectiveTypeId? type = null, string subject = null,
            IEnumerable<MissionId> prerequisites = null)
        {
            MissionObjectiveTypeId resolved = type ?? MissionObjectiveTypeIds.PhotographSpecific;
            TagId tag = resolved.Equals(MissionObjectiveTypeIds.InteractTag) || resolved.Equals(MissionObjectiveTypeIds.DiscoverCount) ? Jungle : default;
            return new MissionDefinition(id, LocalizationKeys.MissionPhotographToucanTitle,
                LocalizationKeys.MissionPhotographToucanSummary, LocalizationKeys.MissionPhotographToucanCompletion,
                new[] { Objective(resolved, subject ?? Toucan.Value, tag, 1) }, prerequisites ?? Array.Empty<MissionId>(),
                RewardId.Parse("reward.mission." + id.Value.Substring("mission.".Length)),
                new EditorialMetadata(EditorialState.Approved, false, "Tests", string.Empty));
        }
        private static MissionObjectiveDefinition Objective(MissionObjectiveTypeId type, string subject, TagId tag, int count) =>
            new MissionObjectiveDefinition(MissionObjectiveId.Parse("mission-objective.test." + type.Value.Substring("mission-objective-type.".Length)),
                type, LocalizationKeys.MissionPhotographToucanObjective, count, subject, tag);
        private static GameplayFact Fact(string id, GameplayFactTypeId type, string subject, params TagId[] tags) =>
            new GameplayFact(GameplayFactId.Parse(id), type, subject, tags, GameplayFactScope.Persistent);

        private sealed class MemoryRepository : IMissionRepository, IEconomyRepository
        {
            public bool IsReadOnly { get; set; }
            public PlayerProgress Current { get; set; } = PlayerProgress.CreateDefault();
            public event Action<PlayerProgress> Changed;
            public void Commit(PlayerProgress progress) { Current = progress; Changed?.Invoke(progress); }
        }
    }
}
