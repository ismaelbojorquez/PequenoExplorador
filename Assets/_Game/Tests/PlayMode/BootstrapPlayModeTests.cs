using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Bootstrap;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PequenoExplorador.Tests.PlayMode
{
    public sealed class BootstrapPlayModeTests
    {
        [UnityTest]
        public IEnumerator BootstrapReachesReadyExactlyOnce()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();

            DiagnosticBootstrap[] bootstraps = Object.FindObjectsByType<DiagnosticBootstrap>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            DiagnosticBootstrap diagnostic = bootstraps[0];

            Assert.That(bootstraps, Has.Length.EqualTo(1));
            Assert.That(diagnostic.gameObject.activeInHierarchy, Is.True);
            Assert.That(diagnostic.gameObject.name, Is.EqualTo(DiagnosticBootstrap.PlaceholderObjectName));
            Assert.That(diagnostic.State, Is.EqualTo(ApplicationState.Ready));
            Assert.That(diagnostic.Environment, Is.EqualTo(ApplicationEnvironment.Development));
            Assert.That(diagnostic.StatusText, Is.EqualTo("Ready"));
        }

        [UnityTest]
        public IEnumerator SceneReloadShutsDownTheOldRootAndDoesNotDuplicateBootstrap()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap previous = Object.FindFirstObjectByType<DiagnosticBootstrap>();

            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap[] bootstraps = Object.FindObjectsByType<DiagnosticBootstrap>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            Assert.That(previous == null, Is.True, "Reload must destroy and shutdown the previous root.");
            Assert.That(bootstraps, Has.Length.EqualTo(1));
            Assert.That(bootstraps[0].State, Is.EqualTo(ApplicationState.Ready));
        }

        [UnityTest]
        public IEnumerator CampJungleCampRepeatsThreeTimesWithoutWorldOrHandleLeak()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForSceneState(SceneFlowState.Camp);
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();

            for (int cycle = 0; cycle < 3; cycle++)
            {
                Task<SceneTransitionResult> enter = bootstrap.GoToExpeditionAsync(CancellationToken.None);
                yield return WaitForTask(enter);
                Assert.That(enter.Result.Outcome, Is.EqualTo(SceneTransitionOutcome.Succeeded));
                AssertSceneContract(bootstrap, SceneFlowState.Expedition, "Jungle", "Camp");

                Task<SceneTransitionResult> back = bootstrap.GoToCampAsync(CancellationToken.None);
                yield return WaitForTask(back);
                Assert.That(back.Result.Outcome, Is.EqualTo(SceneTransitionOutcome.Succeeded));
                AssertSceneContract(bootstrap, SceneFlowState.Camp, "Camp", "Jungle");
                Assert.That(bootstrap.State, Is.EqualTo(ApplicationState.Ready),
                    "Persistent application services must survive world unload.");
            }

            Task shutdown = bootstrap.ShutdownSceneFlowAsync();
            yield return WaitForTask(shutdown);
            Assert.That(bootstrap.SceneFlow.ActiveHandleCount, Is.Zero);
        }

        [UnityTest]
        public IEnumerator SimulatedDevelopmentFailureIsVisibleAndRetryRecovers()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForSceneState(SceneFlowState.Camp);
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            bootstrap.SimulateNextSceneFailureForDevelopment();
            LogAssert.Expect(
                LogType.Error,
                "PE_LOG level=Error subsystem=SceneFlow event=TransitionFailed detail=SceneLoadInvalidOperationException");

            Task<SceneTransitionResult> failed = bootstrap.GoToExpeditionAsync(CancellationToken.None);
            yield return WaitForTask(failed);
            Assert.That(failed.Result.Outcome, Is.EqualTo(SceneTransitionOutcome.Failed));
            Assert.That(bootstrap.SceneFlow.HasRecoverableError, Is.True);
            Assert.That(SceneManager.GetSceneByName("Camp").isLoaded, Is.True);
            Assert.That(SceneManager.GetSceneByName("Jungle").isLoaded, Is.False);

            Task<SceneTransitionResult> retry = bootstrap.GoToExpeditionAsync(CancellationToken.None);
            yield return WaitForTask(retry);
            Assert.That(retry.Result.Outcome, Is.EqualTo(SceneTransitionOutcome.Succeeded));
            AssertSceneContract(bootstrap, SceneFlowState.Expedition, "Jungle", "Camp");
        }

        private static IEnumerator WaitForReady()
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (Time.realtimeSinceStartup < deadline)
            {
                DiagnosticBootstrap[] candidates = Object.FindObjectsByType<DiagnosticBootstrap>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
                DiagnosticBootstrap bootstrap = candidates.Length == 1 ? candidates[0] : null;
                if (bootstrap != null && bootstrap.State == ApplicationState.Ready &&
                    bootstrap.StatusText == "Ready" && bootstrap.SceneFlow != null &&
                    bootstrap.SceneFlow.Current == SceneFlowState.Camp &&
                    !bootstrap.SceneFlow.IsTransitioning)
                {
                    yield break;
                }

                if (bootstrap != null && bootstrap.State == ApplicationState.Failed)
                {
                    Assert.Fail("Bootstrap entered recoverable failure instead of Ready.");
                }

                yield return null;
            }

            Assert.Fail("Bootstrap did not reach Ready within 20 seconds.");
        }

        private static IEnumerator WaitForSceneState(SceneFlowState expected)
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (Time.realtimeSinceStartup < deadline)
            {
                DiagnosticBootstrap[] candidates = Object.FindObjectsByType<DiagnosticBootstrap>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
                DiagnosticBootstrap bootstrap = candidates.Length == 1 ? candidates[0] : null;
                if (bootstrap != null && bootstrap.SceneFlow != null &&
                    !bootstrap.SceneFlow.IsTransitioning && bootstrap.SceneFlow.Current == expected)
                {
                    yield break;
                }

                yield return null;
            }

            Assert.Fail("Scene flow did not reach " + expected + " within 20 seconds.");
        }

        private static IEnumerator WaitForTask(Task task)
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (!task.IsCompleted && Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }

            Assert.That(task.IsCompleted, Is.True, "Async scene operation did not complete within 20 seconds.");
            if (task.IsFaulted)
            {
                Assert.Fail(task.Exception?.ToString());
            }
        }

        private static void AssertSceneContract(
            DiagnosticBootstrap bootstrap,
            SceneFlowState expectedState,
            string loadedScene,
            string unloadedScene)
        {
            Assert.That(bootstrap.SceneFlow.Current, Is.EqualTo(expectedState));
            Assert.That(bootstrap.SceneFlow.ActiveHandleCount, Is.EqualTo(1));
            Assert.That(SceneManager.GetSceneByName(loadedScene).isLoaded, Is.True);
            Assert.That(SceneManager.GetSceneByName(unloadedScene).isLoaded, Is.False);
            Assert.That(Object.FindObjectsByType<DiagnosticBootstrap>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None), Has.Length.EqualTo(1));
        }
    }
}
