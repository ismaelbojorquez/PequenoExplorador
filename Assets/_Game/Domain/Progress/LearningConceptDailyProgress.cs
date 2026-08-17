using System;
using System.Globalization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Domain.Progress
{
    public sealed class LearningConceptDailyProgress
    {
        public LearningConceptDailyProgress(LearningConceptId conceptId, string localDate, int seenCount, int completedCount)
        {
            if (!conceptId.IsValid) throw new ArgumentException("Concept ID is invalid.", nameof(conceptId));
            if (!DateTime.TryParseExact(localDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                throw new ArgumentException("Local date must use yyyy-MM-dd.", nameof(localDate));
            if (seenCount < 0 || completedCount < 0 || completedCount > seenCount)
                throw new ArgumentOutOfRangeException(nameof(completedCount), "Concept aggregates require 0 <= completed <= seen.");
            ConceptId = conceptId; LocalDate = localDate; SeenCount = seenCount; CompletedCount = completedCount;
        }
        public LearningConceptId ConceptId { get; }
        public string LocalDate { get; }
        public int SeenCount { get; }
        public int CompletedCount { get; }
        public LearningConceptDailyProgress AddSeen() => new LearningConceptDailyProgress(ConceptId, LocalDate, checked(SeenCount + 1), CompletedCount);
        public LearningConceptDailyProgress AddCompleted() => new LearningConceptDailyProgress(ConceptId, LocalDate, SeenCount, checked(CompletedCount + 1));
    }
}
