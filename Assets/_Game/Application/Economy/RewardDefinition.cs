using System;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Application.Content;

namespace PequenoExplorador.Application.Economy
{
    public enum RewardSourceKind { Discovery = 1, Mission = 2, Activity = 3, Collection = 4, Development = 5 }

    public sealed class RewardDefinition : IRewardDefinition
    {
        public RewardDefinition(RewardId id, ExplorerStars amount, RewardSourceKind sourceKind, string sourceId)
        {
            if (!id.IsValid) throw new ArgumentException("Reward ID is required.", nameof(id));
            if (amount.Value <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Reward must grant at least one star.");
            if (!Enum.IsDefined(typeof(RewardSourceKind), sourceKind)) throw new ArgumentOutOfRangeException(nameof(sourceKind));
            if (string.IsNullOrWhiteSpace(sourceId)) throw new ArgumentException("Reward source ID is required.", nameof(sourceId));
            Id = id; Amount = amount; SourceKind = sourceKind; SourceId = sourceId;
        }
        public RewardId Id { get; }
        public ExplorerStars Amount { get; }
        public RewardSourceKind SourceKind { get; }
        public string SourceId { get; }
    }
}
