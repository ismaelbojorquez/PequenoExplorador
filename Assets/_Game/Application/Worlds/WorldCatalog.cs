using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Worlds
{
    public sealed class WorldCatalogEntry
    {
        public WorldCatalogEntry(WorldManifest manifest, WorldAvailabilityState availability)
        {
            Manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            Availability = availability;
        }
        public WorldManifest Manifest { get; }
        public WorldAvailabilityState Availability { get; }
    }

    public sealed class WorldCatalog : IWorldCatalog
    {
        private readonly IReadOnlyDictionary<WorldId, WorldCatalogEntry> _index;
        private readonly IReadOnlyCollection<WorldCatalogEntry> _ordered;

        public WorldCatalog(IEnumerable<WorldCatalogEntry> entries)
        {
            WorldCatalogEntry[] ordered = (entries ?? Array.Empty<WorldCatalogEntry>())
                .OrderBy(entry => entry?.Manifest.Id.Value, StringComparer.Ordinal).ToArray();
            if (ordered.Any(entry => entry == null)) throw new ArgumentException("World catalog cannot contain null entries.", nameof(entries));
            var index = new Dictionary<WorldId, WorldCatalogEntry>();
            foreach (WorldCatalogEntry entry in ordered)
                if (!index.TryAdd(entry.Manifest.Id, entry)) throw new ArgumentException("Duplicate world ID: " + entry.Manifest.Id, nameof(entries));
            _index = new ReadOnlyDictionary<WorldId, WorldCatalogEntry>(index);
            _ordered = Array.AsReadOnly(ordered);
        }

        public static WorldCatalog Empty { get; } = new WorldCatalog(Array.Empty<WorldCatalogEntry>());
        public IReadOnlyCollection<WorldCatalogEntry> Worlds => _ordered;
        public bool TryGet(WorldId id, out WorldCatalogEntry entry) => _index.TryGetValue(id, out entry);
    }
}
