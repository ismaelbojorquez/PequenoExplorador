using System;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Domain.Progress
{
    public sealed class EquippedCosmetic : IEquatable<EquippedCosmetic>
    {
        public EquippedCosmetic(CustomizationSlotId slotId, CosmeticId cosmeticId)
        {
            if (!slotId.IsValid || !cosmeticId.IsValid) throw new ArgumentException("Equipped cosmetic IDs must be valid.");
            SlotId = slotId;
            CosmeticId = cosmeticId;
        }

        public CustomizationSlotId SlotId { get; }
        public CosmeticId CosmeticId { get; }
        public bool Equals(EquippedCosmetic other) => other != null && SlotId.Equals(other.SlotId) && CosmeticId.Equals(other.CosmeticId);
        public override bool Equals(object obj) => Equals(obj as EquippedCosmetic);
        public override int GetHashCode() => (SlotId.GetHashCode() * 397) ^ CosmeticId.GetHashCode();
    }
}
