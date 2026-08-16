using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Infrastructure.Save
{
    internal sealed class DecodedSaveData
    {
        public DecodedSaveData(PlayerProgress progress, int saveSequence)
        {
            Progress = progress;
            SaveSequence = saveSequence;
        }

        public PlayerProgress Progress { get; }
        public int SaveSequence { get; }
    }
}
