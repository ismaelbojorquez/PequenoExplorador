using System;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Missions
{
    public sealed class MissionObjectiveDefinition
    {
        public MissionObjectiveDefinition(MissionObjectiveId id, MissionObjectiveTypeId typeId,
            LocalizedKey label, int targetCount, string subjectId, TagId requiredTag)
        {
            if (!id.IsValid || !typeId.IsValid) throw new ArgumentException("Mission objective IDs are invalid.");
            if (string.IsNullOrWhiteSpace(label.Table) || string.IsNullOrWhiteSpace(label.Entry))
                throw new ArgumentException("Mission objective label is required.", nameof(label));
            if (targetCount < 1) throw new ArgumentOutOfRangeException(nameof(targetCount));
            Id = id;
            TypeId = typeId;
            Label = label;
            TargetCount = targetCount;
            SubjectId = subjectId ?? string.Empty;
            RequiredTag = requiredTag;
        }

        public MissionObjectiveId Id { get; }
        public MissionObjectiveTypeId TypeId { get; }
        public LocalizedKey Label { get; }
        public int TargetCount { get; }
        public string SubjectId { get; }
        public TagId RequiredTag { get; }
    }
}
