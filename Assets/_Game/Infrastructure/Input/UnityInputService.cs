using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace PequenoExplorador.Infrastructure.Input
{
    public sealed class UnityInputService : IInputService, IDisposable
    {
        private const int MousePointerId = -1;
        private readonly InputActionAsset _sourceAsset;
        private readonly GestureRecognizer _recognizer;
        private readonly bool _allowDebugMap;
        private InputActionAsset _runtimeAsset;
        private InputActionMap _activeMap;
        private InputActionMap _debugMap;
        private InputAction _pointAction;
        private InputAction _pressAction;
        private InputAction _backAction;
        private InputAction _debugToggleAction;
        private bool _mousePressed;
        private bool _enhancedTouchWasEnabled;
        private bool _initialized;

        public UnityInputService(InputActionAsset sourceAsset, GestureThresholds thresholds, bool allowDebugMap)
        {
            _sourceAsset = sourceAsset != null ? sourceAsset : throw new ArgumentNullException(nameof(sourceAsset));
            _recognizer = new GestureRecognizer(thresholds ?? throw new ArgumentNullException(nameof(thresholds)));
            _recognizer.IntentRaised += ForwardIntent;
            _allowDebugMap = allowDebugMap;
            CurrentMap = InputMapId.UI;
        }

        public string ServiceId => "Input";
        public event Action<InputIntent> IntentRaised;
        public event Action BackRequested;
        public InputMapId CurrentMap { get; private set; }
        public bool DebugMapEnabled => _debugMap != null && _debugMap.enabled;

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_initialized) return Task.CompletedTask;
            _runtimeAsset = UnityEngine.Object.Instantiate(_sourceAsset);
            _runtimeAsset.name = _sourceAsset.name + " (Runtime)";
            _enhancedTouchWasEnabled = EnhancedTouchSupport.enabled;
            if (!_enhancedTouchWasEnabled) EnhancedTouchSupport.Enable();
            ActivateMap(CurrentMap);
            if (_allowDebugMap)
            {
                _debugMap = RequireMap(InputMapId.Debug);
                _debugToggleAction = _debugMap.FindAction("ToggleOverlay", true);
                _debugToggleAction.performed += OnDebugToggle;
                _debugMap.Enable();
            }
            _initialized = true;
            return Task.CompletedTask;
        }

        public void SetMap(InputMapId map)
        {
            if (map == InputMapId.Debug) throw new ArgumentException("Debug is an additive map, not a product context.", nameof(map));
            if (CurrentMap == map) return;
            _recognizer.SetMap(map);
            CurrentMap = map;
            if (_initialized) ActivateMap(map);
        }

        public void Tick(double unscaledTime)
        {
            if (!_initialized || CurrentMap == InputMapId.None) return;
            // InputTestFixture and domain reload can reset the Input System while the scene root
            // is still alive for one frame. Restore the adapter-owned EnhancedTouch state safely.
            if (!EnhancedTouchSupport.enabled) EnhancedTouchSupport.Enable();
            var activeTouches = Touch.activeTouches;
            if (activeTouches.Count > 0)
            {
                if (_mousePressed)
                {
                    _recognizer.CancelPointer(MousePointerId);
                    _mousePressed = false;
                }
                for (int index = 0; index < activeTouches.Count; index++)
                {
                    Touch touch = activeTouches[index];
                    var point = new ScreenPoint(touch.screenPosition.x, touch.screenPosition.y);
                    switch (touch.phase)
                    {
                        case UnityEngine.InputSystem.TouchPhase.Began:
                            _recognizer.BeginPointer(touch.touchId, point, unscaledTime);
                            break;
                        case UnityEngine.InputSystem.TouchPhase.Moved:
                        case UnityEngine.InputSystem.TouchPhase.Stationary:
                            _recognizer.MovePointer(touch.touchId, point, unscaledTime);
                            break;
                        case UnityEngine.InputSystem.TouchPhase.Ended:
                            _recognizer.EndPointer(touch.touchId, point, unscaledTime);
                            break;
                        case UnityEngine.InputSystem.TouchPhase.Canceled:
                            _recognizer.CancelPointer(touch.touchId);
                            break;
                    }
                }
            }
            else
            {
                TickPrimaryPointer(unscaledTime);
            }
            _recognizer.AdvanceTime(unscaledTime);
        }

        public void CancelActiveGestures()
        {
            _recognizer.CancelAll();
            _mousePressed = false;
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            CancelActiveGestures();
            UnwireActiveMap();
            if (_debugToggleAction != null) _debugToggleAction.performed -= OnDebugToggle;
            _debugMap?.Disable();
            if (!_enhancedTouchWasEnabled && EnhancedTouchSupport.enabled) EnhancedTouchSupport.Disable();
            if (_runtimeAsset != null) UnityEngine.Object.Destroy(_runtimeAsset);
            _runtimeAsset = null;
            _debugMap = null;
            _debugToggleAction = null;
            _initialized = false;
        }

        public void Dispose()
        {
            Shutdown();
            _recognizer.IntentRaised -= ForwardIntent;
        }

        private void TickPrimaryPointer(double time)
        {
            if (_pointAction == null || _pressAction == null) return;
            Vector2 position = _pointAction.ReadValue<Vector2>();
            var point = new ScreenPoint(position.x, position.y);
            bool pressed = _pressAction.IsPressed();
            if (pressed && !_mousePressed)
            {
                _mousePressed = true;
                _recognizer.BeginPointer(MousePointerId, point, time);
            }
            else if (pressed)
            {
                _recognizer.MovePointer(MousePointerId, point, time);
            }
            else if (_mousePressed)
            {
                _mousePressed = false;
                _recognizer.EndPointer(MousePointerId, point, time);
            }
        }

        private void ActivateMap(InputMapId map)
        {
            UnwireActiveMap();
            if (map == InputMapId.None) return;
            _activeMap = RequireMap(map);
            _pointAction = _activeMap.FindAction("Point", true);
            _pressAction = _activeMap.FindAction("PrimaryPress", true);
            _backAction = _activeMap.FindAction("Back", true);
            _backAction.performed += OnBack;
            _activeMap.Enable();
        }

        private void UnwireActiveMap()
        {
            if (_backAction != null) _backAction.performed -= OnBack;
            _activeMap?.Disable();
            _activeMap = null;
            _pointAction = null;
            _pressAction = null;
            _backAction = null;
        }

        private InputActionMap RequireMap(InputMapId map) =>
            _runtimeAsset.FindActionMap(map.ToString(), true);

        private void OnBack(InputAction.CallbackContext context)
        {
            IntentRaised?.Invoke(new InputIntent(CurrentMap, InputGestureKind.Back, -1, default, default));
            BackRequested?.Invoke();
        }

        private void OnDebugToggle(InputAction.CallbackContext context) =>
            IntentRaised?.Invoke(new InputIntent(InputMapId.Debug, InputGestureKind.DebugToggle, -1, default, default));

        private void ForwardIntent(InputIntent intent) => IntentRaised?.Invoke(intent);
    }
}
