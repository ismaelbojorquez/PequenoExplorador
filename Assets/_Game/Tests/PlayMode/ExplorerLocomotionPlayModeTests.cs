using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Explorer;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Presentation.Explorer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PequenoExplorador.Tests.PlayMode
{
    public sealed class ExplorerLocomotionPlayModeTests
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
        public IEnumerator InputTapMovesInvalidTapRecoversAndUiMapsSuspend()
        {
            Mouse mouse = InputSystem.AddDevice<Mouse>();
            yield return LoadExpedition();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            ExplorerLocomotionRoot root = bootstrap.ExplorerRoot;
            Assert.That(root, Is.Not.Null);
            Assert.That(root.Agent.isOnNavMesh, Is.True);

            Vector3 destination = new Vector3(4f, 0f, 2f);
            Vector3 screen = Camera.main.WorldToScreenPoint(destination);
            _inputFixture.Set(mouse.position, new Vector2(screen.x, screen.y));
            _inputFixture.Press(mouse.leftButton);
            yield return null;
            _inputFixture.Release(mouse.leftButton);
            yield return WaitForMoving(root);
            Assert.That(root.State, Is.EqualTo(ExplorerLocomotionState.PathPending)
                .Or.EqualTo(ExplorerLocomotionState.Moving));

            Vector3 obstacleScreen = Camera.main.WorldToScreenPoint(new Vector3(-2.5f, 0.75f, 1.5f));
            Assert.That(root.TryHandleScreenTap(new ScreenPoint(obstacleScreen.x, obstacleScreen.y)), Is.False);
            Assert.That(root.State, Is.EqualTo(ExplorerLocomotionState.InvalidDestination));
            Assert.That(root.TryHandleScreenTap(new ScreenPoint(screen.x, screen.y)), Is.True);

            bootstrap.SetInputMap(InputMapId.UI);
            yield return null;
            Assert.That(root.State, Is.EqualTo(ExplorerLocomotionState.Suspended));
            Assert.That(root.Agent.hasPath, Is.False);
            bootstrap.SetInputMap(InputMapId.Photography);
            yield return null;
            Assert.That(root.State, Is.EqualTo(ExplorerLocomotionState.Suspended));
        }

        [UnityTest]
        public IEnumerator RepeatedSceneCyclesReleaseRootsAndReducedMotionSnapsCamera()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            for (int cycle = 0; cycle < 3; cycle++)
            {
                Task<SceneTransitionResult> enter = bootstrap.GoToExpeditionAsync(CancellationToken.None);
                yield return WaitForTask(enter);
                Assert.That(bootstrap.ExplorerRoot, Is.Not.Null, "cycle " + cycle);
                Assert.That(Object.FindObjectsByType<ExplorerLocomotionRoot>(
                    FindObjectsInactive.Include, FindObjectsSortMode.None).Length, Is.EqualTo(1));
                bootstrap.ExplorerRoot.SetReduceMotion(true);
                Assert.That(bootstrap.ExplorerRoot.ReduceMotion, Is.True);

                for (int tap = 0; tap < 12; tap++)
                {
                    Vector3 point = new Vector3(-5f + tap * 0.75f, 0f, (tap % 2 == 0) ? 3f : -3f);
                    Vector3 screen = Camera.main.WorldToScreenPoint(point);
                    bootstrap.ExplorerRoot.TryHandleScreenTap(new ScreenPoint(screen.x, screen.y));
                }

                Task<SceneTransitionResult> back = bootstrap.GoToCampAsync(CancellationToken.None);
                yield return WaitForTask(back);
                Assert.That(bootstrap.ExplorerRoot, Is.Null);
                Assert.That(Object.FindObjectsByType<ExplorerLocomotionRoot>(
                    FindObjectsInactive.Include, FindObjectsSortMode.None), Is.Empty);
            }
        }

        [UnityTest]
        public IEnumerator EditorBatchFpsSampleIsFiniteAndSceneUnloadCancelsMovement()
        {
            yield return LoadExpedition();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            ExplorerLocomotionRoot root = bootstrap.ExplorerRoot;
            Vector3 screen = Camera.main.WorldToScreenPoint(new Vector3(5f, 0f, 4f));
            Assert.That(root.TryHandleScreenTap(new ScreenPoint(screen.x, screen.y)), Is.True);

            const int frames = 60;
            float started = Time.realtimeSinceStartup;
            for (int index = 0; index < frames; index++) yield return null;
            float elapsed = Time.realtimeSinceStartup - started;
            float fps = elapsed > 0f ? frames / elapsed : 0f;
            Debug.Log($"PE_EXPLORER_FPS_BASIC environment=EditorBatch frames={frames} fps={fps:0.0}");
            Assert.That(float.IsNaN(fps) || float.IsInfinity(fps), Is.False);
            Assert.That(fps, Is.GreaterThan(0f));

            Task<SceneTransitionResult> back = bootstrap.GoToCampAsync(CancellationToken.None);
            yield return WaitForTask(back);
            Assert.That(root == null, Is.True, "Addressable unload must destroy and unbind the explorer root.");
        }

        private static IEnumerator LoadExpedition()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Task<SceneTransitionResult> enter = bootstrap.GoToExpeditionAsync(CancellationToken.None);
            yield return WaitForTask(enter);
            Assert.That(enter.Result.IsSuccess, Is.True, enter.Result.ErrorCode);
            Assert.That(bootstrap.Input.CurrentMap, Is.EqualTo(InputMapId.Explorer));
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

        private static IEnumerator WaitForMoving(ExplorerLocomotionRoot root)
        {
            float deadline = Time.realtimeSinceStartup + 3f;
            while (Time.realtimeSinceStartup < deadline)
            {
                if (root.State == ExplorerLocomotionState.PathPending ||
                    root.State == ExplorerLocomotionState.Moving) yield break;
                yield return null;
            }
            Assert.Fail("Explorer did not accept the tap destination.");
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
