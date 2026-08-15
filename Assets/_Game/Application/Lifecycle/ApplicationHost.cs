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
        private CancellationTokenSource _initializationCancellation;
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
            var initializationCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _initializationCancellation = initializationCancellation;
            _initializationTask = completion.Task;
            SetState(ApplicationState.Initializing);
            _ = CompleteInitializationAsync(completion, initializationCancellation);
            return _initializationTask;
        }

        public void Shutdown()
        {
            if (_disposed || State == ApplicationState.Shutdown)
            {
                return;
            }

            if (State == ApplicationState.Initializing || State == ApplicationState.ShuttingDown)
            {
                SetState(ApplicationState.ShuttingDown);
                _initializationCancellation?.Cancel();
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
            IApplicationService initializingService = null;
            try
            {
                foreach (IApplicationService service in _services)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    initializingService = service;
                    await service.InitializeAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    _initializedServices.Add(service);
                    initializingService = null;
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
                ShutdownService(initializingService);
                ShutdownInitializedServices();
                SetState(ApplicationState.Shutdown);
                throw;
            }
            catch (Exception exception)
            {
                ShutdownService(initializingService);
                ShutdownInitializedServices();
                if (cancellationToken.IsCancellationRequested)
                {
                    SetState(ApplicationState.Shutdown);
                    throw new OperationCanceledException(cancellationToken);
                }

                FailureCode = exception.GetType().Name;
                _logger.Write(new AppLogEntry(AppLogLevel.Error, "Bootstrap", "InitializationFailed", FailureCode));
                SetState(ApplicationState.Failed);
                throw;
            }
        }

        private async Task CompleteInitializationAsync(
            TaskCompletionSource<bool> completion,
            CancellationTokenSource initializationCancellation)
        {
            try
            {
                await InitializeCoreAsync(initializationCancellation.Token);
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
            finally
            {
                if (ReferenceEquals(_initializationCancellation, initializationCancellation))
                {
                    _initializationCancellation = null;
                }

                initializationCancellation.Dispose();
            }
        }

        private void ShutdownInitializedServices()
        {
            for (int index = _initializedServices.Count - 1; index >= 0; index--)
            {
                ShutdownService(_initializedServices[index]);
            }

            _initializedServices.Clear();
        }

        private void ShutdownService(IApplicationService service)
        {
            if (service == null)
            {
                return;
            }

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
