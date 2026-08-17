namespace PequenoExplorador.Application.Photography
{
    public enum PhotoCaptureOutcome
    {
        NotReady = 0,
        CapturedNew = 1,
        CapturedRepeated = 2,
        CapturedWithoutThumbnail = 3,
        ExistingPhotoKept = 4,
        Busy = 5,
        Cancelled = 6,
        Unavailable = 7
    }
}
