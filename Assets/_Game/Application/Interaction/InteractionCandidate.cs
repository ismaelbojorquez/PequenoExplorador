using System;

namespace PequenoExplorador.Application.Interaction
{
    public readonly struct InteractionCandidate
    {
        public InteractionCandidate(IInteractable target, float rayDistance)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            if (float.IsNaN(rayDistance) || rayDistance < 0f)
                throw new ArgumentOutOfRangeException(nameof(rayDistance));
            RayDistance = rayDistance;
        }

        public IInteractable Target { get; }
        public float RayDistance { get; }
    }
}
