using System;
using NUnit.Framework;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Tutorial;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Infrastructure.Save;
using UnityEngine;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class TutorialCoordinatorTests
    {
        [Test]
        public void SemanticStepsAdvanceOnceAndCompleteInOrder()
        {
            var repository = new MemoryRepository();
            var coordinator = Create(repository);
            coordinator.Initialize();
            Assert.That(coordinator.Snapshot.NeedsGuideChoice, Is.True);
            coordinator.SelectGuidance(GuidanceMode.Standard);
            Assert.That(coordinator.Signal(TutorialTrigger.MovementAccepted), Is.False, "Wrong actions cannot skip a step.");
            foreach (TutorialTrigger trigger in Triggers)
            {
                int before = coordinator.Snapshot.Progress.StepIndex;
                Assert.That(coordinator.Signal(trigger), Is.True);
                Assert.That(coordinator.Signal(trigger), Is.False, "Duplicate semantic outcomes are idempotent.");
                Assert.That(coordinator.Snapshot.Progress.StepIndex, Is.GreaterThan(before));
            }
            Assert.That(coordinator.Snapshot.Progress.Status, Is.EqualTo(TutorialProgressStatus.Completed));
        }

        [Test]
        public void GuidanceControlsHelpTimingWithoutChangingContent()
        {
            var repository = new MemoryRepository();
            var coordinator = Create(repository); coordinator.Initialize(); coordinator.SelectGuidance(GuidanceMode.MoreGuidance);
            coordinator.Tick(5.9); Assert.That(coordinator.Snapshot.HelpLevel, Is.Zero);
            coordinator.Tick(.2); Assert.That(coordinator.Snapshot.HelpLevel, Is.EqualTo(1));
            Assert.That(coordinator.Snapshot.Step.Id, Is.EqualTo("tutorial-step.0"));
            Assert.That(repository.Current.Preferences.GuidanceMode, Is.EqualTo(GuidanceMode.MoreGuidance));
        }

        [Test]
        public void AllowedActionGateNeverBlocksBackOrPauseBecauseTheyAreOutsideTheGate()
        {
            var coordinator = Create(new MemoryRepository()); coordinator.Initialize(); coordinator.SelectGuidance(GuidanceMode.Standard);
            Assert.That(coordinator.Allows(TutorialAction.EnterExpedition), Is.True);
            Assert.That(coordinator.Allows(TutorialAction.Move), Is.False);
            coordinator.Skip();
            Assert.That(coordinator.Allows(TutorialAction.Move), Is.True);
            Assert.That(coordinator.Snapshot.Progress.Status, Is.EqualTo(TutorialProgressStatus.Skipped));
        }

        [Test]
        public void ResumeAndReplayPreserveOnlyBoundedState()
        {
            var repository = new MemoryRepository();
            var first = Create(repository); first.Initialize(); first.SelectGuidance(GuidanceMode.Standard);
            first.Signal(TutorialTrigger.ExpeditionEntered); first.Signal(TutorialTrigger.MovementAccepted);
            var resumed = Create(repository); resumed.Initialize();
            Assert.That(resumed.Snapshot.Progress.StepIndex, Is.EqualTo(2));
            Assert.That(resumed.Snapshot.Step.Id, Is.EqualTo("tutorial-step.2"));
            resumed.Replay();
            Assert.That(resumed.Snapshot.Progress.StepIndex, Is.Zero);
            Assert.That(resumed.Snapshot.Progress.Status, Is.EqualTo(TutorialProgressStatus.InProgress));
        }

        [Test]
        public void ContentVersionChangeInvalidatesTutorialWithoutTouchingOtherProgress()
        {
            var repository = new MemoryRepository
            {
                Current = PlayerProgress.CreateDefault().WithStars(9).WithTutorialState(
                    new TutorialProgress("tutorial.vertical-slice", 99, 4, TutorialProgressStatus.InProgress))
            };
            var coordinator = Create(repository); coordinator.Initialize();
            Assert.That(coordinator.Snapshot.NeedsGuideChoice, Is.True);
            Assert.That(coordinator.Snapshot.Progress.ContentVersion, Is.EqualTo(1));
            Assert.That(repository.Current.Stars, Is.EqualTo(9));
        }

        [Test]
        public void V11MigrationAddsNotStartedTutorialAndPreservesCustomization()
        {
            PlayerProgressV11Dto source = PlayerProgressV11Dto.Create("0.1", 4, Array.Empty<string>(),
                Array.Empty<DiscoveryProgressV4Dto>(), Array.Empty<string>(), Array.Empty<PhotoProgressV6Dto>(),
                Array.Empty<string>(), PlayerPreferencesV3Dto.Create(0, "es", 1, 1, 1, 1, 1, true),
                Array.Empty<string>(), Array.Empty<EconomyLedgerEntryV7Dto>(), Array.Empty<MissionProgressV8Dto>(),
                Array.Empty<string>(), 0, Array.Empty<LearningSessionV9Dto>(), Array.Empty<LearningConceptDailyV9Dto>(),
                Array.Empty<string>(), new[] { "cosmetic.hat.sun" },
                new[] { EquippedCosmeticV11Dto.Create("customization-slot.hat", "cosmetic.hat.sun") }, SaveMetadataV1Dto.Create(3));
            PlayerProgressV12Dto migrated = JsonUtility.FromJson<PlayerProgressV12Dto>(
                new V11ToV12TutorialMigration().Migrate(JsonUtility.ToJson(source)));
            Assert.That(migrated.Tutorial.TutorialId, Is.EqualTo("tutorial.vertical-slice"));
            Assert.That(migrated.Tutorial.ContentVersion, Is.Zero);
            Assert.That(migrated.Tutorial.Status, Is.Zero);
            Assert.That(migrated.UnlockedCosmeticIds, Is.EqualTo(new[] { "cosmetic.hat.sun" }));
            Assert.That(migrated.Stars, Is.EqualTo(4));
        }

        [Test]
        public void CurrentSerializerRoundTripsTutorialVersionAndMidStep()
        {
            var serializer = new UnityJsonSaveSerializer();
            PlayerProgress expected = PlayerProgress.CreateDefault().WithTutorialState(
                new TutorialProgress("tutorial.vertical-slice", 1, 2, TutorialProgressStatus.InProgress));
            SaveEnvelopeData envelope = serializer.DeserializeEnvelope(serializer.Serialize(expected, "0.1", 2));
            serializer.ValidateChecksum(envelope);
            DecodedSaveData decoded = serializer.DeserializeCurrentPayload(envelope.Payload);
            Assert.That(decoded.Progress.Tutorial.TutorialId, Is.EqualTo("tutorial.vertical-slice"));
            Assert.That(decoded.Progress.Tutorial.ContentVersion, Is.EqualTo(1));
            Assert.That(decoded.Progress.Tutorial.StepIndex, Is.EqualTo(2));
            Assert.That(decoded.Progress.Tutorial.Status, Is.EqualTo(TutorialProgressStatus.InProgress));
        }

        private static readonly TutorialTrigger[] Triggers = { TutorialTrigger.ExpeditionEntered, TutorialTrigger.MovementAccepted,
            TutorialTrigger.InteractionCompleted, TutorialTrigger.PhotoCaptured, TutorialTrigger.Continue,
            TutorialTrigger.CampReturned, TutorialTrigger.AlbumOpened };

        private static TutorialCoordinator Create(MemoryRepository repository)
        {
            var steps = new TutorialStepDefinition[Triggers.Length];
            for (int index = 0; index < steps.Length; index++) steps[index] = new TutorialStepDefinition(
                "tutorial-step." + index, Triggers[index], (TutorialAction)(1 << index), (TutorialSpotlight)index,
                new LocalizedKey("UI", "ui.tutorial.step." + index), new AudioCueId("audio.voice.tutorial.step-" + index), 12, 6);
            return new TutorialCoordinator(new TutorialDefinition("tutorial.vertical-slice", 1, steps), repository);
        }

        private sealed class MemoryRepository : ITutorialProgressRepository
        {
            public PlayerProgress Current { get; set; } = PlayerProgress.CreateDefault();
            public void Commit(PlayerProgress progress) => Current = progress;
        }
    }
}
