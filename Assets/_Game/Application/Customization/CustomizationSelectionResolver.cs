using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Customization
{
    public sealed class CustomizationSelectionResolver
    {
        private readonly ICustomizationCatalog _catalog;
        public CustomizationSelectionResolver(ICustomizationCatalog catalog) => _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));

        public IReadOnlyList<CosmeticDefinition> Resolve(PlayerProgress progress)
        {
            if (progress == null) throw new ArgumentNullException(nameof(progress));
            var result = new List<CosmeticDefinition>();
            foreach (CustomizationSlotDefinition slot in _catalog.Slots)
            {
                EquippedCosmetic saved = progress.EquippedCosmetics.FirstOrDefault(value => value.SlotId == slot.Id);
                CosmeticDefinition selected = null;
                if (saved != null && _catalog.TryGetCosmetic(saved.CosmeticId, out CosmeticDefinition candidate) &&
                    candidate.SlotId == slot.Id && IsAvailable(candidate, progress) && IsCompatible(candidate, result))
                    selected = candidate;
                if (selected == null && _catalog.TryGetCosmetic(slot.DefaultCosmeticId, out CosmeticDefinition fallback) && IsCompatible(fallback, result))
                    selected = fallback;
                if (selected != null) result.Add(selected);
            }
            return result;
        }

        public bool IsAvailable(CosmeticDefinition definition, PlayerProgress progress) => definition.IsInitiallyUnlocked ||
            progress.UnlockedCosmeticIds.Contains(definition.Id.Value, StringComparer.Ordinal);

        public static bool IsCompatible(CosmeticDefinition candidate, IEnumerable<CosmeticDefinition> selected)
        {
            CosmeticDefinition[] others = selected.Where(value => value != null && value.SlotId != candidate.SlotId).ToArray();
            var otherTags = new HashSet<CosmeticCompatibilityTagId>(others.SelectMany(value => value.CompatibilityTags));
            if (candidate.BlockedTags.Any(otherTags.Contains)) return false;
            var candidateTags = new HashSet<CosmeticCompatibilityTagId>(candidate.CompatibilityTags);
            return !others.Any(value => value.BlockedTags.Any(candidateTags.Contains));
        }
    }
}
