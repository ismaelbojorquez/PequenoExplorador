using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Camp
{
    public sealed class CampCatalog : ICampCatalog
    {
        private readonly Dictionary<CampStationId, CampStationDefinition> _stations;
        private readonly Dictionary<CampUpgradeId, CampUpgradeDefinition> _upgrades;

        public CampCatalog(IEnumerable<CampStationDefinition> stations, IEnumerable<CampUpgradeDefinition> upgrades)
        {
            CampStationDefinition[] stationArray = (stations ?? throw new ArgumentNullException(nameof(stations))).ToArray();
            CampUpgradeDefinition[] upgradeArray = (upgrades ?? throw new ArgumentNullException(nameof(upgrades))).ToArray();
            if (stationArray.Any(value => value == null) || upgradeArray.Any(value => value == null))
                throw new ArgumentException("Camp catalog cannot contain null definitions.");
            if (stationArray.Select(value => value.Id).Distinct().Count() != stationArray.Length)
                throw new ArgumentException("Camp station IDs must be unique.", nameof(stations));
            if (stationArray.Select(value => value.ActionId).Distinct().Count() != stationArray.Length)
                throw new ArgumentException("Camp station action IDs must be unique.", nameof(stations));
            if (upgradeArray.Select(value => value.Id).Distinct().Count() != upgradeArray.Length)
                throw new ArgumentException("Camp upgrade IDs must be unique.", nameof(upgrades));
            _stations = stationArray.ToDictionary(value => value.Id);
            _upgrades = upgradeArray.ToDictionary(value => value.Id);
            foreach (CampUpgradeDefinition upgrade in upgradeArray)
            {
                if (!_stations.ContainsKey(upgrade.StationId))
                    throw new ArgumentException($"Camp upgrade '{upgrade.Id}' references missing station '{upgrade.StationId}'.", nameof(upgrades));
                foreach (CampUpgradeId prerequisite in upgrade.Prerequisites)
                    if (!_upgrades.ContainsKey(prerequisite))
                        throw new ArgumentException($"Camp upgrade '{upgrade.Id}' references missing prerequisite '{prerequisite}'.", nameof(upgrades));
            }
            EnsureAcyclic(upgradeArray);
            Stations = stationArray.OrderBy(value => value.DisplayOrder).ThenBy(value => value.Id.Value, StringComparer.Ordinal).ToArray();
            Upgrades = upgradeArray.OrderBy(value => value.Id.Value, StringComparer.Ordinal).ToArray();
        }

        public static CampCatalog Empty { get; } = new CampCatalog(Array.Empty<CampStationDefinition>(), Array.Empty<CampUpgradeDefinition>());
        public IReadOnlyList<CampStationDefinition> Stations { get; }
        public IReadOnlyList<CampUpgradeDefinition> Upgrades { get; }
        public bool TryGetStation(CampStationId id, out CampStationDefinition definition) => _stations.TryGetValue(id, out definition);
        public bool TryGetUpgrade(CampUpgradeId id, out CampUpgradeDefinition definition) => _upgrades.TryGetValue(id, out definition);

        private static void EnsureAcyclic(IEnumerable<CampUpgradeDefinition> upgrades)
        {
            Dictionary<CampUpgradeId, CampUpgradeDefinition> byId = upgrades.ToDictionary(value => value.Id);
            var visiting = new HashSet<CampUpgradeId>();
            var visited = new HashSet<CampUpgradeId>();
            foreach (CampUpgradeId id in byId.Keys)
                Visit(id, byId, visiting, visited);
        }

        private static void Visit(CampUpgradeId id, IReadOnlyDictionary<CampUpgradeId, CampUpgradeDefinition> byId,
            ISet<CampUpgradeId> visiting, ISet<CampUpgradeId> visited)
        {
            if (visited.Contains(id)) return;
            if (!visiting.Add(id)) throw new ArgumentException($"Camp upgrade prerequisite cycle includes '{id}'.");
            foreach (CampUpgradeId prerequisite in byId[id].Prerequisites) Visit(prerequisite, byId, visiting, visited);
            visiting.Remove(id);
            visited.Add(id);
        }
    }
}
