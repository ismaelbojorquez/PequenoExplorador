using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Learning
{
    public readonly struct LearningSubmission
    {
        public LearningSubmission(LearningOptionId optionId) => OptionId = optionId;
        public LearningOptionId OptionId { get; }
    }

    public readonly struct LearningEvaluation
    {
        public LearningEvaluation(bool accepted, bool correct) { Accepted = accepted; Correct = correct; }
        public bool Accepted { get; }
        public bool Correct { get; }
    }

    public interface ILearningActivityStrategy
    {
        LearningActivityTypeId TypeId { get; }
        LearningEvaluation Evaluate(LearningActivityDefinition definition, LearningSession session, LearningSubmission submission);
    }
}
