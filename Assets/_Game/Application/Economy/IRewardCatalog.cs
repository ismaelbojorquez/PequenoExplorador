using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Economy
{
    public interface IRewardCatalog
    {
        bool TryGet(RewardId id, out RewardDefinition definition);
        bool TryGetDiscoveryReward(DiscoveryId discoveryId, out RewardDefinition definition);
    }
}
