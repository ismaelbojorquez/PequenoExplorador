using System;
using System.Linq;
using NUnit.Framework;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Configuration;
using UnityEngine;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class RuntimeConfigurationTests
    {
        [Test]
        public void LocalAssetsMapToExactlyOneDevelopmentAndReleaseProfile()
        {
            AppConfigAsset[] assets = Resources.LoadAll<AppConfigAsset>(AppConfigResourcePaths.Folder);

            bool valid = AppConfigCatalog.TryCreate(assets, out AppConfigCatalog catalog, out var violations);

            Assert.That(valid, Is.True, string.Join("\n", violations));
            Assert.That(assets, Has.Length.EqualTo(2));
            Assert.That(catalog.GetRequired(BuildProfile.Development).Features.Enabled, Is.EquivalentTo(new[]
            {
                FeatureFlag.DevelopmentDiagnostics,
                FeatureFlag.SimulatedSceneFailure,
                FeatureFlag.MockAds,
                FeatureFlag.MockPurchases
            }));
            Assert.That(catalog.GetRequired(BuildProfile.Release).Features.Enabled, Is.Empty);
        }

        [Test]
        public void DefaultsAreTypedValidAndKeepReleaseFailClosed()
        {
            AppConfig development = AppConfigDefaults.Create(BuildProfile.Development);
            AppConfig release = AppConfigDefaults.Create(BuildProfile.Release);

            Assert.That(AppConfigValidator.Validate(development), Is.Empty);
            Assert.That(AppConfigValidator.Validate(release), Is.Empty);
            Assert.That(development.SceneTransitionTimeout, Is.EqualTo(TimeSpan.FromSeconds(20)));
            Assert.That(development.AutosaveDebounce, Is.EqualTo(TimeSpan.FromMilliseconds(500)));
            Assert.That(release.Features.Enabled, Is.Empty);
        }

        [TestCase(FeatureFlag.DevelopmentDiagnostics)]
        [TestCase(FeatureFlag.SimulatedSceneFailure)]
        [TestCase(FeatureFlag.MockAds)]
        [TestCase(FeatureFlag.MockPurchases)]
        [TestCase(FeatureFlag.Cheats)]
        [TestCase(FeatureFlag.ParentalGateBypass)]
        public void ReleaseRejectsEveryUnsafeFeatureFlag(FeatureFlag unsafeFlag)
        {
            IAppConfig defaults = AppConfigDefaults.Create(BuildProfile.Release);
            var invalid = new AppConfig(
                BuildProfile.Release,
                defaults.ProductName,
                defaults.AppVersion,
                defaults.RandomSeed,
                defaults.SceneTransitionTimeout,
                defaults.AutosaveDebounce,
                new FeatureFlags(new[] { unsafeFlag }));

            Assert.That(
                AppConfigValidator.Validate(invalid),
                Has.Some.Contains("Release forbids feature flag"));
        }

        [Test]
        public void MapperRejectsDuplicateFlags()
        {
            AppConfigAsset asset = ScriptableObject.CreateInstance<AppConfigAsset>();
            try
            {
                asset.name = "InvalidDevelopmentConfig";
                asset.ConfigureForEditorAndTests(
                    BuildProfile.Development,
                    AppConfigDefaults.ProductName,
                    AppConfigDefaults.DevelopmentAppVersion,
                    AppConfigDefaults.RandomSeed,
                    AppConfigDefaults.SceneTransitionTimeoutSeconds,
                    AppConfigDefaults.AutosaveDebounceMilliseconds,
                    FeatureFlag.MockAds,
                    FeatureFlag.MockAds);

                bool valid = AppConfigMapper.TryMap(asset, out _, out var violations);

                Assert.That(valid, Is.False);
                Assert.That(violations.Any(value => value.Contains("duplicate feature flag IDs")), Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(asset);
            }
        }

        [Test]
        public void MapperRejectsInvalidRuntimeBudgets()
        {
            AppConfigAsset asset = ScriptableObject.CreateInstance<AppConfigAsset>();
            try
            {
                asset.name = "InvalidBudgetConfig";
                asset.ConfigureForEditorAndTests(
                    BuildProfile.Development,
                    AppConfigDefaults.ProductName,
                    AppConfigDefaults.DevelopmentAppVersion,
                    AppConfigDefaults.RandomSeed,
                    0,
                    -1);

                bool valid = AppConfigMapper.TryMap(asset, out _, out var violations);

                Assert.That(valid, Is.False);
                Assert.That(violations.Any(value => value.Contains("SceneTransitionTimeout")), Is.True);
                Assert.That(violations.Any(value => value.Contains("AutosaveDebounce")), Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(asset);
            }
        }

        [Test]
        public void TemporaryOverrideIsScopedAndRestoresResourceProfile()
        {
            IAppConfig original = BuildProfileConfiguration.Resolve();
            IAppConfig replacement = AppConfigDefaults.Create(BuildProfile.Release);

            using (BuildProfileConfiguration.PushOverrideForTests(replacement))
            {
                Assert.That(BuildProfileConfiguration.Resolve(), Is.SameAs(replacement));
            }

            Assert.That(BuildProfileConfiguration.Resolve().Profile, Is.EqualTo(original.Profile));
        }
    }
}
