using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Missions;

namespace PequenoExplorador.Application.Photography
{
    public readonly struct PhotoCaptureResult
    {
        public PhotoCaptureResult(PhotoCaptureOutcome outcome, PhotoEvaluation evaluation, DiscoverResult discovery, PhotoProgress photo,
            GrantRewardResult reward = default)
            : this(outcome, evaluation, discovery, photo, reward, default)
        {
        }

        public PhotoCaptureResult(PhotoCaptureOutcome outcome, PhotoEvaluation evaluation, DiscoverResult discovery, PhotoProgress photo,
            GrantRewardResult reward, MissionFactResult mission)
        {
            Outcome = outcome;
            Evaluation = evaluation;
            Discovery = discovery;
            Photo = photo;
            Reward = reward;
            Mission = mission;
        }
        public PhotoCaptureOutcome Outcome { get; }
        public PhotoEvaluation Evaluation { get; }
        public DiscoverResult Discovery { get; }
        public PhotoProgress Photo { get; }
        public GrantRewardResult Reward { get; }
        public MissionFactResult Mission { get; }
        public bool ProgressCaptured => Outcome == PhotoCaptureOutcome.CapturedNew || Outcome == PhotoCaptureOutcome.CapturedRepeated ||
                                        Outcome == PhotoCaptureOutcome.CapturedWithoutThumbnail || Outcome == PhotoCaptureOutcome.ExistingPhotoKept;
    }
}
