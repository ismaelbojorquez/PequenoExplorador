using System;
using System.Linq;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Album;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Camp;
using PequenoExplorador.Application.Accessibility;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Customization;
using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.Logging;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Learning;
using PequenoExplorador.Application.Messaging;
using PequenoExplorador.Application.Missions;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Application.Tutorial;
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
            : this(configuration, ContentCatalog.Empty, WorldCatalog.Empty, null, null, null, null, fileStore, null, null)
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
            IPhotoStore photoStore = null,
            IRewardCatalog rewardCatalog = null,
            IMissionCatalog missionCatalog = null,
            ILearningCatalog learningCatalog = null,
            ICampCatalog campCatalog = null,
            ICustomizationCatalog customizationCatalog = null,
            TutorialDefinition tutorialDefinition = null)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            contentCatalog ??= ContentCatalog.Empty;
            worldCatalog ??= WorldCatalog.Empty;
            rewardCatalog ??= RewardCatalog.Empty;
            missionCatalog ??= MissionCatalog.Empty;
            learningCatalog ??= LearningCatalog.Empty;
            campCatalog ??= PequenoExplorador.Application.Camp.CampCatalog.Empty;
            customizationCatalog ??= PequenoExplorador.Application.Customization.CustomizationCatalog.Empty;
#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
            if (configuration.Profile == BuildProfile.Development && rewardCatalog is RewardCatalog concreteRewards)
            {
                rewardCatalog = new RewardCatalog(concreteRewards.Definitions.Concat(new[]
                {
                    new RewardDefinition(RewardId.Parse("reward.debug.explorer-stars"), new ExplorerStars(1),
                        RewardSourceKind.Development, "development.debug")
                }));
            }
#endif
            Rewards = rewardCatalog;

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
                    new V5ToV6PhotoProgressMigration(),
                    new V6ToV7EconomyMigration(),
                    new V7ToV8MissionMigration(),
                    new V8ToV9LearningMigration(),
                    new V9ToV10CampMigration(),
                    new V10ToV11CustomizationMigration(),
                    new V11ToV12TutorialMigration()
                });
            SaveCoordinator = new AutosaveCoordinator(save, logger, configuration.AutosaveDebounce);
            TutorialRepository = new PlayerProgressTutorialRepository(save, SaveCoordinator);
            Tutorial = tutorialDefinition == null ? null : new TutorialCoordinator(tutorialDefinition, TutorialRepository);
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
            EconomyRepository = new PlayerProgressEconomyRepository(save, SaveCoordinator);
            GrantRewards = new GrantRewardUseCase(Rewards, EconomyRepository);
            SpendStars = new SpendStarsUseCase(EconomyRepository);
            CampCatalog = campCatalog;
            PurchaseCampUpgrade = new PurchaseCampUpgradeUseCase(campCatalog, EconomyRepository);
            CustomizationCatalog = customizationCatalog;
            UnlockCosmetic = new UnlockCosmeticUseCase(customizationCatalog, EconomyRepository);
            EquipCosmetic = new EquipCosmeticUseCase(customizationCatalog, EconomyRepository);
            MissionRepository = new PlayerProgressMissionRepository(save, SaveCoordinator);
            MissionStrategies = new MissionObjectiveStrategyRegistry(new IMissionObjectiveStrategy[]
            {
                new DiscoverCountObjectiveStrategy(),
                new PhotographSpecificObjectiveStrategy(),
                new InteractTagObjectiveStrategy()
            });
            Missions = new MissionCoordinator(missionCatalog, MissionStrategies, MissionRepository, GrantRewards);
            TimeSpan localOffset = TimeZoneInfo.Local.GetUtcOffset(clock.UtcNow.UtcDateTime);
            LearningRepository = new PlayerProgressLearningRepository(save, SaveCoordinator);
            LearningStrategies = new LearningActivityStrategyRegistry(new ILearningActivityStrategy[] { new SingleChoiceActivityStrategy() });
            Learning = new LearningCoordinator(learningCatalog, LearningStrategies, LearningRepository, GrantRewards,
                Missions, clock, configuration.Profile == BuildProfile.Development, localOffset);
            bool allowUnapprovedDiscovery = configuration.Profile == BuildProfile.Development;
            Discoveries = new DiscoverUseCase(
                contentCatalog,
                discoveryRepository,
                clock,
                allowUnapprovedDiscovery,
                localOffset);
            DiscoveryQueries = new DiscoveryProgressQueries(contentCatalog, discoveryRepository);
            AlbumQueries = new AlbumQueryService(contentCatalog, discoveryRepository, PhotoRepository);
            DiscoveryInteraction = new DiscoveryInteractionAction(Discoveries);
            PhotographyInteraction = new PhotographyInteractionAction(Missions, contentCatalog);
            LearningInteraction = new LearningInteractionAction(Missions, contentCatalog);
            ILocalizationService localization = new UnityLocalizationService(
                save,
                logger,
                configuration.Profile,
                SaveCoordinator);
            IAudioService audio = audioHost != null && audioCatalog != null
                ? CreateAudioService(audioHost, audioCatalog, save, SaveCoordinator, localization, logger)
                : new HeadlessAudioService(save, SaveCoordinator);
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
        public AlbumQueryService AlbumQueries { get; }
        public DiscoveryInteractionAction DiscoveryInteraction { get; }
        public PhotographyInteractionAction PhotographyInteraction { get; }
        public LearningInteractionAction LearningInteraction { get; }
        public IPhotoStore PhotoStore { get; }
        public IPhotoProgressRepository PhotoRepository { get; }
        public IEconomyRepository EconomyRepository { get; }
        public GrantRewardUseCase GrantRewards { get; }
        public SpendStarsUseCase SpendStars { get; }
        public ICampCatalog CampCatalog { get; }
        public PurchaseCampUpgradeUseCase PurchaseCampUpgrade { get; }
        public ICustomizationCatalog CustomizationCatalog { get; }
        public UnlockCosmeticUseCase UnlockCosmetic { get; }
        public EquipCosmeticUseCase EquipCosmetic { get; }
        public IRewardCatalog Rewards { get; }
        public IMissionRepository MissionRepository { get; }
        public MissionObjectiveStrategyRegistry MissionStrategies { get; }
        public MissionCoordinator Missions { get; }
        public ILearningRepository LearningRepository { get; }
        public LearningActivityStrategyRegistry LearningStrategies { get; }
        public LearningCoordinator Learning { get; }
        public ITutorialProgressRepository TutorialRepository { get; }
        public TutorialCoordinator Tutorial { get; }
#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
        public DevelopmentPhotoStoreFailure PhotoFailure { get; }
#endif

        private static IAudioService CreateAudioService(
            UnityEngine.GameObject host,
            AudioCueCatalogAsset catalog,
            ISaveService save,
            AutosaveCoordinator checkpoints,
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
                catalog.Voice,
                checkpoints: checkpoints);
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
