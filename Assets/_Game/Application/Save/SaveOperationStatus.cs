namespace PequenoExplorador.Application.Save
{
    public enum SaveOperationStatus
    {
        Saved = 0,
        BlockedByFutureVersion = 1,
        Failed = 2
    }
}
