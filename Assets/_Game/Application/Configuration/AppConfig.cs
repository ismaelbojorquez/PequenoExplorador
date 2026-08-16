using System;

namespace PequenoExplorador.Application.Configuration
{
    public sealed class AppConfig : IAppConfig
    {
        public AppConfig(
            BuildProfile profile,
            string productName,
            string appVersion,
            int randomSeed,
            TimeSpan sceneTransitionTimeout,
            TimeSpan autosaveDebounce,
            IFeatureFlags features)
        {
            Profile = profile;
            ProductName = productName;
            AppVersion = appVersion;
            RandomSeed = randomSeed;
            SceneTransitionTimeout = sceneTransitionTimeout;
            AutosaveDebounce = autosaveDebounce;
            Features = features ?? throw new ArgumentNullException(nameof(features));
        }

        public BuildProfile Profile { get; }
        public string ProductName { get; }
        public string AppVersion { get; }
        public int RandomSeed { get; }
        public TimeSpan SceneTransitionTimeout { get; }
        public TimeSpan AutosaveDebounce { get; }
        public IFeatureFlags Features { get; }
    }
}
