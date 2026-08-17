using PequenoExplorador.Domain.Economy;

namespace PequenoExplorador.Application.Economy
{
    public enum GrantRewardOutcome { Granted, AlreadyProcessed, MissingDefinition, SourceMismatch, Overflow, ReadOnly }
    public enum SpendStarsOutcome { Spent, AlreadyProcessed, Insufficient, ReadOnly }

    public readonly struct GrantRewardResult
    {
        public GrantRewardResult(GrantRewardOutcome outcome, ExplorerStars balance, ExplorerStars amount)
        { Outcome = outcome; Balance = balance; Amount = amount; }
        public GrantRewardOutcome Outcome { get; }
        public ExplorerStars Balance { get; }
        public ExplorerStars Amount { get; }
        public bool Granted => Outcome == GrantRewardOutcome.Granted;
    }

    public readonly struct SpendStarsResult
    {
        public SpendStarsResult(SpendStarsOutcome outcome, ExplorerStars balance, ExplorerStars amount)
        { Outcome = outcome; Balance = balance; Amount = amount; }
        public SpendStarsOutcome Outcome { get; }
        public ExplorerStars Balance { get; }
        public ExplorerStars Amount { get; }
        public bool Spent => Outcome == SpendStarsOutcome.Spent;
    }
}
