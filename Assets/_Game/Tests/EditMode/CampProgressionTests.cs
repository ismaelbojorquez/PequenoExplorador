using System;
using System.Collections.Generic;
using NUnit.Framework;
using PequenoExplorador.Application.Camp;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Content.Camp;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Infrastructure.Save;
using UnityEditor;
using UnityEngine;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class CampProgressionTests
    {
        private static readonly CampStationId Station = CampStationId.Parse("camp-station.observation");
        private static readonly CampUpgradeId Upgrade = CampUpgradeId.Parse("camp-upgrade.observation-corner");

        [Test]
        public void CatalogIsDataDrivenAndRejectsDuplicatesMissingReferencesAndCycles()
        {
            CampCatalog valid = new CampCatalog(new[] { StationDefinition() }, new[] { UpgradeDefinition(Upgrade) });
            Assert.That(valid.TryGetUpgrade(Upgrade, out CampUpgradeDefinition resolved), Is.True);
            Assert.That(resolved.StarCost.Value, Is.EqualTo(3));

            Assert.Throws<ArgumentException>(() => new CampCatalog(
                new[] { StationDefinition(), StationDefinition() }, Array.Empty<CampUpgradeDefinition>()));
            Assert.Throws<ArgumentException>(() => new CampCatalog(
                new[] { StationDefinition() }, new[] { UpgradeDefinition(Upgrade, station: CampStationId.Parse("camp-station.missing")) }));

            CampUpgradeId second = CampUpgradeId.Parse("camp-upgrade.second");
            Assert.Throws<ArgumentException>(() => new CampCatalog(new[] { StationDefinition() }, new[]
            {
                UpgradeDefinition(Upgrade, prerequisites: new[] { second }),
                UpgradeDefinition(second, prerequisites: new[] { Upgrade })
            }));
        }

        [Test]
        public void PurchaseIsAtomicIdempotentAndFriendlyWhenStarsAreInsufficient()
        {
            var repository = new MemoryRepository { Current = PlayerProgress.CreateDefault().WithStars(2) };
            var useCase = new PurchaseCampUpgradeUseCase(Catalog(), repository);
            PurchaseCampUpgradeResult insufficient = useCase.Execute(Upgrade);
            Assert.That(insufficient.Outcome, Is.EqualTo(PurchaseCampUpgradeOutcome.InsufficientStars));
            Assert.That(repository.Current.Stars, Is.EqualTo(2));
            Assert.That(repository.Current.UnlockedCampUpgradeIds, Is.Empty);

            repository.Current = repository.Current.WithStars(3);
            PurchaseCampUpgradeResult purchased = useCase.Execute(Upgrade);
            PurchaseCampUpgradeResult duplicate = useCase.Execute(Upgrade);
            Assert.That(purchased.Outcome, Is.EqualTo(PurchaseCampUpgradeOutcome.Purchased));
            Assert.That(purchased.Balance.Value, Is.Zero);
            Assert.That(duplicate.Outcome, Is.EqualTo(PurchaseCampUpgradeOutcome.AlreadyUnlocked));
            Assert.That(repository.Current.Stars, Is.Zero);
            Assert.That(repository.Current.UnlockedCampUpgradeIds, Is.EqualTo(new[] { Upgrade.Value }));
            Assert.That(repository.Current.ProcessedEconomyTransactionIds.Count, Is.EqualTo(1));
            Assert.That(repository.Current.EconomyLedger.Count, Is.EqualTo(1));
        }

        [Test]
        public void FailedCommitLeavesSpendAndUnlockUntouchedAndCanRetry()
        {
            var repository = new MemoryRepository
            {
                Current = PlayerProgress.CreateDefault().WithStars(3),
                FailNextCommit = true
            };
            var useCase = new PurchaseCampUpgradeUseCase(Catalog(), repository);
            Assert.Throws<InvalidOperationException>(() => useCase.Execute(Upgrade));
            Assert.That(repository.Current.Stars, Is.EqualTo(3));
            Assert.That(repository.Current.UnlockedCampUpgradeIds, Is.Empty);
            Assert.That(repository.Current.ProcessedEconomyTransactionIds, Is.Empty);

            Assert.That(useCase.Execute(Upgrade).Outcome, Is.EqualTo(PurchaseCampUpgradeOutcome.Purchased));
            Assert.That(repository.Current.Stars, Is.Zero);
            Assert.That(repository.Current.UnlockedCampUpgradeIds, Is.EqualTo(new[] { Upgrade.Value }));
        }

        [Test]
        public void PrerequisiteReadOnlyAndMissingDefinitionDoNotMutateProgress()
        {
            CampUpgradeId first = CampUpgradeId.Parse("camp-upgrade.first");
            CampUpgradeId second = CampUpgradeId.Parse("camp-upgrade.second");
            CampCatalog catalog = new CampCatalog(new[] { StationDefinition() }, new[]
            {
                UpgradeDefinition(first), UpgradeDefinition(second, prerequisites: new[] { first })
            });
            var repository = new MemoryRepository { Current = PlayerProgress.CreateDefault().WithStars(20) };
            var useCase = new PurchaseCampUpgradeUseCase(catalog, repository);
            Assert.That(useCase.Execute(second).Outcome, Is.EqualTo(PurchaseCampUpgradeOutcome.PrerequisiteLocked));
            Assert.That(useCase.Execute(CampUpgradeId.Parse("camp-upgrade.missing")).Outcome, Is.EqualTo(PurchaseCampUpgradeOutcome.MissingDefinition));
            repository.IsReadOnly = true;
            Assert.That(useCase.Execute(first).Outcome, Is.EqualTo(PurchaseCampUpgradeOutcome.ReadOnly));
            Assert.That(repository.Current.Stars, Is.EqualTo(20));
            Assert.That(repository.Current.UnlockedCampUpgradeIds, Is.Empty);
        }

        [Test]
        public void V9MigrationPreservesProgressAndInitializesCampUnlocksEmpty()
        {
            PlayerProgressV9Dto source = PlayerProgressV9Dto.Create("0.1", 7, new[] { "world.jungle" },
                Array.Empty<DiscoveryProgressV4Dto>(), Array.Empty<string>(), Array.Empty<PhotoProgressV6Dto>(),
                new[] { "mission.done" }, PlayerPreferencesV3Dto.Create(0, "es", 1, 1, 1, 1, 1, true),
                Array.Empty<string>(), Array.Empty<EconomyLedgerEntryV7Dto>(), Array.Empty<MissionProgressV8Dto>(),
                Array.Empty<string>(), 0, Array.Empty<LearningSessionV9Dto>(), Array.Empty<LearningConceptDailyV9Dto>(),
                SaveMetadataV1Dto.Create(5));
            string migrated = new V9ToV10CampMigration().Migrate(JsonUtility.ToJson(source));
            PlayerProgressV10Dto result = JsonUtility.FromJson<PlayerProgressV10Dto>(migrated);
            Assert.That(result.Stars, Is.EqualTo(7));
            Assert.That(result.WorldIds, Is.EqualTo(new[] { "world.jungle" }));
            Assert.That(result.CompletedMissionIds, Is.EqualTo(new[] { "mission.done" }));
            Assert.That(result.UnlockedCampUpgradeIds, Is.Empty);
        }

        [Test]
        public void ReleaseRejectsPlaceholderCampUpgrade()
        {
            CampCatalogAsset asset = AssetDatabase.LoadAssetAtPath<CampCatalogAsset>(
                "Assets/_Game/Content/Camp/CampCatalog.asset");
            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.TryBuild(ContentValidationMode.Release, out _, out IReadOnlyList<string> violations), Is.False);
            Assert.That(violations, Has.Some.StartsWith("CAMP005"));
        }

        private static CampCatalog Catalog() => new CampCatalog(new[] { StationDefinition() }, new[] { UpgradeDefinition(Upgrade) });

        private static CampStationDefinition StationDefinition() => new CampStationDefinition(Station,
            CampStationActionId.Parse("camp-action.observation"), Key("camp.station.observation"),
            Key("camp.station.observation.description"), 0, true, false);

        private static CampUpgradeDefinition UpgradeDefinition(CampUpgradeId id,
            CampStationId? station = null, IEnumerable<CampUpgradeId> prerequisites = null) =>
            new CampUpgradeDefinition(id, station ?? Station, Key("camp.upgrade.name"), Key("camp.upgrade.description"),
                Key("camp.upgrade.preview"), new ExplorerStars(3), RewardId.Parse("reward.camp.upgrade"),
                VisualAssetId.Parse("visual.camp.before"), VisualAssetId.Parse("visual.camp.after"),
                prerequisites ?? Array.Empty<CampUpgradeId>(), true);

        private static LocalizedKey Key(string entry) => new LocalizedKey("UI", entry);

        private sealed class MemoryRepository : IEconomyRepository
        {
            public bool IsReadOnly { get; set; }
            public bool FailNextCommit { get; set; }
            public PlayerProgress Current { get; set; } = PlayerProgress.CreateDefault();
            public event Action<PlayerProgress> Changed;
            public void Commit(PlayerProgress progress)
            {
                if (FailNextCommit) { FailNextCommit = false; throw new InvalidOperationException("Injected commit failure"); }
                Current = progress; Changed?.Invoke(progress);
            }
        }
    }
}
