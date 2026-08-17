using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class LearningSessionV9Dto
    {
        [SerializeField] private string activityId;
        [SerializeField] private int status;
        [SerializeField] private int attempts;
        [SerializeField] private int hintLevel;
        public string ActivityId => activityId; public int Status => status; public int Attempts => attempts; public int HintLevel => hintLevel;
        public static LearningSessionV9Dto Create(string id, int state, int attemptCount, int hint) => new LearningSessionV9Dto { activityId = id, status = state, attempts = attemptCount, hintLevel = hint };
    }

    [Serializable]
    internal sealed class LearningConceptDailyV9Dto
    {
        [SerializeField] private string conceptId;
        [SerializeField] private string localDate;
        [SerializeField] private int seenCount;
        [SerializeField] private int completedCount;
        public string ConceptId => conceptId; public string LocalDate => localDate; public int SeenCount => seenCount; public int CompletedCount => completedCount;
        public static LearningConceptDailyV9Dto Create(string id, string date, int seen, int completed) => new LearningConceptDailyV9Dto { conceptId = id, localDate = date, seenCount = seen, completedCount = completed };
    }
}
