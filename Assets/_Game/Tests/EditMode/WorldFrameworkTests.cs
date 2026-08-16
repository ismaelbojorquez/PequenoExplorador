using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Application.Worlds;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Content.Worlds;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Editor;
using PequenoExplorador.Editor.BuildTools;
using PequenoExplorador.Tests.EditMode.Fixtures;
using UnityEditor;
using UnityEngine;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class WorldFrameworkTests
    {
        [Test]
        public void CanonicalJungleManifestMapsAndReleaseRejectsDraft()
        {
            ContentCatalogAsset contentAsset = AssetDatabase.LoadAssetAtPath<ContentCatalogAsset>(ContentFoundationSetup.CatalogPath);
            Assert.That(contentAsset.TryBuildRuntimeCatalog(ContentValidationMode.Development, out ContentCatalog content, out var contentErrors), Is.True, string.Join("\n", contentErrors));
            WorldCatalogAsset worlds = AssetDatabase.LoadAssetAtPath<WorldCatalogAsset>(WorldFoundationSetup.CatalogPath);
            Assert.That(worlds.TryBuildRuntimeCatalog(content, ContentValidationMode.Development, out WorldCatalog catalog, out var errors), Is.True, string.Join("\n", errors));
            Assert.That(catalog.TryGet(WorldId.Parse("world.jungle"), out WorldCatalogEntry jungle), Is.True);
            Assert.That(jungle.Manifest.Scene, Is.EqualTo(SceneContentId.Parse("scene/jungle")));
            Assert.That(jungle.Manifest.ContentCatalogIds.Single(), Is.EqualTo(content.Id));
            Assert.That(jungle.Manifest.SpawnPoint, Is.EqualTo(SpawnPointId.Parse("spawn.jungle.entry")));
            Assert.That(jungle.Manifest.Checkpoints.Single(), Is.EqualTo(CheckpointId.Parse("checkpoint.jungle.entry")));
            Assert.That(WorldCatalogValidationService.Validate(ContentValidationMode.Development, false), Is.Empty);
            Assert.That(WorldCatalogValidationService.Validate(ContentValidationMode.Release, true).Any(error => error.Contains("WORLD018")), Is.True);
            Assert.That(File.ReadAllText("artifacts/reports/world-catalog-release.md"), Does.Contain("`FAIL`"));
        }

        [Test]
        public void DuplicateWorldIdsAreRejectedActionably()
        {
            ContentCatalogAsset contentAsset = AssetDatabase.LoadAssetAtPath<ContentCatalogAsset>(ContentFoundationSetup.CatalogPath);
            contentAsset.TryBuildRuntimeCatalog(ContentValidationMode.Development, out ContentCatalog content, out _);
            WorldCatalogAsset original = AssetDatabase.LoadAssetAtPath<WorldCatalogAsset>(WorldFoundationSetup.CatalogPath);
            WorldCatalogAsset duplicate = UnityEngine.Object.Instantiate(original);
            var serialized = new SerializedObject(duplicate);
            SerializedProperty worlds = serialized.FindProperty("_worlds");
            worlds.arraySize = 2;
            worlds.GetArrayElementAtIndex(1).objectReferenceValue = worlds.GetArrayElementAtIndex(0).objectReferenceValue;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            Assert.That(WorldCatalogCompiler.TryCompile(duplicate, content, ContentValidationMode.Development, null, out _, out var errors), Is.False);
            Assert.That(errors.Any(error => error.Contains("WORLD004") && error.Contains("world.jungle")), Is.True);
            UnityEngine.Object.DestroyImmediate(duplicate);
        }

        [Test]
        public async Task FakeSecondWorldLoadsWithoutChangingCoordinator()
        {
            WorldManifest jungle = Manifest("world.jungle", "scene/jungle");
            WorldManifest fake = Manifest("world.test-ocean", "scene/test-ocean");
            var catalog = new WorldCatalog(new[]
            {
                new WorldCatalogEntry(jungle, WorldAvailabilityState.Available),
                new WorldCatalogEntry(fake, WorldAvailabilityState.Available)
            });
            var loader = new FakeSceneContentLoader();
            var flow = new SceneFlowService(loader, new RecordingLogger(), TimeSpan.FromSeconds(1), SceneContentId.Parse("scene/camp"));
            var useCase = new WorldLoadUseCase(catalog, flow);
            await flow.GoToCampAsync(CancellationToken.None);

            WorldLoadResult jungleResult = await useCase.EnterAsync(jungle.Id, CancellationToken.None);
            int loadsWithJungleActive = loader.LoadCount;
            WorldLoadResult directSwitch = await useCase.EnterAsync(fake.Id, CancellationToken.None);
            await useCase.ReturnToCampAsync(CancellationToken.None);
            WorldLoadResult result = await useCase.EnterAsync(fake.Id, CancellationToken.None);

            Assert.That(jungleResult.Outcome, Is.EqualTo(WorldLoadOutcome.Succeeded));
            Assert.That(directSwitch.Outcome, Is.EqualTo(WorldLoadOutcome.Failed));
            Assert.That(directSwitch.ErrorCode, Is.EqualTo("ReturnToCampBeforeChangingWorld"));
            Assert.That(loader.LoadCount, Is.EqualTo(loadsWithJungleActive + 2),
                "Only the explicit Camp return and fake-world entry may add loads.");
            Assert.That(result.Outcome, Is.EqualTo(WorldLoadOutcome.Succeeded));
            Assert.That(useCase.ActiveWorld, Is.SameAs(fake));
            Assert.That(loader.LastLoadedContent, Is.EqualTo(SceneContentId.Parse("scene/test-ocean")));
            Assert.That(catalog.Worlds.Select(entry => entry.Manifest.Id.Value), Is.EqualTo(new[] { "world.jungle", "world.test-ocean" }));
        }

        [Test]
        public async Task LockedAndMissingWorldsDoNotLoadOrMutateProgress()
        {
            WorldManifest locked = Manifest("world.test-locked", "scene/test-locked");
            var catalog = new WorldCatalog(new[] { new WorldCatalogEntry(locked, WorldAvailabilityState.Locked) });
            var loader = new FakeSceneContentLoader();
            var flow = new SceneFlowService(loader, new RecordingLogger(), TimeSpan.FromSeconds(1), SceneContentId.Parse("scene/camp"));
            var useCase = new WorldLoadUseCase(catalog, flow);
            PlayerProgress progress = PlayerProgress.CreateDefault();
            int originalStars = progress.Stars;
            string[] originalWorldIds = progress.WorldIds.ToArray();
            await flow.GoToCampAsync(CancellationToken.None);
            int loadsAfterCamp = loader.LoadCount;

            WorldLoadResult unavailable = await useCase.EnterAsync(locked.Id, CancellationToken.None);
            WorldLoadResult missing = await useCase.EnterAsync(WorldId.Parse("world.retired"), CancellationToken.None);

            Assert.That(unavailable.Outcome, Is.EqualTo(WorldLoadOutcome.Unavailable));
            Assert.That(missing.Outcome, Is.EqualTo(WorldLoadOutcome.Missing));
            Assert.That(loader.LoadCount, Is.EqualTo(loadsAfterCamp));
            Assert.That(useCase.ActiveWorld, Is.Null);
            Assert.That(progress.Stars, Is.EqualTo(originalStars));
            Assert.That(progress.WorldIds, Is.EqualTo(originalWorldIds),
                "World lookup must not rewrite save/progress for retired IDs.");
        }

        private static WorldManifest Manifest(string worldId, string scene) => new WorldManifest(
            WorldId.Parse(worldId), 1, "test", LocalizationKeys.WorldJungle, SceneContentId.Parse(scene),
            new[] { "scene", "world-test" }, SpawnPointId.Parse("spawn.test.entry"),
            new[] { CheckpointId.Parse("checkpoint.test.entry") },
            new[] { ContentCatalogId.Parse("catalog.test") }, new AudioCueId("audio.music.test"),
            new AudioCueId("audio.ambience.test"), Array.Empty<WorldRequirementId>(), 1024,
            new EditorialMetadata(EditorialState.Approved, false, "Test", string.Empty));
    }
}
