using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Infrastructure.Save;
using UnityEngine;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class EconomyTests
    {
        private static readonly RewardId Reward = RewardId.Parse("reward.discovery.keel-billed-toucan.first");
        private const string Discovery = "discovery.jungle.keel-billed-toucan";

        [Test]
        public void ExplorerStarsNeverBecomeNegativeAndDetectOverflow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExplorerStars(-1));
            Assert.That(new ExplorerStars(3).TrySpend(new ExplorerStars(4), out ExplorerStars unchanged), Is.False);
            Assert.That(unchanged.Value, Is.EqualTo(3));
            Assert.That(new ExplorerStars(int.MaxValue).TryAdd(new ExplorerStars(1), out _), Is.False);
        }

        [Test]
        public void GrantIsDataDrivenIdempotentAndRejectsWrongSourceOrOverflow()
        {
            var repository = new MemoryEconomyRepository();
            var useCase = new GrantRewardUseCase(Catalog(), repository);
            EconomyTransactionId transaction = EconomyTransactionId.Parse("economy-tx.discovery.toucan");
            GrantRewardResult first = useCase.Execute(Reward, transaction, RewardSourceKind.Discovery, Discovery);
            GrantRewardResult retry = useCase.Execute(Reward, transaction, RewardSourceKind.Discovery, Discovery);
            GrantRewardResult wrong = useCase.Execute(Reward, EconomyTransactionId.Parse("economy-tx.discovery.wrong"), RewardSourceKind.Activity, Discovery);
            repository.Current = PlayerProgress.CreateDefault().WithStars(int.MaxValue);
            GrantRewardResult overflow = useCase.Execute(Reward, EconomyTransactionId.Parse("economy-tx.discovery.overflow"), RewardSourceKind.Discovery, Discovery);
            Assert.That(first.Outcome, Is.EqualTo(GrantRewardOutcome.Granted));
            Assert.That(first.Balance.Value, Is.EqualTo(1));
            Assert.That(retry.Outcome, Is.EqualTo(GrantRewardOutcome.AlreadyProcessed));
            Assert.That(wrong.Outcome, Is.EqualTo(GrantRewardOutcome.SourceMismatch));
            Assert.That(overflow.Outcome, Is.EqualTo(GrantRewardOutcome.Overflow));
        }

        [Test]
        public void SpendIsAtomicFriendlyAndIdempotent()
        {
            var repository = new MemoryEconomyRepository { Current = PlayerProgress.CreateDefault().WithStars(3) };
            var spend = new SpendStarsUseCase(repository);
            RewardId reason = RewardId.Parse("reward.camp.preview-upgrade");
            SpendStarsResult insufficient = spend.Execute(new ExplorerStars(4), reason, EconomyTransactionId.Parse("economy-tx.spend.insufficient"));
            SpendStarsResult first = spend.Execute(new ExplorerStars(2), reason, EconomyTransactionId.Parse("economy-tx.spend.camp-1"));
            SpendStarsResult retry = spend.Execute(new ExplorerStars(2), reason, EconomyTransactionId.Parse("economy-tx.spend.camp-1"));
            Assert.That(insufficient.Outcome, Is.EqualTo(SpendStarsOutcome.Insufficient));
            Assert.That(first.Outcome, Is.EqualTo(SpendStarsOutcome.Spent));
            Assert.That(retry.Outcome, Is.EqualTo(SpendStarsOutcome.AlreadyProcessed));
            Assert.That(repository.Current.Stars, Is.EqualTo(1));
        }

        [Test]
        public void FailedCommitCanRetryWithoutDuplicateAndLedgerIsBounded()
        {
            var repository = new MemoryEconomyRepository { FailNextCommit = true };
            var grant = new GrantRewardUseCase(Catalog(), repository);
            Assert.Throws<InvalidOperationException>(() => grant.Execute(Reward, EconomyTransactionId.Parse("economy-tx.retry.crash"),
                RewardSourceKind.Discovery, Discovery));
            Assert.That(repository.Current.Stars, Is.Zero);
            Assert.That(grant.Execute(Reward, EconomyTransactionId.Parse("economy-tx.retry.crash"), RewardSourceKind.Discovery, Discovery).Granted, Is.True);
            for (int index = 0; index < 40; index++)
                grant.Execute(Reward, EconomyTransactionId.Parse("economy-tx.batch." + index), RewardSourceKind.Discovery, Discovery);
            Assert.That(repository.Current.Stars, Is.EqualTo(41));
            Assert.That(repository.Current.ProcessedEconomyTransactionIds.Count, Is.EqualTo(41), "Durable idempotency keys do not expire with diagnostic ledger.");
            Assert.That(repository.Current.EconomyLedger.Count, Is.EqualTo(GrantRewardUseCase.LedgerCapacity));
        }

        [Test]
        public void V6MigrationPreservesBalanceAndStartsEconomyMetadataEmpty()
        {
            string source = JsonUtility.ToJson(PlayerProgressV6Dto.Create("0.1", 7, Array.Empty<string>(),
                Array.Empty<DiscoveryProgressV4Dto>(), Array.Empty<string>(), Array.Empty<PhotoProgressV6Dto>(),
                Array.Empty<string>(), PlayerPreferencesV3Dto.Create(0, "es", 1, 1, 1, 1, 1, true), SaveMetadataV1Dto.Create(2)));
            string migrated = new V6ToV7EconomyMigration().Migrate(source);
            PlayerProgressV7Dto dto = JsonUtility.FromJson<PlayerProgressV7Dto>(migrated);
            Assert.That(dto.Stars, Is.EqualTo(7));
            Assert.That(dto.ProcessedEconomyTransactionIds, Is.Empty);
            Assert.That(dto.EconomyLedger, Is.Empty);
        }

        private static RewardCatalog Catalog() => new RewardCatalog(new[]
        {
            new RewardDefinition(Reward, new ExplorerStars(1), RewardSourceKind.Discovery, Discovery)
        });

        private sealed class MemoryEconomyRepository : IEconomyRepository
        {
            public bool IsReadOnly { get; set; }
            public PlayerProgress Current { get; set; } = PlayerProgress.CreateDefault();
            public bool FailNextCommit { get; set; }
            public event Action<PlayerProgress> Changed;
            public void Commit(PlayerProgress progress)
            {
                if (FailNextCommit) { FailNextCommit = false; throw new InvalidOperationException("Injected crash before commit"); }
                Current = progress; Changed?.Invoke(progress);
            }
        }
    }
}
