using System;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Photography
{
    public sealed class PlayerProgressPhotoRepository : IPhotoProgressRepository
    {
        private readonly ISaveService _save;
        private readonly AutosaveCoordinator _autosave;
        public PlayerProgressPhotoRepository(ISaveService save, AutosaveCoordinator autosave)
        {
            _save = save ?? throw new ArgumentNullException(nameof(save));
            _autosave = autosave ?? throw new ArgumentNullException(nameof(autosave));
        }
        public bool IsReadOnly => _save.IsReadOnly;
        public PlayerProgress Current => _autosave.Latest;
        public void Commit(PlayerProgress progress) => _autosave.RequestCheckpoint(progress ?? throw new ArgumentNullException(nameof(progress)));
    }
}
