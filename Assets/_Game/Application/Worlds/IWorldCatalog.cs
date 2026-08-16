using System.Collections.Generic;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Worlds
{
    public interface IWorldCatalog
    {
        IReadOnlyCollection<WorldCatalogEntry> Worlds { get; }
        bool TryGet(WorldId id, out WorldCatalogEntry entry);
    }
}
