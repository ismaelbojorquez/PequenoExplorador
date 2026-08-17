using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PequenoExplorador.Application.Customization;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Content.Customization;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Editor;
using PequenoExplorador.Infrastructure.Save;
using UnityEditor;
using UnityEngine;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class CustomizationTests
    {
        private static readonly CustomizationSlotId Hair = CustomizationSlotId.Parse("customization-slot.hair");
        private static readonly CustomizationSlotId Hat = CustomizationSlotId.Parse("customization-slot.hat");
        private static readonly CosmeticId Curls = CosmeticId.Parse("cosmetic.hair.curls");
        private static readonly CosmeticId Puffs = CosmeticId.Parse("cosmetic.hair.puffs");
        private static readonly CosmeticId NoHat = CosmeticId.Parse("cosmetic.hat.none");
        private static readonly CosmeticId SunHat = CosmeticId.Parse("cosmetic.hat.sun");
        private static readonly CosmeticCompatibilityTagId WideHair = CosmeticCompatibilityTagId.Parse("cosmetic-tag.hair.volume-wide");

        [Test]
        public void CatalogRejectsDuplicateIdsAndRequiresFreeFallbacks()
        {
            Assert.That(Catalog().Slots.Count, Is.EqualTo(2));
            Assert.Throws<ArgumentException>(() => new CustomizationCatalog(new[] { Slot(Hair, Curls, 0), Slot(Hair, Curls, 1) }, Cosmetics()));
            Assert.Throws<ArgumentException>(() => new CustomizationCatalog(new[] { Slot(Hair, Puffs, 0), Slot(Hat, NoHat, 1) }, new[]
            {
                Cosmetic("hair.curls", Hair, true), Cosmetic("hair.puffs", Hair, false, 1), Cosmetic("hat.none", Hat, true)
            }));
        }

        [Test]
        public void DefaultsResolveAndRemovedSavedOptionFallsBackSafely()
        {
            CustomizationCatalog catalog = Catalog();
            PlayerProgress progress = PlayerProgress.CreateDefault().WithCustomizationState(Array.Empty<string>(), new[]
            { new EquippedCosmetic(Hair, CosmeticId.Parse("cosmetic.hair.removed")) });
            IReadOnlyList<CosmeticDefinition> resolved = new CustomizationSelectionResolver(catalog).Resolve(progress);
            Assert.That(resolved.Single(value => value.SlotId == Hair).Id, Is.EqualTo(Curls));
            Assert.That(resolved.Single(value => value.SlotId == Hat).Id, Is.EqualTo(NoHat));
        }

        [Test]
        public void StarUnlockIsAtomicIdempotentAndEquipRemainsSeparate()
        {
            var repository = new MemoryRepository { Current = PlayerProgress.CreateDefault().WithStars(2) };
            var unlock = new UnlockCosmeticUseCase(Catalog(), repository);
            var equip = new EquipCosmeticUseCase(Catalog(), repository);
            Assert.That(unlock.Execute(SunHat).Outcome, Is.EqualTo(UnlockCosmeticOutcome.Unlocked));
            Assert.That(repository.Current.Stars, Is.Zero);
            Assert.That(repository.Current.UnlockedCosmeticIds, Is.EqualTo(new[] { SunHat.Value }));
            Assert.That(repository.Current.EquippedCosmetics, Is.Empty, "unlock must not silently equip");
            Assert.That(unlock.Execute(SunHat).Outcome, Is.EqualTo(UnlockCosmeticOutcome.AlreadyAvailable));
            Assert.That(equip.Execute(SunHat).Outcome, Is.EqualTo(EquipCosmeticOutcome.Equipped));
            Assert.That(repository.Current.ProcessedEconomyTransactionIds.Count, Is.EqualTo(1));
            Assert.That(repository.Current.EconomyLedger.Count, Is.EqualTo(1));
        }

        [Test]
        public void FailedAtomicCommitLeavesStarsAndOwnershipUntouchedAndRetryWorks()
        {
            var repository = new MemoryRepository { Current = PlayerProgress.CreateDefault().WithStars(2), FailNextCommit = true };
            var useCase = new UnlockCosmeticUseCase(Catalog(), repository);
            Assert.Throws<InvalidOperationException>(() => useCase.Execute(SunHat));
            Assert.That(repository.Current.Stars, Is.EqualTo(2));
            Assert.That(repository.Current.UnlockedCosmeticIds, Is.Empty);
            Assert.That(useCase.Execute(SunHat).Outcome, Is.EqualTo(UnlockCosmeticOutcome.Unlocked));
            Assert.That(repository.Current.Stars, Is.Zero);
        }

        [Test]
        public void IncompatibleCombinationIsRejectedWithoutChangingEquipment()
        {
            CustomizationCatalog catalog = Catalog();
            PlayerProgress progress = PlayerProgress.CreateDefault().WithStars(2)
                .WithCustomizationState(new[] { SunHat.Value }, new[] { new EquippedCosmetic(Hair, Puffs) });
            var repository = new MemoryRepository { Current = progress };
            EquipCosmeticResult result = new EquipCosmeticUseCase(catalog, repository).Execute(SunHat);
            Assert.That(result.Outcome, Is.EqualTo(EquipCosmeticOutcome.Incompatible));
            Assert.That(repository.Current.EquippedCosmetics.Count, Is.EqualTo(1));
            Assert.That(repository.Current.EquippedCosmetics[0].CosmeticId, Is.EqualTo(Puffs));
        }

        [Test]
        public void ProgressPrerequisiteAndReadOnlyStateDoNotSpendOrUnlock()
        {
            CosmeticDefinition binoculars = Cosmetic("tool.binoculars", CustomizationSlotId.Parse("customization-slot.tool"), false, 0,
                required: CampUpgradeId.Parse("camp-upgrade.observation-corner"));
            CosmeticDefinition camera = Cosmetic("tool.camera", binoculars.SlotId, true);
            var catalog = new CustomizationCatalog(new[] { Slot(binoculars.SlotId, camera.Id, 0) }, new[] { camera, binoculars });
            var repository = new MemoryRepository { Current = PlayerProgress.CreateDefault().WithStars(9) };
            var useCase = new UnlockCosmeticUseCase(catalog, repository);
            Assert.That(useCase.Execute(binoculars.Id).Outcome, Is.EqualTo(UnlockCosmeticOutcome.PrerequisiteLocked));
            repository.IsReadOnly = true;
            Assert.That(useCase.Execute(binoculars.Id).Outcome, Is.EqualTo(UnlockCosmeticOutcome.ReadOnly));
            Assert.That(repository.Current.Stars, Is.EqualTo(9));
        }

        [Test]
        public void V10MigrationPreservesProgressAndInitializesCustomizationEmpty()
        {
            PlayerProgressV10Dto source = PlayerProgressV10Dto.Create("0.1", 7, new[] { "world.jungle" },
                Array.Empty<DiscoveryProgressV4Dto>(), Array.Empty<string>(), Array.Empty<PhotoProgressV6Dto>(),
                Array.Empty<string>(), PlayerPreferencesV3Dto.Create(0, "es", 1, 1, 1, 1, 1, true),
                Array.Empty<string>(), Array.Empty<EconomyLedgerEntryV7Dto>(), Array.Empty<MissionProgressV8Dto>(),
                Array.Empty<string>(), 0, Array.Empty<LearningSessionV9Dto>(), Array.Empty<LearningConceptDailyV9Dto>(),
                new[] { "camp-upgrade.observation-corner" }, SaveMetadataV1Dto.Create(3));
            string migrated = new V10ToV11CustomizationMigration().Migrate(JsonUtility.ToJson(source));
            PlayerProgressV11Dto result = JsonUtility.FromJson<PlayerProgressV11Dto>(migrated);
            Assert.That(result.Stars, Is.EqualTo(7));
            Assert.That(result.UnlockedCampUpgradeIds, Is.EqualTo(new[] { "camp-upgrade.observation-corner" }));
            Assert.That(result.UnlockedCosmeticIds, Is.Empty);
            Assert.That(result.EquippedCosmetics, Is.Empty);
        }

        [Test]
        public void ReleaseRejectsPlaceholderCosmeticsAndDevelopmentValidatorPasses()
        {
            CustomizationCatalogAsset asset = AssetDatabase.LoadAssetAtPath<CustomizationCatalogAsset>(CustomizationFoundationSetup.CatalogPath);
            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.TryBuild(ContentValidationMode.Development, out CustomizationCatalog catalog, out _), Is.True);
            Assert.That(catalog.Cosmetics.Count, Is.EqualTo(20));
            Assert.That(asset.TryBuild(ContentValidationMode.Release, out _, out IReadOnlyList<string> violations), Is.False);
            Assert.That(violations, Has.Some.StartsWith("CUSTOM005"));
        }

        private static CustomizationCatalog Catalog() => new CustomizationCatalog(new[] { Slot(Hair, Curls, 0), Slot(Hat, NoHat, 1) }, Cosmetics());
        private static IEnumerable<CosmeticDefinition> Cosmetics() => new[]
        {
            Cosmetic("hair.curls", Hair, true),
            Cosmetic("hair.puffs", Hair, true, tags: new[] { WideHair }),
            Cosmetic("hat.none", Hat, true),
            Cosmetic("hat.sun", Hat, false, 2, blocked: new[] { WideHair })
        };
        private static CustomizationSlotDefinition Slot(CustomizationSlotId slot, CosmeticId fallback, int order) =>
            new CustomizationSlotDefinition(slot, Key("slot." + slot.Value), order, fallback);
        private static CosmeticDefinition Cosmetic(string slug, CustomizationSlotId slot, bool initial, int cost = 0,
            IEnumerable<CosmeticCompatibilityTagId> tags = null, IEnumerable<CosmeticCompatibilityTagId> blocked = null,
            CampUpgradeId required = default) => new CosmeticDefinition(CosmeticId.Parse("cosmetic." + slug), slot,
            Key("cosmetic." + slug), VisualAssetId.Parse("visual.cosmetic." + slug), new CustomizationColor(120, 90, 60, 255), initial,
            new ExplorerStars(cost), cost > 0 ? RewardId.Parse("reward.cosmetic." + slug) : default, required,
            tags ?? Array.Empty<CosmeticCompatibilityTagId>(), blocked ?? Array.Empty<CosmeticCompatibilityTagId>(), true);
        private static LocalizedKey Key(string value) => new LocalizedKey("UI", "test.customization." + value);

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
