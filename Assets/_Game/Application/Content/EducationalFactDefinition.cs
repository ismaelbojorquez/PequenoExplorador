using System;
using System.Collections.Generic;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Content
{
    public sealed class EducationalFactDefinition
    {
        private readonly IReadOnlyList<ContentSourceId> _sourceIds;
        public EducationalFactDefinition(
            EducationalFactId id,
            LocalizedKey childCopy,
            string claimForReview,
            IEnumerable<ContentSourceId> sourceIds,
            EditorialMetadata editorial)
        {
            if (!id.IsValid) throw new ArgumentException("Fact ID is invalid.", nameof(id));
            Id = id;
            ChildCopy = childCopy;
            ClaimForReview = claimForReview ?? string.Empty;
            _sourceIds = Array.AsReadOnly(sourceIds == null ? Array.Empty<ContentSourceId>() : new List<ContentSourceId>(sourceIds).ToArray());
            Editorial = editorial ?? throw new ArgumentNullException(nameof(editorial));
        }
        public EducationalFactId Id { get; }
        public LocalizedKey ChildCopy { get; }
        public string ClaimForReview { get; }
        public IReadOnlyList<ContentSourceId> SourceIds => _sourceIds;
        public EditorialMetadata Editorial { get; }
    }
}
