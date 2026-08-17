using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.Learning;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Application.Tutorial;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Presentation.Camp;
using PequenoExplorador.Presentation.Interaction;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace PequenoExplorador.Tests.PlayMode
{
    public sealed class VerticalSliceJourneyPlayModeTests
    {
        [UnityTest]
        public IEnumerator GuidedJourneyAndRepeatSessionPersistRecoverAndDoNotDuplicateRewards()
        {
            float journeyStarted = Time.realtimeSinceStartup;
            long memoryStarted = Profiler.GetTotalAllocatedMemoryLong();
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Assert.That(bootstrap.VerticalSliceBuildMarker, Is.EqualTo(VerticalSliceBuildInfo.Marker));
            Assert.That(VerticalSliceBuildInfo.JourneyVersion, Is.EqualTo(1));

            Task reset = bootstrap.ResetProgressForTestsAsync(CancellationToken.None);
            yield return WaitFor(reset);
            bootstrap.Tutorial.Initialize();
            Assert.That(bootstrap.TutorialView.IsGuideChoiceVisible, Is.True);
            bootstrap.TutorialView.MoreGuidanceButton.onClick.Invoke();
            yield return null;

            InvokeCampStation(bootstrap, "camp-station.expedition");
            yield return WaitForScene(bootstrap, SceneFlowState.Expedition);
            Assert.That(bootstrap.MissionView.ActivateVisible, Is.False,
                "The Vertical Slice mission activates through the expedition flow, not a debug shortcut.");

            Vector3 movementScreen = Camera.main.WorldToScreenPoint(new Vector3(4f, 0f, 2f));
            Assert.That(bootstrap.ExplorerRoot.TryHandleScreenTap(
                new ScreenPoint(movementScreen.x, movementScreen.y)), Is.True);
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.StepIndex, Is.EqualTo(2));
            yield return WaitForExplorerSettled(bootstrap);

            yield return OpenToucanActivity(bootstrap, "guided");
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.StepIndex, Is.EqualTo(2),
                "The interaction instruction remains until the activity is understood.");
            Assert.That(bootstrap.LearningView.Submit(1).Outcome, Is.EqualTo(ActivityOutcome.TryAgain));
            Assert.That(bootstrap.LearningView.RequestHint().Outcome, Is.EqualTo(ActivityOutcome.Hint));

            Task english = bootstrap.SetLocaleAsync(LocaleCode.English, persist: true, CancellationToken.None);
            yield return WaitFor(english);
            yield return null;
            Assert.That(bootstrap.LearningView.TitleText, Is.EqualTo("What would the toucan choose?"));
            Assert.That(bootstrap.LearningView.Submit(0).Outcome, Is.EqualTo(ActivityOutcome.Completed));
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.StepIndex, Is.EqualTo(3));
            yield return ExitLearningToPhotography(bootstrap);
            yield return CaptureReadyPhoto(bootstrap, 1);

            Assert.That(bootstrap.LastDiscoveryResult.Count, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentProgress.Discoveries.Count, Is.EqualTo(1),
                "Capture orchestration must retain discovery progress after reward and mission side effects.");
            Assert.That(bootstrap.CurrentProgress.Stars, Is.EqualTo(4),
                "Activity, first discovery and mission grant exactly four stars before the Camp spend.");
            Assert.That(bootstrap.MissionView.ActivateVisible, Is.False);
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.StepIndex, Is.EqualTo(4));

            int stepBeforePause = bootstrap.Tutorial.Snapshot.Progress.StepIndex;
            bootstrap.gameObject.SendMessage("OnApplicationPause", true);
            yield return null;
            bootstrap.gameObject.SendMessage("OnApplicationPause", false);
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.StepIndex, Is.EqualTo(stepBeforePause));
            Assert.That(bootstrap.CurrentProgress.Discoveries.Count, Is.EqualTo(1));

            bootstrap.TutorialView.ContinueButton.onClick.Invoke();
            Assert.That(bootstrap.CurrentProgress.Discoveries.Count, Is.EqualTo(1));
            bootstrap.PhotographyView.ExitButton.onClick.Invoke();
            bootstrap.SceneTransitionView.ReturnCampButton.onClick.Invoke();
            yield return WaitForScene(bootstrap, SceneFlowState.Camp);
            Assert.That(bootstrap.CurrentProgress.Discoveries.Count, Is.EqualTo(1),
                "Returning to Camp must preserve discovery progress in the shared checkpoint owner.");

            Task spanish = bootstrap.SetLocaleAsync(LocaleCode.Spanish, persist: true, CancellationToken.None);
            yield return WaitFor(spanish);
            Assert.That(bootstrap.CurrentProgress.Discoveries.Count, Is.EqualTo(1),
                "Persisting a preference must merge with, not replace, cross-feature progress.");
            InvokeCampStation(bootstrap, "camp-station.album");
            yield return null;
            Assert.That(bootstrap.AlbumView.IsVisible, Is.True);
            Assert.That(bootstrap.AlbumView.Snapshot.Discovered, Is.EqualTo(1));
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.Status, Is.EqualTo(TutorialProgressStatus.Completed));

            foreach ((int width, int height) in new[] { (1024, 768), (1280, 720), (1600, 720), (1280, 800) })
            {
                Screen.SetResolution(width, height, FullScreenMode.Windowed);
                yield return null;
                Canvas.ForceUpdateCanvases();
                AssertTargets(bootstrap, width, height);
            }

            Assert.That(bootstrap.AlbumView.TryHandleBack(), Is.True);
            bootstrap.CampHubView.UpgradeButton.onClick.Invoke();
            Assert.That(bootstrap.CampHubView.IsPreviewVisible, Is.True);
            bootstrap.CampHubView.ConfirmUpgradeButton.onClick.Invoke();
            yield return null;
            Assert.That(bootstrap.CampHubView.CurrentUpgradeUnlocked, Is.True);
            Assert.That(bootstrap.CurrentProgress.Stars, Is.EqualTo(1));

            // A later session repeats the discovery loop without tutorial gating or extra rewards.
            Assert.That(bootstrap.TutorialView.IsInstructionVisible, Is.False);
            InvokeCampStation(bootstrap, "camp-station.expedition");
            yield return WaitForScene(bootstrap, SceneFlowState.Expedition);
            yield return OpenToucanActivity(bootstrap, "repeat");
            Assert.That(bootstrap.LearningView.LastOutcome, Is.EqualTo(ActivityOutcome.AlreadyCompleted));
            yield return ExitLearningToPhotography(bootstrap);
            for (int repetition = 0; repetition < 3; repetition++)
                yield return CaptureReadyPhoto(bootstrap, repetition + 2);
            Assert.That(bootstrap.LastDiscoveryResult.Count, Is.EqualTo(4));
            Assert.That(bootstrap.CurrentProgress.Stars, Is.EqualTo(1),
                "Three repeat captures must not duplicate activity, discovery or mission rewards.");
            bootstrap.PhotographyView.ExitButton.onClick.Invoke();
            bootstrap.SceneTransitionView.ReturnCampButton.onClick.Invoke();
            yield return WaitForScene(bootstrap, SceneFlowState.Camp);

            Task flush = bootstrap.FlushSaveAsync(CancellationToken.None);
            yield return WaitFor(flush);
            int previousBootstrap = bootstrap.GetInstanceID();
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return WaitForReady(previousBootstrap);
            bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.Status, Is.EqualTo(TutorialProgressStatus.Completed));
            Assert.That(bootstrap.TutorialView.IsGuideChoiceVisible, Is.False);
            Assert.That(bootstrap.TutorialView.IsInstructionVisible, Is.False);
            Assert.That(bootstrap.CampHubView.CurrentUpgradeUnlocked, Is.True);
            Assert.That(bootstrap.CurrentProgress.Stars, Is.EqualTo(1));

            // Two explicit commits guarantee a valid backup before injecting a truncated primary.
            bootstrap.RequestSaveCheckpoint();
            yield return WaitFor(bootstrap.FlushSaveAsync(CancellationToken.None));
            bootstrap.RequestSaveCheckpoint();
            yield return WaitFor(bootstrap.FlushSaveAsync(CancellationToken.None));
            string saveDirectory = Path.Combine(UnityEngine.Application.persistentDataPath, "Save");
            string primary = Path.Combine(saveDirectory, "player-progress.json");
            string backup = Path.Combine(saveDirectory, "player-progress.backup.json");
            Assert.That(File.Exists(primary), Is.True);
            Assert.That(File.Exists(backup), Is.True);
            File.WriteAllText(primary, "{\"schemaVersion\":12");

            previousBootstrap = bootstrap.GetInstanceID();
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return WaitForReady(previousBootstrap);
            bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Assert.That(bootstrap.SaveLoadResult.UserNotice, Is.EqualTo(SaveUserNotice.ProgressRecovered));
            Assert.That(bootstrap.CampHubView.CurrentUpgradeUnlocked, Is.True);
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.Status, Is.EqualTo(TutorialProgressStatus.Completed));

            const int sampleFrames = 30;
            float sampleStarted = Time.realtimeSinceStartup;
            for (int frame = 0; frame < sampleFrames; frame++) yield return null;
            float sampleSeconds = Time.realtimeSinceStartup - sampleStarted;
            float fps = sampleSeconds <= 0f ? 0f : sampleFrames / sampleSeconds;
            long memoryFinished = Profiler.GetTotalAllocatedMemoryLong();
            Debug.Log($"PE_VERTICAL_SLICE_JOURNEY marker={VerticalSliceBuildInfo.Marker} version={VerticalSliceBuildInfo.JourneyVersion} " +
                      $"elapsedSeconds={Time.realtimeSinceStartup - journeyStarted:0.000} editorBatchFps={fps:0.0} " +
                      $"totalAllocatedDeltaBytes={memoryFinished - memoryStarted} repeats=3 discoveryCount=4 starsAfterUpgrade=1 " +
                      "environment=EditorBatch deviceProfiling=false offline=true");
            Assert.That(float.IsNaN(fps) || float.IsInfinity(fps), Is.False);
            Assert.That(fps, Is.GreaterThan(0f));

            Task cleanup = bootstrap.ResetProgressForTestsAsync(CancellationToken.None);
            yield return WaitFor(cleanup);
        }

        private static IEnumerator OpenToucanActivity(DiagnosticBootstrap bootstrap, string stage)
        {
            WorldInteractableView toucan = bootstrap.InteractionRoot.Targets.Single(item =>
                item.RawInteractionId == "interaction.jungle.keel-billed-toucan");
            Vector3 screen = Camera.main.WorldToScreenPoint(toucan.transform.position + Vector3.up * 0.7f);
            Assert.That(bootstrap.InteractionRoot.TryHandleTap(new ScreenPoint(screen.x, screen.y)), Is.True);
            float deadline = Time.realtimeSinceStartup + 10f;
            while (Time.realtimeSinceStartup < deadline &&
                   bootstrap.InteractionRoot.Coordinator.Snapshot.State != InteractionOutcome.Ready) yield return null;
            Assert.That(bootstrap.InteractionRoot.Coordinator.Snapshot.State, Is.EqualTo(InteractionOutcome.Ready),
                "Toucan interaction did not become ready during " + stage + ".");
            bootstrap.InteractionPrompt.ActionButton.onClick.Invoke();
            yield return null;
            Assert.That(bootstrap.LearningView.IsVisible, Is.True);
        }

        private static IEnumerator ExitLearningToPhotography(DiagnosticBootstrap bootstrap)
        {
            Button exit = bootstrap.LearningView.GetComponentsInChildren<Button>(true)
                .Single(button => button.name == "Exit");
            exit.onClick.Invoke();
            float deadline = Time.realtimeSinceStartup + 3f;
            while (Time.realtimeSinceStartup < deadline && !bootstrap.PhotographyRoot.IsActive) yield return null;
            Assert.That(bootstrap.PhotographyRoot.IsActive, Is.True);
            Assert.That(bootstrap.LearningView.IsVisible, Is.False);
        }

        private static IEnumerator CaptureReadyPhoto(DiagnosticBootstrap bootstrap, int expectedDiscoveryCount)
        {
            bootstrap.PhotographyRoot.ActiveTarget.SetSampleOverrideForEditorAndTests(
                new PhotoFrameSample(0.30f, 3f, true, 0.05f, 1f));
            int attempts = bootstrap.PhotographyRoot.CaptureAttemptCount;
            bootstrap.PhotographyView.ShutterButton.onClick.Invoke();
            float deadline = Time.realtimeSinceStartup + 6f;
            while (Time.realtimeSinceStartup < deadline &&
                   bootstrap.PhotographyRoot.CaptureAttemptCount == attempts) yield return null;
            Assert.That(bootstrap.PhotographyRoot.CaptureAttemptCount, Is.EqualTo(attempts + 1));
            while (Time.realtimeSinceStartup < deadline &&
                   bootstrap.LastDiscoveryResult.Count < expectedDiscoveryCount)
                yield return null;
            Assert.That(bootstrap.PhotographyRoot.LastCapture.ProgressCaptured, Is.True);
            Assert.That(bootstrap.LastDiscoveryResult.Count, Is.EqualTo(expectedDiscoveryCount));
        }

        private static void InvokeCampStation(DiagnosticBootstrap bootstrap, string stationId)
        {
            CampStationButtonView station = bootstrap.CampHubView.StationButtons.Single(item =>
                item.StationId.Value == stationId);
            Button button = station.GetComponentInChildren<Button>(true);
            Assert.That(button, Is.Not.Null);
            Assert.That(button.interactable, Is.True);
            button.onClick.Invoke();
        }

        private static void AssertTargets(DiagnosticBootstrap bootstrap, int width, int height)
        {
            foreach (Button button in bootstrap.AlbumView.GetComponentsInChildren<Button>(true)
                         .Concat(bootstrap.CampHubView.GetComponentsInChildren<Button>(true)))
            {
                Rect rect = ((RectTransform)button.transform).rect;
                Assert.That(rect.width, Is.GreaterThanOrEqualTo(64f), button.name + " width at " + width + "x" + height);
                Assert.That(rect.height, Is.GreaterThanOrEqualTo(64f), button.name + " height at " + width + "x" + height);
            }
        }

        private static IEnumerator WaitForScene(DiagnosticBootstrap bootstrap, SceneFlowState expected)
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (Time.realtimeSinceStartup < deadline &&
                   (bootstrap.SceneFlow.Current != expected || bootstrap.SceneFlow.IsTransitioning)) yield return null;
            Assert.That(bootstrap.SceneFlow.Current, Is.EqualTo(expected));
            Assert.That(bootstrap.SceneFlow.IsTransitioning, Is.False);
        }

        private static IEnumerator WaitForExplorerSettled(DiagnosticBootstrap bootstrap)
        {
            float deadline = Time.realtimeSinceStartup + 12f;
            while (Time.realtimeSinceStartup < deadline)
            {
                var state = bootstrap.ExplorerRoot.State;
                if (state == PequenoExplorador.Application.Explorer.ExplorerLocomotionState.Arrived ||
                    state == PequenoExplorador.Application.Explorer.ExplorerLocomotionState.Idle) yield break;
                yield return null;
            }
            Assert.Fail("Explorer did not settle after the tutorial movement.");
        }

        private static IEnumerator WaitForReady(int previousBootstrap = 0)
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (Time.realtimeSinceStartup < deadline)
            {
                DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
                if (bootstrap != null && bootstrap.GetInstanceID() != previousBootstrap &&
                    bootstrap.State == ApplicationState.Ready && bootstrap.SceneFlow?.Current == SceneFlowState.Camp &&
                    !bootstrap.SceneFlow.IsTransitioning) yield break;
                yield return null;
            }
            Assert.Fail("Bootstrap did not reach Ready at Camp.");
        }

        private static IEnumerator WaitFor(Task task)
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (!task.IsCompleted && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(task.IsCompleted, Is.True);
            if (task.IsFaulted) throw task.Exception;
        }
    }
}
