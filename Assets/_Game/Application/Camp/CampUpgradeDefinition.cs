using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;

namespace PequenoExplorador.Application.Camp
{
    public sealed class CampUpgradeDefinition
    {
        private readonly CampUpgradeId[] _prerequisites;

        public CampUpgradeDefinition(
            CampUpgradeId id,
            CampStationId stationId,
            LocalizedKey displayName,
            LocalizedKey description,
            LocalizedKey previewCopy,
            ExplorerStars starCost,
            RewardId spendReasonId,
            VisualAssetId beforeVisualId,
            VisualAssetId afterVisualId,
            IEnumerable<CampUpgradeId> prerequisites,
            bool isPlaceholder)
        {
            if (!id.IsValid || !stationId.IsValid || !spendReasonId.IsValid ||
                !beforeVisualId.IsValid || !afterVisualId.IsValid)
                throw new ArgumentException("Camp upgrade IDs must be valid.");
            if (starCost.Value <= 0) throw new ArgumentOutOfRangeException(nameof(starCost));
            _prerequisites = (prerequisites ?? throw new ArgumentNullException(nameof(prerequisites))).ToArray();
            if (_prerequisites.Any(value => !value.IsValid) || _prerequisites.Distinct().Count() != _prerequisites.Length)
                throw new ArgumentException("Camp upgrade prerequisites must be valid and unique.", nameof(prerequisites));
            if (_prerequisites.Contains(id)) throw new ArgumentException("A Camp upgrade cannot require itself.", nameof(prerequisites));
            Id = id;
            StationId = stationId;
            DisplayName = displayName;
            Description = description;
            PreviewCopy = previewCopy;
            StarCost = starCost;
            SpendReasonId = spendReasonId;
            BeforeVisualId = beforeVisualId;
            AfterVisualId = afterVisualId;
            IsPlaceholder = isPlaceholder;
        }

        public CampUpgradeId Id { get; }
        public CampStationId StationId { get; }
        public LocalizedKey DisplayName { get; }
        public LocalizedKey Description { get; }
        public LocalizedKey PreviewCopy { get; }
        public ExplorerStars StarCost { get; }
        public RewardId SpendReasonId { get; }
        public VisualAssetId BeforeVisualId { get; }
        public VisualAssetId AfterVisualId { get; }
        public IReadOnlyList<CampUpgradeId> Prerequisites => _prerequisites;
        public bool IsPlaceholder { get; }
        public EconomyTransactionId TransactionId => EconomyTransactionId.Parse("economy-tx." + Id.Value);
    }
}
