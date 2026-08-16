namespace PequenoExplorador.Application.Discovery
{
    public enum DiscoverOutcome
    {
        First = 0,
        Repeated = 1,
        AlreadyProcessed = 2,
        MissingContent = 3,
        UnapprovedContent = 4,
        SaveReadOnly = 5
    }
}
