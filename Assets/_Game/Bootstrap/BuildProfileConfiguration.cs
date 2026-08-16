using System;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Content.Configuration;
using UnityEngine;

namespace PequenoExplorador.Bootstrap
{
    internal static class BuildProfileConfiguration
    {
#if UNITY_EDITOR
        private static IAppConfig _testOverride;
#endif

        public static IAppConfig Resolve()
        {
#if UNITY_EDITOR
            if (_testOverride != null)
            {
                return _testOverride;
            }
#endif

            BuildProfile selected;
#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
            selected = BuildProfile.Development;
#else
            selected = BuildProfile.Release;
#endif

            AppConfigAsset[] assets = Resources.LoadAll<AppConfigAsset>(AppConfigResourcePaths.Folder);
            if (!AppConfigCatalog.TryCreate(assets, out AppConfigCatalog catalog, out var violations))
            {
                throw new InvalidOperationException(
                    "Runtime configuration is invalid:\n" + string.Join("\n", violations));
            }

            return catalog.GetRequired(selected);
        }

#if UNITY_EDITOR
        internal static IDisposable PushOverrideForTests(IAppConfig config)
        {
            if (_testOverride != null)
            {
                throw new InvalidOperationException("A temporary AppConfig test override is already active.");
            }

            var violations = AppConfigValidator.Validate(config);
            if (violations.Count > 0)
            {
                throw new ArgumentException(
                    "Test AppConfig override is invalid:\n" + string.Join("\n", violations),
                    nameof(config));
            }

            _testOverride = config;
            return new TestOverrideScope();
        }

        private sealed class TestOverrideScope : IDisposable
        {
            private bool _disposed;

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _testOverride = null;
                _disposed = true;
            }
        }
#endif
    }
}
