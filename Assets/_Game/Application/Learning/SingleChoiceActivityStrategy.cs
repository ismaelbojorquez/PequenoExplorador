using System;
using System.Linq;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Learning
{
    public static class LearningActivityTypeIds
    {
        public static readonly LearningActivityTypeId SingleChoice = LearningActivityTypeId.Parse("activity-type.single-choice");
    }

    public sealed class SingleChoiceActivityStrategy : ILearningActivityStrategy
    {
        public LearningActivityTypeId TypeId => LearningActivityTypeIds.SingleChoice;
        public LearningEvaluation Evaluate(LearningActivityDefinition definition, LearningSession session, LearningSubmission submission)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (session == null) throw new ArgumentNullException(nameof(session));
            bool accepted = submission.OptionId.IsValid && definition.Options.Any(item => item.Id.Equals(submission.OptionId));
            return new LearningEvaluation(accepted, accepted && definition.CorrectOptionId.Equals(submission.OptionId));
        }
    }
}
