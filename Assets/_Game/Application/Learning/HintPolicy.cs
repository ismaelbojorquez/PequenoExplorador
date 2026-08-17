using System;

namespace PequenoExplorador.Application.Learning
{
    public sealed class HintPolicy
    {
        public HintPolicy(int firstAutomaticHintAttempt, int maximumLevel)
        {
            if (firstAutomaticHintAttempt < 1) throw new ArgumentOutOfRangeException(nameof(firstAutomaticHintAttempt));
            if (maximumLevel < 1) throw new ArgumentOutOfRangeException(nameof(maximumLevel));
            FirstAutomaticHintAttempt = firstAutomaticHintAttempt;
            MaximumLevel = maximumLevel;
        }
        public int FirstAutomaticHintAttempt { get; }
        public int MaximumLevel { get; }
        public int ResolveLevel(int attempts, int currentLevel) => attempts < FirstAutomaticHintAttempt
            ? currentLevel
            : Math.Min(MaximumLevel, Math.Max(currentLevel + 1, attempts - FirstAutomaticHintAttempt + 1));
    }
}
