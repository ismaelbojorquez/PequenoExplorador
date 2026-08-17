using System.Collections.Generic;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Camp
{
    public interface ICampCatalog
    {
        IReadOnlyList<CampStationDefinition> Stations { get; }
        IReadOnlyList<CampUpgradeDefinition> Upgrades { get; }
        bool TryGetStation(CampStationId id, out CampStationDefinition definition);
        bool TryGetUpgrade(CampUpgradeId id, out CampUpgradeDefinition definition);
    }
}
