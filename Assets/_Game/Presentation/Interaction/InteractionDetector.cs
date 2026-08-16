using System;
using System.Collections.Generic;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.Interaction;
using UnityEngine;

namespace PequenoExplorador.Presentation.Interaction
{
    [DisallowMultipleComponent]
    public sealed class InteractionDetector : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float _rayDistance = 100f;

        private readonly RaycastHit[] _hits = new RaycastHit[16];
        private readonly List<InteractionCandidate> _candidates = new List<InteractionCandidate>(8);
        private readonly Dictionary<Collider, WorldInteractableView> _index =
            new Dictionary<Collider, WorldInteractableView>();
        private Camera _camera;
        private InteractionCoordinator _coordinator;

        public int IndexedColliderCount => _index.Count;

        public void Bind(
            Camera worldCamera,
            InteractionCoordinator coordinator,
            IEnumerable<WorldInteractableView> targets)
        {
            _camera = worldCamera ?? throw new ArgumentNullException(nameof(worldCamera));
            _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
            _index.Clear();
            foreach (WorldInteractableView target in targets ?? Array.Empty<WorldInteractableView>())
            {
                if (target == null) continue;
                foreach (Collider collider in target.TargetColliders)
                {
                    if (!_index.TryAdd(collider, target))
                        throw new InvalidOperationException($"Collider '{collider.name}' belongs to more than one interactable.");
                }
            }
            if (_index.Count == 0)
                throw new InvalidOperationException("Interaction detector has no indexed colliders.");
        }

        public void Unbind()
        {
            _index.Clear();
            _candidates.Clear();
            _camera = null;
            _coordinator = null;
        }

        public bool TryHandle(ScreenPoint screenPoint)
        {
            if (_camera == null || _coordinator == null) return false;
            Ray ray = _camera.ScreenPointToRay(new Vector3(screenPoint.X, screenPoint.Y));
            int hitCount = Physics.RaycastNonAlloc(
                ray,
                _hits,
                _rayDistance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Collide);
            _candidates.Clear();
            for (int index = 0; index < hitCount; index++)
            {
                RaycastHit hit = _hits[index];
                if (_index.TryGetValue(hit.collider, out WorldInteractableView target) && target.IsAlive)
                    _candidates.Add(new InteractionCandidate(target, hit.distance));
            }
            if (!InteractionTargetSelector.TrySelect(_candidates, out IInteractable selected)) return false;
            var view = (WorldInteractableView)selected;
            Vector3 point = view.InteractionPoint.position;
            _coordinator.Focus(
                selected,
                new PequenoExplorador.Application.Explorer.WorldPosition(point.x, point.y, point.z));
            return true;
        }
    }
}
