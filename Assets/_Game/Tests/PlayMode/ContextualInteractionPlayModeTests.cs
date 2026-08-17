using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Presentation.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PequenoExplorador.Tests.PlayMode
{
    public sealed class ContextualInteractionPlayModeTests
    {
        private InputTestFixture _inputFixture;

        [SetUp]
        public void SetUp()
        {
            _inputFixture = new InputTestFixture();
            _inputFixture.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (DiagnosticBootstrap bootstrap in Object.FindObjectsByType<DiagnosticBootstrap>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
                Object.DestroyImmediate(bootstrap.gameObject);
            _inputFixture.TearDown();
            _inputFixture = null;
        }

        [UnityTest]
        public IEnumerator PointerTapApproachesShowsLocalizedPromptAndPreventsDoubleInteraction()
        {
            Mouse mouse = InputSystem.AddDevice<Mouse>();
            yield return LoadExpedition();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            WorldInteractableView animal = Target(bootstrap, "interaction.jungle.keel-billed-toucan");
            Assert.That(animal.transform.Find("VS_ToucanPicoCanoa"), Is.Not.Null);
            Assert.That(animal.GetComponentsInChildren<Renderer>(true).Length, Is.GreaterThanOrEqualTo(8));
            Assert.That(animal.transform.Find("PH_FIXTURE_ANIMAL_VISUAL"), Is.Null);
            Vector3 screen = Camera.main.WorldToScreenPoint(animal.transform.position + Vector3.up * 0.7f);
            _inputFixture.Set(mouse.position, new Vector2(screen.x, screen.y));
            _inputFixture.Press(mouse.leftButton);
            yield return null;
            _inputFixture.Release(mouse.leftButton);

            yield return WaitForInteraction(
                bootstrap,
                InteractionOutcome.Ready,
                8f);
            Assert.That(bootstrap.InteractionPrompt.IsVisible, Is.True);
            Assert.That(bootstrap.InteractionPrompt.ActionButton.gameObject.activeSelf, Is.True);
            Assert.That(bootstrap.InteractionPrompt.NameText, Is.EqualTo("Tucán pico canoa"));
            for (int index = 0; index < 8; index++) bootstrap.InteractionPrompt.ActionButton.onClick.Invoke();
            yield return null;
            Assert.That(animal.ActivationCount, Is.EqualTo(1));
            Assert.That(bootstrap.InteractionRoot.Coordinator.Snapshot.State,
                Is.EqualTo(InteractionOutcome.Completed));
        }

        [UnityTest]
        public IEnumerator ThreeNeutralFixturesShareCoreAndUnavailableUsesFriendlyPrompt()
        {
            yield return LoadExpedition();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Assert.That(bootstrap.InteractionRoot.TargetCount, Is.EqualTo(3));
            Assert.That(bootstrap.InteractionRoot.Detector.IndexedColliderCount, Is.EqualTo(3));
            Assert.That(bootstrap.InteractionRoot.Targets.Select(item => item.GetType()).Distinct().Single(),
                Is.EqualTo(typeof(WorldInteractableView)));

            WorldInteractableView unavailable = Target(bootstrap, "interaction.fixture.object");
            Vector3 screen = Camera.main.WorldToScreenPoint(unavailable.transform.position + Vector3.up * 0.6f);
            Assert.That(bootstrap.InteractionRoot.TryHandleTap(new ScreenPoint(screen.x, screen.y)), Is.True);
            yield return null;
            Assert.That(bootstrap.InteractionRoot.Coordinator.Snapshot.State,
                Is.EqualTo(InteractionOutcome.Unavailable));
            Assert.That(bootstrap.InteractionPrompt.StatusText,
                Is.EqualTo("Todavía no podemos mirar esto. Probemos con otra cosa."));
            Assert.That(bootstrap.InteractionPrompt.ActionButton.gameObject.activeSelf, Is.False);
            Assert.That(bootstrap.InteractionPrompt.CancelButton.gameObject.activeSelf, Is.True);
            bootstrap.InteractionPrompt.CancelButton.onClick.Invoke();
            yield return null;
            Assert.That(bootstrap.InteractionRoot.Coordinator.Snapshot.State,
                Is.EqualTo(InteractionOutcome.Cancelled));
            Assert.That(bootstrap.InteractionPrompt.IsVisible, Is.False);
        }

        [UnityTest]
        public IEnumerator AnimalDiscoveryPersistsAcrossWorldReloadAndRepeatsWithoutUniqueGrant()
        {
            yield return LoadExpedition();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Task reset = bootstrap.ResetProgressForTestsAsync(CancellationToken.None);
            yield return WaitForTask(reset);

            yield return ActivateAnimal(bootstrap);
            Assert.That(bootstrap.LastDiscoveryResult.Outcome, Is.EqualTo(DiscoverOutcome.First));
            Assert.That(bootstrap.LastDiscoveryResult.GrantsUniqueReward, Is.True);
            Assert.That(bootstrap.LastDiscoveryResult.Count, Is.EqualTo(1));
            Task flush = bootstrap.FlushSaveAsync(CancellationToken.None);
            yield return WaitForTask(flush);

            Task<SceneTransitionResult> back = bootstrap.GoToCampAsync(CancellationToken.None);
            yield return WaitForTask(back);
            Task<SceneTransitionResult> enter = bootstrap.GoToExpeditionAsync(CancellationToken.None);
            yield return WaitForTask(enter);
            Assert.That(enter.Result.IsSuccess, Is.True, enter.Result.ErrorCode);

            yield return ActivateAnimal(bootstrap);
            Assert.That(bootstrap.LastDiscoveryResult.Outcome, Is.EqualTo(DiscoverOutcome.Repeated));
            Assert.That(bootstrap.LastDiscoveryResult.GrantsUniqueReward, Is.False);
            Assert.That(bootstrap.LastDiscoveryResult.Count, Is.EqualTo(2));
        }

        [UnityTest]
        public IEnumerator UiOpenDestroyedTargetAndSceneUnloadCancelFocusCleanly()
        {
            yield return LoadExpedition();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            WorldInteractableView plant = Target(bootstrap, "interaction.fixture.plant");
            Vector3 screen = Camera.main.WorldToScreenPoint(plant.transform.position + Vector3.up * 0.7f);
            Assert.That(bootstrap.InteractionRoot.TryHandleTap(new ScreenPoint(screen.x, screen.y)), Is.True);
            bootstrap.SetInputMap(InputMapId.UI);
            yield return null;
            Assert.That(bootstrap.InteractionRoot.Coordinator.Snapshot.State,
                Is.EqualTo(InteractionOutcome.Suspended));
            Assert.That(bootstrap.InteractionPrompt.IsVisible, Is.False);

            bootstrap.SetInputMap(InputMapId.Explorer);
            Assert.That(bootstrap.InteractionRoot.TryHandleTap(new ScreenPoint(screen.x, screen.y)), Is.True);
            Object.Destroy(plant.gameObject);
            yield return null;
            Assert.That(bootstrap.InteractionRoot.Coordinator.Snapshot.State,
                Is.EqualTo(InteractionOutcome.Missing));
            Assert.That(bootstrap.InteractionPrompt.IsVisible, Is.False);

            WorldInteractableView animal = Target(bootstrap, "interaction.jungle.keel-billed-toucan");
            screen = Camera.main.WorldToScreenPoint(animal.transform.position + Vector3.up * 0.7f);
            bootstrap.InteractionRoot.TryHandleTap(new ScreenPoint(screen.x, screen.y));
            Task<SceneTransitionResult> back = bootstrap.GoToCampAsync(CancellationToken.None);
            yield return WaitForTask(back);
            Assert.That(bootstrap.InteractionRoot, Is.Null);
            Assert.That(bootstrap.InteractionPrompt.IsVisible, Is.False);
            Assert.That(Object.FindObjectsByType<InteractionSceneRoot>(
                FindObjectsInactive.Include, FindObjectsSortMode.None), Is.Empty);
        }

        private static WorldInteractableView Target(DiagnosticBootstrap bootstrap, string id) =>
            bootstrap.InteractionRoot.Targets.Single(item => item != null && item.RawInteractionId == id);

        private static IEnumerator LoadExpedition()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Task<SceneTransitionResult> enter = bootstrap.GoToExpeditionAsync(CancellationToken.None);
            yield return WaitForTask(enter);
            Assert.That(enter.Result.IsSuccess, Is.True, enter.Result.ErrorCode);
            Assert.That(bootstrap.InteractionRoot, Is.Not.Null);
        }

        private static IEnumerator ActivateAnimal(DiagnosticBootstrap bootstrap)
        {
            WorldInteractableView animal = Target(bootstrap, "interaction.jungle.keel-billed-toucan");
            Vector3 screen = Camera.main.WorldToScreenPoint(animal.transform.position + Vector3.up * 0.7f);
            Assert.That(bootstrap.InteractionRoot.TryHandleTap(new ScreenPoint(screen.x, screen.y)), Is.True);
            yield return WaitForInteraction(bootstrap, InteractionOutcome.Ready, 8f);
            bootstrap.InteractionPrompt.ActionButton.onClick.Invoke();
            yield return null;
            Assert.That(bootstrap.InteractionRoot.Coordinator.Snapshot.State,
                Is.EqualTo(InteractionOutcome.Completed));
        }

        private static IEnumerator WaitForInteraction(
            DiagnosticBootstrap bootstrap,
            InteractionOutcome expected,
            float seconds)
        {
            float deadline = Time.realtimeSinceStartup + seconds;
            while (Time.realtimeSinceStartup < deadline)
            {
                if (bootstrap.InteractionRoot.Coordinator.Snapshot.State == expected) yield break;
                yield return null;
            }
            Assert.Fail("Interaction did not reach " + expected + ".");
        }

        private static IEnumerator WaitForReady()
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (Time.realtimeSinceStartup < deadline)
            {
                DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
                if (bootstrap != null && bootstrap.State == ApplicationState.Ready &&
                    bootstrap.SceneFlow?.Current == SceneFlowState.Camp && !bootstrap.SceneFlow.IsTransitioning)
                    yield break;
                yield return null;
            }
            Assert.Fail("Bootstrap did not become Ready.");
        }

        private static IEnumerator WaitForTask(Task task)
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (!task.IsCompleted && Time.realtimeSinceStartup < deadline) yield return null;
            if (task.IsFaulted) throw task.Exception;
            Assert.That(task.IsCompleted, Is.True);
        }
    }
}
