using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Economy
{
    public sealed class RewardCatalog : IRewardCatalog
    {
        private readonly Dictionary<RewardId, RewardDefinition> _byId;
        private readonly Dictionary<DiscoveryId, RewardDefinition> _byDiscovery;
        private readonly RewardDefinition[] _definitions;
        public RewardCatalog(IEnumerable<RewardDefinition> definitions)
        {
            RewardDefinition[] items = (definitions ?? throw new ArgumentNullException(nameof(definitions))).ToArray();
            if (items.Any(item => item == null)) throw new ArgumentException("Reward catalog cannot contain null definitions.", nameof(definitions));
            if (items.Select(item => item.Id).Distinct().Count() != items.Length) throw new ArgumentException("Reward IDs must be unique.", nameof(definitions));
            _byId = items.ToDictionary(item => item.Id);
            _definitions = items.OrderBy(item => item.Id.Value, StringComparer.Ordinal).ToArray();
            _byDiscovery = new Dictionary<DiscoveryId, RewardDefinition>();
            foreach (RewardDefinition item in items.Where(item => item.SourceKind == RewardSourceKind.Discovery))
            {
                DiscoveryId source = DiscoveryId.Parse(item.SourceId);
                if (_byDiscovery.ContainsKey(source)) throw new ArgumentException("A discovery can reference only one unique reward.", nameof(definitions));
                _byDiscovery.Add(source, item);
            }
        }
        public static RewardCatalog Empty { get; } = new RewardCatalog(Array.Empty<RewardDefinition>());
        public IReadOnlyList<RewardDefinition> Definitions => _definitions;
        public bool TryGet(RewardId id, out RewardDefinition definition) => _byId.TryGetValue(id, out definition);
        public bool TryGetDiscoveryReward(DiscoveryId discoveryId, out RewardDefinition definition) => _byDiscovery.TryGetValue(discoveryId, out definition);
    }
}
