using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Infrastructure.Photography;
using PequenoExplorador.Tests.EditMode.Fixtures;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class PhotographyTests
    {
        private static readonly DiscoveryId Toucan = DiscoveryId.Parse("discovery.jungle.keel-billed-toucan");
        private static readonly PhotoTarget Target = new PhotoTarget(Toucan, PhotoEvaluationSettings.ChildFriendlyDefault);

        [Test]
        public void EvaluatorUsesCoverageDistanceOcclusionCenterAndOrientationWithGenerousBoundaries()
        {
            var evaluator = new PhotoTargetEvaluator();
            PhotoEvaluation ready = evaluator.Evaluate(Target, new PhotoFrameSample(0.08f, 10f, true, 0.36f, 0.35f));
            PhotoEvaluation far = evaluator.Evaluate(Target, new PhotoFrameSample(0.20f, 10.01f, true, 0.1f, 1f));
            PhotoEvaluation occluded = evaluator.Evaluate(Target, new PhotoFrameSample(0.20f, 4f, false, 0.1f, 1f));
            PhotoEvaluation offCenter = evaluator.Evaluate(Target, new PhotoFrameSample(0.20f, 4f, true, 0.37f, 1f));
            PhotoEvaluation wrongFacing = evaluator.Evaluate(Target, new PhotoFrameSample(0.20f, 4f, true, 0.1f, 0.34f));
            Assert.That(ready.IsReady, Is.True);
            Assert.That(ready.Guidance, Is.EqualTo(PhotoGuidance.Ready));
            Assert.That(far.Guidance, Is.EqualTo(PhotoGuidance.MoveCloser));
            Assert.That(occluded.IsReady || offCenter.IsReady || wrongFacing.IsReady, Is.False);
            Assert.That(ready.ScorePermille, Is.GreaterThan(0));
        }

        [Test]
        public async Task CaptureIsIdempotentKeepsBestAndStorageFailurePreservesDiscovery()
        {
            var repository = new MemoryProgressRepository();
            DiscoverUseCase discover = CreateDiscover(repository);
            var renderer = new FakeRenderer();
            var store = new FakeStore();
            var useCase = new CapturePhotoUseCase(new PhotoTargetEvaluator(), renderer, store, repository, discover);
            var target = new FakePhotographable(new PhotoFrameSample(0.20f, 4f, true, 0.1f, 1f));

            PhotoCaptureResult first = await useCase.ExecuteAsync(target, "capture-1", CancellationToken.None);
            PhotoCaptureResult retry = await useCase.ExecuteAsync(target, "capture-1", CancellationToken.None);
            target.Current = new PhotoFrameSample(0.08f, 10f, true, 0.36f, 0.35f);
            PhotoCaptureResult lower = await useCase.ExecuteAsync(target, "capture-2", CancellationToken.None);
            store.Fail = true;
            target.Current = new PhotoFrameSample(0.45f, 2f, true, 0.02f, 1f);
            PhotoCaptureResult failedStorage = await useCase.ExecuteAsync(target, "capture-3", CancellationToken.None);

            Assert.That(first.Outcome, Is.EqualTo(PhotoCaptureOutcome.CapturedNew));
            Assert.That(retry.Outcome, Is.EqualTo(PhotoCaptureOutcome.ExistingPhotoKept));
            Assert.That(lower.Outcome, Is.EqualTo(PhotoCaptureOutcome.ExistingPhotoKept));
            Assert.That(failedStorage.Outcome, Is.EqualTo(PhotoCaptureOutcome.CapturedWithoutThumbnail));
            Assert.That(repository.Current.Discoveries.Single().Count, Is.EqualTo(3), "Duplicate capture key does not count; two new keys do.");
            Assert.That(repository.Current.Photos.Count, Is.EqualTo(1), "Failed storage keeps the previous best thumbnail metadata.");
            Assert.That(renderer.Calls, Is.EqualTo(2), "Equal/lower scores skip rendering.");
        }

        [Test]
        public async Task CancelledBeforeCaptureDoesNotMutateProgressAndConcurrentShutterReturnsBusy()
        {
            var repository = new MemoryProgressRepository();
            var renderer = new FakeRenderer { Gate = new TaskCompletionSource<bool>() };
            var useCase = new CapturePhotoUseCase(new PhotoTargetEvaluator(), renderer, new FakeStore(), repository, CreateDiscover(repository));
            var target = new FakePhotographable(new PhotoFrameSample(0.30f, 3f, true, 0.05f, 1f));
            var cancelled = new CancellationTokenSource(); cancelled.Cancel();
            PhotoCaptureResult cancelledResult = await useCase.ExecuteAsync(target, "capture-cancel", cancelled.Token);
            Task<PhotoCaptureResult> first = useCase.ExecuteAsync(target, "capture-a", CancellationToken.None);
            await Task.Yield();
            PhotoCaptureResult busy = await useCase.ExecuteAsync(target, "capture-b", CancellationToken.None);
            renderer.Gate.SetResult(true);
            await first;
            Assert.That(cancelledResult.Outcome, Is.EqualTo(PhotoCaptureOutcome.Cancelled));
            Assert.That(busy.Outcome, Is.EqualTo(PhotoCaptureOutcome.Busy));
            Assert.That(repository.Current.Discoveries.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task LocalStoreUsesSafeBoundedFileManifestAndCleansTemporaryFiles()
        {
            string directory = Path.Combine(Path.GetTempPath(), "pe-photo-test-" + Guid.NewGuid().ToString("N"));
            try
            {
                Directory.CreateDirectory(directory);
                File.WriteAllBytes(Path.Combine(directory, "orphan.tmp"), new byte[] { 1 });
                File.WriteAllBytes(Path.Combine(directory, "orphan.png"), new byte[] { 1 });
                var store = new LocalPhotoStore(directory);
                await store.InitializeAsync(CancellationToken.None);
                var thumbnail = new PhotoThumbnail(Enumerable.Repeat((byte)7, 256).ToArray(), 384, 216);
                PhotoStoreResult result = await store.SaveAsync(Toucan, 800, thumbnail, CancellationToken.None);
                PhotoStoreResult improved = await store.SaveAsync(Toucan, 900,
                    new PhotoThumbnail(Enumerable.Repeat((byte)8, 300).ToArray(), 384, 216), CancellationToken.None);
                Assert.That(result.FileReference, Is.EqualTo("discovery_jungle_keel-billed-toucan-800.png"));
                Assert.That(improved.FileReference, Is.EqualTo("discovery_jungle_keel-billed-toucan-900.png"));
                Assert.That(File.Exists(Path.Combine(directory, result.FileReference)), Is.False, "Old best is removed after manifest commit.");
                Assert.That(File.Exists(Path.Combine(directory, improved.FileReference)), Is.True);
                Assert.That(File.Exists(Path.Combine(directory, LocalPhotoStore.ManifestFileName)), Is.True);
                Assert.That(Directory.GetFiles(directory, "*.tmp"), Is.Empty);
                Assert.That(Directory.GetFiles(directory, "*.png").Length, Is.EqualTo(1), "Orphan and retired photo are cleaned.");
                Assert.That(store.EntryCount, Is.EqualTo(1));
                Assert.Throws<ArgumentException>(() => LocalPhotoStore.SafeFileName(default));
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    new PhotoThumbnail(new byte[LocalPhotoStore.MaximumFileBytes + 1], 384, 216));
            }
            finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
        }

        private static DiscoverUseCase CreateDiscover(MemoryProgressRepository repository)
        {
            var definition = new DiscoveryDefinition(Toucan, WorldId.Parse("world.jungle"), CategoryId.Parse("category.animals"),
                Array.Empty<TagId>(), Array.Empty<EducationalFactId>(), LocalizationKeys.KeelBilledToucanName,
                AudioCueIds.ConfirmFeedback, VisualAssetId.Parse("visual.discovery.toucan"),
                new EditorialMetadata(EditorialState.Approved, false, "Tests", string.Empty));
            return new DiscoverUseCase(new ContentCatalog(new[] { definition }, Array.Empty<DiscoveryIdAlias>()), repository,
                new ManualClock(new DateTimeOffset(2026, 8, 16, 12, 0, 0, TimeSpan.Zero)), false, TimeSpan.Zero);
        }

        private sealed class FakePhotographable : IPhotographable
        {
            public FakePhotographable(PhotoFrameSample current) => Current = current;
            public PhotoFrameSample Current { get; set; }
            public PhotoTarget Target => PhotographyTests.Target;
            public bool IsAlive => true;
            public PhotoFrameSample Sample() => Current;
        }

        private sealed class FakeRenderer : IPhotoThumbnailRenderer
        {
            public int Calls { get; private set; }
            public TaskCompletionSource<bool> Gate { get; set; }
            public async Task<PhotoThumbnail> CaptureAsync(CancellationToken cancellationToken)
            {
                Calls++;
                if (Gate != null) await Gate.Task;
                cancellationToken.ThrowIfCancellationRequested();
                return new PhotoThumbnail(new byte[] { 1, 2, 3 }, 384, 216);
            }
        }

        private sealed class FakeStore : IPhotoStore
        {
            public bool Fail { get; set; }
            public string ServiceId => "Photos";
            public Task InitializeAsync(CancellationToken token) => Task.CompletedTask;
            public void Shutdown() { }
            public Task DeleteAllAsync(CancellationToken token) => Task.CompletedTask;
            public Task<PhotoStoreResult> SaveAsync(DiscoveryId id, int score, PhotoThumbnail thumbnail, CancellationToken token)
            {
                if (Fail) throw new IOException("Injected");
                return Task.FromResult(new PhotoStoreResult(LocalPhotoStore.SafeFileName(id, score), thumbnail.PngBytes.Length));
            }
        }

        private sealed class MemoryProgressRepository : IDiscoveryProgressRepository, IPhotoProgressRepository
        {
            public PlayerProgress Current { get; private set; } = PlayerProgress.CreateDefault();
            public bool IsReadOnly => false;
            public void Commit(PlayerProgress progress) => Current = progress;
        }
    }
}
