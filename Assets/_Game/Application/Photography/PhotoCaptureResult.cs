using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Photography
{
    public readonly struct PhotoCaptureResult
    {
        public PhotoCaptureResult(PhotoCaptureOutcome outcome, PhotoEvaluation evaluation, DiscoverResult discovery, PhotoProgress photo)
        {
            Outcome = outcome;
            Evaluation = evaluation;
            Discovery = discovery;
            Photo = photo;
        }
        public PhotoCaptureOutcome Outcome { get; }
        public PhotoEvaluation Evaluation { get; }
        public DiscoverResult Discovery { get; }
        public PhotoProgress Photo { get; }
        public bool ProgressCaptured => Outcome == PhotoCaptureOutcome.CapturedNew || Outcome == PhotoCaptureOutcome.CapturedRepeated ||
                                        Outcome == PhotoCaptureOutcome.CapturedWithoutThumbnail || Outcome == PhotoCaptureOutcome.ExistingPhotoKept;
    }
}
