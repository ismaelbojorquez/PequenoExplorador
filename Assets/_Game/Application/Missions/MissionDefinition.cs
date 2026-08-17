using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Missions
{
    public sealed class MissionDefinition : IMissionDefinition
    {
        private readonly MissionObjectiveDefinition[] _objectives;
        private readonly MissionId[] _prerequisites;

        public MissionDefinition(MissionId id, LocalizedKey title, LocalizedKey summary,
            LocalizedKey completion, IEnumerable<MissionObjectiveDefinition> objectives,
            IEnumerable<MissionId> prerequisites, RewardId rewardId, EditorialMetadata editorial)
        {
            if (!id.IsValid) throw new ArgumentException("Mission ID is invalid.", nameof(id));
            if (string.IsNullOrWhiteSpace(title.Entry) || string.IsNullOrWhiteSpace(summary.Entry) ||
                string.IsNullOrWhiteSpace(completion.Entry))
                throw new ArgumentException("Mission localized keys are required.");
            _objectives = (objectives ?? throw new ArgumentNullException(nameof(objectives))).ToArray();
            _prerequisites = (prerequisites ?? Array.Empty<MissionId>()).ToArray();
            if (_objectives.Length == 0 || _objectives.Length > 4 || _objectives.Any(item => item == null))
                throw new ArgumentException("A mission requires one to four objectives.", nameof(objectives));
            if (_objectives.Select(item => item.Id).Distinct().Count() != _objectives.Length)
                throw new ArgumentException("Mission objective IDs must be unique.", nameof(objectives));
            if (_prerequisites.Any(item => !item.IsValid) || _prerequisites.Distinct().Count() != _prerequisites.Length || _prerequisites.Contains(id))
                throw new ArgumentException("Mission prerequisites are invalid.", nameof(prerequisites));
            Id = id;
            Title = title;
            Summary = summary;
            Completion = completion;
            RewardId = rewardId;
            Editorial = editorial ?? throw new ArgumentNullException(nameof(editorial));
        }

        public MissionId Id { get; }
        public LocalizedKey Title { get; }
        public LocalizedKey Summary { get; }
        public LocalizedKey Completion { get; }
        public IReadOnlyList<MissionObjectiveDefinition> Objectives => _objectives;
        public IReadOnlyList<MissionId> Prerequisites => _prerequisites;
        public RewardId RewardId { get; }
        public bool HasReward => RewardId.IsValid;
        public EditorialMetadata Editorial { get; }
        public bool AutoClaimReward => true;
        public bool Expires => false;
    }
}
