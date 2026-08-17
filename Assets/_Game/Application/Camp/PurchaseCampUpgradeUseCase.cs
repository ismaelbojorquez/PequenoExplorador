using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Camp
{
    public enum PurchaseCampUpgradeOutcome
    {
        Purchased,
        AlreadyUnlocked,
        MissingDefinition,
        PrerequisiteLocked,
        InsufficientStars,
        ReadOnly
    }

    public readonly struct PurchaseCampUpgradeResult
    {
        public PurchaseCampUpgradeResult(PurchaseCampUpgradeOutcome outcome, CampUpgradeDefinition definition,
            ExplorerStars balance)
        { Outcome = outcome; Definition = definition; Balance = balance; }
        public PurchaseCampUpgradeOutcome Outcome { get; }
        public CampUpgradeDefinition Definition { get; }
        public ExplorerStars Balance { get; }
        public bool Purchased => Outcome == PurchaseCampUpgradeOutcome.Purchased;
    }

    public sealed class PurchaseCampUpgradeUseCase
    {
        private readonly ICampCatalog _catalog;
        private readonly IEconomyRepository _repository;

        public PurchaseCampUpgradeUseCase(ICampCatalog catalog, IEconomyRepository repository)
        { _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog)); _repository = repository ?? throw new ArgumentNullException(nameof(repository)); }

        public PurchaseCampUpgradeResult Execute(CampUpgradeId id)
        {
            PlayerProgress current = _repository.Current;
            if (!_catalog.TryGetUpgrade(id, out CampUpgradeDefinition definition))
                return new PurchaseCampUpgradeResult(PurchaseCampUpgradeOutcome.MissingDefinition, null, current.Wallet);
            if (_repository.IsReadOnly)
                return new PurchaseCampUpgradeResult(PurchaseCampUpgradeOutcome.ReadOnly, definition, current.Wallet);
            if (current.UnlockedCampUpgradeIds.Contains(id.Value, StringComparer.Ordinal))
                return new PurchaseCampUpgradeResult(PurchaseCampUpgradeOutcome.AlreadyUnlocked, definition, current.Wallet);
            if (definition.Prerequisites.Any(value => !current.UnlockedCampUpgradeIds.Contains(value.Value, StringComparer.Ordinal)))
                return new PurchaseCampUpgradeResult(PurchaseCampUpgradeOutcome.PrerequisiteLocked, definition, current.Wallet);
            if (!current.Wallet.TrySpend(definition.StarCost, out ExplorerStars balance))
                return new PurchaseCampUpgradeResult(PurchaseCampUpgradeOutcome.InsufficientStars, definition, current.Wallet);

            EconomyTransactionId transaction = definition.TransactionId;
            if (current.ProcessedEconomyTransactionIds.Contains(transaction.Value, StringComparer.Ordinal))
                return new PurchaseCampUpgradeResult(PurchaseCampUpgradeOutcome.AlreadyUnlocked, definition, current.Wallet);
            var transactions = new List<string>(current.ProcessedEconomyTransactionIds) { transaction.Value };
            EconomyLedgerEntry[] ledger = GrantRewardUseCase.Append(current.EconomyLedger,
                new EconomyLedgerEntry(transaction, EconomyTransactionKind.Spend, definition.SpendReasonId,
                    definition.StarCost, balance));
            var unlocked = new List<string>(current.UnlockedCampUpgradeIds) { id.Value };
            PlayerProgress next = current.WithEconomyAndCampUpgrade(balance, transactions, ledger, unlocked);
            _repository.Commit(next);
            return new PurchaseCampUpgradeResult(PurchaseCampUpgradeOutcome.Purchased, definition, balance);
        }
    }
}
