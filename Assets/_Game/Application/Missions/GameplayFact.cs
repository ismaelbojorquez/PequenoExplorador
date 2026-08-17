using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Missions
{
    public enum GameplayFactScope
    {
        World = 1,
        Persistent = 2
    }

    public sealed class GameplayFact
    {
        private readonly TagId[] _tags;

        public GameplayFact(GameplayFactId id, GameplayFactTypeId typeId, string subjectId,
            IEnumerable<TagId> tags, GameplayFactScope scope, int quantity = 1, long sequence = 0)
        {
            if (!id.IsValid) throw new ArgumentException("Gameplay fact ID is invalid.", nameof(id));
            if (!typeId.IsValid) throw new ArgumentException("Gameplay fact type ID is invalid.", nameof(typeId));
            if (string.IsNullOrWhiteSpace(subjectId)) throw new ArgumentException("Gameplay fact subject is required.", nameof(subjectId));
            if (!Enum.IsDefined(typeof(GameplayFactScope), scope)) throw new ArgumentOutOfRangeException(nameof(scope));
            if (quantity < 1) throw new ArgumentOutOfRangeException(nameof(quantity));
            if (sequence < 0) throw new ArgumentOutOfRangeException(nameof(sequence));
            _tags = (tags ?? Array.Empty<TagId>()).ToArray();
            if (_tags.Any(tag => !tag.IsValid) || _tags.Distinct().Count() != _tags.Length)
                throw new ArgumentException("Gameplay fact tags must be valid and unique.", nameof(tags));
            Id = id;
            TypeId = typeId;
            SubjectId = subjectId;
            Scope = scope;
            Quantity = quantity;
            Sequence = sequence;
        }

        public GameplayFactId Id { get; }
        public GameplayFactTypeId TypeId { get; }
        public string SubjectId { get; }
        public IReadOnlyList<TagId> Tags => _tags;
        public GameplayFactScope Scope { get; }
        public int Quantity { get; }
        public long Sequence { get; }
        public GameplayFact WithSequence(long sequence) =>
            new GameplayFact(Id, TypeId, SubjectId, _tags, Scope, Quantity, sequence);
    }

    public static class GameplayFactTypes
    {
        public static readonly GameplayFactTypeId Discovery = GameplayFactTypeId.Parse("gameplay-fact-type.discovery");
        public static readonly GameplayFactTypeId Photograph = GameplayFactTypeId.Parse("gameplay-fact-type.photograph");
        public static readonly GameplayFactTypeId Interaction = GameplayFactTypeId.Parse("gameplay-fact-type.interaction");
        public static readonly GameplayFactTypeId LearningCompleted = GameplayFactTypeId.Parse("gameplay-fact-type.learning-completed");
    }
}
