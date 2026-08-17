using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Economy
{
    public sealed class SpendStarsUseCase
    {
        private readonly IEconomyRepository _repository;
        public SpendStarsUseCase(IEconomyRepository repository) => _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        public SpendStarsResult Execute(ExplorerStars amount, RewardId reasonId, EconomyTransactionId transactionId)
        {
            PlayerProgress current = _repository.Current;
            if (amount.Value <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Spend must be positive.");
            if (!reasonId.IsValid || !transactionId.IsValid) throw new ArgumentException("Spend requires stable reason and transaction IDs.");
            if (_repository.IsReadOnly) return new SpendStarsResult(SpendStarsOutcome.ReadOnly, current.Wallet, amount);
            if (current.ProcessedEconomyTransactionIds.Contains(transactionId.Value, StringComparer.Ordinal))
                return new SpendStarsResult(SpendStarsOutcome.AlreadyProcessed, current.Wallet, amount);
            if (!current.Wallet.TrySpend(amount, out ExplorerStars balance))
                return new SpendStarsResult(SpendStarsOutcome.Insufficient, current.Wallet, amount);
            var ids = new List<string>(current.ProcessedEconomyTransactionIds) { transactionId.Value };
            EconomyLedgerEntry[] ledger = GrantRewardUseCase.Append(current.EconomyLedger,
                new EconomyLedgerEntry(transactionId, EconomyTransactionKind.Spend, reasonId, amount, balance));
            _repository.Commit(current.WithEconomy(balance, ids, ledger));
            return new SpendStarsResult(SpendStarsOutcome.Spent, balance, amount);
        }
    }
}
