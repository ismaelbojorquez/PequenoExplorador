using System;
using System.Collections.Generic;
using PequenoExplorador.Application.Interaction;
using UnityEngine;

namespace PequenoExplorador.Presentation.Interaction
{
    [DisallowMultipleComponent]
    public sealed class WorldInteractableView : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _interactionId;
        [SerializeField] private Transform _interactionPoint;
        [SerializeField] private Collider[] _targetColliders = Array.Empty<Collider>();
        [SerializeField] private GameObject _focusIndicator;
        [SerializeField] private bool _available = true;

        private InteractionDefinition _definition;
        private bool _destroyed;

        public InteractionDefinition Definition => _definition;
        public bool IsAvailable => _available;
        public bool IsAlive => !_destroyed && this != null && gameObject != null;
        public string RawInteractionId => _interactionId;
        public Transform InteractionPoint => _interactionPoint;
        public IReadOnlyList<Collider> TargetColliders => _targetColliders ?? Array.Empty<Collider>();
        public int ActivationCount { get; private set; }

        public void Bind(InteractionDefinition definition)
        {
            _definition = definition ?? throw new ArgumentNullException(nameof(definition));
            if (!string.Equals(_interactionId, definition.Id.Value, StringComparison.Ordinal))
                throw new InvalidOperationException(
                    $"Fixture '{name}' expects '{_interactionId}' but received '{definition.Id}'.");
            ValidateReferences();
            SetFocused(false);
        }

        public void SetFocused(bool focused)
        {
            if (_focusIndicator != null) _focusIndicator.SetActive(focused);
        }

        public void SetAvailableForTests(bool available) => _available = available;

        public InteractionResult Interact(InteractionContext context)
        {
            if (!IsAlive || _definition == null || !_available)
                return new InteractionResult(
                    InteractionOutcome.Unavailable,
                    _definition?.Id ?? default,
                    _definition?.Unavailable ?? default,
                    _definition?.UnavailableAudioCue ?? default);
            ActivationCount++;
            return new InteractionResult(
                InteractionOutcome.Completed,
                _definition.Id,
                _definition.Prompt,
                default);
        }

        private void ValidateReferences()
        {
            if (_interactionPoint == null || _targetColliders == null || _targetColliders.Length == 0 ||
                Array.Exists(_targetColliders, collider => collider == null) || _focusIndicator == null)
                throw new InvalidOperationException(
                    $"PH_ interactable '{name}' requires point, colliders and focus indicator.");
        }

        private void OnDestroy() => _destroyed = true;
    }
}
