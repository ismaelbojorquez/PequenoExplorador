using System;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.Logging;
using PequenoExplorador.Application.Messaging;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Infrastructure.Ads;
using PequenoExplorador.Infrastructure.Analytics;
using PequenoExplorador.Infrastructure.Logging;
using PequenoExplorador.Infrastructure.Messaging;
using PequenoExplorador.Infrastructure.Purchases;
using PequenoExplorador.Infrastructure.Random;
using PequenoExplorador.Infrastructure.Time;
using PequenoExplorador.Infrastructure.SceneFlow;
using ApplicationContext = PequenoExplorador.Application.AppContext;

namespace PequenoExplorador.Bootstrap
{
    internal sealed class ServiceRegistry : IDisposable
    {
        public ServiceRegistry(BootstrapConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            IAppLogger logger = new UnityStructuredLogger();
            IClock clock = new SystemClock();
            IRandomSource random = new SeededRandomSource(configuration.RandomSeed);
            IMessageBus messages = new InMemoryMessageBus();
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
                TimeSpan.FromSeconds(20));

#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
            if (configuration.Environment == ApplicationEnvironment.Development)
            {
                ads = new MockAdsService();
                purchases = new MockPurchaseService();
            }
            else
            {
                ads = new NoAdsService();
                purchases = new UnavailablePurchaseService();
            }
#else
            if (configuration.Environment == ApplicationEnvironment.Development)
            {
                throw new InvalidOperationException("Development service profile is not compiled into Release players.");
            }

            ads = new NoAdsService();
            purchases = new UnavailablePurchaseService();
#endif

            Context = new ApplicationContext(
                configuration.Environment,
                clock,
                random,
                logger,
                messages,
                analytics,
                ads,
                purchases,
                sceneFlow);
            Host = new ApplicationHost(
                new IApplicationService[]
                {
                    messages,
                    analytics,
                    ads,
                    purchases
                },
                logger);
        }

        public ApplicationContext Context { get; }

        public ApplicationHost Host { get; }

#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
        public DevelopmentSceneLoadFailure SceneFailure { get; }
#endif

        public void Dispose()
        {
            Host.Dispose();
        }
    }
}
