using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Presentation.Bootstrap;
using UnityEngine;

namespace PequenoExplorador.Bootstrap
{
    /// <summary>
    /// Unity lifecycle adapter and the project's only composition root.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DiagnosticBootstrap : MonoBehaviour
    {
        public const string ProductName = "Pequeño Explorador: Aprende Jugando";
        public const string DevelopmentVersion = "0.1.0-dev";
        public const string PlaceholderObjectName = "PH_UI_DIAGNOSTIC";

        [SerializeField] private BootstrapStatusView _statusView;

        private CancellationTokenSource _lifetimeCancellation;
        private BootstrapConfiguration _configuration;
        private ServiceRegistry _services;

        public ApplicationState State => _services == null
            ? ApplicationState.Created
            : _services.Host.State;

        public ApplicationEnvironment Environment => _configuration == null
            ? ApplicationEnvironment.Release
            : _configuration.Environment;

        public string StatusText => _statusView == null ? string.Empty : _statusView.CurrentStatus;

        private void Awake()
        {
            if (_statusView == null)
            {
                throw new InvalidOperationException("BootstrapStatusView must be wired in the Bootstrap scene.");
            }

            _configuration = BuildProfileConfiguration.Resolve();
            _services = new ServiceRegistry(_configuration);
            _lifetimeCancellation = new CancellationTokenSource();
            _statusView.SetDevelopmentDiagnosticsVisible(_configuration.DevelopmentDiagnosticsEnabled);
            _statusView.ShowInitializing();
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
            _services.Host.Shutdown();
            _statusView?.ShowShutdown();
        }

        private async Task InitializeAndRenderAsync(CancellationToken cancellationToken)
        {
            try
            {
                await InitializeAsync(cancellationToken);
                _statusView.ShowReady();
            }
            catch (OperationCanceledException)
            {
                _statusView.ShowShutdown();
            }
            catch (Exception)
            {
                _statusView.ShowRecoverableFailure();
            }
        }

        private void OnDestroy()
        {
            Shutdown();
            _services?.Dispose();
            _services = null;
            _lifetimeCancellation?.Dispose();
            _lifetimeCancellation = null;
        }
    }
}
