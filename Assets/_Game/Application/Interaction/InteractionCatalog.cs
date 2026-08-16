using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Interaction
{
    public sealed class InteractionCatalog : IInteractionCatalog
    {
        private readonly IReadOnlyDictionary<InteractionId, InteractionDefinition> _index;
        private readonly IReadOnlyCollection<InteractionDefinition> _ordered;

        public InteractionCatalog(IEnumerable<InteractionDefinition> definitions)
        {
            InteractionDefinition[] ordered = (definitions ?? Array.Empty<InteractionDefinition>())
                .OrderBy(item => item?.Id.Value, StringComparer.Ordinal).ToArray();
            var index = new Dictionary<InteractionId, InteractionDefinition>();
            foreach (InteractionDefinition definition in ordered)
            {
                if (definition == null) throw new ArgumentException("Interaction catalog cannot contain null definitions.");
                if (!index.TryAdd(definition.Id, definition))
                    throw new ArgumentException("Duplicate interaction ID: " + definition.Id);
            }
            _index = new ReadOnlyDictionary<InteractionId, InteractionDefinition>(index);
            _ordered = Array.AsReadOnly(ordered);
        }

        public IReadOnlyCollection<InteractionDefinition> Definitions => _ordered;
        public bool TryGet(InteractionId id, out InteractionDefinition definition) =>
            _index.TryGetValue(id, out definition);

        public bool TryGet(string rawId, out InteractionDefinition definition)
        {
            definition = null;
            return InteractionId.TryParse(rawId, out InteractionId id) && _index.TryGetValue(id, out definition);
        }
    }
}
