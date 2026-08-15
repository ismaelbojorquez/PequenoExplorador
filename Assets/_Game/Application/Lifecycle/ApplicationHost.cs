using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Logging;

namespace PequenoExplorador.Application.Lifecycle
{
    public sealed class ApplicationHost : IDisposable
    {
        private readonly IReadOnlyList<IApplicationService> _services;
        private readonly List<IApplicationService> _initializedServices = new List<IApplicationService>();
        private readonly IAppLogger _logger;
        private Task _initializationTask;
        private bool _disposed;

        public ApplicationHost(IEnumerable<IApplicationService> services, IAppLogger logger)
        {
            _services = (services ?? throw new ArgumentNullException(nameof(services))).ToArray();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (_services.Any(service => service == null))
            {
                throw new ArgumentException("Application services cannot contain null entries.", nameof(services));
            }

            if (_services.Select(service => service.ServiceId).Distinct(StringComparer.Ordinal).Count() != _services.Count)
            {
                throw new ArgumentException("Application service IDs must be unique.", nameof(services));
            }
        }

        public ApplicationState State { get; private set; } = ApplicationState.Created;

        public string FailureCode { get; private set; } = string.Empty;

        public IReadOnlyList<string> ServiceOrder => _services.Select(service => service.ServiceId).ToArray();

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (State == ApplicationState.Ready)
            {
                return Task.CompletedTask;
            }

            if (State == ApplicationState.Initializing)
            {
                return _initializationTask;
            }

            if (State == ApplicationState.ShuttingDown || State == ApplicationState.Shutdown)
            {
                throw new InvalidOperationException("A shutdown application host cannot initialize again.");
            }

            FailureCode = string.Empty;
            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _initializationTask = completion.Task;
            SetState(ApplicationState.Initializing);
            _ = CompleteInitializationAsync(completion, cancellationToken);
            return _initializationTask;
        }

        public void Shutdown()
        {
            if (_disposed || State == ApplicationState.Shutdown)
            {
                return;
            }

            SetState(ApplicationState.ShuttingDown);
            ShutdownInitializedServices();
            SetState(ApplicationState.Shutdown);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Shutdown();
            _disposed = true;
        }

        private async Task InitializeCoreAsync(CancellationToken cancellationToken)
        {
            try
            {
                foreach (IApplicationService service in _services)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await service.InitializeAsync(cancellationToken);
                    _initializedServices.Add(service);
                    _logger.Write(new AppLogEntry(
                        AppLogLevel.Info,
                        "Bootstrap",
                        "ServiceInitialized",
                        service.ServiceId));
                }

                SetState(ApplicationState.Ready);
                _logger.Write(new AppLogEntry(AppLogLevel.Info, "Bootstrap", "ApplicationReady", "ServicesReady"));
            }
            catch (OperationCanceledException)
            {
                ShutdownInitializedServices();
                SetState(ApplicationState.Shutdown);
                throw;
            }
            catch (Exception exception)
            {
                FailureCode = exception.GetType().Name;
                _logger.Write(new AppLogEntry(AppLogLevel.Error, "Bootstrap", "InitializationFailed", FailureCode));
                ShutdownInitializedServices();
                SetState(ApplicationState.Failed);
                throw;
            }
        }

        private async Task CompleteInitializationAsync(
            TaskCompletionSource<bool> completion,
            CancellationToken cancellationToken)
        {
            try
            {
                await InitializeCoreAsync(cancellationToken);
                completion.TrySetResult(true);
            }
            catch (OperationCanceledException)
            {
                completion.TrySetCanceled();
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
        }

        private void ShutdownInitializedServices()
        {
            for (int index = _initializedServices.Count - 1; index >= 0; index--)
            {
                IApplicationService service = _initializedServices[index];
                try
                {
                    service.Shutdown();
                    _logger.Write(new AppLogEntry(
                        AppLogLevel.Info,
                        "Bootstrap",
                        "ServiceShutdown",
                        service.ServiceId));
                }
                catch (Exception exception)
                {
                    _logger.Write(new AppLogEntry(
                        AppLogLevel.Error,
                        "Bootstrap",
                        "ServiceShutdownFailed",
                        exception.GetType().Name));
                }
            }

            _initializedServices.Clear();
        }

        private void SetState(ApplicationState state)
        {
            State = state;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ApplicationHost));
            }
        }
    }
}
