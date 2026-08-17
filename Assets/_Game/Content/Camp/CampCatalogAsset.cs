using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Camp;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Content.Data;
using UnityEngine;

namespace PequenoExplorador.Content.Camp
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Camp/Camp Catalog", fileName = "CampCatalog")]
    public sealed class CampCatalogAsset : ScriptableObject
    {
        [SerializeField] private CampStationDefinitionAsset[] _stations = Array.Empty<CampStationDefinitionAsset>();
        [SerializeField] private CampUpgradeDefinitionAsset[] _upgrades = Array.Empty<CampUpgradeDefinitionAsset>();
        public IReadOnlyList<CampStationDefinitionAsset> Stations => _stations ?? Array.Empty<CampStationDefinitionAsset>();
        public IReadOnlyList<CampUpgradeDefinitionAsset> Upgrades => _upgrades ?? Array.Empty<CampUpgradeDefinitionAsset>();

        public bool TryBuild(ContentValidationMode mode, out CampCatalog catalog, out IReadOnlyList<string> violations)
        {
            var errors = new List<string>();
            if (Stations.Any(value => value == null)) errors.Add("CAMP001 station catalog contains a missing reference.");
            if (Upgrades.Any(value => value == null)) errors.Add("CAMP002 upgrade catalog contains a missing reference.");
            CampStationDefinition[] stations = Stations.Where(value => value != null).Select(value =>
            {
                try { return value.ToRuntime(); }
                catch (Exception exception) { errors.Add($"CAMP003 invalid station '{value.name}': {exception.Message}"); return null; }
            }).Where(value => value != null).ToArray();
            CampUpgradeDefinition[] upgrades = Upgrades.Where(value => value != null).Select(value =>
            {
                try
                {
                    EditorialMetadata editorial = value.Editorial?.ToRuntime();
                    if (editorial == null) errors.Add($"CAMP004 upgrade '{value.name}' has no editorial metadata.");
                    else if (mode == ContentValidationMode.Release && !editorial.IsReleaseApproved)
                        errors.Add($"CAMP005 Release rejects {editorial.State}/placeholder upgrade '{value.RawId}'.");
                    return value.ToRuntime();
                }
                catch (Exception exception) { errors.Add($"CAMP006 invalid upgrade '{value.name}': {exception.Message}"); return null; }
            }).Where(value => value != null).ToArray();
            try { catalog = errors.Count == 0 ? new CampCatalog(stations, upgrades) : null; }
            catch (Exception exception) { errors.Add("CAMP007 catalog failed: " + exception.Message); catalog = null; }
            violations = errors;
            return errors.Count == 0;
        }
    }
}
