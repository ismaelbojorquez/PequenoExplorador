using System;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Customization
{
    public sealed class CustomizationSlotDefinition
    {
        public CustomizationSlotDefinition(CustomizationSlotId id, LocalizedKey displayName, int displayOrder, CosmeticId defaultCosmeticId)
        {
            if (!id.IsValid || !defaultCosmeticId.IsValid) throw new ArgumentException("Customization slot IDs must be valid.");
            if (displayOrder < 0) throw new ArgumentOutOfRangeException(nameof(displayOrder));
            Id = id; DisplayName = displayName; DisplayOrder = displayOrder; DefaultCosmeticId = defaultCosmeticId;
        }
        public CustomizationSlotId Id { get; }
        public LocalizedKey DisplayName { get; }
        public int DisplayOrder { get; }
        public CosmeticId DefaultCosmeticId { get; }
    }
}
