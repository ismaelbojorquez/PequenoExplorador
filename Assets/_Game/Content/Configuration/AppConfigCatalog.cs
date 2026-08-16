using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PequenoExplorador.Application.Configuration;

namespace PequenoExplorador.Content.Configuration
{
    public sealed class AppConfigCatalog
    {
        private readonly IReadOnlyDictionary<BuildProfile, IAppConfig> _profiles;

        private AppConfigCatalog(IDictionary<BuildProfile, IAppConfig> profiles)
        {
            _profiles = new ReadOnlyDictionary<BuildProfile, IAppConfig>(profiles);
        }

        public static bool TryCreate(
            IEnumerable<AppConfigAsset> assets,
            out AppConfigCatalog catalog,
            out IReadOnlyList<string> violations)
        {
            var errors = new List<string>();
            var profiles = new Dictionary<BuildProfile, IAppConfig>();
            AppConfigAsset[] sources = (assets ?? Array.Empty<AppConfigAsset>())
                .Where(asset => asset != null)
                .OrderBy(asset => asset.name, StringComparer.Ordinal)
                .ToArray();

            foreach (AppConfigAsset source in sources)
            {
                if (!AppConfigMapper.TryMap(source, out AppConfig config, out IReadOnlyList<string> mappingErrors))
                {
                    errors.AddRange(mappingErrors);
                    continue;
                }

                if (profiles.ContainsKey(config.Profile))
                {
                    errors.Add($"CONFIG104 duplicate profile {config.Profile}; exactly one asset is required.");
                    continue;
                }

                profiles.Add(config.Profile, config);
            }

            foreach (BuildProfile required in new[] { BuildProfile.Development, BuildProfile.Release })
            {
                if (!profiles.ContainsKey(required))
                {
                    errors.Add($"CONFIG105 missing required profile {required}.");
                }
            }

            if (sources.Length != 2)
            {
                errors.Add($"CONFIG106 expected exactly 2 AppConfig assets but found {sources.Length}.");
            }

            violations = new ReadOnlyCollection<string>(errors);
            catalog = errors.Count == 0 ? new AppConfigCatalog(profiles) : null;
            return errors.Count == 0;
        }

        public IAppConfig GetRequired(BuildProfile profile)
        {
            if (!_profiles.TryGetValue(profile, out IAppConfig config))
            {
                throw new InvalidOperationException($"Validated AppConfig profile {profile} is unavailable.");
            }

            return config;
        }
    }
}
