using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Infrastructure.Ads;
using PequenoExplorador.Infrastructure.Analytics;
using PequenoExplorador.Infrastructure.Purchases;
using PequenoExplorador.Tests.EditMode.Fixtures;
using UnityEditor;
using UnityEditor.Build;
using ApplicationContext = PequenoExplorador.Application.AppContext;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class ServiceProfileTests
    {
        [Test]
        public async Task DevelopmentUsesOnlyLocalNullAndMockServices()
        {
            using var registry = new ServiceRegistry(
                new BootstrapConfiguration(ApplicationEnvironment.Development, 42));

            await registry.Host.InitializeAsync(CancellationToken.None);
            ServiceOperationResult adResult = await registry.Context.Ads.TryShowAsync(
                "TestPlacement",
                CancellationToken.None);
            ServiceOperationResult purchaseResult = await registry.Context.Purchases.TryPurchaseAsync(
                "TestProduct",
                CancellationToken.None);

            Assert.That(registry.Context.Analytics, Is.TypeOf<NullAnalyticsService>());
            Assert.That(registry.Context.Ads, Is.TypeOf<MockAdsService>());
            Assert.That(registry.Context.Purchases, Is.TypeOf<MockPurchaseService>());
            Assert.That(adResult.Status, Is.EqualTo(ServiceOperationStatus.Simulated));
            Assert.That(purchaseResult.Status, Is.EqualTo(ServiceOperationStatus.Simulated));
            Assert.That(((MockAdsService)registry.Context.Ads).SimulatedShowCount, Is.EqualTo(1));
            Assert.That(((MockPurchaseService)registry.Context.Purchases).SimulatedPurchaseCount, Is.EqualTo(1));
        }

        [Test]
        public async Task ReleaseIsFailClosedAndContainsNoSimulatorSelection()
        {
            var configuration = new BootstrapConfiguration(ApplicationEnvironment.Release, 42);
            using var registry = new ServiceRegistry(configuration);

            await registry.Host.InitializeAsync(CancellationToken.None);
            ServiceOperationResult adResult = await registry.Context.Ads.TryShowAsync(
                "IgnoredPlacement",
                CancellationToken.None);
            ServiceOperationResult purchaseResult = await registry.Context.Purchases.TryPurchaseAsync(
                "IgnoredProduct",
                CancellationToken.None);

            Assert.That(configuration.DevelopmentDiagnosticsEnabled, Is.False);
            Assert.That(registry.Context.Analytics, Is.TypeOf<NullAnalyticsService>());
            Assert.That(registry.Context.Ads, Is.TypeOf<NoAdsService>());
            Assert.That(registry.Context.Purchases, Is.TypeOf<UnavailablePurchaseService>());
            Assert.That(adResult.Status, Is.EqualTo(ServiceOperationStatus.Disabled));
            Assert.That(purchaseResult.Status, Is.EqualTo(ServiceOperationStatus.Unavailable));
        }

        [Test]
        public void ClockAndRandomAreExplicitlyInjectableAndDeterministic()
        {
            DateTimeOffset expected = new DateTimeOffset(2026, 8, 14, 12, 0, 0, TimeSpan.Zero);
            var manualClock = new ManualClock(expected);
            using var first = new ServiceRegistry(
                new BootstrapConfiguration(ApplicationEnvironment.Release, 1234));
            using var second = new ServiceRegistry(
                new BootstrapConfiguration(ApplicationEnvironment.Release, 1234));
            var context = new ApplicationContext(
                ApplicationEnvironment.Release,
                manualClock,
                first.Context.Random,
                first.Context.Logger,
                first.Context.Messages,
                first.Context.Analytics,
                first.Context.Ads,
                first.Context.Purchases);

            Assert.That(context.Clock.UtcNow, Is.EqualTo(expected));
            Assert.That(first.Context.Random.Next(10000), Is.EqualTo(second.Context.Random.Next(10000)));
        }

        [Test]
        public void RegistryDeclaresTheOnlyAuthorizedInitializationOrder()
        {
            using var registry = new ServiceRegistry(
                new BootstrapConfiguration(ApplicationEnvironment.Release, 1));

            Assert.That(registry.Host.ServiceOrder, Is.EqualTo(new[]
            {
                "MessageBus",
                "Analytics",
                "Ads",
                "Purchases"
            }));
        }

        [Test]
        public void DevelopmentServiceSymbolIsNotPersistedInAndroidPlayerSettings()
        {
            string symbols = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android);

            Assert.That(symbols.Split(';'), Does.Not.Contain("PE_DEVELOPMENT_SERVICES"));
        }
    }
}
