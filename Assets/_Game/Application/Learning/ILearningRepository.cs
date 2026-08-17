using System;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Learning
{
    public interface ILearningRepository
    {
        bool IsReadOnly { get; }
        PlayerProgress Current { get; }
        event Action<PlayerProgress> Changed;
        void Commit(PlayerProgress progress);
    }
}
