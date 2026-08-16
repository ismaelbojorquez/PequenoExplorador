using System.Collections.Generic;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Content
{
    public interface IContentCatalog
    {
        IReadOnlyCollection<DiscoveryDefinition> Discoveries { get; }
        bool TryGetCategory(CategoryId id, out CategoryDefinition definition);
        bool TryGetTag(TagId id, out TagDefinition definition);
        bool TryGetFact(EducationalFactId id, out EducationalFactDefinition definition);
        bool TryGetSource(ContentSourceId id, out ContentSourceRecord record);
        bool TryGetDiscovery(DiscoveryId id, out DiscoveryDefinition definition);
        bool TryResolveDiscovery(DiscoveryId idOrAlias, out DiscoveryDefinition definition);
    }
}
