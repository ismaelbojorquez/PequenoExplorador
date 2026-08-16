using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Discovery
{
    public interface IDiscoveryProgressRepository
    {
        bool IsReadOnly { get; }
        PlayerProgress Current { get; }
        void Commit(PlayerProgress progress);
    }
}
