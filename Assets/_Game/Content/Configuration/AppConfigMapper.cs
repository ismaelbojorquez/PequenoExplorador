using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PequenoExplorador.Application.Configuration;

namespace PequenoExplorador.Content.Configuration
{
    public static class AppConfigMapper
    {
        public static bool TryMap(
            AppConfigAsset source,
            out AppConfig config,
            out IReadOnlyList<string> violations)
        {
            var errors = new List<string>();
            config = null;
            if (source == null)
            {
                errors.Add("CONFIG101 AppConfigAsset is required.");
                violations = new ReadOnlyCollection<string>(errors);
                return false;
            }

            FeatureFlag[] enabled = source.EnabledFeatures.ToArray();
            if (enabled.Distinct().Count() != enabled.Length)
            {
                errors.Add($"CONFIG102 {source.name} contains duplicate feature flag IDs.");
            }

            FeatureFlags flags = null;
            if (errors.Count == 0)
            {
                try
                {
                    flags = new FeatureFlags(enabled);
                }
                catch (ArgumentException exception)
                {
                    errors.Add($"CONFIG103 {source.name}: {exception.Message}");
                }
            }

            if (errors.Count == 0)
            {
                config = new AppConfig(
                    source.Profile,
                    source.ProductName,
                    source.AppVersion,
                    source.RandomSeed,
                    TimeSpan.FromSeconds(source.SceneTransitionTimeoutSeconds),
                    TimeSpan.FromMilliseconds(source.AutosaveDebounceMilliseconds),
                    flags);
                errors.AddRange(AppConfigValidator.Validate(config));
            }

            violations = new ReadOnlyCollection<string>(errors);
            if (errors.Count > 0)
            {
                config = null;
                return false;
            }

            return true;
        }
    }
}
