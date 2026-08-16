using System;
using System.Collections.Generic;

namespace PequenoExplorador.Application.Interaction
{
    public static class InteractionTargetSelector
    {
        public static bool TrySelect(
            IEnumerable<InteractionCandidate> candidates,
            out IInteractable selected)
        {
            selected = null;
            float selectedDistance = float.PositiveInfinity;
            foreach (InteractionCandidate candidate in candidates ?? Array.Empty<InteractionCandidate>())
            {
                IInteractable target = candidate.Target;
                if (!target.IsAlive || target.Definition == null) continue;
                if (selected == null || IsPreferred(target, candidate.RayDistance, selected, selectedDistance))
                {
                    selected = target;
                    selectedDistance = candidate.RayDistance;
                }
            }
            return selected != null;
        }

        private static bool IsPreferred(
            IInteractable candidate,
            float candidateDistance,
            IInteractable current,
            float currentDistance)
        {
            int priority = candidate.Definition.Priority.CompareTo(current.Definition.Priority);
            if (priority != 0) return priority > 0;
            int distance = candidateDistance.CompareTo(currentDistance);
            if (distance != 0) return distance < 0;
            return candidate.Definition.Id.CompareTo(current.Definition.Id) < 0;
        }
    }
}
