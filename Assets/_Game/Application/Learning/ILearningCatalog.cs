using System.Collections.Generic;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Learning
{
    public interface ILearningCatalog
    {
        IReadOnlyList<LearningActivityDefinition> Activities { get; }
        IReadOnlyList<LearningConceptDefinition> Concepts { get; }
        bool TryGetActivity(ActivityId id, out LearningActivityDefinition definition);
        bool TryGetConcept(LearningConceptId id, out LearningConceptDefinition definition);
    }
}
