using System.Collections.Generic;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Missions
{
    public interface IMissionCatalog
    {
        IReadOnlyList<MissionDefinition> Missions { get; }
        bool TryGet(MissionId id, out MissionDefinition definition);
    }
}
