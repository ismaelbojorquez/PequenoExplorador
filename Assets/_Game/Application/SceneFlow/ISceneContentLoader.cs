using System;
using System.Threading;
using System.Threading.Tasks;

namespace PequenoExplorador.Application.SceneFlow
{
    public interface ISceneContentLoader
    {
        int ActiveHandleCount { get; }

        Task<ISceneContentHandle> LoadAsync(
            SceneContentId contentId,
            IProgress<float> progress,
            CancellationToken cancellationToken);

        Task UnloadAsync(ISceneContentHandle handle, CancellationToken cancellationToken);
    }
}
