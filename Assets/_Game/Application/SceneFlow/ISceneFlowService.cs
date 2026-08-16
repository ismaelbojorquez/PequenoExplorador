using System;
using System.Threading;
using System.Threading.Tasks;

namespace PequenoExplorador.Application.SceneFlow
{
    public interface ISceneFlowService
    {
        event Action<SceneFlowSnapshot> Changed;

        SceneFlowSnapshot Snapshot { get; }

        Task<SceneTransitionResult> GoToCampAsync(CancellationToken cancellationToken);
        Task<SceneTransitionResult> GoToExpeditionAsync(SceneContentId contentId, CancellationToken cancellationToken);
        Task<SceneTransitionResult> RetryAsync(CancellationToken cancellationToken);
        Task ShutdownAsync();
    }
}
