using PequenoExplorador.Application.Economy;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Missions
{
    public enum MissionActivationOutcome { Activated, AlreadyActive, AlreadyCompleted, Missing, PrerequisitesMissing, ReadOnly }
    public enum MissionFactOutcome { Progressed, Completed, Ignored, Duplicate, ReadOnly }

    public readonly struct MissionActivationResult
    {
        public MissionActivationResult(MissionActivationOutcome outcome, MissionProgress progress)
        { Outcome = outcome; Progress = progress; }
        public MissionActivationOutcome Outcome { get; }
        public MissionProgress Progress { get; }
    }

    public readonly struct MissionFactResult
    {
        public MissionFactResult(MissionFactOutcome outcome, MissionId missionId, GrantRewardResult reward)
        { Outcome = outcome; MissionId = missionId; Reward = reward; }
        public MissionFactOutcome Outcome { get; }
        public MissionId MissionId { get; }
        public GrantRewardResult Reward { get; }
    }
}
