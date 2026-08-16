using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Discovery
{
    public readonly struct DiscoverResult
    {
        public DiscoverResult(
            DiscoverOutcome outcome,
            DiscoveryId discoveryId,
            DiscoveryProgress progress)
        {
            Outcome = outcome;
            DiscoveryId = discoveryId;
            Progress = progress;
        }

        public DiscoverOutcome Outcome { get; }
        public DiscoveryId DiscoveryId { get; }
        public DiscoveryProgress Progress { get; }
        public int Count => Progress?.Count ?? 0;
        public bool GrantsUniqueReward => Outcome == DiscoverOutcome.First;
        public bool ChangedProgress => Outcome == DiscoverOutcome.First || Outcome == DiscoverOutcome.Repeated;
    }
}
