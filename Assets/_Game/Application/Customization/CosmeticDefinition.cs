using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;

namespace PequenoExplorador.Application.Customization
{
    public sealed class CosmeticDefinition
    {
        private readonly CosmeticCompatibilityTagId[] _compatibilityTags;
        private readonly CosmeticCompatibilityTagId[] _blockedTags;

        public CosmeticDefinition(CosmeticId id, CustomizationSlotId slotId, LocalizedKey displayName,
            VisualAssetId visualId, CustomizationColor color, bool initiallyUnlocked, ExplorerStars starCost,
            RewardId spendReasonId, CampUpgradeId requiredCampUpgradeId,
            IEnumerable<CosmeticCompatibilityTagId> compatibilityTags,
            IEnumerable<CosmeticCompatibilityTagId> blockedTags, bool isPlaceholder)
        {
            if (!id.IsValid || !slotId.IsValid || !visualId.IsValid) throw new ArgumentException("Cosmetic IDs must be valid.");
            if (starCost.Value > 0 && !spendReasonId.IsValid) throw new ArgumentException("Paid cosmetics require a stable spend reason.");
            if (initiallyUnlocked && (starCost.Value > 0 || requiredCampUpgradeId.IsValid))
                throw new ArgumentException("Initial cosmetics cannot have an unlock cost or prerequisite.");
            if (!initiallyUnlocked && starCost.Value == 0 && !requiredCampUpgradeId.IsValid)
                throw new ArgumentException("Locked cosmetics require stars or a progress prerequisite.");
            _compatibilityTags = CopyTags(compatibilityTags, nameof(compatibilityTags));
            _blockedTags = CopyTags(blockedTags, nameof(blockedTags));
            Id = id; SlotId = slotId; DisplayName = displayName; VisualId = visualId; Color = color;
            IsInitiallyUnlocked = initiallyUnlocked; StarCost = starCost; SpendReasonId = spendReasonId;
            RequiredCampUpgradeId = requiredCampUpgradeId; IsPlaceholder = isPlaceholder;
        }

        public CosmeticId Id { get; }
        public CustomizationSlotId SlotId { get; }
        public LocalizedKey DisplayName { get; }
        public VisualAssetId VisualId { get; }
        public CustomizationColor Color { get; }
        public bool IsInitiallyUnlocked { get; }
        public ExplorerStars StarCost { get; }
        public RewardId SpendReasonId { get; }
        public CampUpgradeId RequiredCampUpgradeId { get; }
        public IReadOnlyList<CosmeticCompatibilityTagId> CompatibilityTags => _compatibilityTags;
        public IReadOnlyList<CosmeticCompatibilityTagId> BlockedTags => _blockedTags;
        public bool IsPlaceholder { get; }
        public EconomyTransactionId TransactionId => EconomyTransactionId.Parse("economy-tx." + Id.Value);

        private static CosmeticCompatibilityTagId[] CopyTags(IEnumerable<CosmeticCompatibilityTagId> values, string name)
        {
            CosmeticCompatibilityTagId[] result = (values ?? throw new ArgumentNullException(name)).ToArray();
            if (result.Any(value => !value.IsValid) || result.Distinct().Count() != result.Length)
                throw new ArgumentException("Cosmetic compatibility tags must be valid and unique.", name);
            return result.OrderBy(value => value.Value, StringComparer.Ordinal).ToArray();
        }
    }
}
