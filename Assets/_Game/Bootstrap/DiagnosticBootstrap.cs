using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.Logging;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Content.Audio;
using PequenoExplorador.Presentation.Audio;
using PequenoExplorador.Presentation.Bootstrap;
using PequenoExplorador.Presentation.SceneFlow;
using UnityEngine;
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

        private CancellationTokenSource _lifetimeCancellation;
        private IAppConfig _configuration;
        private ServiceRegistry _services;
        private Task _sceneShutdownTask;
        private bool _destroying;
        private bool _applicationPaused;
        private bool _applicationFocused = true;
        private bool _diagnosticsEnabled;

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

            if (_audioView == null || _audioCatalog == null)
            {
                throw new InvalidOperationException("Audio view and cue catalog must be wired in the Bootstrap scene.");
            }

            _configuration = BuildProfileConfiguration.Resolve();
            _services = new ServiceRegistry(_configuration, gameObject, _audioCatalog);
            _lifetimeCancellation = new CancellationTokenSource();
            _statusView.BindLocalization(_services.Context.Localization);
            _statusView.ConfigureProduct(_configuration);
            _diagnosticsEnabled = _configuration.Features.IsEnabled(FeatureFlag.DevelopmentDiagnostics);
            _statusView.SetDevelopmentDiagnosticsVisible(_diagnosticsEnabled);
            _statusView.ShowInitializing();
            _sceneFlowView.Bind(
                _services.Context.SceneFlow,
                _services.Context.Localization,
                _diagnosticsEnabled);
            _sceneFlowView.EnterJungleRequested += EnterJungle;
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
                SceneTransitionResult transition = await _services.Context.SceneFlow.GoToCampAsync(cancellationToken);
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

        public Task<SceneTransitionResult> GoToCampAsync(CancellationToken cancellationToken)
        {
            return _services.Context.SceneFlow.GoToCampAsync(cancellationToken);
        }

        public Task<SceneTransitionResult> GoToExpeditionAsync(CancellationToken cancellationToken)
        {
            return _services.Context.SceneFlow.GoToExpeditionAsync(cancellationToken);
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
                _services.SaveCoordinator.RequestCheckpoint(_services.Context.Save.Current);
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

        private async void EnterJungle()
        {
            await GoToExpeditionAsync(_lifetimeCancellation.Token);
        }

        private async void ReturnCamp()
        {
            await GoToCampAsync(_lifetimeCancellation.Token);
        }

        private async void RetrySceneTransition()
        {
            await _services.Context.SceneFlow.RetryAsync(_lifetimeCancellation.Token);
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
            if (_sceneFlowView != null)
            {
                _sceneFlowView.EnterJungleRequested -= EnterJungle;
                _sceneFlowView.ReturnCampRequested -= ReturnCamp;
                _sceneFlowView.RetryRequested -= RetrySceneTransition;
                _sceneFlowView.SimulateFailureRequested -= SimulateNextSceneFailure;
                _sceneFlowView.Unbind();
            }
            _audioView?.Unbind();

            Shutdown();
            _services?.Dispose();
            _services = null;
            _lifetimeCancellation?.Dispose();
            _lifetimeCancellation = null;
        }
    }
}
