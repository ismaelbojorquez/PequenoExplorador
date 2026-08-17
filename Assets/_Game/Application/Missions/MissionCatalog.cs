using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Missions
{
    public sealed class MissionCatalog : IMissionCatalog
    {
        private readonly MissionDefinition[] _missions;
        private readonly Dictionary<MissionId, MissionDefinition> _byId;

        public MissionCatalog(IEnumerable<MissionDefinition> missions)
        {
            _missions = (missions ?? throw new ArgumentNullException(nameof(missions)))
                .OrderBy(item => item?.Id.Value, StringComparer.Ordinal).ToArray();
            if (_missions.Any(item => item == null) || _missions.Select(item => item.Id).Distinct().Count() != _missions.Length)
                throw new ArgumentException("Mission definitions must be non-null and unique.", nameof(missions));
            _byId = _missions.ToDictionary(item => item.Id);
            foreach (MissionDefinition mission in _missions)
                foreach (MissionId prerequisite in mission.Prerequisites)
                    if (!_byId.ContainsKey(prerequisite))
                        throw new ArgumentException($"Mission '{mission.Id}' prerequisite '{prerequisite}' is missing.", nameof(missions));
            foreach (MissionDefinition mission in _missions)
                if (Reaches(mission.Id, mission.Id, new HashSet<MissionId>()))
                    throw new ArgumentException($"Mission prerequisite cycle reaches '{mission.Id}'.", nameof(missions));
        }

        public static MissionCatalog Empty { get; } = new MissionCatalog(Array.Empty<MissionDefinition>());
        public IReadOnlyList<MissionDefinition> Missions => _missions;
        public bool TryGet(MissionId id, out MissionDefinition definition) => _byId.TryGetValue(id, out definition);

        private bool Reaches(MissionId root, MissionId current, HashSet<MissionId> visited)
        {
            if (!visited.Add(current) || !_byId.TryGetValue(current, out MissionDefinition definition)) return false;
            foreach (MissionId next in definition.Prerequisites)
                if (next.Equals(root) || Reaches(root, next, new HashSet<MissionId>(visited))) return true;
            return false;
        }
    }
}
