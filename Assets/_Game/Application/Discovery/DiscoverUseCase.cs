using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Discovery
{
    public sealed class DiscoverUseCase
    {
        private readonly IContentCatalog _catalog;
        private readonly IDiscoveryProgressRepository _repository;
        private readonly IClock _clock;
        private readonly bool _allowUnapprovedContent;
        private readonly TimeSpan _localUtcOffset;

        public DiscoverUseCase(
            IContentCatalog catalog,
            IDiscoveryProgressRepository repository,
            IClock clock,
            bool allowUnapprovedContent,
            TimeSpan localUtcOffset)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            if (localUtcOffset < TimeSpan.FromHours(-14) || localUtcOffset > TimeSpan.FromHours(14))
                throw new ArgumentOutOfRangeException(nameof(localUtcOffset));
            _allowUnapprovedContent = allowUnapprovedContent;
            _localUtcOffset = localUtcOffset;
        }

        public DiscoverResult Execute(DiscoveryId requestedId, DiscoveryGrantId grantId)
        {
            if (!requestedId.IsValid) return new DiscoverResult(DiscoverOutcome.MissingContent, requestedId, null);
            if (!grantId.IsValid) throw new ArgumentException("Grant ID is invalid.", nameof(grantId));
            if (!_catalog.TryResolveDiscovery(requestedId, out DiscoveryDefinition definition))
                return new DiscoverResult(DiscoverOutcome.MissingContent, requestedId, null);
            if (!definition.Editorial.IsReleaseApproved && !_allowUnapprovedContent)
                return new DiscoverResult(DiscoverOutcome.UnapprovedContent, definition.Id, null);
            if (_repository.IsReadOnly)
                return new DiscoverResult(DiscoverOutcome.SaveReadOnly, definition.Id, null);

            PlayerProgress current = _repository.Current;
            DiscoveryProgress existing = current.Discoveries.FirstOrDefault(item => item.Id == definition.Id);
            if (current.ProcessedDiscoveryGrantIds.Contains(grantId.Value, StringComparer.Ordinal))
                return new DiscoverResult(DiscoverOutcome.AlreadyProcessed, definition.Id, existing);

            var discoveries = new List<DiscoveryProgress>(current.Discoveries);
            DiscoveryProgress updated;
            DiscoverOutcome outcome;
            if (existing == null)
            {
                string localDay = _clock.UtcNow.ToOffset(_localUtcOffset)
                    .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                updated = new DiscoveryProgress(definition.Id, 1, localDay);
                discoveries.Add(updated);
                outcome = DiscoverOutcome.First;
            }
            else
            {
                updated = existing.Increment();
                discoveries[discoveries.IndexOf(existing)] = updated;
                outcome = DiscoverOutcome.Repeated;
            }

            var grants = new List<string>(current.ProcessedDiscoveryGrantIds) { grantId.Value };
            _repository.Commit(current.WithDiscoveryState(discoveries, grants));
            return new DiscoverResult(outcome, definition.Id, updated);
        }
    }
}
