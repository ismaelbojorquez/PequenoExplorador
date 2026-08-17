using System;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Learning
{
    public sealed class LearningConceptDefinition
    {
        public LearningConceptDefinition(LearningConceptId id, LocalizedKey label, EditorialMetadata editorial)
        {
            if (!id.IsValid) throw new ArgumentException("Concept ID is invalid.", nameof(id));
            if (string.IsNullOrWhiteSpace(label.Table) || string.IsNullOrWhiteSpace(label.Entry)) throw new ArgumentException("Concept label is invalid.", nameof(label));
            Id = id; Label = label; Editorial = editorial ?? throw new ArgumentNullException(nameof(editorial));
        }
        public LearningConceptId Id { get; }
        public LocalizedKey Label { get; }
        public EditorialMetadata Editorial { get; }
    }
}
