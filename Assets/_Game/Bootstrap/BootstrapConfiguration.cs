using PequenoExplorador.Application;

namespace PequenoExplorador.Bootstrap
{
    public sealed class BootstrapConfiguration
    {
        public BootstrapConfiguration(ApplicationEnvironment environment, int randomSeed)
        {
            Environment = environment;
            RandomSeed = randomSeed;
        }

        public ApplicationEnvironment Environment { get; }
        public int RandomSeed { get; }
        public bool DevelopmentDiagnosticsEnabled => Environment == ApplicationEnvironment.Development;
    }
}
