using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Application.UI;
using PequenoExplorador.Application.Worlds;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Presentation.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace PequenoExplorador.Tests.PlayMode
{
    public sealed class UiCompositionPlayModeTests
    {
        [UnityTest]
        public IEnumerator ApplyingStatesLeavesOnlyAuthorizedRootsRaycastable()
        {
            var owner = new GameObject("UI Composition Test Owner");
            UiCompositionCoordinator coordinator = owner.AddComponent<UiCompositionCoordinator>();
            var roots = new List<GameObject>();
            var bindings = new List<UiCompositionCoordinator.SurfaceBinding>();
            foreach (UiSurfaceId id in Enum.GetValues(typeof(UiSurfaceId)))
            {
                var root = new GameObject(id.ToString(), typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster));
                roots.Add(root);
                bindings.Add(new UiCompositionCoordinator.SurfaceBinding(id, root));
            }
            coordinator.ConfigureForEditorAndTests(bindings);
            coordinator.Initialize();

            foreach (AppUiState state in Enum.GetValues(typeof(AppUiState)))
            {
                coordinator.SetTutorialVisible(false);
                coordinator.Apply(state);
                Assert.That(Visible(bindings), Is.EquivalentTo(bindings
                    .Where(value => UiCompositionPolicy.IsVisible(state, value.Id, false))
                    .Select(value => value.Id)), state + " without tutorial");
                coordinator.SetTutorialVisible(true);
                Assert.That(Visible(bindings), Is.EquivalentTo(bindings
                    .Where(value => UiCompositionPolicy.IsVisible(state, value.Id, true))
                    .Select(value => value.Id)), state + " with tutorial");
            }

            coordinator.SetTutorialVisible(false);
            coordinator.Apply(AppUiState.Camp);
            yield return null;
            Assert.That(Visible(bindings), Is.EquivalentTo(new[] { UiSurfaceId.Camp }));

            coordinator.SetTutorialVisible(true);
            coordinator.Apply(AppUiState.Expedition);
            yield return null;
            Assert.That(Visible(bindings), Is.EquivalentTo(new[] { UiSurfaceId.Interaction, UiSurfaceId.Tutorial }));

            coordinator.Apply(AppUiState.Pause);
            yield return null;
            Assert.That(Visible(bindings), Is.EquivalentTo(new[] { UiSurfaceId.InputFoundation }));

            UnityEngine.Object.Destroy(owner);
            foreach (GameObject root in roots) UnityEngine.Object.Destroy(root);
            yield return null;
        }

        [UnityTest]
        public IEnumerator BootstrapCampExpeditionResizeAndResumeKeepCompositionExclusive()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<DiagnosticBootstrap>();

            Assert.That(UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None),
                Has.Length.EqualTo(1));
            AssertComposition(bootstrap, AppUiState.Camp, UiSurfaceId.Camp);

            Task<WorldLoadResult> enter = bootstrap.EnterWorldAsync(
                WorldId.Parse("world.jungle"), CancellationToken.None);
            yield return WaitForTask(enter);
            Assert.That(enter.Result.IsSuccess, Is.True);
            AssertComposition(bootstrap, AppUiState.Expedition, expectedPrimary: null);
            Assert.That(bootstrap.UiComposition.IsSurfaceVisible(UiSurfaceId.Interaction), Is.True);
            Assert.That(bootstrap.UiComposition.IsSurfaceVisible(UiSurfaceId.Camp), Is.False);

            int recoveries = bootstrap.SurfaceLifecycle.RecoveryCount;
            foreach ((int width, int height) in new[] { (1024, 768), (1920, 1080), (2400, 1080), (2560, 1600) })
            {
                Screen.SetResolution(width, height, FullScreenMode.Windowed);
                yield return null;
                bootstrap.SurfaceLifecycle.NotifyApplicationResumed();
                yield return null;
                yield return null;
                yield return null;
                AssertComposition(bootstrap, AppUiState.Expedition, expectedPrimary: null);
                Assert.That(Camera.main, Is.Not.Null, width + "x" + height);
                Assert.That(Camera.main.enabled, Is.True, width + "x" + height);
            }
            Assert.That(bootstrap.SurfaceLifecycle.RecoveryCount, Is.GreaterThanOrEqualTo(recoveries + 4));

            Task<SceneTransitionResult> camp = bootstrap.GoToCampAsync(CancellationToken.None);
            yield return WaitForTask(camp);
            Assert.That(camp.Result.Outcome, Is.EqualTo(SceneTransitionOutcome.Succeeded));
            AssertComposition(bootstrap, AppUiState.Camp, UiSurfaceId.Camp);
        }

        private static UiSurfaceId[] Visible(IEnumerable<UiCompositionCoordinator.SurfaceBinding> bindings) =>
            bindings.Where(value => value.CanvasGroup.alpha > 0.99f && value.CanvasGroup.interactable &&
                                    value.CanvasGroup.blocksRaycasts && value.Raycaster.enabled)
                .Select(value => value.Id).ToArray();

        private static void AssertComposition(
            DiagnosticBootstrap bootstrap,
            AppUiState expectedState,
            UiSurfaceId? expectedPrimary)
        {
            Assert.That(bootstrap.UiState, Is.EqualTo(expectedState));
            UiCompositionCoordinator.SurfaceBinding[] raycastable = bootstrap.UiComposition.Surfaces
                .Where(value => value.CanvasGroup.alpha > 0.99f && value.CanvasGroup.interactable &&
                                value.CanvasGroup.blocksRaycasts && value.Raycaster.enabled)
                .ToArray();
            UiCompositionCoordinator.SurfaceBinding[] primaries = raycastable
                .Where(value => UiCompositionPolicy.Role(value.Id) == UiSurfaceRole.Primary).ToArray();
            if (expectedPrimary.HasValue)
                Assert.That(primaries.Select(value => value.Id), Is.EquivalentTo(new[] { expectedPrimary.Value }));
            else
                Assert.That(primaries, Is.Empty);
            foreach (UiCompositionCoordinator.SurfaceBinding binding in bootstrap.UiComposition.Surfaces.Except(raycastable))
            {
                Assert.That(binding.CanvasGroup.alpha, Is.Zero, binding.Id + " alpha");
                Assert.That(binding.CanvasGroup.interactable, Is.False, binding.Id + " interactable");
                Assert.That(binding.CanvasGroup.blocksRaycasts, Is.False, binding.Id + " blocksRaycasts");
                Assert.That(binding.Raycaster.enabled, Is.False, binding.Id + " raycaster");
            }
        }

        private static IEnumerator WaitForReady()
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (Time.realtimeSinceStartup < deadline)
            {
                DiagnosticBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<DiagnosticBootstrap>();
                if (bootstrap != null && bootstrap.State == ApplicationState.Ready &&
                    bootstrap.SceneFlow != null && bootstrap.SceneFlow.Current == SceneFlowState.Camp &&
                    !bootstrap.SceneFlow.IsTransitioning) yield break;
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
