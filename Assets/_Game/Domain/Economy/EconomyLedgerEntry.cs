using System;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Domain.Economy
{
    public enum EconomyTransactionKind { Grant = 1, Spend = 2 }

    public sealed class EconomyLedgerEntry
    {
        public EconomyLedgerEntry(EconomyTransactionId transactionId, EconomyTransactionKind kind,
            RewardId rewardId, ExplorerStars amount, ExplorerStars balanceAfter)
        {
            if (!transactionId.IsValid) throw new ArgumentException("Transaction ID is required.", nameof(transactionId));
            if (!Enum.IsDefined(typeof(EconomyTransactionKind), kind)) throw new ArgumentOutOfRangeException(nameof(kind));
            if (!rewardId.IsValid) throw new ArgumentException("Reward/reason ID is required.", nameof(rewardId));
            TransactionId = transactionId;
            Kind = kind;
            RewardId = rewardId;
            Amount = amount;
            BalanceAfter = balanceAfter;
        }
        public EconomyTransactionId TransactionId { get; }
        public EconomyTransactionKind Kind { get; }
        public RewardId RewardId { get; }
        public ExplorerStars Amount { get; }
        public ExplorerStars BalanceAfter { get; }
    }
}
