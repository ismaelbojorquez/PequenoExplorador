using System;
using PequenoExplorador.Application.Explorer;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.Tutorial;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace PequenoExplorador.Presentation.Explorer
{
    [DisallowMultipleComponent]
    public sealed class ExplorerLocomotionRoot : MonoBehaviour, IInteractionApproach
    {
        public const string PlaceholderRootName = "PH_EXPLORER_RUNTIME";

        [Header("Required scene references")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Transform _visual;
        [SerializeField] private GameObject _destinationMarker;
        [SerializeField] private GameObject _invalidMarker;

        [Header("Tap and path")]
        [SerializeField, Min(0.1f)] private float _rayDistance = 100f;
        [SerializeField, Min(0.05f)] private float _navMeshSampleRadius = 0.75f;
        [SerializeField, Min(0.05f)] private float _arrivalSpeed = 0.08f;
        [SerializeField, Min(0.1f)] private float _feedbackSeconds = 0.8f;

        [Header("Assisted camera")]
        [SerializeField] private Vector3 _cameraOffset = new Vector3(0f, 7.5f, -6.5f);
        [SerializeField] private Vector3 _cameraLookOffset = new Vector3(0f, 0.7f, 0f);
        [SerializeField, Min(0.01f)] private float _cameraDampingSeconds = 0.22f;
        [SerializeField] private Vector2 _cameraBoundsX = new Vector2(-8f, 8f);
        [SerializeField] private Vector2 _cameraBoundsZ = new Vector2(-9f, 7f);

        private IInputService _input;
        private Camera _camera;
        private ExplorerLocomotionController _controller;
        private Vector3 _cameraVelocity;
        private Vector3 _visualBasePosition;
        private float _feedbackUntil;
        private bool _reduceMotion;
        private bool _applicationPaused;
        private bool _applicationFocused = true;
        private bool _bound;
        private IExplorerTapHandler _tapHandler;
        private Transform _photographyFocus;
        private Func<TutorialAction, bool> _tutorialGate;

        public event Action MovementAccepted;

        public ExplorerLocomotionState State => _controller?.State ?? ExplorerLocomotionState.Idle;
        public bool IsBound => _bound;
        public bool ReduceMotion => _reduceMotion;
        public NavMeshAgent Agent => _agent;
        public WorldPosition Position => _agent == null
            ? default
            : new WorldPosition(_agent.transform.position.x, _agent.transform.position.y, _agent.transform.position.z);

        public void Bind(IInputService input, Camera worldCamera, bool reduceMotion = false)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (worldCamera == null) throw new ArgumentNullException(nameof(worldCamera));
            ValidateReferences();
            Unbind();
            _input = input;
            _camera = worldCamera;
            _reduceMotion = reduceMotion;
            _visualBasePosition = _visual.localPosition;
            DetachFeedbackMarkers();
            PlaceAgentOnNavMesh();
            _controller = new ExplorerLocomotionController(
                new UnityNavMeshPathNavigator(_agent),
                new ExplorerLocomotionSettings(_agent.stoppingDistance, _arrivalSpeed));
            _input.IntentRaised += HandleIntent;
            _input.MapChanged += HandleMapChanged;
            _bound = true;
            ApplySuspension();
            SnapCamera();
            HideFeedback();
        }

        public void Unbind()
        {
            if (_input != null)
            {
                _input.IntentRaised -= HandleIntent;
                _input.MapChanged -= HandleMapChanged;
            }
            _controller?.Cancel();
            _controller = null;
            _tapHandler = null;
            _photographyFocus = null;
            _input = null;
            _camera = null;
            _bound = false;
            HideFeedback();
        }

        public void SetReduceMotion(bool enabled)
        {
            _reduceMotion = enabled;
            if (enabled) SnapCamera();
        }

        public void SetTapHandler(IExplorerTapHandler tapHandler) => _tapHandler = tapHandler;
        public void SetTutorialGate(Func<TutorialAction, bool> gate) => _tutorialGate = gate;

        public void SetPhotographyFocus(Transform focus)
        {
            _photographyFocus = focus;
            if (focus != null) CancelMovement();
        }

        public bool TryMoveTo(WorldPosition destination)
        {
            if (!_bound || _controller == null || _input.CurrentMap != InputMapId.Explorer ||
                (_tutorialGate != null && !_tutorialGate(TutorialAction.Move))) return false;
            bool accepted = _controller.MoveTo(destination);
            if (accepted) MovementAccepted?.Invoke();
            return accepted;
        }

        public void CancelMovement() => _controller?.Cancel();

        public bool TryHandleScreenTap(ScreenPoint screenPoint)
        {
            if (!_bound || _controller == null || _input.CurrentMap != InputMapId.Explorer)
                return false;
            if (_tutorialGate != null && !_tutorialGate(TutorialAction.Move)) return false;

            Ray ray = _camera.ScreenPointToRay(new Vector3(screenPoint.X, screenPoint.Y));
            if (!Physics.Raycast(ray, out RaycastHit hit, _rayDistance, Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction.Ignore) ||
                hit.collider.GetComponentInParent<WalkableSurfaceMarker>() == null ||
                !NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, _navMeshSampleRadius, NavMesh.AllAreas))
            {
                _controller.RejectDestination();
                ShowFeedback(_invalidMarker, hit.point);
                return false;
            }

            bool accepted = _controller.MoveTo(new WorldPosition(
                navHit.position.x, navHit.position.y, navHit.position.z));
            ShowFeedback(accepted ? _destinationMarker : _invalidMarker, navHit.position);
            if (accepted) MovementAccepted?.Invoke();
            return accepted;
        }

        private void Update()
        {
            _controller?.Tick();
            AnimatePlaceholder();
            if (_feedbackUntil > 0f && Time.unscaledTime >= _feedbackUntil) HideFeedback();
        }

        private void LateUpdate()
        {
            if (!_bound || _camera == null || _agent == null) return;
            Vector3 desired = ClampCameraPosition(_agent.transform.position + _cameraOffset);
            _camera.transform.position = _reduceMotion
                ? desired
                : Vector3.SmoothDamp(
                    _camera.transform.position,
                    desired,
                    ref _cameraVelocity,
                    _cameraDampingSeconds,
                    Mathf.Infinity,
                    Time.unscaledDeltaTime);
            _camera.transform.LookAt(_photographyFocus != null
                ? _photographyFocus.position
                : _agent.transform.position + _cameraLookOffset, Vector3.up);
        }

        private void HandleIntent(InputIntent intent)
        {
            if (intent.Map == InputMapId.Explorer && intent.Kind == InputGestureKind.Tap &&
                !IsPointerOverUi(intent.PointerId))
            {
                if (_tapHandler == null || !_tapHandler.TryHandleTap(intent.Position))
                    TryHandleScreenTap(intent.Position);
            }
        }

        private void HandleMapChanged(InputMapId map) => ApplySuspension();

        private void ApplySuspension()
        {
            _controller?.SetSuspended(_applicationPaused || !_applicationFocused || _input == null ||
                                      _input.CurrentMap != InputMapId.Explorer);
        }

        private void OnApplicationPause(bool paused)
        {
            _applicationPaused = paused;
            ApplySuspension();
        }

        private void OnApplicationFocus(bool focused)
        {
            _applicationFocused = focused;
            ApplySuspension();
        }

        private void DetachFeedbackMarkers()
        {
            _destinationMarker.transform.SetParent(transform.parent, true);
            _invalidMarker.transform.SetParent(transform.parent, true);
        }

        private void PlaceAgentOnNavMesh()
        {
            if (_agent.isOnNavMesh) return;
            if (!NavMesh.SamplePosition(_agent.transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas) ||
                !_agent.Warp(hit.position))
                throw new InvalidOperationException("PH_ explorer spawn is not on the baked Jungle NavMesh.");
        }

        private void AnimatePlaceholder()
        {
            if (_visual == null) return;
            bool moving = State == ExplorerLocomotionState.Moving;
            float bob = moving && !_reduceMotion ? Mathf.Sin(Time.time * 8f) * 0.045f : 0f;
            _visual.localPosition = _visualBasePosition + new Vector3(0f, bob, 0f);
        }

        private void ShowFeedback(GameObject target, Vector3 position)
        {
            HideFeedback();
            if (target == null) return;
            target.transform.position = position + Vector3.up * 0.03f;
            target.SetActive(true);
            _feedbackUntil = Time.unscaledTime + _feedbackSeconds;
        }

        private void HideFeedback()
        {
            if (_destinationMarker != null) _destinationMarker.SetActive(false);
            if (_invalidMarker != null) _invalidMarker.SetActive(false);
            _feedbackUntil = 0f;
        }

        private void SnapCamera()
        {
            if (_camera == null || _agent == null) return;
            _cameraVelocity = Vector3.zero;
            _camera.transform.position = ClampCameraPosition(_agent.transform.position + _cameraOffset);
            _camera.transform.LookAt(_photographyFocus != null
                ? _photographyFocus.position
                : _agent.transform.position + _cameraLookOffset, Vector3.up);
        }

        private Vector3 ClampCameraPosition(Vector3 value)
        {
            value.x = Mathf.Clamp(value.x, _cameraBoundsX.x, _cameraBoundsX.y);
            value.z = Mathf.Clamp(value.z, _cameraBoundsZ.x, _cameraBoundsZ.y);
            return value;
        }

        private static bool IsPointerOverUi(int pointerId) =>
            EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(pointerId);

        private void ValidateReferences()
        {
            if (_agent == null || _visual == null || _destinationMarker == null || _invalidMarker == null)
                throw new InvalidOperationException("PH_ explorer root is missing required references.");
        }

        private void OnDisable() => Unbind();
        private void OnDestroy() => Unbind();
    }
}
