using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Customization;
using PequenoExplorador.Content.Data;
using UnityEngine;

namespace PequenoExplorador.Content.Customization
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Customization/Catalog", fileName = "CustomizationCatalog")]
    public sealed class CustomizationCatalogAsset : ScriptableObject
    {
        [SerializeField] private CustomizationSlotDefinitionAsset[] _slots = Array.Empty<CustomizationSlotDefinitionAsset>();
        [SerializeField] private CosmeticDefinitionAsset[] _cosmetics = Array.Empty<CosmeticDefinitionAsset>();
        public IReadOnlyList<CustomizationSlotDefinitionAsset> Slots => _slots ?? Array.Empty<CustomizationSlotDefinitionAsset>();
        public IReadOnlyList<CosmeticDefinitionAsset> Cosmetics => _cosmetics ?? Array.Empty<CosmeticDefinitionAsset>();

        public bool TryBuild(ContentValidationMode mode, out CustomizationCatalog catalog, out IReadOnlyList<string> violations)
        {
            var errors = new List<string>();
            if (Slots.Any(value => value == null)) errors.Add("CUSTOM001 slot catalog contains a missing reference.");
            if (Cosmetics.Any(value => value == null)) errors.Add("CUSTOM002 cosmetic catalog contains a missing reference.");
            CustomizationSlotDefinition[] slots = Slots.Where(value => value != null).Select(value =>
            { try { return value.ToRuntime(); } catch (Exception exception) { errors.Add($"CUSTOM003 invalid slot '{value.name}': {exception.Message}"); return null; } }).Where(value => value != null).ToArray();
            CosmeticDefinition[] cosmetics = Cosmetics.Where(value => value != null).Select(value =>
            {
                try
                {
                    EditorialMetadata editorial = value.Editorial?.ToRuntime();
                    if (editorial == null) errors.Add($"CUSTOM004 cosmetic '{value.name}' has no editorial metadata.");
                    else if (mode == ContentValidationMode.Release && !editorial.IsReleaseApproved)
                        errors.Add($"CUSTOM005 Release rejects {editorial.State}/placeholder cosmetic '{value.RawId}'.");
                    return value.ToRuntime();
                }
                catch (Exception exception) { errors.Add($"CUSTOM006 invalid cosmetic '{value.name}': {exception.Message}"); return null; }
            }).Where(value => value != null).ToArray();
            try { catalog = errors.Count == 0 ? new CustomizationCatalog(slots, cosmetics) : null; }
            catch (Exception exception) { errors.Add("CUSTOM007 catalog failed: " + exception.Message); catalog = null; }
            violations = errors;
            return errors.Count == 0;
        }
    }
}
