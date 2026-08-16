using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Bootstrap;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

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
            Assert.That(diagnostic.Profile, Is.EqualTo(BuildProfile.Development));
            Assert.That(diagnostic.ConfiguredProductName, Is.EqualTo(AppConfigDefaults.ProductName));
            Assert.That(diagnostic.ConfiguredAppVersion, Is.EqualTo(AppConfigDefaults.DevelopmentAppVersion));
            Assert.That(diagnostic.CurrentLocaleCode, Is.EqualTo(LocaleCode.Spanish));
            Assert.That(diagnostic.StatusText, Is.EqualTo("Listo"));
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

        [UnityTest]
        public IEnumerator LocaleSwitchUpdatesVisibleUiPersistsAndPseudoFitsTargetResolutions()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();

            Task english = bootstrap.SetLocaleAsync(LocaleCode.English, persist: true, CancellationToken.None);
            yield return WaitForTask(english);
            yield return null;
            Assert.That(bootstrap.CurrentLocaleCode, Is.EqualTo(LocaleCode.English));
            Assert.That(bootstrap.StatusText, Is.EqualTo("Ready"));
            Assert.That(AllTexts().Any(text => text.text == "Go to the jungle"), Is.True);

            Task pseudo = bootstrap.SetLocaleAsync(LocaleCode.Pseudo, persist: false, CancellationToken.None);
            yield return WaitForTask(pseudo);
            yield return null;
            Assert.That(bootstrap.CurrentLocaleCode, Is.EqualTo(LocaleCode.Pseudo));
            Assert.That(bootstrap.StatusText, Is.Not.EqualTo("Listo"));
            Assert.That(bootstrap.StatusText.Length, Is.GreaterThan("Listo".Length));

            foreach ((int width, int height) in new[] { (1280, 720), (1920, 1080) })
            {
                Screen.SetResolution(width, height, FullScreenMode.Windowed);
                yield return null;
                Canvas.ForceUpdateCanvases();
                foreach (Text text in AllTexts().Where(text => text.gameObject.activeInHierarchy && !string.IsNullOrEmpty(text.text)))
                {
                    Rect rect = text.rectTransform.rect;
                    Assert.That(rect.width, Is.GreaterThan(0f), text.gameObject.name + " width");
                    Assert.That(rect.height, Is.GreaterThan(0f), text.gameObject.name + " height");
                    Assert.That(
                        text.resizeTextForBestFit || text.preferredHeight <= rect.height + 2f,
                        Is.True,
                        text.gameObject.name + " clips vertically at " + width + "x" + height);
                }
            }

            Task spanish = bootstrap.SetLocaleAsync(LocaleCode.Spanish, persist: true, CancellationToken.None);
            yield return WaitForTask(spanish);
            yield return null;
            Assert.That(bootstrap.StatusText, Is.EqualTo("Listo"));
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
                    bootstrap.StatusText == "Listo" && bootstrap.SceneFlow != null &&
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

        private static Text[] AllTexts()
        {
            return Object.FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }
    }
}
