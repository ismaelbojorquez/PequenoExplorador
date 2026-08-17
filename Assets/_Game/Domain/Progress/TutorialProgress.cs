using System;

namespace PequenoExplorador.Domain.Progress
{
    public enum TutorialProgressStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2,
        Skipped = 3
    }

    public sealed class TutorialProgress
    {
        public TutorialProgress(string tutorialId, int contentVersion, int stepIndex, TutorialProgressStatus status)
        {
            if (string.IsNullOrWhiteSpace(tutorialId)) throw new ArgumentException("Tutorial ID is required.", nameof(tutorialId));
            if (contentVersion < 0) throw new ArgumentOutOfRangeException(nameof(contentVersion));
            if (stepIndex < 0) throw new ArgumentOutOfRangeException(nameof(stepIndex));
            if (!Enum.IsDefined(typeof(TutorialProgressStatus), status)) throw new ArgumentOutOfRangeException(nameof(status));
            TutorialId = tutorialId;
            ContentVersion = contentVersion;
            StepIndex = stepIndex;
            Status = status;
        }

        public string TutorialId { get; }
        public int ContentVersion { get; }
        public int StepIndex { get; }
        public TutorialProgressStatus Status { get; }

        public static TutorialProgress CreateDefault() =>
            new TutorialProgress("tutorial.vertical-slice", 0, 0, TutorialProgressStatus.NotStarted);
    }
}
