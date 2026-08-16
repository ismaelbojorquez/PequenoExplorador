using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PequenoExplorador.Application.Configuration
{
    public static class AppConfigValidator
    {
        public static IReadOnlyList<string> Validate(IAppConfig config)
        {
            var violations = new List<string>();
            if (config == null)
            {
                violations.Add("CONFIG001 AppConfig is required.");
                return new ReadOnlyCollection<string>(violations);
            }

            if (config.Profile != BuildProfile.Development && config.Profile != BuildProfile.Release)
            {
                violations.Add("CONFIG002 BuildProfile must be Development or Release.");
            }

            if (string.IsNullOrWhiteSpace(config.ProductName))
            {
                violations.Add("CONFIG003 ProductName is required.");
            }

            if (string.IsNullOrWhiteSpace(config.AppVersion))
            {
                violations.Add("CONFIG004 AppVersion is required.");
            }

            if (config.SceneTransitionTimeout < TimeSpan.FromSeconds(1) ||
                config.SceneTransitionTimeout > TimeSpan.FromMinutes(2))
            {
                violations.Add("CONFIG005 SceneTransitionTimeout must be between 1 and 120 seconds.");
            }

            if (config.AutosaveDebounce < TimeSpan.Zero || config.AutosaveDebounce > TimeSpan.FromSeconds(10))
            {
                violations.Add("CONFIG006 AutosaveDebounce must be between 0 and 10000 milliseconds.");
            }

            if (config.Features == null)
            {
                violations.Add("CONFIG007 Feature flags are required.");
            }
            else if (config.Profile == BuildProfile.Release)
            {
                foreach (FeatureFlag flag in config.Features.Enabled)
                {
                    violations.Add($"CONFIG008 Release forbids feature flag {flag}.");
                }
            }

            return new ReadOnlyCollection<string>(violations);
        }
    }
}
