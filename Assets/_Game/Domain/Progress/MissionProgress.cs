using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Domain.Progress
{
    public enum MissionProgressStatus
    {
        Active = 1,
        Completed = 2
    }

    public sealed class MissionProgress
    {
        private readonly MissionObjectiveProgress[] _objectives;

        public MissionProgress(MissionId id, MissionProgressStatus status, long activationSequence,
            IEnumerable<MissionObjectiveProgress> objectives)
        {
            if (!id.IsValid) throw new ArgumentException("Mission ID is invalid.", nameof(id));
            if (!Enum.IsDefined(typeof(MissionProgressStatus), status)) throw new ArgumentOutOfRangeException(nameof(status));
            if (activationSequence < 0) throw new ArgumentOutOfRangeException(nameof(activationSequence));
            _objectives = (objectives ?? throw new ArgumentNullException(nameof(objectives))).ToArray();
            if (_objectives.Any(item => item == null)) throw new ArgumentException("Mission objectives cannot contain null.", nameof(objectives));
            if (_objectives.Select(item => item.Id).Distinct().Count() != _objectives.Length)
                throw new ArgumentException("Mission objective progress IDs must be unique.", nameof(objectives));
            Id = id;
            Status = status;
            ActivationSequence = activationSequence;
        }

        public MissionId Id { get; }
        public MissionProgressStatus Status { get; }
        public long ActivationSequence { get; }
        public IReadOnlyList<MissionObjectiveProgress> Objectives => _objectives;
        public bool IsCompleted => Status == MissionProgressStatus.Completed;
        public MissionProgress With(IEnumerable<MissionObjectiveProgress> objectives, bool completed) =>
            new MissionProgress(Id, completed ? MissionProgressStatus.Completed : MissionProgressStatus.Active,
                ActivationSequence, objectives);
    }
}
