using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Accessibility;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.Logging;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Application.Worlds;
using PequenoExplorador.Content.Audio;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Content.Input;
using PequenoExplorador.Content.Interaction;
using PequenoExplorador.Content.Worlds;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Audio;
using PequenoExplorador.Presentation.Bootstrap;
using PequenoExplorador.Presentation.Input;
using PequenoExplorador.Presentation.Explorer;
using PequenoExplorador.Presentation.Interaction;
using PequenoExplorador.Presentation.SceneFlow;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using AudioSettingsModel = PequenoExplorador.Application.Audio.AudioSettings;

namespace PequenoExplorador.Bootstrap
{
    /// <summary>
    /// Unity lifecycle adapter and the project's only composition root.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DiagnosticBootstrap : MonoBehaviour
    {
        public const string PlaceholderObjectName = "PH_UI_DIAGNOSTIC";

        [SerializeField] private BootstrapStatusView _statusView;
        [SerializeField] private SceneTransitionView _sceneFlowView;
        [SerializeField] private AudioDiagnosticView _audioView;
        [SerializeField] private AudioCueCatalogAsset _audioCatalog;
        [SerializeField] private ContentCatalogAsset _contentCatalog;
        [SerializeField] private WorldCatalogAsset _worldCatalog;
        [SerializeField] private InteractionCatalogAsset _interactionCatalog;
        [SerializeField] private InputActionAsset _inputActions;
        [SerializeField] private GestureThresholdsAsset _gestureThresholds;
        [SerializeField] private SafeAreaFitter[] _safeAreaFitters = Array.Empty<SafeAreaFitter>();
        [SerializeField] private InputPauseView _pauseView;
        [SerializeField] private TouchDiagnosticOverlay _touchOverlay;
        [SerializeField] private DeviceAspectOverlay _aspectOverlay;
        [SerializeField] private Camera _worldCamera;
        [SerializeField] private InteractionPromptView _interactionPrompt;

        private CancellationTokenSource _lifetimeCancellation;
        private IAppConfig _configuration;
        private ServiceRegistry _services;
        private Task _sceneShutdownTask;
        private bool _destroying;
        private bool _applicationPaused;
        private bool _applicationFocused = true;
        private bool _diagnosticsEnabled;
        private InputMapId _inputMapBeforePause = InputMapId.UI;
        private ExplorerLocomotionRoot _explorerRoot;
        private InteractionSceneRoot _interactionRoot;
        private InteractionCatalog _runtimeInteractions;

        public ApplicationState State => _services == null
            ? ApplicationState.Created
            : _services.Host.State;

        public BuildProfile Profile => _configuration == null
            ? BuildProfile.Unknown
            : _configuration.Profile;

        public string ConfiguredProductName => _configuration?.ProductName ?? string.Empty;

        public string ConfiguredAppVersion => _configuration?.AppVersion ?? string.Empty;

        public string CurrentLocaleCode => _services?.Context.Localization.CurrentLocaleCode ?? string.Empty;

        public string StatusText => _statusView == null ? string.Empty : _statusView.CurrentStatus;

        public SceneFlowSnapshot SceneFlow => _services == null
            ? null
            : _services.Context.SceneFlow.Snapshot;

        public SaveLoadResult SaveLoadResult => _services == null
            ? null
            : _services.Context.Save.LastLoadResult;

        public IAudioService Audio => _services?.Context.Audio;
        public IContentCatalog Content => _services?.Context.Content;
        public IWorldCatalog Worlds => _services?.Context.Worlds;
        public IWorldSession WorldSession => _services?.Context.WorldSession;
        public IInputService Input => _services?.Context.Input;
        public ISafeAreaService SafeArea => _services?.Context.SafeArea;
        public IHapticsService Haptics => _services?.Context.Haptics;
        public bool IsPauseVisible => _pauseView != null && _pauseView.IsVisible;
        public ExplorerLocomotionRoot ExplorerRoot => _explorerRoot;
        public InteractionSceneRoot InteractionRoot => _interactionRoot;
        public InteractionPromptView InteractionPrompt => _interactionPrompt;
        public DiscoverResult LastDiscoveryResult => _services == null
            ? default
            : _services.DiscoveryInteraction.LastResult;

        private void Awake()
        {
            if (_statusView == null)
            {
                throw new InvalidOperationException("BootstrapStatusView must be wired in the Bootstrap scene.");
            }

            if (_sceneFlowView == null)
            {
                throw new InvalidOperationException("SceneTransitionView must be wired in the Bootstrap scene.");
            }

            if (_audioView == null || _audioCatalog == null || _contentCatalog == null ||
                _worldCatalog == null || _interactionCatalog == null)
            {
                throw new InvalidOperationException("Audio, content and world catalogs must be wired in the Bootstrap scene.");
            }
            if (_inputActions == null || _gestureThresholds == null || _pauseView == null ||
                _touchOverlay == null || _aspectOverlay == null || _safeAreaFitters.Length == 0 ||
                _worldCamera == null || _interactionPrompt == null)
            {
                throw new InvalidOperationException("Input actions, thresholds, pause, overlays and safe-area fitters must be wired.");
            }

            _configuration = BuildProfileConfiguration.Resolve();
            ContentValidationMode contentMode = _configuration.Profile == BuildProfile.Release
                ? ContentValidationMode.Release
                : ContentValidationMode.Development;
            if (!_contentCatalog.TryBuildRuntimeCatalog(contentMode, out ContentCatalog runtimeCatalog, out var contentViolations))
                throw new InvalidOperationException("Runtime content catalog is invalid:\n" + string.Join("\n", contentViolations));
            if (!_worldCatalog.TryBuildRuntimeCatalog(runtimeCatalog, contentMode, out WorldCatalog runtimeWorlds, out var worldViolations))
                throw new InvalidOperationException("Runtime world catalog is invalid:\n" + string.Join("\n", worldViolations));
            if (!_interactionCatalog.TryBuildRuntimeCatalog(
                    contentMode,
                    out _runtimeInteractions,
                    out var interactionViolations))
                throw new InvalidOperationException(
                    "Runtime interaction catalog is invalid:\n" + string.Join("\n", interactionViolations));
            _services = new ServiceRegistry(
                _configuration,
                runtimeCatalog,
                runtimeWorlds,
                gameObject,
                _audioCatalog,
                _inputActions,
                _gestureThresholds);
            _lifetimeCancellation = new CancellationTokenSource();
            _statusView.BindLocalization(_services.Context.Localization);
            _statusView.ConfigureProduct(_configuration);
            _diagnosticsEnabled = _configuration.Features.IsEnabled(FeatureFlag.DevelopmentDiagnostics);
            foreach (SafeAreaFitter fitter in _safeAreaFitters) fitter?.Bind(_services.Context.SafeArea);
            _pauseView.Bind(_services.Context.Localization);
            _pauseView.ResumeRequested += ResumeFromPause;
            _touchOverlay.Bind(_services.Context.Input, _services.Context.SafeArea, _diagnosticsEnabled);
            _aspectOverlay.Bind(_services.Context.SafeArea, _diagnosticsEnabled);
            _services.Context.Input.BackRequested += HandleBackRequested;
            _services.Context.SceneFlow.Changed += HandleSceneFlowChanged;
            _statusView.SetDevelopmentDiagnosticsVisible(_diagnosticsEnabled);
            _statusView.ShowInitializing();
            _sceneFlowView.Bind(
                _services.Context.SceneFlow,
                _services.Context.Worlds,
                _services.Context.Localization,
                _diagnosticsEnabled);
            _sceneFlowView.WorldRequested += EnterWorld;
            _sceneFlowView.ReturnCampRequested += ReturnCamp;
            _sceneFlowView.RetryRequested += RetrySceneTransition;
            _sceneFlowView.SimulateFailureRequested += SimulateNextSceneFailure;
        }

        private async void Start()
        {
            await InitializeAndRenderAsync(_lifetimeCancellation.Token);
        }

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (_services == null)
            {
                throw new InvalidOperationException("Bootstrap has not completed Awake.");
            }

            return _services.Host.InitializeAsync(cancellationToken);
        }

        public async void RetryInitialization()
        {
            if (State != ApplicationState.Failed || _lifetimeCancellation == null)
            {
                return;
            }

            await InitializeAndRenderAsync(_lifetimeCancellation.Token);
        }

        public void Shutdown()
        {
            if (_services == null)
            {
                return;
            }

            _lifetimeCancellation?.Cancel();
            _sceneShutdownTask = _sceneShutdownTask ?? _services.Context.SceneFlow.ShutdownAsync();
            _services.Host.Shutdown();
            _statusView?.ShowShutdown();
        }

        private async Task InitializeAndRenderAsync(CancellationToken cancellationToken)
        {
            try
            {
                await InitializeAsync(cancellationToken);
                _audioView.Bind(_services.Context.Audio, _services.Context.Localization, _diagnosticsEnabled);
                _services.Context.Audio.Play(AudioCueIds.CampMusic);
                _services.Context.Audio.Play(AudioCueIds.CampAmbience);
                WorldLoadResult transition = await _services.Context.WorldSession.ReturnToCampAsync(cancellationToken);
                if (!transition.IsSuccess)
                {
                    throw new InvalidOperationException(transition.ErrorCode);
                }

                if (!_destroying)
                {
                    _statusView.ShowReady(_services.Context.Save.LastLoadResult.UserNotice);
                }
            }
            catch (OperationCanceledException)
            {
                if (!_destroying)
                {
                    _statusView.ShowShutdown();
                }
            }
            catch (Exception)
            {
                if (!_destroying)
                {
                    _statusView.ShowRecoverableFailure();
                }
            }
        }

        public async Task<SceneTransitionResult> GoToCampAsync(CancellationToken cancellationToken)
        {
            _services.Context.Input.SetMap(InputMapId.UI);
            UnbindExplorerScene();
            WorldLoadResult worldResult = await _services.Context.WorldSession.ReturnToCampAsync(cancellationToken);
            if (worldResult.IsSuccess)
            {
                _services.Context.Input.SetMap(InputMapId.UI);
                _services.Context.Audio.Play(AudioCueIds.CampMusic);
                _services.Context.Audio.Play(AudioCueIds.CampAmbience);
            }
            return ToSceneResult(worldResult);
        }

        public async Task<SceneTransitionResult> GoToExpeditionAsync(CancellationToken cancellationToken)
        {
            WorldCatalogEntry firstAvailable = _services.Context.Worlds.Worlds
                .FirstOrDefault(entry => entry.Availability == WorldAvailabilityState.Available);
            WorldLoadResult worldResult = firstAvailable == null
                ? await _services.Context.WorldSession.EnterAsync(default, cancellationToken)
                : await EnterWorldAsync(firstAvailable.Manifest.Id, cancellationToken);
            return ToSceneResult(worldResult);
        }

        public async Task<WorldLoadResult> EnterWorldAsync(WorldId worldId, CancellationToken cancellationToken)
        {
            _services.Context.Input.SetMap(InputMapId.UI);
            WorldLoadResult result = await _services.Context.WorldSession.EnterAsync(worldId, cancellationToken);
            if (result.IsSuccess)
            {
                BindExplorerScene();
                _services.Context.Input.SetMap(InputMapId.Explorer);
                _services.Context.Audio.Play(result.Manifest.MusicCue);
                _services.Context.Audio.Play(result.Manifest.AmbienceCue);
            }
            _sceneFlowView.ShowWorldResult(result);
            return result;
        }

        public Task ShutdownSceneFlowAsync()
        {
            _sceneShutdownTask = _sceneShutdownTask ?? _services.Context.SceneFlow.ShutdownAsync();
            return _sceneShutdownTask;
        }

        public void RequestSaveCheckpoint()
        {
            if (_services?.Context.Save.Current != null && !_services.Context.Save.IsReadOnly)
            {
                _services.SaveCoordinator.RequestCheckpoint(_services.SaveCoordinator.Latest);
            }
        }

        public Task FlushSaveAsync(CancellationToken cancellationToken)
        {
            if (_services == null)
            {
                return Task.CompletedTask;
            }

            return _services.SaveCoordinator.FlushAsync(cancellationToken);
        }

        public Task SetLocaleAsync(
            string localeCode,
            bool persist,
            CancellationToken cancellationToken)
        {
            if (_services == null)
            {
                throw new InvalidOperationException("Bootstrap has not completed Awake.");
            }

            return _services.Context.Localization.SetLocaleAsync(localeCode, persist, cancellationToken);
        }

        public AudioPlayResult PlayAudio(AudioCueId cueId) => _services.Context.Audio.Play(cueId);
        public AudioPlayResult ReplayInstruction() => _services.Context.Audio.ReplayLastInstruction();
        public Task UpdateAudioSettingsAsync(AudioSettingsModel settings, CancellationToken cancellationToken) =>
            _services.Context.Audio.UpdateSettingsAsync(settings, cancellationToken);

        public void SetInputMap(InputMapId map) => _services.Context.Input.SetMap(map);

#if UNITY_EDITOR
        public async Task ResetProgressForTestsAsync(CancellationToken cancellationToken)
        {
            await _services.SaveCoordinator.FlushAsync(cancellationToken);
            SaveOperationResult result = await _services.Context.Save.ResetAsync(cancellationToken);
            if (!result.IsSuccess) throw new InvalidOperationException(result.ErrorCode);
        }

        public void ConfigureInputForEditorAndTests(
            InputActionAsset inputActions,
            GestureThresholdsAsset gestureThresholds,
            SafeAreaFitter[] safeAreaFitters,
            InputPauseView pauseView,
            TouchDiagnosticOverlay touchOverlay,
            DeviceAspectOverlay aspectOverlay)
        {
            _inputActions = inputActions;
            _gestureThresholds = gestureThresholds;
            _safeAreaFitters = safeAreaFitters ?? Array.Empty<SafeAreaFitter>();
            _pauseView = pauseView;
            _touchOverlay = touchOverlay;
            _aspectOverlay = aspectOverlay;
        }

        public void ConfigureContentForEditorAndTests(ContentCatalogAsset contentCatalog) => _contentCatalog = contentCatalog;
        public void ConfigureWorldsForEditorAndTests(WorldCatalogAsset worldCatalog) => _worldCatalog = worldCatalog;
        public void ConfigureInteractionsForEditorAndTests(
            InteractionCatalogAsset interactionCatalog,
            InteractionPromptView interactionPrompt)
        {
            _interactionCatalog = interactionCatalog;
            _interactionPrompt = interactionPrompt;
        }
        public void ConfigureExplorerCameraForEditorAndTests(Camera worldCamera) => _worldCamera = worldCamera;
#endif

        private async void EnterWorld(WorldManifest manifest)
        {
            await EnterWorldAsync(manifest.Id, _lifetimeCancellation.Token);
        }

        private async void ReturnCamp()
        {
            await GoToCampAsync(_lifetimeCancellation.Token);
        }

        private async void RetrySceneTransition()
        {
            WorldLoadResult result = await _services.Context.WorldSession.RetryAsync(_lifetimeCancellation.Token);
            if (result.IsSuccess)
            {
                BindExplorerScene();
                _services.Context.Input.SetMap(InputMapId.Explorer);
                _services.Context.Audio.Play(result.Manifest.MusicCue);
                _services.Context.Audio.Play(result.Manifest.AmbienceCue);
            }
            _sceneFlowView.ShowWorldResult(result);
        }

        private void SimulateNextSceneFailure()
        {
#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
            if (_configuration.Features.IsEnabled(FeatureFlag.SimulatedSceneFailure))
            {
                SimulateNextSceneFailureForDevelopment();
            }
#endif
        }

        private void HandleBackRequested()
        {
            if (_pauseView.IsVisible)
            {
                ResumeFromPause();
                return;
            }

            _inputMapBeforePause = _services.Context.Input.CurrentMap;
            RequestSaveCheckpoint();
            _services.Context.Input.SetMap(InputMapId.UI);
            _pauseView.Show(true);
        }

        private void ResumeFromPause()
        {
            _pauseView.Show(false);
            _services.Context.Input.SetMap(_inputMapBeforePause == InputMapId.None
                ? ResolveInputMap(_services.Context.SceneFlow.Snapshot)
                : _inputMapBeforePause);
        }

        private void HandleSceneFlowChanged(SceneFlowSnapshot snapshot)
        {
            if (!_pauseView.IsVisible && !snapshot.IsTransitioning)
                _services.Context.Input.SetMap(ResolveInputMap(snapshot));
        }

        private void BindExplorerScene()
        {
            UnbindExplorerScene();
            ExplorerLocomotionRoot found = null;
            InteractionSceneRoot interaction = null;
            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);
                if (!scene.isLoaded || scene.path == gameObject.scene.path) continue;
                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    ExplorerLocomotionRoot candidate = root.GetComponentInChildren<ExplorerLocomotionRoot>(true);
                    if (candidate != null)
                    {
                        if (found != null)
                            throw new InvalidOperationException("Expedition contains more than one explorer scene root.");
                        found = candidate;
                    }

                    InteractionSceneRoot sceneInteraction = root.GetComponentInChildren<InteractionSceneRoot>(true);
                    if (sceneInteraction != null)
                    {
                        if (interaction != null)
                            throw new InvalidOperationException("Expedition contains more than one interaction scene root.");
                        interaction = sceneInteraction;
                    }
                }
            }

            if (found == null)
                throw new InvalidOperationException("Expedition scene is missing PH_ explorer scene root.");
            if (interaction == null)
                throw new InvalidOperationException("Expedition scene is missing PH_ interaction scene root.");
            try
            {
                found.Bind(_services.Context.Input, _worldCamera);
                interaction.Bind(
                    _runtimeInteractions,
                    found,
                    _services.Context.Clock,
                    _services.Context.Input,
                    _worldCamera,
                    _services.DiscoveryInteraction);
                found.SetTapHandler(interaction);
                _interactionPrompt.Bind(
                    interaction.Coordinator,
                    _services.Context.Localization,
                    _services.Context.Audio,
                    _services.DiscoveryInteraction,
                    _diagnosticsEnabled);
                _explorerRoot = found;
                _interactionRoot = interaction;
            }
            catch
            {
                _interactionPrompt.Unbind();
                interaction.Unbind();
                found.Unbind();
                throw;
            }
        }

        private void UnbindExplorerScene()
        {
            _interactionPrompt?.Unbind();
            _explorerRoot?.SetTapHandler(null);
            _interactionRoot?.Unbind();
            _explorerRoot?.Unbind();
            _interactionRoot = null;
            _explorerRoot = null;
        }

        private static InputMapId ResolveInputMap(SceneFlowSnapshot snapshot)
        {
            if (snapshot == null || snapshot.IsTransitioning) return InputMapId.UI;
            return snapshot.Current == SceneFlowState.Expedition ? InputMapId.Explorer : InputMapId.UI;
        }

        private async void OnApplicationPause(bool paused)
        {
            _applicationPaused = paused;
            UpdateAudioSuspension();
            if (!paused || _destroying || _services == null)
            {
                return;
            }

            await FlushCurrentProgressWithBudgetAsync(TimeSpan.FromSeconds(1));
        }

        private void OnApplicationFocus(bool focused)
        {
            _applicationFocused = focused;
            UpdateAudioSuspension();
        }

        private void UpdateAudioSuspension()
        {
            _services?.Context.Audio.SetApplicationSuspended(_applicationPaused || !_applicationFocused);
        }

        private void OnApplicationQuit()
        {
            if (!_destroying && _services != null)
            {
                _ = FlushCurrentProgressWithBudgetAsync(TimeSpan.FromMilliseconds(250));
            }
        }

        private async Task FlushCurrentProgressWithBudgetAsync(TimeSpan budget)
        {
            try
            {
                RequestSaveCheckpoint();
                using var cancellation = new CancellationTokenSource(budget);
                await FlushSaveAsync(cancellation.Token);
            }
            catch (OperationCanceledException)
            {
                // Checkpoints are authoritative; quit/pause never blocks indefinitely.
            }
            catch (Exception exception)
            {
                _services?.Context.Logger.Write(new AppLogEntry(
                    AppLogLevel.Error,
                    "Save",
                    "LifecycleFlushFailed",
                    exception.GetType().Name));
            }
        }

#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
        public void SimulateNextSceneFailureForDevelopment()
        {
            if (_configuration == null ||
                !_configuration.Features.IsEnabled(FeatureFlag.SimulatedSceneFailure))
            {
                throw new InvalidOperationException(
                    "Simulated scene failure is disabled by the active build profile.");
            }

            _services.SceneFailure.FailNextLoad();
        }
#endif

        private void OnDestroy()
        {
            _destroying = true;
            UnbindExplorerScene();
            if (_sceneFlowView != null)
            {
                _sceneFlowView.WorldRequested -= EnterWorld;
                _sceneFlowView.ReturnCampRequested -= ReturnCamp;
                _sceneFlowView.RetryRequested -= RetrySceneTransition;
                _sceneFlowView.SimulateFailureRequested -= SimulateNextSceneFailure;
                _sceneFlowView.Unbind();
            }
            _audioView?.Unbind();
            if (_services != null)
            {
                _services.Context.Input.BackRequested -= HandleBackRequested;
                _services.Context.SceneFlow.Changed -= HandleSceneFlowChanged;
            }
            if (_pauseView != null)
            {
                _pauseView.ResumeRequested -= ResumeFromPause;
                _pauseView.Unbind();
            }
            _touchOverlay?.Unbind();
            _aspectOverlay?.Unbind();
            foreach (SafeAreaFitter fitter in _safeAreaFitters) fitter?.Unbind();

            Shutdown();
            _services?.Dispose();
            _services = null;
            _lifetimeCancellation?.Dispose();
            _lifetimeCancellation = null;
        }

        private static SceneTransitionResult ToSceneResult(WorldLoadResult result)
        {
            SceneTransitionOutcome outcome;
            switch (result.Outcome)
            {
                case WorldLoadOutcome.Succeeded: outcome = SceneTransitionOutcome.Succeeded; break;
                case WorldLoadOutcome.AlreadyThere: outcome = SceneTransitionOutcome.AlreadyThere; break;
                case WorldLoadOutcome.Busy: outcome = SceneTransitionOutcome.Busy; break;
                case WorldLoadOutcome.Canceled: outcome = SceneTransitionOutcome.Canceled; break;
                case WorldLoadOutcome.Missing:
                case WorldLoadOutcome.Unavailable:
                case WorldLoadOutcome.Failed:
                default: outcome = SceneTransitionOutcome.Failed; break;
            }
            return new SceneTransitionResult(outcome, result.ErrorCode);
        }
    }
}
