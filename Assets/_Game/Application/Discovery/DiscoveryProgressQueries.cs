using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Discovery
{
    public sealed class DiscoveryProgressQueries
    {
        private readonly IContentCatalog _catalog;
        private readonly IDiscoveryProgressRepository _repository;

        public DiscoveryProgressQueries(IContentCatalog catalog, IDiscoveryProgressRepository repository)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public DiscoveryProgressSummary ForWorld(WorldId worldId) =>
            Summarize(item => item.WorldId.Equals(worldId));

        public DiscoveryProgressSummary ForCategory(CategoryId categoryId) =>
            Summarize(item => item.CategoryId.Equals(categoryId));

        private DiscoveryProgressSummary Summarize(Func<DiscoveryDefinition, bool> predicate)
        {
            DiscoveryDefinition[] eligible = _catalog.Discoveries
                .Where(item => item.Editorial.IsReleaseApproved)
                .Where(predicate)
                .ToArray();
            var discovered = new HashSet<DiscoveryId>(_repository.Current.Discoveries.Select(item => item.Id));
            return new DiscoveryProgressSummary(
                eligible.Count(item => discovered.Contains(item.Id)),
                eligible.Length);
        }
    }
}
