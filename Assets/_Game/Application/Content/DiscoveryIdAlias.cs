using System;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Content
{
    public readonly struct DiscoveryIdAlias
    {
        public DiscoveryIdAlias(DiscoveryId previous, DiscoveryId current)
        {
            if (!previous.IsValid || !current.IsValid || previous == current)
                throw new ArgumentException("Alias requires distinct valid discovery IDs.");
            Previous = previous;
            Current = current;
        }
        public DiscoveryId Previous { get; }
        public DiscoveryId Current { get; }
    }
}
