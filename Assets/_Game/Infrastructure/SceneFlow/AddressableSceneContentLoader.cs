using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.SceneFlow;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace PequenoExplorador.Infrastructure.SceneFlow
{
    public sealed class AddressableSceneContentLoader : ISceneContentLoader
    {
        private readonly object _gate = new object();
        private readonly HashSet<AddressableSceneHandle> _owned = new HashSet<AddressableSceneHandle>();
#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
        private readonly DevelopmentSceneLoadFailure _failure;

        public AddressableSceneContentLoader(DevelopmentSceneLoadFailure failure)
        {
            _failure = failure;
        }
#else
        public AddressableSceneContentLoader()
        {
        }
#endif

        public int ActiveHandleCount
        {
            get
            {
                lock (_gate)
                {
                    return _owned.Count;
                }
            }
        }

        public async Task<ISceneContentHandle> LoadAsync(
            SceneContentId contentId,
            IProgress<float> progress,
            CancellationToken cancellationToken)
        {
#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
            if (_failure != null && _failure.Consume(contentId))
            {
                throw new InvalidOperationException("SimulatedDevelopmentSceneLoadFailure");
            }
#endif
            cancellationToken.ThrowIfCancellationRequested();
            string address = LocalSceneAddresses.For(contentId);
            AsyncOperationHandle<SceneInstance> operation = Addressables.LoadSceneAsync(
                (object)address,
                LoadSceneMode.Additive,
                true,
                100,
                SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
            bool canceled = false;

            while (!operation.IsDone)
            {
                canceled |= cancellationToken.IsCancellationRequested;
                progress?.Report(operation.PercentComplete);
                await Task.Yield();
            }

            progress?.Report(1f);
            if (operation.Status != AsyncOperationStatus.Succeeded)
            {
                Exception cause = operation.OperationException;
                if (operation.IsValid())
                {
                    Addressables.Release(operation);
                }

                if (canceled || cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(cancellationToken);
                }

                throw new InvalidOperationException("Addressable scene load failed.", cause);
            }

            var handle = new AddressableSceneHandle(contentId, operation);
            lock (_gate)
            {
                _owned.Add(handle);
            }

            if (canceled || cancellationToken.IsCancellationRequested)
            {
                await UnloadAsync(handle, CancellationToken.None);
                throw new OperationCanceledException(cancellationToken);
            }

            return handle;
        }

        public async Task UnloadAsync(ISceneContentHandle handle, CancellationToken cancellationToken)
        {
            if (!(handle is AddressableSceneHandle addressableHandle))
            {
                throw new ArgumentException("Handle was not created by this Addressables adapter.", nameof(handle));
            }

            if (!addressableHandle.TryBeginRelease())
            {
                return;
            }

            if (!addressableHandle.Operation.IsValid())
            {
                RemoveOwned(addressableHandle);
                return;
            }

            if (!addressableHandle.Operation.Result.Scene.isLoaded)
            {
                Addressables.Release(addressableHandle.Operation);
                RemoveOwned(addressableHandle);
                return;
            }

            AsyncOperationHandle<SceneInstance> operation = Addressables.UnloadSceneAsync(
                addressableHandle.Operation,
                autoReleaseHandle: false);
            while (!operation.IsDone)
            {
                await Task.Yield();
            }

            AsyncOperationStatus status = operation.Status;
            Exception failure = operation.OperationException;
            Addressables.Release(operation);
            RemoveOwned(addressableHandle);

            if (status != AsyncOperationStatus.Succeeded)
            {
                throw new InvalidOperationException(
                    "Addressable scene unload failed.",
                    failure);
            }
        }

        private void RemoveOwned(AddressableSceneHandle handle)
        {
            lock (_gate)
            {
                _owned.Remove(handle);
            }
        }

        private sealed class AddressableSceneHandle : ISceneContentHandle
        {
            private int _releaseStarted;

            public AddressableSceneHandle(
                SceneContentId contentId,
                AsyncOperationHandle<SceneInstance> operation)
            {
                ContentId = contentId;
                Operation = operation;
            }

            public SceneContentId ContentId { get; }
            public bool IsReleased => Volatile.Read(ref _releaseStarted) != 0;
            public AsyncOperationHandle<SceneInstance> Operation { get; }

            public bool TryBeginRelease()
            {
                return Interlocked.Exchange(ref _releaseStarted, 1) == 0;
            }
        }
    }
}
