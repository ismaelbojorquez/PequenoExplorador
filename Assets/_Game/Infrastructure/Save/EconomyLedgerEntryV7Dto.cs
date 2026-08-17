using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class EconomyLedgerEntryV7Dto
    {
        [SerializeField] private string transactionId;
        [SerializeField] private int kind;
        [SerializeField] private string rewardId;
        [SerializeField] private int amount;
        [SerializeField] private int balanceAfter;
        public string TransactionId => transactionId;
        public int Kind => kind;
        public string RewardId => rewardId;
        public int Amount => amount;
        public int BalanceAfter => balanceAfter;
        public static EconomyLedgerEntryV7Dto Create(string id, int transactionKind, string reward, int starAmount, int resultingBalance) =>
            new EconomyLedgerEntryV7Dto { transactionId = id, kind = transactionKind, rewardId = reward, amount = starAmount, balanceAfter = resultingBalance };
    }
}
