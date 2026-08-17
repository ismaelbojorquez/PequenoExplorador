using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Learning
{
    public sealed class LearningCatalog : ILearningCatalog
    {
        private readonly Dictionary<ActivityId, LearningActivityDefinition> _activities;
        private readonly Dictionary<LearningConceptId, LearningConceptDefinition> _concepts;
        private readonly LearningActivityDefinition[] _orderedActivities;
        private readonly LearningConceptDefinition[] _orderedConcepts;
        public LearningCatalog(IEnumerable<LearningActivityDefinition> activities, IEnumerable<LearningConceptDefinition> concepts)
        {
            _orderedActivities = (activities ?? throw new ArgumentNullException(nameof(activities))).OrderBy(item => item.Id.Value, StringComparer.Ordinal).ToArray();
            _orderedConcepts = (concepts ?? throw new ArgumentNullException(nameof(concepts))).OrderBy(item => item.Id.Value, StringComparer.Ordinal).ToArray();
            if (_orderedActivities.Any(item => item == null) || _orderedActivities.Select(item => item.Id).Distinct().Count() != _orderedActivities.Length) throw new ArgumentException("Activity IDs must be unique.");
            if (_orderedConcepts.Any(item => item == null) || _orderedConcepts.Select(item => item.Id).Distinct().Count() != _orderedConcepts.Length) throw new ArgumentException("Concept IDs must be unique.");
            _activities = _orderedActivities.ToDictionary(item => item.Id);
            _concepts = _orderedConcepts.ToDictionary(item => item.Id);
            if (_orderedActivities.SelectMany(item => item.Concepts).Any(id => !_concepts.ContainsKey(id))) throw new ArgumentException("Activity references a missing concept.");
        }
        public static LearningCatalog Empty { get; } = new LearningCatalog(Array.Empty<LearningActivityDefinition>(), Array.Empty<LearningConceptDefinition>());
        public IReadOnlyList<LearningActivityDefinition> Activities => _orderedActivities;
        public IReadOnlyList<LearningConceptDefinition> Concepts => _orderedConcepts;
        public bool TryGetActivity(ActivityId id, out LearningActivityDefinition definition) => _activities.TryGetValue(id, out definition);
        public bool TryGetConcept(LearningConceptId id, out LearningConceptDefinition definition) => _concepts.TryGetValue(id, out definition);
    }
}
