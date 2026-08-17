using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Tutorial
{
    public enum TutorialTrigger
    {
        ExpeditionEntered,
        MovementAccepted,
        InteractionCompleted,
        PhotoCaptured,
        Continue,
        CampReturned,
        AlbumOpened
    }

    [Flags]
    public enum TutorialAction
    {
        None = 0,
        EnterExpedition = 1 << 0,
        Move = 1 << 1,
        Interact = 1 << 2,
        Photograph = 1 << 3,
        Continue = 1 << 4,
        ReturnCamp = 1 << 5,
        OpenAlbum = 1 << 6
    }

    public enum TutorialSpotlight
    {
        Expedition,
        Ground,
        Interactable,
        Shutter,
        DiscoveryReward,
        ReturnCamp,
        Album
    }

    public sealed class TutorialStepDefinition
    {
        public TutorialStepDefinition(string id, TutorialTrigger trigger, TutorialAction allowedActions,
            TutorialSpotlight spotlight, LocalizedKey instruction, AudioCueId voiceCue,
            double standardHelpSeconds, double moreGuidanceHelpSeconds)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Tutorial step ID is required.", nameof(id));
            if (!Enum.IsDefined(typeof(TutorialTrigger), trigger)) throw new ArgumentOutOfRangeException(nameof(trigger));
            if (!Enum.IsDefined(typeof(TutorialSpotlight), spotlight)) throw new ArgumentOutOfRangeException(nameof(spotlight));
            if (string.IsNullOrWhiteSpace(instruction.Table) || string.IsNullOrWhiteSpace(instruction.Entry))
                throw new ArgumentException("Tutorial instruction key is required.", nameof(instruction));
            if (string.IsNullOrWhiteSpace(voiceCue.Value)) throw new ArgumentException("Tutorial voice cue is required.", nameof(voiceCue));
            if (standardHelpSeconds <= 0 || moreGuidanceHelpSeconds <= 0 || moreGuidanceHelpSeconds > standardHelpSeconds)
                throw new ArgumentOutOfRangeException(nameof(standardHelpSeconds));
            Id = id; Trigger = trigger; AllowedActions = allowedActions; Spotlight = spotlight;
            Instruction = instruction; VoiceCue = voiceCue;
            StandardHelpSeconds = standardHelpSeconds; MoreGuidanceHelpSeconds = moreGuidanceHelpSeconds;
        }
        public string Id { get; }
        public TutorialTrigger Trigger { get; }
        public TutorialAction AllowedActions { get; }
        public TutorialSpotlight Spotlight { get; }
        public LocalizedKey Instruction { get; }
        public AudioCueId VoiceCue { get; }
        public double StandardHelpSeconds { get; }
        public double MoreGuidanceHelpSeconds { get; }
    }

    public sealed class TutorialDefinition
    {
        private readonly TutorialStepDefinition[] _steps;
        public TutorialDefinition(string id, int version, IEnumerable<TutorialStepDefinition> steps)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Tutorial ID is required.", nameof(id));
            if (version <= 0) throw new ArgumentOutOfRangeException(nameof(version));
            _steps = (steps ?? throw new ArgumentNullException(nameof(steps))).ToArray();
            if (_steps.Length == 0 || _steps.Any(value => value == null) ||
                _steps.Select(value => value.Id).Distinct(StringComparer.Ordinal).Count() != _steps.Length)
                throw new ArgumentException("Tutorial steps must be non-empty and uniquely identified.", nameof(steps));
            Id = id; Version = version;
        }
        public string Id { get; }
        public int Version { get; }
        public IReadOnlyList<TutorialStepDefinition> Steps => _steps;
    }

    public sealed class TutorialSnapshot
    {
        public TutorialSnapshot(TutorialProgress progress, TutorialStepDefinition step, GuidanceMode guidanceMode,
            int helpLevel, bool needsGuideChoice)
        { Progress = progress; Step = step; GuidanceMode = guidanceMode; HelpLevel = helpLevel; NeedsGuideChoice = needsGuideChoice; }
        public TutorialProgress Progress { get; }
        public TutorialStepDefinition Step { get; }
        public GuidanceMode GuidanceMode { get; }
        public int HelpLevel { get; }
        public bool NeedsGuideChoice { get; }
        public bool IsActive => Progress.Status == TutorialProgressStatus.InProgress && Step != null;
    }

    public interface ITutorialProgressRepository
    {
        PlayerProgress Current { get; }
        void Commit(PlayerProgress progress);
    }
}
