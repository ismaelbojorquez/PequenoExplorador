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
            LearningOptionDefinition selected = submission.OptionId.IsValid
                ? definition.Options.FirstOrDefault(item => item.Id.Equals(submission.OptionId))
                : null;
            bool accepted = selected != null;
            bool correct = accepted && (definition.CorrectTagId.IsValid
                ? selected.TagId == definition.CorrectTagId
                : definition.CorrectOptionId.Equals(submission.OptionId));
            return new LearningEvaluation(accepted, correct);
        }
    }
}
