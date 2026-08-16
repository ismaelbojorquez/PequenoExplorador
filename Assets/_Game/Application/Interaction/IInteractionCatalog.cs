using System.Collections.Generic;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Interaction
{
    public interface IInteractionCatalog
    {
        IReadOnlyCollection<InteractionDefinition> Definitions { get; }
        bool TryGet(InteractionId id, out InteractionDefinition definition);
        bool TryGet(string rawId, out InteractionDefinition definition);
    }
}
