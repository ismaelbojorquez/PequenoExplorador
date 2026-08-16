using System;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Save
{
    public sealed class SaveLoadResult
    {
        public SaveLoadResult(
            PlayerProgress progress,
            SaveLoadStatus status,
            SaveUserNotice userNotice,
            int sourceSchemaVersion)
        {
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            Status = status;
            UserNotice = userNotice;
            SourceSchemaVersion = sourceSchemaVersion;
        }

        public PlayerProgress Progress { get; }
        public SaveLoadStatus Status { get; }
        public SaveUserNotice UserNotice { get; }
        public int SourceSchemaVersion { get; }
    }
}
