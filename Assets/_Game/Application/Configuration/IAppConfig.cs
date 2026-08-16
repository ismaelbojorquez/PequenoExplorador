using System;

namespace PequenoExplorador.Application.Configuration
{
    public interface IAppConfig
    {
        BuildProfile Profile { get; }
        string ProductName { get; }
        string AppVersion { get; }
        int RandomSeed { get; }
        TimeSpan SceneTransitionTimeout { get; }
        TimeSpan AutosaveDebounce { get; }
        IFeatureFlags Features { get; }
    }
}
