namespace PequenoExplorador.Application.Save
{
    public enum SaveLoadStatus
    {
        Loaded = 0,
        DefaultCreated = 1,
        Migrated = 2,
        RecoveredBackup = 3,
        FutureVersion = 4
    }
}
