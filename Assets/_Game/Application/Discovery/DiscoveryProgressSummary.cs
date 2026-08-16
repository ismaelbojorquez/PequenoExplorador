namespace PequenoExplorador.Application.Discovery
{
    public readonly struct DiscoveryProgressSummary
    {
        public DiscoveryProgressSummary(int discovered, int total)
        {
            Discovered = discovered;
            Total = total;
        }

        public int Discovered { get; }
        public int Total { get; }
        public float Ratio => Total == 0 ? 0f : (float)Discovered / Total;
    }
}
