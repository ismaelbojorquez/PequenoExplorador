using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Accessibility;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Presentation.Accessibility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PequenoExplorador.Tests.PlayMode
{
    public sealed class MobileInputPlayModeTests
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
        public IEnumerator EscapeBackOpensPauseCheckpointsAndSecondBackResumes()
        {
            Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            _inputFixture.Press(keyboard.escapeKey);
            yield return null;
            _inputFixture.Release(keyboard.escapeKey);
            yield return null;
            Assert.That(bootstrap.IsPauseVisible, Is.True);
            Assert.That(bootstrap.Input.CurrentMap, Is.EqualTo(InputMapId.UI));

            _inputFixture.Press(keyboard.escapeKey);
            yield return null;
            _inputFixture.Release(keyboard.escapeKey);
            yield return null;
            Assert.That(bootstrap.IsPauseVisible, Is.False);
        }

        [UnityTest]
        public IEnumerator SceneStateSelectsUiAndExplorerMapsWithoutEnablingDebugAsProductMap()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Assert.That(bootstrap.Input.CurrentMap, Is.EqualTo(InputMapId.UI));
            Assert.That(bootstrap.Input.DebugMapEnabled, Is.True, "Debug map is additive only in Development.");

            Task<SceneTransitionResult> enter = bootstrap.GoToExpeditionAsync(CancellationToken.None);
            yield return WaitForTask(enter);
            Assert.That(bootstrap.Input.CurrentMap, Is.EqualTo(InputMapId.Explorer));
            Task<SceneTransitionResult> back = bootstrap.GoToCampAsync(CancellationToken.None);
            yield return WaitForTask(back);
            Assert.That(bootstrap.Input.CurrentMap, Is.EqualTo(InputMapId.UI));
        }

        [UnityTest]
        public IEnumerator MultiTouchDoesNotEmitTwoAccidentalTapsAndSafeAreaFitsRequiredRatios()
        {
            Touchscreen touchscreen = InputSystem.AddDevice<Touchscreen>();
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            yield return WaitForReady();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            var intents = new List<InputIntent>();
            bootstrap.Input.IntentRaised += intents.Add;

            _inputFixture.BeginTouch(1, new Vector2(100, 100), screen: touchscreen);
            _inputFixture.BeginTouch(2, new Vector2(160, 100), screen: touchscreen);
            yield return null;
            _inputFixture.EndTouch(1, new Vector2(100, 100), screen: touchscreen);
            _inputFixture.EndTouch(2, new Vector2(160, 100), screen: touchscreen);
            yield return null;
            Assert.That(intents.Count(intent => intent.Kind == InputGestureKind.Tap), Is.Zero);

            SafeAreaFitter[] fitters = Object.FindObjectsByType<SafeAreaFitter>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Assert.That(fitters, Is.Not.Empty);
            foreach (DeviceAspectPreset preset in DeviceAspectPresets.Landscape)
            {
                var snapshot = new SafeAreaSnapshot(
                    preset.Width, preset.Height, preset.LeftInset, 0f, preset.RightInset, 0f,
                    DisplayOrientation.LandscapeLeft);
                foreach (SafeAreaFitter fitter in fitters)
                {
                    fitter.Apply(snapshot);
                    Assert.That(fitter.Applied, Is.EqualTo(snapshot), preset.Id);
                    Assert.That(fitter.GetComponent<RectTransform>().anchorMin.x, Is.EqualTo(preset.LeftInset).Within(0.0001f));
                }
            }
            bootstrap.Input.IntentRaised -= intents.Add;
        }

        private static IEnumerator WaitForReady()
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (Time.realtimeSinceStartup < deadline)
            {
                DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
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
