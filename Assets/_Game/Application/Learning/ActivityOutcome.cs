using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Missions;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Learning
{
    public enum ActivityOutcome
    {
        Started = 1, Resumed = 2, TryAgain = 3, Hint = 4, Completed = 5, Exited = 6,
        Restarted = 7, AlreadyCompleted = 8, Missing = 9, Unavailable = 10,
        ReadOnly = 11, InvalidOption = 12, NotActive = 13
    }

    public readonly struct LearningActivityResult
    {
        public LearningActivityResult(ActivityOutcome outcome, LearningSession session,
            GrantRewardResult reward = default, MissionFactResult mission = default)
        { Outcome = outcome; Session = session; Reward = reward; Mission = mission; }
        public ActivityOutcome Outcome { get; }
        public LearningSession Session { get; }
        public GrantRewardResult Reward { get; }
        public MissionFactResult Mission { get; }
    }
}
