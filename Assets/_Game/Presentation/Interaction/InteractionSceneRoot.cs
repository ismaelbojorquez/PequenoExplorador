using System;
using System.Linq;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Application.Learning;
using PequenoExplorador.Application.Tutorial;
using UnityEngine;

namespace PequenoExplorador.Presentation.Interaction
{
    [DisallowMultipleComponent]
    public sealed class InteractionSceneRoot : MonoBehaviour, IExplorerTapHandler
    {
        public const string PlaceholderRootName = "PH_INTERACTION_FIXTURES";

        [SerializeField] private InteractionDetector _detector;
        [SerializeField] private WorldInteractableView[] _targets = Array.Empty<WorldInteractableView>();

        private IInputService _input;
        private bool _applicationPaused;
        private bool _applicationFocused = true;
        private Func<TutorialAction, bool> _tutorialGate;

        public InteractionCoordinator Coordinator { get; private set; }
        public int TargetCount => _targets?.Length ?? 0;
        public InteractionDetector Detector => _detector;
        public WorldInteractableView[] Targets => _targets;
        public void SetTutorialGate(Func<TutorialAction, bool> gate) => _tutorialGate = gate;

        public void Bind(
            IInteractionCatalog catalog,
            IInteractionApproach approach,
            IClock clock,
            IInputService input,
            Camera worldCamera,
            IInteractionAction directDiscoveryAction = null,
            LearningInteractionAction learningAction = null)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (_detector == null || _targets == null || _targets.Length == 0 ||
                _targets.Any(target => target == null))
                throw new InvalidOperationException("PH_ interaction scene root requires detector and targets.");
            Unbind();
            foreach (WorldInteractableView target in _targets)
            {
                if (!catalog.TryGet(target.RawInteractionId, out InteractionDefinition definition))
                    throw new InvalidOperationException(
                        $"Interaction catalog is missing '{target.RawInteractionId}'.");
                if (definition.HasDirectDiscovery && directDiscoveryAction == null)
                    throw new InvalidOperationException(
                        $"Interaction '{definition.Id}' requires its explicit discovery action.");
                if (definition.HasLearningActivity && learningAction == null)
                    throw new InvalidOperationException($"Interaction '{definition.Id}' requires its explicit learning action.");
                // Activity is the primary action when a definition also has a discovery. The
                // activity presenter explicitly continues into photography after completion.
                target.Bind(definition, definition.HasLearningActivity ? learningAction :
                    definition.HasDirectDiscovery ? directDiscoveryAction : null);
            }
            Coordinator = new InteractionCoordinator(approach, clock);
            Coordinator.Changed += HandleChanged;
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _input.MapChanged += HandleMapChanged;
            _detector.Bind(worldCamera, Coordinator, _targets);
            ApplySuspension();
        }

        public void Unbind()
        {
            if (_input != null) _input.MapChanged -= HandleMapChanged;
            if (Coordinator != null)
            {
                Coordinator.Changed -= HandleChanged;
                Coordinator.Cancel();
            }
            if (_detector != null) _detector.Unbind();
            foreach (WorldInteractableView target in _targets ?? Array.Empty<WorldInteractableView>())
                if (target != null) target.SetFocused(false);
            Coordinator = null;
            _input = null;
        }

        public bool TryHandleTap(ScreenPoint screenPoint) =>
            Coordinator != null && (_tutorialGate == null || _tutorialGate(TutorialAction.Interact)) && _detector.TryHandle(screenPoint);

        private void Update() => Coordinator?.Tick();

        private void HandleChanged(InteractionSnapshot snapshot)
        {
            foreach (WorldInteractableView target in _targets)
                if (target != null)
                    target.SetFocused(snapshot.HasFocus && target.Definition.Id == snapshot.Definition.Id);
        }

        private void HandleMapChanged(InputMapId map) => ApplySuspension();

        private void ApplySuspension()
        {
            Coordinator?.SetSuspended(
                _applicationPaused || !_applicationFocused || _input == null ||
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

        private void OnDisable() => Unbind();
        private void OnDestroy() => Unbind();
    }
}
