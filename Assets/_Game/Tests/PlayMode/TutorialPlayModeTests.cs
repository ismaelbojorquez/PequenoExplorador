using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Tutorial;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Domain.Progress;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace PequenoExplorador.Tests.PlayMode
{
    public sealed class TutorialPlayModeTests
    {
        [UnityTest]
        public IEnumerator TutorialResumesAfterReloadSupportsLocalesAndCompletesWithoutPollingObjects()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return WaitForReady();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Task reset = bootstrap.ResetProgressForTestsAsync(CancellationToken.None); yield return WaitFor(reset);
            bootstrap.Tutorial.Initialize(); yield return null;
            Assert.That(bootstrap.TutorialView.IsGuideChoiceVisible, Is.True);
            bootstrap.TutorialView.MoreGuidanceButton.onClick.Invoke(); yield return null;
            Assert.That(bootstrap.TutorialView.GestureVisible, Is.True, "More guide gives an immediate visual gesture for no-reading support.");
            Assert.That(bootstrap.Tutorial.Signal(TutorialTrigger.MovementAccepted), Is.False, "Wrong semantic action does not advance.");
            bootstrap.Tutorial.Signal(TutorialTrigger.ExpeditionEntered);
            bootstrap.Tutorial.Signal(TutorialTrigger.MovementAccepted);
            bootstrap.RequestSaveCheckpoint();
            Task flush = bootstrap.FlushSaveAsync(CancellationToken.None); yield return WaitFor(flush);
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.StepIndex, Is.EqualTo(2));
            Assert.That(bootstrap.PersistedTutorial.StepIndex, Is.EqualTo(2), "Checkpoint must reach the atomic save service before reload.");

            int previousBootstrapId = bootstrap.GetInstanceID();
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return WaitForReady(previousBootstrapId);
            bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.StepIndex, Is.EqualTo(2), "Mid-step state resumes after app reload.");
            int beforePause = bootstrap.Tutorial.Snapshot.Progress.StepIndex;
            bootstrap.gameObject.SendMessage("OnApplicationPause", true);
            yield return null;
            bootstrap.gameObject.SendMessage("OnApplicationPause", false);
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.StepIndex, Is.EqualTo(beforePause), "Waiting or pausing cannot auto-complete a step.");

            string spanish = bootstrap.TutorialView.InstructionText;
            Task englishTask = bootstrap.SetLocaleAsync(LocaleCode.English, persist: false, CancellationToken.None); yield return WaitFor(englishTask); yield return null;
            string english = bootstrap.TutorialView.InstructionText;
            Assert.That(spanish, Is.Not.Empty); Assert.That(english, Is.Not.Empty); Assert.That(english, Is.Not.EqualTo(spanish));

            Assert.That(bootstrap.Tutorial.Signal(TutorialTrigger.InteractionCompleted), Is.True);
            Assert.That(bootstrap.Tutorial.Signal(TutorialTrigger.PhotoCaptured), Is.True);
            bootstrap.TutorialView.ContinueButton.onClick.Invoke();
            Assert.That(bootstrap.Tutorial.Signal(TutorialTrigger.CampReturned), Is.True);
            Assert.That(bootstrap.Tutorial.Signal(TutorialTrigger.AlbumOpened), Is.True);
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.Status, Is.EqualTo(TutorialProgressStatus.Completed));
            Assert.That(bootstrap.TutorialView.IsInstructionVisible, Is.False);

            var ratios = new[] { new Vector2Int(1024, 768), new Vector2Int(1280, 720), new Vector2Int(1600, 720), new Vector2Int(1280, 800) };
            foreach (Vector2Int ratio in ratios)
            {
                Screen.SetResolution(ratio.x, ratio.y, false); yield return null; Canvas.ForceUpdateCanvases();
                foreach (Button button in bootstrap.TutorialView.GetComponentsInChildren<Button>(true))
                {
                    RectTransform rect = (RectTransform)button.transform;
                    Assert.That(rect.rect.width, Is.GreaterThanOrEqualTo(64f), button.name + " width at " + ratio);
                    Assert.That(rect.rect.height, Is.GreaterThanOrEqualTo(64f), button.name + " height at " + ratio);
                }
            }
            Task cleanup = bootstrap.ResetProgressForTestsAsync(CancellationToken.None); yield return WaitFor(cleanup);
        }

        [UnityTest]
        public IEnumerator SkipIsEquallyAccessibleAndReplayRestartsVersionedTutorial()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single); yield return WaitForReady();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Task reset = bootstrap.ResetProgressForTestsAsync(CancellationToken.None); yield return WaitFor(reset);
            bootstrap.Tutorial.Initialize(); bootstrap.TutorialView.StandardGuidanceButton.onClick.Invoke(); yield return null;
            Assert.That(bootstrap.TutorialView.SkipButton.gameObject.activeInHierarchy, Is.True);
            bootstrap.TutorialView.SkipButton.onClick.Invoke(); yield return null;
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.Status, Is.EqualTo(TutorialProgressStatus.Skipped));
            bootstrap.Tutorial.Replay(); yield return null;
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.Status, Is.EqualTo(TutorialProgressStatus.InProgress));
            Assert.That(bootstrap.Tutorial.Snapshot.Progress.StepIndex, Is.Zero);
            Task cleanup = bootstrap.ResetProgressForTestsAsync(CancellationToken.None); yield return WaitFor(cleanup);
        }

        private static IEnumerator WaitForReady(int previousBootstrapId = 0)
        {
            float timeout = Time.realtimeSinceStartup + 20f;
            while (Time.realtimeSinceStartup < timeout)
            {
                DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
                if (bootstrap != null && bootstrap.GetInstanceID() != previousBootstrapId &&
                    bootstrap.State == ApplicationState.Ready &&
                    bootstrap.SceneFlow?.Current == PequenoExplorador.Application.SceneFlow.SceneFlowState.Camp &&
                    !bootstrap.SceneFlow.IsTransitioning && bootstrap.Tutorial?.Snapshot.Progress.ContentVersion == 1) yield break;
                yield return null;
            }
            Assert.Fail("Bootstrap did not reach Ready.");
        }

        private static IEnumerator WaitFor(Task task)
        {
            while (!task.IsCompleted) yield return null;
            if (task.IsFaulted) throw task.Exception;
        }
    }
}
