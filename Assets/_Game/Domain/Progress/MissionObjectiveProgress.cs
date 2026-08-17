using System;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Domain.Progress
{
    public sealed class MissionObjectiveProgress
    {
        public MissionObjectiveProgress(MissionObjectiveId id, int count)
        {
            if (!id.IsValid) throw new ArgumentException("Mission objective ID is invalid.", nameof(id));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            Id = id;
            Count = count;
        }

        public MissionObjectiveId Id { get; }
        public int Count { get; }
        public MissionObjectiveProgress WithCount(int count) => new MissionObjectiveProgress(Id, count);
    }
}
