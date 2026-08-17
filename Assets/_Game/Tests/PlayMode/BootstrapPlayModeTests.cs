using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Learning;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.Explorer;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Application.Worlds;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Domain.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using PequenoExplorador.Presentation.Learning;

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
            Task spanish = diagnostic.SetLocaleAsync(LocaleCode.Spanish, persist: false, CancellationToken.None);
            yield return WaitForTask(spanish);
            yield return null;

            Assert.That(bootstraps, Has.Length.EqualTo(1));
            Assert.That(diagnostic.gameObject.activeInHierarchy, Is.True);
            Assert.That(diagnostic.gameObject.name, Is.EqualTo(DiagnosticBootstrap.PlaceholderObjectName));
            Assert.That(diagnostic.State, Is.EqualTo(ApplicationState.Ready));
            Assert.That(diagnostic.Profile, Is.EqualTo(BuildProfile.Development));
            Assert.That(diagnostic.ConfiguredProductName, Is.EqualTo(AppConfigDefaults.ProductName));
            Assert.That(diagnostic.ConfiguredAppVersion, Is.EqualTo(AppConfigDefaults.DevelopmentAppVersion));
            Assert.That(diagnostic.CurrentLocaleCode, Is.EqualTo(LocaleCode.Spanish));
            Assert.That(diagnostic.StatusText, Is.EqualTo("Listo"));
            Assert.That(diagnostic.Worlds.Worlds, Has.Count.EqualTo(1));
            Assert.That(diagnostic.Worlds.Worlds.Single().Manifest.Id, Is.EqualTo(WorldId.Parse("world.jungle")));
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
                Task<WorldLoadResult> enter = bootstrap.EnterWorldAsync(
                    WorldId.Parse("world.jungle"),
                    CancellationToken.None);
                yield return WaitForTask(enter);
                Assert.That(enter.Result.Outcome, Is.EqualTo(WorldLoadOutcome.Succeeded));
                Assert.That(bootstrap.WorldSession.ActiveWorld.Id, Is.EqualTo(WorldId.Parse("world.jungle")));
                AssertSceneContract(bootstrap, SceneFlowState.Expedition, "Jungle", "Camp");

                Task<SceneTransitionResult> back = bootstrap.GoToCampAsync(CancellationToken.None);
                yield return WaitForTask(back);
                Assert.That(back.Result.Outcome, Is.EqualTo(SceneTransitionOutcome.Succeeded));
                Assert.That(bootstrap.WorldSession.ActiveWorld, Is.Null);
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
            Assert.That(AllTexts().Any(text => text.text == "Jungle Expedition"), Is.True);

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

        [UnityTest]
        public IEnumerator AudioFrameworkDucksReplaysSuspendsAndSurvivesWorldTransitionsWithoutDuplicates()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Assert.That(bootstrap.Audio, Is.Not.Null);
            Assert.That(bootstrap.GetComponents<AudioSource>(), Has.Length.EqualTo(7));

            SubtitleModel subtitle = SubtitleModel.Hidden;
            bootstrap.Audio.SubtitleChanged += value => subtitle = value;
            AudioPlayResult voice = bootstrap.PlayAudio(AudioCueIds.ExploreInstruction);
            Assert.That(voice.IsAccepted, Is.True);
            Assert.That(bootstrap.Audio.IsVoiceDucking, Is.True);
            Assert.That(subtitle.Visible, Is.True);
            Assert.That(subtitle.TextKey, Is.EqualTo(LocalizationKeys.AudioExploreInstruction));
            Assert.That(bootstrap.ReplayInstruction().IsAccepted, Is.True);
            float duckDeadline = Time.realtimeSinceStartup + 2f;
            while (bootstrap.Audio.IsVoiceDucking && Time.realtimeSinceStartup < duckDeadline) yield return null;
            Assert.That(bootstrap.Audio.IsVoiceDucking, Is.False, "Ducking must restore after queued voice completes.");
            Assert.That(bootstrap.PlayAudio(new AudioCueId("audio.missing.playmode")).Status, Is.EqualTo(AudioPlayStatus.Missing));

            bootstrap.Audio.SetApplicationSuspended(true);
            Assert.That(bootstrap.PlayAudio(AudioCueIds.ConfirmFeedback).Status, Is.EqualTo(AudioPlayStatus.Suspended));
            bootstrap.Audio.SetApplicationSuspended(false);
            Assert.That(bootstrap.PlayAudio(AudioCueIds.ConfirmFeedback).IsAccepted, Is.True);

            Task<SceneTransitionResult> enter = bootstrap.GoToExpeditionAsync(CancellationToken.None);
            yield return WaitForTask(enter);
            Task<SceneTransitionResult> back = bootstrap.GoToCampAsync(CancellationToken.None);
            yield return WaitForTask(back);
            Assert.That(bootstrap.GetComponents<AudioSource>(), Has.Length.EqualTo(7));
            Assert.That(Object.FindObjectsByType<DiagnosticBootstrap>(FindObjectsInactive.Include, FindObjectsSortMode.None), Has.Length.EqualTo(1));

            Task english = bootstrap.SetLocaleAsync(LocaleCode.English, persist: false, CancellationToken.None);
            yield return WaitForTask(english);
            Assert.That(bootstrap.PlayAudio(AudioCueIds.JungleName).IsAccepted, Is.True);
        }

        [UnityTest]
        public IEnumerator LearningFixtureSupportsFriendlyRetryHintLocaleReplayAndPersistentCompletion()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            bootstrap.ResetLearningFixtureForTests();
            Assert.That(bootstrap.LearningView.OptionCount, Is.EqualTo(3));
            Button[] optionButtons = bootstrap.LearningView.GetComponentsInChildren<Button>(true)
                .Where(button => button.name.StartsWith("Option ", System.StringComparison.Ordinal))
                .OrderBy(button => button.name, System.StringComparer.Ordinal)
                .ToArray();
            Assert.That(optionButtons, Has.Length.EqualTo(3));
            optionButtons[1].onClick.Invoke();
            Assert.That(bootstrap.LearningView.LastOutcome, Is.EqualTo(ActivityOutcome.TryAgain));
            Assert.That(bootstrap.LearningView.FeedbackText, Does.Contain("probar"));
            Assert.That(bootstrap.LearningView.RequestHint().Outcome, Is.EqualTo(ActivityOutcome.Hint));
            Assert.That(bootstrap.LearningView.Exit().Outcome, Is.EqualTo(ActivityOutcome.Exited));
            Assert.That(bootstrap.LearningView.StartFixture().Outcome, Is.EqualTo(ActivityOutcome.Resumed));

            Task english = bootstrap.SetLocaleAsync(LocaleCode.English, persist: false, CancellationToken.None);
            yield return WaitForTask(english); yield return null;
            Assert.That(bootstrap.LearningView.TitleText, Is.EqualTo("Look and find"));
            Assert.That(bootstrap.LearningView.Submit(0).Outcome, Is.EqualTo(ActivityOutcome.Completed));
            bootstrap.LearningView.Replay();
            Task flush = bootstrap.FlushSaveAsync(CancellationToken.None); yield return WaitForTask(flush);

            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null; yield return WaitForReady();
            DiagnosticBootstrap reloaded = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            LearningActivityResult completed = reloaded.LearningView.StartFixture();
            Assert.That(completed.Outcome, Is.EqualTo(ActivityOutcome.AlreadyCompleted));
            Assert.That(completed.Reward.Outcome, Is.EqualTo(PequenoExplorador.Application.Economy.GrantRewardOutcome.AlreadyProcessed));
        }

        [UnityTest]
        public IEnumerator ToucanInteractionRunsFriendlyFeedingActivityThenContinuesToPhotography()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Task reset = bootstrap.ResetProgressForTestsAsync(CancellationToken.None);
            yield return WaitForTask(reset);
            Task<WorldLoadResult> enter = bootstrap.EnterWorldAsync(WorldId.Parse("world.jungle"), CancellationToken.None);
            yield return WaitForTask(enter);
            Assert.That(enter.Result.IsSuccess, Is.True);

            var toucan = bootstrap.InteractionRoot.Targets.Single(item => item.RawInteractionId == "interaction.jungle.keel-billed-toucan");
            InteractionResult opened = toucan.Interact(new InteractionContext(
                new WorldPosition(0f, 0f, 0f), new WorldPosition(0f, 0f, 0f),
                new System.DateTimeOffset(2026, 8, 17, 12, 0, 0, System.TimeSpan.Zero)));
            Assert.That(opened.IsSuccess, Is.True);
            yield return null;
            Assert.That(bootstrap.PhotographyRoot.IsActive, Is.True);
            bootstrap.PhotographyRoot.ActiveTarget.SetSampleOverrideForEditorAndTests(
                new PequenoExplorador.Application.Photography.PhotoFrameSample(0.30f, 3f, true, 0.05f, 1f));
            bootstrap.PhotographyView.ShutterButton.onClick.Invoke();
            float deadline = Time.realtimeSinceStartup + 6f;
            while (Time.realtimeSinceStartup < deadline && !bootstrap.PhotographyRoot.LastCapture.ProgressCaptured)
                yield return null;
            Assert.That(bootstrap.PhotographyRoot.LastCapture.ProgressCaptured, Is.True);
            Assert.That(bootstrap.PhotographyView.LearnButton.gameObject.activeSelf, Is.True);
            bootstrap.PhotographyView.LearnButton.onClick.Invoke();
            yield return null;
            Assert.That(bootstrap.LearningView.IsVisible, Is.True);
            Assert.That(bootstrap.LearningView.ActiveActivityId, Is.EqualTo(LearningActivityView.ToucanActivityId));

            bootstrap.LearningView.SetReduceMotion(true);
            Assert.That(bootstrap.LearningView.Submit(1).Outcome, Is.EqualTo(ActivityOutcome.TryAgain));
            AnimalLearningReactionView reaction = Object.FindFirstObjectByType<AnimalLearningReactionView>();
            Assert.That(reaction.LastReaction.Value, Is.EqualTo("learning-reaction.toucan.neutral"));
            Assert.That(reaction.LastUsedReducedMotion, Is.True);
            Assert.That(bootstrap.LearningView.RequestHint().Outcome, Is.EqualTo(ActivityOutcome.Hint));

            Task english = bootstrap.SetLocaleAsync(LocaleCode.English, persist: false, CancellationToken.None);
            yield return WaitForTask(english); yield return null;
            Assert.That(bootstrap.LearningView.TitleText, Is.EqualTo("What would the toucan choose?"));
            LearningActivityResult complete = bootstrap.LearningView.Submit(0);
            Assert.That(complete.Outcome, Is.EqualTo(ActivityOutcome.Completed));
            Assert.That(complete.Reward.Outcome, Is.EqualTo(PequenoExplorador.Application.Economy.GrantRewardOutcome.Granted));
            Assert.That(bootstrap.LearningView.FeedbackText, Is.EqualTo("It mostly eats fruit."));
            bootstrap.LearningView.Replay();

            Button exit = bootstrap.LearningView.GetComponentsInChildren<Button>(true).Single(button => button.name == "Exit");
            exit.onClick.Invoke();
            yield return null;
            Assert.That(bootstrap.LearningView.IsVisible, Is.False);
            Assert.That(bootstrap.PhotographyRoot.IsActive, Is.True);
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
                    bootstrap.SceneFlow != null &&
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
