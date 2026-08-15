using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.SceneFlow;

namespace PequenoExplorador.Tests.EditMode.Fixtures
{
    internal sealed class FakeSceneContentLoader : ISceneContentLoader
    {
        private readonly List<FakeHandle> _active = new List<FakeHandle>();

        public Func<SceneContentId, CancellationToken, Task> BeforeLoad { get; set; }
        public bool FailNextLoad { get; set; }
        public int LoadCount { get; private set; }
        public int UnloadCount { get; private set; }
        public int ActiveHandleCount => _active.Count;

        public async Task<ISceneContentHandle> LoadAsync(
            SceneContentId contentId,
            IProgress<float> progress,
            CancellationToken cancellationToken)
        {
            if (BeforeLoad != null)
            {
                await BeforeLoad(contentId, cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (FailNextLoad)
            {
                FailNextLoad = false;
                throw new InvalidOperationException("ControlledFakeLoadFailure");
            }

            LoadCount++;
            progress?.Report(1f);
            var handle = new FakeHandle(contentId);
            _active.Add(handle);
            return handle;
        }

        public Task UnloadAsync(ISceneContentHandle handle, CancellationToken cancellationToken)
        {
            var fake = (FakeHandle)handle;
            if (fake.Release())
            {
                _active.Remove(fake);
                UnloadCount++;
            }

            return Task.CompletedTask;
        }

        private sealed class FakeHandle : ISceneContentHandle
        {
            public FakeHandle(SceneContentId contentId)
            {
                ContentId = contentId;
            }

            public SceneContentId ContentId { get; }
            public bool IsReleased { get; private set; }

            public bool Release()
            {
                if (IsReleased)
                {
                    return false;
                }

                IsReleased = true;
                return true;
            }
        }
    }
}
