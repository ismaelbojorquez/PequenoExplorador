using System;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.Logging;
using PequenoExplorador.Application.Messaging;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Infrastructure.Ads;
using PequenoExplorador.Infrastructure.Analytics;
using PequenoExplorador.Infrastructure.Logging;
using PequenoExplorador.Infrastructure.Messaging;
using PequenoExplorador.Infrastructure.Purchases;
using PequenoExplorador.Infrastructure.Random;
using PequenoExplorador.Infrastructure.Save;
using PequenoExplorador.Infrastructure.Time;
using PequenoExplorador.Infrastructure.SceneFlow;
using ApplicationContext = PequenoExplorador.Application.AppContext;

namespace PequenoExplorador.Bootstrap
{
    internal sealed class ServiceRegistry : IDisposable
    {
        private readonly IDisposable _fileStoreLifetime;

        public ServiceRegistry(IAppConfig configuration, IFileStore fileStore = null)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var configViolations = AppConfigValidator.Validate(configuration);
            if (configViolations.Count > 0)
            {
                throw new ArgumentException(
                    "Runtime AppConfig is invalid:\n" + string.Join("\n", configViolations),
                    nameof(configuration));
            }

            IAppLogger logger = new UnityStructuredLogger();
            IClock clock = new SystemClock();
            IRandomSource random = new SeededRandomSource(configuration.RandomSeed);
            IMessageBus messages = new InMemoryMessageBus();
            IFileStore resolvedFileStore = fileStore ?? new LocalFileStore(
                System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Save"));
            _fileStoreLifetime = resolvedFileStore as IDisposable;
            ISaveService save = new LocalSaveService(
                resolvedFileStore,
                configuration.AppVersion,
                logger,
                new ISaveMigration[] { new LegacyV0ToV1Migration() });
            IAnalyticsService analytics = new NullAnalyticsService();
            IAdsService ads;
            IPurchaseService purchases;
#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
            SceneFailure = new DevelopmentSceneLoadFailure();
            var sceneLoader = new AddressableSceneContentLoader(SceneFailure);
#else
            var sceneLoader = new AddressableSceneContentLoader();
#endif
            ISceneFlowService sceneFlow = new SceneFlowService(
                sceneLoader,
                logger,
                configuration.SceneTransitionTimeout);

#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
            ads = configuration.Features.IsEnabled(FeatureFlag.MockAds)
                ? new MockAdsService()
                : new NoAdsService();
            purchases = configuration.Features.IsEnabled(FeatureFlag.MockPurchases)
                ? new MockPurchaseService()
                : new UnavailablePurchaseService();
#else
            if (configuration.Profile == BuildProfile.Development)
            {
                throw new InvalidOperationException("Development service profile is not compiled into Release players.");
            }

            ads = new NoAdsService();
            purchases = new UnavailablePurchaseService();
#endif

            Context = new ApplicationContext(
                configuration,
                clock,
                random,
                logger,
                messages,
                save,
                analytics,
                ads,
                purchases,
                sceneFlow);
            Host = new ApplicationHost(
                new IApplicationService[]
                {
                    messages,
                    save,
                    analytics,
                    ads,
                    purchases
                },
                logger);
            SaveCoordinator = new AutosaveCoordinator(save, logger, configuration.AutosaveDebounce);
        }

        public ApplicationContext Context { get; }

        public ApplicationHost Host { get; }

        public AutosaveCoordinator SaveCoordinator { get; }

#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
        public DevelopmentSceneLoadFailure SceneFailure { get; }
#endif

        public void Dispose()
        {
            SaveCoordinator.Dispose();
            Host.Dispose();
            _fileStoreLifetime?.Dispose();
        }
    }
}
