using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Missions
{
    public readonly struct MissionObjectiveEvaluation
    {
        public MissionObjectiveEvaluation(bool matched, int count)
        {
            Matched = matched;
            Count = count;
        }
        public bool Matched { get; }
        public int Count { get; }
    }

    public interface IMissionObjectiveStrategy
    {
        MissionObjectiveTypeId TypeId { get; }
        MissionObjectiveEvaluation Evaluate(MissionObjectiveDefinition objective, int currentCount, GameplayFact fact);
    }

    public static class MissionObjectiveTypeIds
    {
        public static readonly MissionObjectiveTypeId DiscoverCount = MissionObjectiveTypeId.Parse("mission-objective-type.discover-count");
        public static readonly MissionObjectiveTypeId PhotographSpecific = MissionObjectiveTypeId.Parse("mission-objective-type.photograph-specific");
        public static readonly MissionObjectiveTypeId InteractTag = MissionObjectiveTypeId.Parse("mission-objective-type.interact-tag");
    }

    public sealed class DiscoverCountObjectiveStrategy : IMissionObjectiveStrategy
    {
        public MissionObjectiveTypeId TypeId => MissionObjectiveTypeIds.DiscoverCount;
        public MissionObjectiveEvaluation Evaluate(MissionObjectiveDefinition objective, int currentCount, GameplayFact fact)
        {
            bool tagMatches = !objective.RequiredTag.IsValid || fact.Tags.Contains(objective.RequiredTag);
            bool matched = fact.TypeId.Equals(GameplayFactTypes.Discovery) && tagMatches;
            return new MissionObjectiveEvaluation(matched, matched ? Math.Min(objective.TargetCount, checked(currentCount + fact.Quantity)) : currentCount);
        }
    }

    public sealed class PhotographSpecificObjectiveStrategy : IMissionObjectiveStrategy
    {
        public MissionObjectiveTypeId TypeId => MissionObjectiveTypeIds.PhotographSpecific;
        public MissionObjectiveEvaluation Evaluate(MissionObjectiveDefinition objective, int currentCount, GameplayFact fact)
        {
            bool matched = fact.TypeId.Equals(GameplayFactTypes.Photograph) &&
                           string.Equals(objective.SubjectId, fact.SubjectId, StringComparison.Ordinal);
            return new MissionObjectiveEvaluation(matched, matched ? Math.Min(objective.TargetCount, checked(currentCount + fact.Quantity)) : currentCount);
        }
    }

    public sealed class InteractTagObjectiveStrategy : IMissionObjectiveStrategy
    {
        public MissionObjectiveTypeId TypeId => MissionObjectiveTypeIds.InteractTag;
        public MissionObjectiveEvaluation Evaluate(MissionObjectiveDefinition objective, int currentCount, GameplayFact fact)
        {
            bool matched = fact.TypeId.Equals(GameplayFactTypes.Interaction) && objective.RequiredTag.IsValid &&
                           fact.Tags.Contains(objective.RequiredTag);
            return new MissionObjectiveEvaluation(matched, matched ? Math.Min(objective.TargetCount, checked(currentCount + fact.Quantity)) : currentCount);
        }
    }

    public sealed class MissionObjectiveStrategyRegistry
    {
        private readonly Dictionary<MissionObjectiveTypeId, IMissionObjectiveStrategy> _strategies;
        public MissionObjectiveStrategyRegistry(IEnumerable<IMissionObjectiveStrategy> strategies)
        {
            IMissionObjectiveStrategy[] values = (strategies ?? throw new ArgumentNullException(nameof(strategies))).ToArray();
            if (values.Any(item => item == null) || values.Select(item => item.TypeId).Distinct().Count() != values.Length)
                throw new ArgumentException("Mission strategies must be non-null and unique.", nameof(strategies));
            _strategies = values.ToDictionary(item => item.TypeId);
        }
        public bool TryGet(MissionObjectiveTypeId id, out IMissionObjectiveStrategy strategy) => _strategies.TryGetValue(id, out strategy);
        public IReadOnlyCollection<MissionObjectiveTypeId> TypeIds => _strategies.Keys;
    }
}
