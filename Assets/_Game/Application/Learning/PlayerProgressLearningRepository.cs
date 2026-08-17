using System;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Learning
{
    public sealed class PlayerProgressLearningRepository : ILearningRepository
    {
        private readonly ISaveService _save;
        private readonly AutosaveCoordinator _autosave;
        public PlayerProgressLearningRepository(ISaveService save, AutosaveCoordinator autosave) { _save = save ?? throw new ArgumentNullException(nameof(save)); _autosave = autosave ?? throw new ArgumentNullException(nameof(autosave)); }
        public bool IsReadOnly => _save.IsReadOnly;
        public PlayerProgress Current => _autosave.Latest;
        public event Action<PlayerProgress> Changed;
        public void Commit(PlayerProgress progress) { if (progress == null) throw new ArgumentNullException(nameof(progress)); if (IsReadOnly) throw new InvalidOperationException("Future-version save is read-only."); _autosave.RequestCheckpoint(progress); Changed?.Invoke(progress); }
    }
}
