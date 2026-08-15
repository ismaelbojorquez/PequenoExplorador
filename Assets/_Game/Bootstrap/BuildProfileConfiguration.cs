using PequenoExplorador.Application;

namespace PequenoExplorador.Bootstrap
{
    internal static class BuildProfileConfiguration
    {
        private const int DefaultRandomSeed = 20260814;

        public static BootstrapConfiguration Resolve()
        {
#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
            return new BootstrapConfiguration(ApplicationEnvironment.Development, DefaultRandomSeed);
#else
            return new BootstrapConfiguration(ApplicationEnvironment.Release, DefaultRandomSeed);
#endif
        }
    }
}
