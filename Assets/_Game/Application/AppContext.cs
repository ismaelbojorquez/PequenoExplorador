using System;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Accessibility;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Application.Logging;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Messaging;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Application.Worlds;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Application.Input;

namespace PequenoExplorador.Application
{
    public sealed class AppContext
    {
        public AppContext(
            IAppConfig configuration,
            IClock clock,
            IRandomSource random,
            IAppLogger logger,
            IMessageBus messages,
            ISaveService save,
            ILocalizationService localization,
            IAudioService audio,
            IContentCatalog content,
            IWorldCatalog worlds,
            IWorldSession worldSession,
            IInputService input,
            ISafeAreaService safeArea,
            IHapticsService haptics,
            IAnalyticsService analytics,
            IAdsService ads,
            IPurchaseService purchases,
            ISceneFlowService sceneFlow)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Clock = clock ?? throw new ArgumentNullException(nameof(clock));
            Random = random ?? throw new ArgumentNullException(nameof(random));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Messages = messages ?? throw new ArgumentNullException(nameof(messages));
            Save = save ?? throw new ArgumentNullException(nameof(save));
            Localization = localization ?? throw new ArgumentNullException(nameof(localization));
            Audio = audio ?? throw new ArgumentNullException(nameof(audio));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Worlds = worlds ?? throw new ArgumentNullException(nameof(worlds));
            WorldSession = worldSession ?? throw new ArgumentNullException(nameof(worldSession));
            Input = input ?? throw new ArgumentNullException(nameof(input));
            SafeArea = safeArea ?? throw new ArgumentNullException(nameof(safeArea));
            Haptics = haptics ?? throw new ArgumentNullException(nameof(haptics));
            Analytics = analytics ?? throw new ArgumentNullException(nameof(analytics));
            Ads = ads ?? throw new ArgumentNullException(nameof(ads));
            Purchases = purchases ?? throw new ArgumentNullException(nameof(purchases));
            SceneFlow = sceneFlow ?? throw new ArgumentNullException(nameof(sceneFlow));
        }

        public IAppConfig Configuration { get; }
        public BuildProfile Profile => Configuration.Profile;
        public IClock Clock { get; }
        public IRandomSource Random { get; }
        public IAppLogger Logger { get; }
        public IMessageBus Messages { get; }
        public ISaveService Save { get; }
        public ILocalizationService Localization { get; }
        public IAudioService Audio { get; }
        public IContentCatalog Content { get; }
        public IWorldCatalog Worlds { get; }
        public IWorldSession WorldSession { get; }
        public IInputService Input { get; }
        public ISafeAreaService SafeArea { get; }
        public IHapticsService Haptics { get; }
        public IAnalyticsService Analytics { get; }
        public IAdsService Ads { get; }
        public IPurchaseService Purchases { get; }
        public ISceneFlowService SceneFlow { get; }
    }
}
