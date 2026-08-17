using System;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Photography
{
    public sealed class PhotoTarget
    {
        public PhotoTarget(DiscoveryId discoveryId, PhotoEvaluationSettings settings)
        {
            if (!discoveryId.IsValid) throw new ArgumentException("Discovery ID is invalid.", nameof(discoveryId));
            DiscoveryId = discoveryId;
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public DiscoveryId DiscoveryId { get; }
        public PhotoEvaluationSettings Settings { get; }
    }
}
