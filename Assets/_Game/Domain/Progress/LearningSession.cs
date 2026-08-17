using System;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Domain.Progress
{
    public enum LearningSessionStatus { Active = 1, Exited = 2, Completed = 3 }

    public sealed class LearningSession
    {
        public LearningSession(ActivityId activityId, LearningSessionStatus status, int attempts, int hintLevel)
        {
            if (!activityId.IsValid) throw new ArgumentException("Activity ID is invalid.", nameof(activityId));
            if (!Enum.IsDefined(typeof(LearningSessionStatus), status)) throw new ArgumentOutOfRangeException(nameof(status));
            if (attempts < 0) throw new ArgumentOutOfRangeException(nameof(attempts));
            if (hintLevel < 0) throw new ArgumentOutOfRangeException(nameof(hintLevel));
            ActivityId = activityId; Status = status; Attempts = attempts; HintLevel = hintLevel;
        }

        public ActivityId ActivityId { get; }
        public LearningSessionStatus Status { get; }
        public int Attempts { get; }
        public int HintLevel { get; }
        public bool IsCompleted => Status == LearningSessionStatus.Completed;
        public LearningSession Resume() => new LearningSession(ActivityId, LearningSessionStatus.Active, Attempts, HintLevel);
        public LearningSession RecordIncorrect(int hintLevel) => new LearningSession(ActivityId, LearningSessionStatus.Active, checked(Attempts + 1), hintLevel);
        public LearningSession Complete() => new LearningSession(ActivityId, LearningSessionStatus.Completed, Attempts, HintLevel);
        public LearningSession Exit() => new LearningSession(ActivityId, LearningSessionStatus.Exited, Attempts, HintLevel);
        public LearningSession Restart() => new LearningSession(ActivityId, LearningSessionStatus.Active, 0, 0);
    }
}
