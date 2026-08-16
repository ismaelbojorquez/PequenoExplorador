using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Logging;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Save
{
    public sealed class AutosaveCoordinator : IDisposable
    {
        private readonly object _gate = new object();
        private readonly ISaveService _saveService;
        private readonly IAppLogger _logger;
        private readonly TimeSpan _debounce;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();
        private TaskCompletionSource<bool> _flushSignal = NewSignal();
        private PlayerProgress _pending;
        private Task _worker = Task.CompletedTask;
        private bool _workerRunning;
        private bool _disposed;

        public AutosaveCoordinator(ISaveService saveService, IAppLogger logger, TimeSpan debounce)
        {
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (debounce < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(debounce));
            }

            _debounce = debounce;
        }

        public PlayerProgress Latest
        {
            get
            {
                lock (_gate)
                {
                    ThrowIfDisposed();
                    return _pending ?? _saveService.Current;
                }
            }
        }

        public void RequestCheckpoint(PlayerProgress progress)
        {
            if (progress == null)
            {
                throw new ArgumentNullException(nameof(progress));
            }

            lock (_gate)
            {
                ThrowIfDisposed();
                _pending = progress;
                if (_workerRunning)
                {
                    return;
                }

                _workerRunning = true;
                _worker = RunWorkerAsync(_lifetime.Token);
            }
        }

        public async Task FlushAsync(CancellationToken cancellationToken)
        {
            Task worker;
            lock (_gate)
            {
                ThrowIfDisposed();
                if (!_workerRunning)
                {
                    return;
                }

                _flushSignal.TrySetResult(true);
                worker = _worker;
            }

            Task cancellation = Task.Delay(Timeout.Infinite, cancellationToken);
            Task completed = await Task.WhenAny(worker, cancellation);
            if (completed != worker)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            await worker;
        }

        public void Dispose()
        {
            lock (_gate)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _lifetime.Cancel();
                _flushSignal.TrySetResult(true);
            }

            _lifetime.Dispose();
        }

        private async Task RunWorkerAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    Task flush;
                    lock (_gate)
                    {
                        flush = _flushSignal.Task;
                    }

                    await Task.WhenAny(Task.Delay(_debounce, cancellationToken), flush);
                    cancellationToken.ThrowIfCancellationRequested();

                    PlayerProgress progress;
                    lock (_gate)
                    {
                        progress = _pending;
                        _pending = null;
                        _flushSignal = NewSignal();
                    }

                    if (progress != null)
                    {
                        SaveOperationResult result = await _saveService.SaveAsync(progress, cancellationToken);
                        if (!result.IsSuccess)
                        {
                            _logger.Write(new AppLogEntry(
                                AppLogLevel.Warning,
                                "Save",
                                "AutosaveDeferred",
                                result.ErrorCode));
                        }
                    }

                    lock (_gate)
                    {
                        if (_pending != null)
                        {
                            continue;
                        }

                        _workerRunning = false;
                        return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                lock (_gate)
                {
                    _workerRunning = false;
                }
            }
            catch (Exception exception)
            {
                _logger.Write(new AppLogEntry(
                    AppLogLevel.Error,
                    "Save",
                    "AutosaveFailed",
                    exception.GetType().Name));
                lock (_gate)
                {
                    _workerRunning = false;
                }
            }
        }

        private static TaskCompletionSource<bool> NewSignal()
        {
            return new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(AutosaveCoordinator));
            }
        }
    }
}
