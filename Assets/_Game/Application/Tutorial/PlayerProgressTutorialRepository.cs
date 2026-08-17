using System;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Tutorial
{
    public sealed class PlayerProgressTutorialRepository : ITutorialProgressRepository
    {
        private readonly ISaveService _save;
        private readonly AutosaveCoordinator _autosave;
        public PlayerProgressTutorialRepository(ISaveService save, AutosaveCoordinator autosave)
        { _save = save ?? throw new ArgumentNullException(nameof(save)); _autosave = autosave ?? throw new ArgumentNullException(nameof(autosave)); }
        public PlayerProgress Current => _autosave.Latest;
        public void Commit(PlayerProgress progress)
        {
            if (progress == null) throw new ArgumentNullException(nameof(progress));
            if (_save.IsReadOnly) throw new InvalidOperationException("Future-version save is read-only.");
            _autosave.RequestCheckpoint(progress);
        }
    }
}
