using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Economy
{
    public sealed class GrantRewardUseCase
    {
        public const int LedgerCapacity = PlayerProgress.EconomyLedgerMaximumEntries;
        private readonly IRewardCatalog _catalog;
        private readonly IEconomyRepository _repository;
        public GrantRewardUseCase(IRewardCatalog catalog, IEconomyRepository repository)
        { _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog)); _repository = repository ?? throw new ArgumentNullException(nameof(repository)); }

        public GrantRewardResult Execute(RewardId rewardId, EconomyTransactionId transactionId,
            RewardSourceKind sourceKind, string sourceId)
        {
            if (!transactionId.IsValid) throw new ArgumentException("A stable economy transaction ID is required.", nameof(transactionId));
            PlayerProgress current = _repository.Current;
            if (_repository.IsReadOnly) return new GrantRewardResult(GrantRewardOutcome.ReadOnly, current.Wallet, default);
            if (!_catalog.TryGet(rewardId, out RewardDefinition definition))
                return new GrantRewardResult(GrantRewardOutcome.MissingDefinition, current.Wallet, default);
            if (definition.SourceKind != sourceKind || !string.Equals(definition.SourceId, sourceId, StringComparison.Ordinal))
                return new GrantRewardResult(GrantRewardOutcome.SourceMismatch, current.Wallet, definition.Amount);
            if (current.ProcessedEconomyTransactionIds.Contains(transactionId.Value, StringComparer.Ordinal))
                return new GrantRewardResult(GrantRewardOutcome.AlreadyProcessed, current.Wallet, definition.Amount);
            if (!current.Wallet.TryAdd(definition.Amount, out ExplorerStars balance))
                return new GrantRewardResult(GrantRewardOutcome.Overflow, current.Wallet, definition.Amount);

            var ids = new List<string>(current.ProcessedEconomyTransactionIds) { transactionId.Value };
            EconomyLedgerEntry[] ledger = Append(current.EconomyLedger,
                new EconomyLedgerEntry(transactionId, EconomyTransactionKind.Grant, rewardId, definition.Amount, balance));
            _repository.Commit(current.WithEconomy(balance, ids, ledger));
            return new GrantRewardResult(GrantRewardOutcome.Granted, balance, definition.Amount);
        }

        internal static EconomyLedgerEntry[] Append(IEnumerable<EconomyLedgerEntry> existing, EconomyLedgerEntry entry) =>
            existing.Concat(new[] { entry }).TakeLast(LedgerCapacity).ToArray();
    }
}
