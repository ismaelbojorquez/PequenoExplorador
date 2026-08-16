using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Worlds
{
    public sealed class WorldLoadUseCase : IWorldSession
    {
        private readonly IWorldCatalog _catalog;
        private readonly ISceneFlowService _sceneFlow;
        private WorldManifest _pendingWorld;

        public WorldLoadUseCase(IWorldCatalog catalog, ISceneFlowService sceneFlow)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _sceneFlow = sceneFlow ?? throw new ArgumentNullException(nameof(sceneFlow));
        }

        public WorldManifest ActiveWorld { get; private set; }

        public async Task<WorldLoadResult> EnterAsync(WorldId worldId, CancellationToken cancellationToken)
        {
            if (!worldId.IsValid || !_catalog.TryGet(worldId, out WorldCatalogEntry entry))
                return new WorldLoadResult(WorldLoadOutcome.Missing, worldId, "WorldMissing");
            if (entry.Availability != WorldAvailabilityState.Available)
                return new WorldLoadResult(WorldLoadOutcome.Unavailable, worldId, "WorldUnavailable", entry.Manifest);
            if (ActiveWorld != null)
            {
                if (ActiveWorld.Id.Equals(worldId))
                    return new WorldLoadResult(WorldLoadOutcome.AlreadyThere, worldId, manifest: entry.Manifest);
                return new WorldLoadResult(
                    WorldLoadOutcome.Failed,
                    worldId,
                    "ReturnToCampBeforeChangingWorld",
                    entry.Manifest);
            }

            SceneTransitionResult transition = await _sceneFlow.GoToExpeditionAsync(entry.Manifest.Scene, cancellationToken);
            WorldLoadOutcome outcome = Map(transition.Outcome);
            if (transition.IsSuccess) { ActiveWorld = entry.Manifest; _pendingWorld = null; }
            else if (outcome == WorldLoadOutcome.Failed || outcome == WorldLoadOutcome.Canceled) _pendingWorld = entry.Manifest;
            return new WorldLoadResult(outcome, worldId, transition.ErrorCode, entry.Manifest);
        }

        public async Task<WorldLoadResult> ReturnToCampAsync(CancellationToken cancellationToken)
        {
            WorldId prior = ActiveWorld?.Id ?? default;
            SceneTransitionResult transition = await _sceneFlow.GoToCampAsync(cancellationToken);
            WorldLoadOutcome outcome = Map(transition.Outcome);
            if (transition.IsSuccess) { ActiveWorld = null; _pendingWorld = null; }
            return new WorldLoadResult(outcome, prior, transition.ErrorCode);
        }

        public async Task<WorldLoadResult> RetryAsync(CancellationToken cancellationToken)
        {
            if (_pendingWorld == null) return new WorldLoadResult(WorldLoadOutcome.Failed, default, "NoWorldRetryAvailable");
            SceneTransitionResult transition = await _sceneFlow.RetryAsync(cancellationToken);
            WorldManifest pending = _pendingWorld;
            WorldLoadOutcome outcome = Map(transition.Outcome);
            if (transition.IsSuccess) { ActiveWorld = pending; _pendingWorld = null; }
            return new WorldLoadResult(outcome, pending.Id, transition.ErrorCode, pending);
        }

        private static WorldLoadOutcome Map(SceneTransitionOutcome outcome)
        {
            switch (outcome)
            {
                case SceneTransitionOutcome.Succeeded: return WorldLoadOutcome.Succeeded;
                case SceneTransitionOutcome.AlreadyThere: return WorldLoadOutcome.AlreadyThere;
                case SceneTransitionOutcome.Busy: return WorldLoadOutcome.Busy;
                case SceneTransitionOutcome.Canceled:
                case SceneTransitionOutcome.TimedOut: return WorldLoadOutcome.Canceled;
                default: return WorldLoadOutcome.Failed;
            }
        }
    }
}
