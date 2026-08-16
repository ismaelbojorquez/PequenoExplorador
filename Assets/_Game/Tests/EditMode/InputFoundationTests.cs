using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PequenoExplorador.Application.Accessibility;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Content.Input;
using PequenoExplorador.Editor;
using PequenoExplorador.Editor.BuildTools;
using PequenoExplorador.Infrastructure.Accessibility;
using UnityEditor;
using UnityEngine.InputSystem;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class InputFoundationTests
    {
        private GestureRecognizer _recognizer;
        private List<InputIntent> _intents;

        [SetUp]
        public void SetUp()
        {
            _recognizer = new GestureRecognizer(GestureThresholds.ChildFriendlyDefault);
            _intents = new List<InputIntent>(512);
            _recognizer.IntentRaised += _intents.Add;
        }

        [Test]
        public void TapHoldAndDragRespectChildFriendlyThresholds()
        {
            _recognizer.BeginPointer(1, new ScreenPoint(10, 10), 0d);
            _recognizer.EndPointer(1, new ScreenPoint(20, 10), 0.2d);
            Assert.That(_intents.Select(intent => intent.Kind), Is.EqualTo(new[] { InputGestureKind.Tap }));

            _intents.Clear();
            _recognizer.BeginPointer(2, new ScreenPoint(10, 10), 1d);
            _recognizer.AdvanceTime(1.7d);
            _recognizer.EndPointer(2, new ScreenPoint(10, 10), 1.8d);
            Assert.That(_intents.Select(intent => intent.Kind), Is.EqualTo(new[] { InputGestureKind.PressAndHold }));

            _intents.Clear();
            _recognizer.MovePointer(99, new ScreenPoint(0, 0), 2d);
            _recognizer.BeginPointer(3, new ScreenPoint(0, 0), 2d);
            _recognizer.MovePointer(3, new ScreenPoint(40, 0), 2.1d);
            _recognizer.MovePointer(3, new ScreenPoint(55, 0), 2.2d);
            _recognizer.EndPointer(3, new ScreenPoint(55, 0), 2.3d);
            Assert.That(_intents.Select(intent => intent.Kind), Is.EqualTo(new[]
            {
                InputGestureKind.DragStarted,
                InputGestureKind.Dragged,
                InputGestureKind.DragEnded
            }));
        }

        [Test]
        public void ExplorerIsTapOnlyAndPhotographyAllowsPinch()
        {
            _recognizer.SetMap(InputMapId.Explorer);
            _recognizer.BeginPointer(1, new ScreenPoint(0, 0), 0d);
            _recognizer.MovePointer(1, new ScreenPoint(100, 0), 0.1d);
            _recognizer.EndPointer(1, new ScreenPoint(100, 0), 0.2d);
            Assert.That(_intents, Is.Empty, "Explorer must not become a hidden drag/joystick control.");

            _recognizer.SetMap(InputMapId.Photography);
            _recognizer.BeginPointer(1, new ScreenPoint(100, 100), 1d);
            _recognizer.BeginPointer(2, new ScreenPoint(200, 100), 1d);
            _recognizer.MovePointer(2, new ScreenPoint(230, 100), 1.1d);
            Assert.That(_intents.Any(intent => intent.Kind == InputGestureKind.Pinch), Is.True);
        }

        [Test]
        public void SecondPointerSuppressesAccidentalTapsAndCancellationCleansState()
        {
            _recognizer.BeginPointer(1, new ScreenPoint(10, 10), 0d);
            _recognizer.BeginPointer(2, new ScreenPoint(20, 20), 0.01d);
            _recognizer.EndPointer(1, new ScreenPoint(10, 10), 0.1d);
            _recognizer.EndPointer(2, new ScreenPoint(20, 20), 0.11d);
            Assert.That(_intents.Any(intent => intent.Kind == InputGestureKind.Tap), Is.False);
            Assert.That(_recognizer.ActivePointerCount, Is.Zero);

            _recognizer.BeginPointer(3, new ScreenPoint(0, 0), 1d);
            _recognizer.SetMap(InputMapId.Parents);
            Assert.That(_intents.Last().Kind, Is.EqualTo(InputGestureKind.Cancelled));
            Assert.That(_recognizer.ActivePointerCount, Is.Zero);
        }

        [Test]
        public void RecognizerHotPathDoesNotAllocateAfterWarmup()
        {
            _recognizer.BeginPointer(1, default, 0d);
            _recognizer.MovePointer(1, new ScreenPoint(40, 0), 0.1d);
            _recognizer.EndPointer(1, new ScreenPoint(40, 0), 0.2d);
            _intents.Clear();
            long before = GC.GetAllocatedBytesForCurrentThread();
            for (int index = 0; index < 100; index++)
            {
                _recognizer.BeginPointer(1, default, index);
                _recognizer.MovePointer(1, new ScreenPoint(40, 0), index + 0.1d);
                _recognizer.EndPointer(1, new ScreenPoint(40, 0), index + 0.2d);
            }
            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
            Assert.That(allocated, Is.LessThanOrEqualTo(256), "Recognizer hot path must remain allocation-stable.");
        }

        [Test]
        public void ActionAssetAndValidatorCoverFiveSemanticMaps()
        {
            InputActionAsset actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputFoundationSetup.ActionsPath);
            Assert.That(actions, Is.Not.Null);
            Assert.That(actions.actionMaps.Select(map => map.name), Is.EquivalentTo(new[]
            {
                "UI", "Explorer", "Photography", "Parents", "Debug"
            }));
            Assert.That(InputFoundationValidationService.Validate(), Is.Empty);
        }

        [Test]
        public void SafeAreaModelsCoverRequiredLandscapeRatiosAndRotation()
        {
            Assert.That(DeviceAspectPresets.Landscape.Select(item => item.Id), Is.EquivalentTo(new[]
            {
                "tablet-4-3", "phone-16-9", "phone-20-9", "tablet-16-10"
            }));
            foreach (DeviceAspectPreset preset in DeviceAspectPresets.Landscape)
            {
                var left = new SafeAreaSnapshot(preset.Width, preset.Height, preset.LeftInset, 0f, preset.RightInset, 0f, DisplayOrientation.LandscapeLeft);
                var right = new SafeAreaSnapshot(preset.Width, preset.Height, preset.RightInset, 0f, preset.LeftInset, 0f, DisplayOrientation.LandscapeRight);
                Assert.That(left.Width, Is.GreaterThanOrEqualTo(0.9f), preset.Id);
                Assert.That(right.Width, Is.EqualTo(left.Width).Within(0.0001f), preset.Id);
            }
        }

        [Test]
        public void HapticsDefaultIsDisabledAndNoOp()
        {
            var haptics = new NoOpHapticsService();
            haptics.InitializeAsync(default).GetAwaiter().GetResult();
            Assert.That(haptics.Enabled, Is.False);
            Assert.DoesNotThrow(() => haptics.Pulse(HapticFeedbackKind.Confirmation));
            haptics.SetEnabled(true);
            Assert.That(haptics.Enabled, Is.True);
            haptics.Shutdown();
            Assert.That(haptics.Enabled, Is.False);
        }
    }
}
