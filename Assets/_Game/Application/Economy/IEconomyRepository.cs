using System;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Economy
{
    public interface IEconomyRepository
    {
        bool IsReadOnly { get; }
        PlayerProgress Current { get; }
        event Action<PlayerProgress> Changed;
        void Commit(PlayerProgress progress);
    }
}
