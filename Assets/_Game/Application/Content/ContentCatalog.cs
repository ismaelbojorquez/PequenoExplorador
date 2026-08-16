using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Content
{
    public sealed class ContentCatalog : IContentCatalog
    {
        private readonly IReadOnlyDictionary<DiscoveryId, DiscoveryDefinition> _discoveries;
        private readonly IReadOnlyDictionary<CategoryId, CategoryDefinition> _categories;
        private readonly IReadOnlyDictionary<TagId, TagDefinition> _tags;
        private readonly IReadOnlyDictionary<EducationalFactId, EducationalFactDefinition> _facts;
        private readonly IReadOnlyDictionary<ContentSourceId, ContentSourceRecord> _sources;
        private readonly IReadOnlyDictionary<DiscoveryId, DiscoveryId> _aliases;
        private readonly IReadOnlyCollection<DiscoveryDefinition> _orderedDiscoveries;

        public ContentCatalog(IEnumerable<DiscoveryDefinition> discoveries, IEnumerable<DiscoveryIdAlias> aliases)
            : this(Array.Empty<CategoryDefinition>(), Array.Empty<TagDefinition>(), Array.Empty<ContentSourceRecord>(),
                Array.Empty<EducationalFactDefinition>(), discoveries, aliases)
        {
        }

        public ContentCatalog(
            IEnumerable<CategoryDefinition> categories,
            IEnumerable<TagDefinition> tags,
            IEnumerable<ContentSourceRecord> sources,
            IEnumerable<EducationalFactDefinition> facts,
            IEnumerable<DiscoveryDefinition> discoveries,
            IEnumerable<DiscoveryIdAlias> aliases)
        {
            DiscoveryDefinition[] raw = (discoveries ?? Array.Empty<DiscoveryDefinition>()).ToArray();
            if (raw.Any(item => item == null)) throw new ArgumentException("Catalog cannot contain null discoveries.", nameof(discoveries));
            DiscoveryDefinition[] ordered = raw.OrderBy(item => item.Id.Value, StringComparer.Ordinal).ToArray();
            var indexed = new Dictionary<DiscoveryId, DiscoveryDefinition>();
            foreach (DiscoveryDefinition definition in ordered)
            {
                if (!indexed.TryAdd(definition.Id, definition)) throw new ArgumentException("Duplicate discovery ID: " + definition.Id, nameof(discoveries));
            }

            var aliasIndex = new Dictionary<DiscoveryId, DiscoveryId>();
            foreach (DiscoveryIdAlias alias in aliases ?? Array.Empty<DiscoveryIdAlias>())
            {
                if (indexed.ContainsKey(alias.Previous) || !indexed.ContainsKey(alias.Current) || !aliasIndex.TryAdd(alias.Previous, alias.Current))
                    throw new ArgumentException("Invalid or duplicate discovery alias: " + alias.Previous, nameof(aliases));
            }

            _discoveries = new ReadOnlyDictionary<DiscoveryId, DiscoveryDefinition>(indexed);
            _categories = Index(categories, item => item.Id, "category");
            _tags = Index(tags, item => item.Id, "tag");
            _sources = Index(sources, item => item.Id, "source");
            _facts = Index(facts, item => item.Id, "fact");
            _aliases = new ReadOnlyDictionary<DiscoveryId, DiscoveryId>(aliasIndex);
            _orderedDiscoveries = Array.AsReadOnly(ordered);
        }

        public static ContentCatalog Empty { get; } = new ContentCatalog(Array.Empty<DiscoveryDefinition>(), Array.Empty<DiscoveryIdAlias>());
        public IReadOnlyCollection<DiscoveryDefinition> Discoveries => _orderedDiscoveries;
        public bool TryGetCategory(CategoryId id, out CategoryDefinition definition) => _categories.TryGetValue(id, out definition);
        public bool TryGetTag(TagId id, out TagDefinition definition) => _tags.TryGetValue(id, out definition);
        public bool TryGetFact(EducationalFactId id, out EducationalFactDefinition definition) => _facts.TryGetValue(id, out definition);
        public bool TryGetSource(ContentSourceId id, out ContentSourceRecord record) => _sources.TryGetValue(id, out record);
        public bool TryGetDiscovery(DiscoveryId id, out DiscoveryDefinition definition) => _discoveries.TryGetValue(id, out definition);
        public bool TryResolveDiscovery(DiscoveryId idOrAlias, out DiscoveryDefinition definition)
        {
            if (_discoveries.TryGetValue(idOrAlias, out definition)) return true;
            return _aliases.TryGetValue(idOrAlias, out DiscoveryId current) && _discoveries.TryGetValue(current, out definition);
        }

        private static IReadOnlyDictionary<TId, TDefinition> Index<TId, TDefinition>(
            IEnumerable<TDefinition> definitions,
            Func<TDefinition, TId> idSelector,
            string kind)
            where TDefinition : class
        {
            var index = new Dictionary<TId, TDefinition>();
            foreach (TDefinition definition in definitions ?? Array.Empty<TDefinition>())
            {
                if (definition == null) throw new ArgumentException("Catalog cannot contain null " + kind + " definitions.", nameof(definitions));
                TId id = idSelector(definition);
                if (!index.TryAdd(id, definition)) throw new ArgumentException("Duplicate " + kind + " ID: " + id, nameof(definitions));
            }
            return new ReadOnlyDictionary<TId, TDefinition>(index);
        }
    }
}
