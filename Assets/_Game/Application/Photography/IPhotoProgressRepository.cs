using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Photography
{
    public interface IPhotoProgressRepository
    {
        bool IsReadOnly { get; }
        PlayerProgress Current { get; }
        void Commit(PlayerProgress progress);
    }
}
