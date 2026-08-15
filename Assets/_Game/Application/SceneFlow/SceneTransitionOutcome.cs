namespace PequenoExplorador.Application.SceneFlow
{
    public enum SceneTransitionOutcome
    {
        Succeeded,
        AlreadyThere,
        Busy,
        Invalid,
        Canceled,
        TimedOut,
        Failed
    }
}
