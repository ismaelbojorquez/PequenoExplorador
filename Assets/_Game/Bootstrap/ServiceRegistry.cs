using System;
using System.Linq;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Accessibility;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.Logging;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Messaging;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Application.Worlds;
using PequenoExplorador.Content.Audio;
using PequenoExplorador.Content.Input;
using PequenoExplorador.Infrastructure.Audio;
using PequenoExplorador.Infrastructure.Accessibility;
using PequenoExplorador.Infrastructure.Input;
using PequenoExplorador.Infrastructure.Ads;
using PequenoExplorador.Infrastructure.Analytics;
using PequenoExplorador.Infrastructure.Logging;
using PequenoExplorador.Infrastructure.Localization;
using PequenoExplorador.Infrastructure.Messaging;
using PequenoExplorador.Infrastructure.Purchases;
using PequenoExplorador.Infrastructure.Random;
using PequenoExplorador.Infrastructure.Save;
using PequenoExplorador.Infrastructure.Time;
using PequenoExplorador.Infrastructure.Photography;
using PequenoExplorador.Infrastructure.SceneFlow;
using ApplicationContext = PequenoExplorador.Application.AppContext;
using UnityEngine.InputSystem;

namespace PequenoExplorador.Bootstrap
{
    internal sealed class ServiceRegistry : IDisposable
    {
        private readonly IDisposable _fileStoreLifetime;

        public ServiceRegistry(IAppConfig configuration, IFileStore fileStore = null)
            : this(configuration, ContentCatalog.Empty, WorldCatalog.Empty, null, null, null, null, fileStore, null)
        {
        }

        public ServiceRegistry(
            IAppConfig configuration,
            IContentCatalog contentCatalog,
            IWorldCatalog worldCatalog,
            UnityEngine.GameObject audioHost,
            AudioCueCatalogAsset audioCatalog,
            InputActionAsset inputActions,
            GestureThresholdsAsset gestureThresholds,
            IFileStore fileStore = null,
            IPhotoStore photoStore = null)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            contentCatalog ??= ContentCatalog.Empty;
            worldCatalog ??= WorldCatalog.Empty;

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
            IInputService input;
            ISafeAreaService safeArea;
            IHapticsService haptics = new NoOpHapticsService();
            if (audioHost != null && inputActions != null && gestureThresholds != null)
            {
                bool debugInput = configuration.Features.IsEnabled(FeatureFlag.DevelopmentDiagnostics);
                var unityInput = new UnityInputService(inputActions, gestureThresholds.ToRuntime(), debugInput);
                var unitySafeArea = new UnitySafeAreaService();
                MobileInputDriver driver = audioHost.AddComponent<MobileInputDriver>();
                driver.Bind(unityInput, unitySafeArea);
                input = unityInput;
                safeArea = unitySafeArea;
            }
            else
            {
                input = new HeadlessInputService();
                safeArea = new StaticSafeAreaService();
            }
            IFileStore resolvedFileStore = fileStore ?? new LocalFileStore(
                System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Save"));
            _fileStoreLifetime = resolvedFileStore as IDisposable;
            ISaveService save = new LocalSaveService(
                resolvedFileStore,
                configuration.AppVersion,
                logger,
                new ISaveMigration[]
                {
                    new LegacyV0ToV1Migration(),
                    new V1ToV2LocalizationMigration(),
                    new V2ToV3AudioMigration(),
                    new V3ToV4DiscoveryMigration(),
                    new V4ToV5ToucanDiscoveryMigration(),
                    new V5ToV6PhotoProgressMigration()
                });
            SaveCoordinator = new AutosaveCoordinator(save, logger, configuration.AutosaveDebounce);
            IPhotoStore resolvedPhotoStore = photoStore ?? (audioHost != null
                ? new LocalPhotoStore(System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Photos"))
                : new MemoryPhotoStore());
#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
            PhotoFailure = new DevelopmentPhotoStoreFailure(resolvedPhotoStore);
            PhotoStore = PhotoFailure;
#else
            PhotoStore = resolvedPhotoStore;
#endif
            var discoveryRepository = new PlayerProgressDiscoveryRepository(save, SaveCoordinator);
            PhotoRepository = new PlayerProgressPhotoRepository(save, SaveCoordinator);
            bool allowUnapprovedDiscovery = configuration.Profile == BuildProfile.Development;
            TimeSpan localOffset = TimeZoneInfo.Local.GetUtcOffset(clock.UtcNow.UtcDateTime);
            Discoveries = new DiscoverUseCase(
                contentCatalog,
                discoveryRepository,
                clock,
                allowUnapprovedDiscovery,
                localOffset);
            DiscoveryQueries = new DiscoveryProgressQueries(contentCatalog, discoveryRepository);
            DiscoveryInteraction = new DiscoveryInteractionAction(Discoveries);
            PhotographyInteraction = new PhotographyInteractionAction();
            ILocalizationService localization = new UnityLocalizationService(
                save,
                logger,
                configuration.Profile);
            IAudioService audio = audioHost != null && audioCatalog != null
                ? CreateAudioService(audioHost, audioCatalog, save, localization, logger)
                : new HeadlessAudioService(save);
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
                configuration.SceneTransitionTimeout,
                LocalSceneAddresses.CampId);
            IWorldSession worldSession = new WorldLoadUseCase(worldCatalog, sceneFlow);

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
                localization,
                audio,
                contentCatalog,
                worldCatalog,
                worldSession,
                input,
                safeArea,
                haptics,
                analytics,
                ads,
                purchases,
                sceneFlow);
            Host = new ApplicationHost(
                new IApplicationService[]
                {
                    messages,
                    input,
                    safeArea,
                    haptics,
                    save,
                    PhotoStore,
                    localization,
                    audio,
                    analytics,
                    ads,
                    purchases
                },
                logger);
        }

        public ApplicationContext Context { get; }

        public ApplicationHost Host { get; }

        public AutosaveCoordinator SaveCoordinator { get; }
        public DiscoverUseCase Discoveries { get; }
        public DiscoveryProgressQueries DiscoveryQueries { get; }
        public DiscoveryInteractionAction DiscoveryInteraction { get; }
        public PhotographyInteractionAction PhotographyInteraction { get; }
        public IPhotoStore PhotoStore { get; }
        public IPhotoProgressRepository PhotoRepository { get; }
#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
        public DevelopmentPhotoStoreFailure PhotoFailure { get; }
#endif

        private static IAudioService CreateAudioService(
            UnityEngine.GameObject host,
            AudioCueCatalogAsset catalog,
            ISaveService save,
            ILocalizationService localization,
            IAppLogger logger)
        {
            if (catalog.Mixer == null || catalog.Music == null || catalog.Ambience == null ||
                catalog.Effects == null || catalog.Voice == null)
            {
                throw new InvalidOperationException("Audio catalog mixer buses are not fully configured.");
            }

            UnityAudioCue[] cues = catalog.Cues.Select(definition => new UnityAudioCue(
                definition.CueId,
                definition.Category,
                definition.Bus,
                definition.Priority,
                definition.CooldownSeconds,
                definition.Gain,
                definition.Loop,
                definition.HasSubtitle ? definition.SubtitleKey : default,
                definition.HasSubtitle,
                definition.SpanishClip,
                definition.EnglishClip,
                definition.IsPlaceholder)).ToArray();
            return new UnityAudioService(
                host,
                cues,
                save,
                localization,
                logger,
                catalog.Music,
                catalog.Ambience,
                catalog.Effects,
                catalog.Voice);
        }

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
