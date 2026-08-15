using System;
using PequenoExplorador.Application.Logging;
using PequenoExplorador.Application.Messaging;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Application.Services;

namespace PequenoExplorador.Application
{
    public sealed class AppContext
    {
        public AppContext(
            ApplicationEnvironment environment,
            IClock clock,
            IRandomSource random,
            IAppLogger logger,
            IMessageBus messages,
            IAnalyticsService analytics,
            IAdsService ads,
            IPurchaseService purchases,
            ISceneFlowService sceneFlow)
        {
            Environment = environment;
            Clock = clock ?? throw new ArgumentNullException(nameof(clock));
            Random = random ?? throw new ArgumentNullException(nameof(random));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Messages = messages ?? throw new ArgumentNullException(nameof(messages));
            Analytics = analytics ?? throw new ArgumentNullException(nameof(analytics));
            Ads = ads ?? throw new ArgumentNullException(nameof(ads));
            Purchases = purchases ?? throw new ArgumentNullException(nameof(purchases));
            SceneFlow = sceneFlow ?? throw new ArgumentNullException(nameof(sceneFlow));
        }

        public ApplicationEnvironment Environment { get; }
        public IClock Clock { get; }
        public IRandomSource Random { get; }
        public IAppLogger Logger { get; }
        public IMessageBus Messages { get; }
        public IAnalyticsService Analytics { get; }
        public IAdsService Ads { get; }
        public IPurchaseService Purchases { get; }
        public ISceneFlowService SceneFlow { get; }
    }
}
