using System;

namespace PequenoExplorador.Application.Configuration
{
    public static class AppConfigDefaults
    {
        public const string ProductName = "Pequeño Explorador: Aprende Jugando";
        public const string DevelopmentAppVersion = "0.1.0-dev";
        public const string ReleaseAppVersion = "0.1.0";
        public const int RandomSeed = 20260814;
        public const int SceneTransitionTimeoutSeconds = 20;
        public const int AutosaveDebounceMilliseconds = 500;

        public static AppConfig Create(BuildProfile profile)
        {
            switch (profile)
            {
                case BuildProfile.Development:
                    return new AppConfig(
                        profile,
                        ProductName,
                        DevelopmentAppVersion,
                        RandomSeed,
                        TimeSpan.FromSeconds(SceneTransitionTimeoutSeconds),
                        TimeSpan.FromMilliseconds(AutosaveDebounceMilliseconds),
                        new FeatureFlags(new[]
                        {
                            FeatureFlag.DevelopmentDiagnostics,
                            FeatureFlag.SimulatedSceneFailure,
                            FeatureFlag.MockAds,
                            FeatureFlag.MockPurchases
                        }));
                case BuildProfile.Release:
                    return new AppConfig(
                        profile,
                        ProductName,
                        ReleaseAppVersion,
                        RandomSeed,
                        TimeSpan.FromSeconds(SceneTransitionTimeoutSeconds),
                        TimeSpan.FromMilliseconds(AutosaveDebounceMilliseconds),
                        FeatureFlags.None);
                default:
                    throw new ArgumentOutOfRangeException(nameof(profile), profile, "A concrete build profile is required.");
            }
        }
    }
}
