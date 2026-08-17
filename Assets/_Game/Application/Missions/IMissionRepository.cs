using System;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Missions
{
    public interface IMissionRepository
    {
        bool IsReadOnly { get; }
        PlayerProgress Current { get; }
        event Action<PlayerProgress> Changed;
        void Commit(PlayerProgress progress);
    }
}
