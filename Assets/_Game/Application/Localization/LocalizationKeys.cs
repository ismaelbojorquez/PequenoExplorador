namespace PequenoExplorador.Application.Localization
{
    public static class LocalizationKeys
    {
        public const string SharedTable = "Shared";
        public const string UiTable = "UI";
        public const string ContentTable = "Content";
        public const string VoiceAssetTable = "Voice";
        public const string IllustrationAssetTable = "Illustrations";

        public static readonly LocalizedKey ProductName = new LocalizedKey(SharedTable, "shared.product.name");
        public static readonly LocalizedKey Version = new LocalizedKey(SharedTable, "shared.build.version");
        public static readonly LocalizedKey SafeFallback = new LocalizedKey(SharedTable, "shared.fallback.safe");
        public static readonly LocalizedKey StarsCount = new LocalizedKey(SharedTable, "shared.progress.stars");

        public static readonly LocalizedKey DiagnosticNotice = new LocalizedKey(UiTable, "ui.diagnostic.notice");
        public static readonly LocalizedKey StatusInitializing = new LocalizedKey(UiTable, "ui.status.initializing");
        public static readonly LocalizedKey StatusReady = new LocalizedKey(UiTable, "ui.status.ready");
        public static readonly LocalizedKey StatusRecovered = new LocalizedKey(UiTable, "ui.status.progress_recovered");
        public static readonly LocalizedKey StatusNewerProtected = new LocalizedKey(UiTable, "ui.status.newer_protected");
        public static readonly LocalizedKey StatusFailure = new LocalizedKey(UiTable, "ui.status.failure");
        public static readonly LocalizedKey StatusStopped = new LocalizedKey(UiTable, "ui.status.stopped");
        public static readonly LocalizedKey TransitionError = new LocalizedKey(UiTable, "ui.transition.error");
        public static readonly LocalizedKey TransitionPreparing = new LocalizedKey(UiTable, "ui.transition.preparing");
        public static readonly LocalizedKey ActionEnterJungle = new LocalizedKey(UiTable, "ui.action.enter_jungle");
        public static readonly LocalizedKey ActionReturnCamp = new LocalizedKey(UiTable, "ui.action.return_camp");
        public static readonly LocalizedKey ActionRetry = new LocalizedKey(UiTable, "ui.action.retry");
        public static readonly LocalizedKey ActionSimulateFailure = new LocalizedKey(UiTable, "ui.action.simulate_failure");
        public static readonly LocalizedKey LocaleSpanish = new LocalizedKey(UiTable, "ui.locale.spanish");
        public static readonly LocalizedKey LocaleEnglish = new LocalizedKey(UiTable, "ui.locale.english");
        public static readonly LocalizedKey LocalePseudo = new LocalizedKey(UiTable, "ui.locale.pseudo");
        public static readonly LocalizedKey PauseTitle = new LocalizedKey(UiTable, "ui.pause.title");
        public static readonly LocalizedKey ActionResume = new LocalizedKey(UiTable, "ui.action.resume");
        public static readonly LocalizedKey WorldUnavailable = new LocalizedKey(UiTable, "ui.world.unavailable");
        public static readonly LocalizedKey WorldMissing = new LocalizedKey(UiTable, "ui.world.missing");
        public static readonly LocalizedKey InteractionApproaching = new LocalizedKey(UiTable, "ui.interaction.approaching");
        public static readonly LocalizedKey InteractionAction = new LocalizedKey(UiTable, "ui.interaction.action");
        public static readonly LocalizedKey InteractionCancel = new LocalizedKey(UiTable, "ui.interaction.cancel");
        public static readonly LocalizedKey InteractionUnavailable = new LocalizedKey(UiTable, "ui.interaction.unavailable");
        public static readonly LocalizedKey InteractionCompleted = new LocalizedKey(UiTable, "ui.interaction.completed");
        public static readonly LocalizedKey InteractionWait = new LocalizedKey(UiTable, "ui.interaction.wait");
        public static readonly LocalizedKey DiscoveryNew = new LocalizedKey(UiTable, "ui.discovery.new");
        public static readonly LocalizedKey DiscoveryRepeated = new LocalizedKey(UiTable, "ui.discovery.repeated");
        public static readonly LocalizedKey DiscoveryDebugCount = new LocalizedKey(UiTable, "ui.discovery.debug_count");

        public static readonly LocalizedKey WorldBoot = new LocalizedKey(ContentTable, "content.world.boot.name");
        public static readonly LocalizedKey WorldCamp = new LocalizedKey(ContentTable, "content.world.camp.name");
        public static readonly LocalizedKey WorldJungle = new LocalizedKey(ContentTable, "content.world.jungle.name");
        public static readonly LocalizedKey WorldCampPlaceholder = new LocalizedKey(ContentTable, "content.world.camp.placeholder");
        public static readonly LocalizedKey WorldJunglePlaceholder = new LocalizedKey(ContentTable, "content.world.jungle.placeholder");
        public static readonly LocalizedKey DiscoveryPlaceholderName = new LocalizedKey(ContentTable, "content.discovery.placeholder.name");
        public static readonly LocalizedKey KeelBilledToucanName = new LocalizedKey(ContentTable, "content.discovery.keel-billed-toucan.name");
        public static readonly LocalizedKey InteractionAnimalPlaceholderName = new LocalizedKey(ContentTable, "content.interaction.fixture.animal.name");
        public static readonly LocalizedKey InteractionPlantPlaceholderName = new LocalizedKey(ContentTable, "content.interaction.fixture.plant.name");
        public static readonly LocalizedKey InteractionObjectPlaceholderName = new LocalizedKey(ContentTable, "content.interaction.fixture.object.name");
        public static readonly LocalizedKey AudioExploreInstruction = new LocalizedKey(ContentTable, "content.audio.instruction.explore");
        public static readonly LocalizedKey AudioJungleName = new LocalizedKey(ContentTable, "content.audio.name.jungle");
        public static readonly LocalizedKey AudioWelcomeNarration = new LocalizedKey(ContentTable, "content.audio.narration.welcome");
    }
}
