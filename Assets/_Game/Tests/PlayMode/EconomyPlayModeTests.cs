using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Missions;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Infrastructure.Save;
using PequenoExplorador.Presentation.Economy;
using UnityEngine;
using UnityEngine.TestTools;

namespace PequenoExplorador.Tests.PlayMode
{
    public sealed class EconomyPlayModeTests
    {
        private static readonly DiscoveryId Toucan = DiscoveryId.Parse("discovery.jungle.keel-billed-toucan");

        [UnityTest]
        public IEnumerator PhotoDiscoveryAndMissionGrantOncePersistAndReduceMotionIsExplicit()
        {
            var store = new MemoryFileStore();
            LocalSaveService save = CreateSave(store);
            yield return Wait(save.InitializeAsync(CancellationToken.None));
            var autosave = new AutosaveCoordinator(save, new SilentLogger(), TimeSpan.Zero);
            var discoveryRepository = new PlayerProgressDiscoveryRepository(save, autosave);
            var photoRepository = new PlayerProgressPhotoRepository(save, autosave);
            var economyRepository = new PlayerProgressEconomyRepository(save, autosave);
            ContentCatalog content = Content();
            var rewards = new RewardCatalog(new[]
            {
                new RewardDefinition(RewardId.Parse("reward.discovery.keel-billed-toucan.first"), new ExplorerStars(1), RewardSourceKind.Discovery, Toucan.Value),
                new RewardDefinition(RewardId.Parse("reward.mission.photograph-toucan.complete"), new ExplorerStars(2), RewardSourceKind.Mission, "mission.vertical-slice.photograph-toucan")
            });
            var missionRepository = new PlayerProgressMissionRepository(save, autosave);
            var missions = new MissionCoordinator(Missions(), new MissionObjectiveStrategyRegistry(new IMissionObjectiveStrategy[]
                { new DiscoverCountObjectiveStrategy(), new PhotographSpecificObjectiveStrategy(), new InteractTagObjectiveStrategy() }),
                missionRepository, new GrantRewardUseCase(rewards, economyRepository));
            Assert.That(missions.Activate(MissionId.Parse("mission.vertical-slice.photograph-toucan")).Outcome, Is.EqualTo(MissionActivationOutcome.Activated));
            var discover = new DiscoverUseCase(content, discoveryRepository,
                new ManualClock(new DateTimeOffset(2026, 8, 17, 1, 0, 0, TimeSpan.Zero)), false, TimeSpan.Zero);
            var capture = new CapturePhotoUseCase(new PhotoTargetEvaluator(), new Renderer(), new PhotoStore(), photoRepository,
                discover, rewards, new GrantRewardUseCase(rewards, economyRepository), missions);
            var target = new Photographable();
            Task<PhotoCaptureResult> first = capture.ExecuteAsync(target, "playmode-1", CancellationToken.None);
            yield return Wait(first);
            Task<PhotoCaptureResult> retry = capture.ExecuteAsync(target, "playmode-2", CancellationToken.None);
            yield return Wait(retry);
            Assert.That(first.Result.Reward.Outcome, Is.EqualTo(GrantRewardOutcome.Granted));
            Assert.That(retry.Result.Reward.Outcome, Is.EqualTo(GrantRewardOutcome.AlreadyProcessed));
            Assert.That(first.Result.Mission.Outcome, Is.EqualTo(MissionFactOutcome.Completed));
            Assert.That(retry.Result.Mission.Outcome, Is.EqualTo(MissionFactOutcome.Ignored));
            Assert.That(autosave.Latest.Stars, Is.EqualTo(3));
            yield return Wait(autosave.FlushAsync(CancellationToken.None));

            LocalSaveService reloaded = CreateSave(store);
            yield return Wait(reloaded.InitializeAsync(CancellationToken.None));
            Assert.That(reloaded.Current.Stars, Is.EqualTo(3));
            Assert.That(reloaded.Current.ProcessedEconomyTransactionIds.Count, Is.EqualTo(2));
            Assert.That(reloaded.Current.CompletedMissionIds, Does.Contain("mission.vertical-slice.photograph-toucan"));
            Assert.That(reloaded.Current.Missions.Single().IsCompleted, Is.True);
            var go = new GameObject("EconomyViewReduceMotionTest");
            try { EconomyView view = go.AddComponent<EconomyView>(); view.SetReduceMotion(true); Assert.That(view.ReduceMotionEnabled, Is.True); }
            finally { UnityEngine.Object.DestroyImmediate(go); autosave.Dispose(); }
        }

        private static MissionCatalog Missions()
        {
            var objective = new MissionObjectiveDefinition(MissionObjectiveId.Parse("mission-objective.photograph-toucan"),
                MissionObjectiveTypeIds.PhotographSpecific, LocalizationKeys.MissionPhotographToucanObjective, 1, Toucan.Value, default);
            return new MissionCatalog(new[] { new MissionDefinition(MissionId.Parse("mission.vertical-slice.photograph-toucan"),
                LocalizationKeys.MissionPhotographToucanTitle, LocalizationKeys.MissionPhotographToucanSummary,
                LocalizationKeys.MissionPhotographToucanCompletion, new[] { objective }, Array.Empty<MissionId>(),
                RewardId.Parse("reward.mission.photograph-toucan.complete"),
                new EditorialMetadata(EditorialState.Approved, false, "Tests", string.Empty)) });
        }

        private static ContentCatalog Content()
        {
            var definition = new DiscoveryDefinition(Toucan, WorldId.Parse("world.jungle"), CategoryId.Parse("category.animals"),
                Array.Empty<TagId>(), Array.Empty<EducationalFactId>(), LocalizationKeys.KeelBilledToucanName,
                AudioCueIds.ConfirmFeedback, VisualAssetId.Parse("visual.discovery.toucan"),
                new EditorialMetadata(EditorialState.Approved, false, "Tests", string.Empty));
            return new ContentCatalog(new[] { definition }, Array.Empty<DiscoveryIdAlias>());
        }

        private static LocalSaveService CreateSave(IFileStore store) => new LocalSaveService(store, "0.1-test", new SilentLogger(),
            new ISaveMigration[] { new LegacyV0ToV1Migration(), new V1ToV2LocalizationMigration(), new V2ToV3AudioMigration(),
                new V3ToV4DiscoveryMigration(), new V4ToV5ToucanDiscoveryMigration(), new V5ToV6PhotoProgressMigration(),
                new V6ToV7EconomyMigration(), new V7ToV8MissionMigration(), new V8ToV9LearningMigration(),
                new V9ToV10CampMigration(), new V10ToV11CustomizationMigration() });
        private static IEnumerator Wait(Task task)
        {
            float deadline = Time.realtimeSinceStartup + 15f;
            while (!task.IsCompleted && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(task.IsCompleted, Is.True); if (task.IsFaulted) Assert.Fail(task.Exception?.ToString());
        }
        private sealed class SilentLogger : PequenoExplorador.Application.Logging.IAppLogger { public void Write(PequenoExplorador.Application.Logging.AppLogEntry entry) { } }
        private sealed class ManualClock : IClock
        {
            public ManualClock(DateTimeOffset now) => UtcNow = now;
            public DateTimeOffset UtcNow { get; }
        }
        private sealed class Photographable : IPhotographable
        {
            public bool IsAlive => true;
            public PhotoTarget Target { get; } = new PhotoTarget(Toucan, PhotoEvaluationSettings.ChildFriendlyDefault);
            public PhotoFrameSample Sample() => new PhotoFrameSample(0.3f, 2f, true, 0.01f, 1f);
        }
        private sealed class Renderer : IPhotoThumbnailRenderer
        { public Task<PhotoThumbnail> CaptureAsync(CancellationToken token) => Task.FromResult(new PhotoThumbnail(new byte[] { 1 }, 64, 64)); }
        private sealed class PhotoStore : IPhotoStore
        {
            public string ServiceId => "Photos"; public Task InitializeAsync(CancellationToken token) => Task.CompletedTask; public void Shutdown() { }
            public Task<PhotoStoreResult> SaveAsync(DiscoveryId id, int score, PhotoThumbnail thumbnail, CancellationToken token) => Task.FromResult(new PhotoStoreResult("toucan.png", 1));
            public Task<PhotoLoadResult> LoadAsync(string fileReference, CancellationToken token) => Task.FromResult(PhotoLoadResult.Missing());
            public Task DeleteAllAsync(CancellationToken token) => Task.CompletedTask;
        }
        private sealed class MemoryFileStore : IFileStore
        {
            private string _primary; private string _backup; private string _temporary;
            public Task<bool> ExistsAsync(SaveFileKind kind, CancellationToken token) { token.ThrowIfCancellationRequested(); return Task.FromResult((kind == SaveFileKind.Primary ? _primary : _backup) != null); }
            public Task<string> ReadTextAsync(SaveFileKind kind, CancellationToken token) { token.ThrowIfCancellationRequested(); string value = kind == SaveFileKind.Primary ? _primary : _backup; if (value == null) throw new FileNotFoundException(); return Task.FromResult(value); }
            public Task WriteTemporaryAsync(string content, CancellationToken token) { token.ThrowIfCancellationRequested(); _temporary = content; return Task.CompletedTask; }
            public Task FlushTemporaryAsync(CancellationToken token) { token.ThrowIfCancellationRequested(); return Task.CompletedTask; }
            public void CommitTemporary(SaveCommitMode mode) { if (mode == SaveCommitMode.RotatePrimaryToBackup) _backup = _primary; _primary = _temporary; _temporary = null; }
            public Task DiscardTemporaryAsync() { _temporary = null; return Task.CompletedTask; }
            public Task DeleteAllAsync(CancellationToken token) { token.ThrowIfCancellationRequested(); _primary = _backup = _temporary = null; return Task.CompletedTask; }
        }
    }
}
