using System.Collections.Generic;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Customization
{
    public interface ICustomizationCatalog
    {
        IReadOnlyList<CustomizationSlotDefinition> Slots { get; }
        IReadOnlyList<CosmeticDefinition> Cosmetics { get; }
        bool TryGetSlot(CustomizationSlotId id, out CustomizationSlotDefinition definition);
        bool TryGetCosmetic(CosmeticId id, out CosmeticDefinition definition);
        IReadOnlyList<CosmeticDefinition> GetForSlot(CustomizationSlotId id);
    }
}
