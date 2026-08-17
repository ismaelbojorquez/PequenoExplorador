using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Learning
{
    public sealed class LearningActivityStrategyRegistry
    {
        private readonly Dictionary<LearningActivityTypeId, ILearningActivityStrategy> _strategies;
        public LearningActivityStrategyRegistry(IEnumerable<ILearningActivityStrategy> strategies)
        {
            ILearningActivityStrategy[] values = (strategies ?? throw new ArgumentNullException(nameof(strategies))).ToArray();
            if (values.Any(item => item == null || !item.TypeId.IsValid) || values.Select(item => item.TypeId).Distinct().Count() != values.Length)
                throw new ArgumentException("Learning strategies require unique valid type IDs.", nameof(strategies));
            _strategies = values.ToDictionary(item => item.TypeId);
        }
        public IReadOnlyCollection<LearningActivityTypeId> TypeIds => _strategies.Keys;
        public bool TryGet(LearningActivityTypeId typeId, out ILearningActivityStrategy strategy) => _strategies.TryGetValue(typeId, out strategy);
    }
}
