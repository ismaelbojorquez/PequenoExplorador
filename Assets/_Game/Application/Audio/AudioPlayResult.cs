namespace PequenoExplorador.Application.Audio
{
    public enum AudioPlayStatus
    {
        Started = 1,
        Queued = 2,
        Replaced = 3,
        Cooldown = 4,
        Missing = 5,
        Disabled = 6,
        Suspended = 7
    }

    public readonly struct AudioPlayResult
    {
        public AudioPlayResult(AudioPlayStatus status, string errorCode = null)
        {
            Status = status;
            ErrorCode = errorCode ?? string.Empty;
        }

        public AudioPlayStatus Status { get; }
        public string ErrorCode { get; }
        public bool IsAccepted => Status == AudioPlayStatus.Started || Status == AudioPlayStatus.Queued || Status == AudioPlayStatus.Replaced;
    }
}
