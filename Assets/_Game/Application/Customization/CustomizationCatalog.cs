using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Customization
{
    public sealed class CustomizationCatalog : ICustomizationCatalog
    {
        private readonly Dictionary<CustomizationSlotId, CustomizationSlotDefinition> _slots;
        private readonly Dictionary<CosmeticId, CosmeticDefinition> _cosmetics;
        private readonly Dictionary<CustomizationSlotId, IReadOnlyList<CosmeticDefinition>> _bySlot;

        public CustomizationCatalog(IEnumerable<CustomizationSlotDefinition> slots, IEnumerable<CosmeticDefinition> cosmetics)
        {
            CustomizationSlotDefinition[] slotArray = (slots ?? throw new ArgumentNullException(nameof(slots))).ToArray();
            CosmeticDefinition[] cosmeticArray = (cosmetics ?? throw new ArgumentNullException(nameof(cosmetics))).ToArray();
            if (slotArray.Any(value => value == null) || cosmeticArray.Any(value => value == null))
                throw new ArgumentException("Customization catalog cannot contain null definitions.");
            if (slotArray.Select(value => value.Id).Distinct().Count() != slotArray.Length)
                throw new ArgumentException("Customization slot IDs must be unique.", nameof(slots));
            if (slotArray.Select(value => value.DisplayOrder).Distinct().Count() != slotArray.Length)
                throw new ArgumentException("Customization slot display orders must be unique.", nameof(slots));
            if (cosmeticArray.Select(value => value.Id).Distinct().Count() != cosmeticArray.Length)
                throw new ArgumentException("Cosmetic IDs must be unique.", nameof(cosmetics));
            _slots = slotArray.ToDictionary(value => value.Id);
            _cosmetics = cosmeticArray.ToDictionary(value => value.Id);
            foreach (CosmeticDefinition cosmetic in cosmeticArray)
                if (!_slots.ContainsKey(cosmetic.SlotId)) throw new ArgumentException($"Cosmetic '{cosmetic.Id}' references missing slot '{cosmetic.SlotId}'.");
            foreach (CustomizationSlotDefinition slot in slotArray)
            {
                if (!_cosmetics.TryGetValue(slot.DefaultCosmeticId, out CosmeticDefinition fallback) ||
                    fallback.SlotId != slot.Id || !fallback.IsInitiallyUnlocked)
                    throw new ArgumentException($"Slot '{slot.Id}' requires an initially unlocked default in the same slot.");
            }
            Slots = slotArray.OrderBy(value => value.DisplayOrder).ToArray();
            Cosmetics = cosmeticArray.OrderBy(value => value.Id.Value, StringComparer.Ordinal).ToArray();
            _bySlot = Slots.ToDictionary(slot => slot.Id, slot => (IReadOnlyList<CosmeticDefinition>)Cosmetics
                .Where(cosmetic => cosmetic.SlotId == slot.Id).OrderBy(value => value.Id.Value, StringComparer.Ordinal).ToArray());
        }

        public static CustomizationCatalog Empty { get; } = new CustomizationCatalog(Array.Empty<CustomizationSlotDefinition>(), Array.Empty<CosmeticDefinition>());
        public IReadOnlyList<CustomizationSlotDefinition> Slots { get; }
        public IReadOnlyList<CosmeticDefinition> Cosmetics { get; }
        public bool TryGetSlot(CustomizationSlotId id, out CustomizationSlotDefinition definition) => _slots.TryGetValue(id, out definition);
        public bool TryGetCosmetic(CosmeticId id, out CosmeticDefinition definition) => _cosmetics.TryGetValue(id, out definition);
        public IReadOnlyList<CosmeticDefinition> GetForSlot(CustomizationSlotId id) => _bySlot.TryGetValue(id, out IReadOnlyList<CosmeticDefinition> values) ? values : Array.Empty<CosmeticDefinition>();
    }
}
