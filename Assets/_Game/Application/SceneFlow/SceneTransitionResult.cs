namespace PequenoExplorador.Application.SceneFlow
{
    public sealed class SceneTransitionResult
    {
        public SceneTransitionResult(SceneTransitionOutcome outcome, string errorCode = "")
        {
            Outcome = outcome;
            ErrorCode = errorCode ?? string.Empty;
        }

        public SceneTransitionOutcome Outcome { get; }
        public string ErrorCode { get; }
        public bool IsSuccess => Outcome == SceneTransitionOutcome.Succeeded ||
                                 Outcome == SceneTransitionOutcome.AlreadyThere;
    }
}
