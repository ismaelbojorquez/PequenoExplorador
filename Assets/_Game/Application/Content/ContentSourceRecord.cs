using System;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Content
{
    public sealed class ContentSourceRecord
    {
        public ContentSourceRecord(
            ContentSourceId id,
            string institution,
            string author,
            string title,
            string reference,
            string consultedOn,
            string reviewer,
            EditorialMetadata editorial)
        {
            if (!id.IsValid) throw new ArgumentException("Source ID is invalid.", nameof(id));
            Id = id;
            Institution = institution ?? string.Empty;
            Author = author ?? string.Empty;
            Title = title ?? string.Empty;
            Reference = reference ?? string.Empty;
            ConsultedOn = consultedOn ?? string.Empty;
            Reviewer = reviewer ?? string.Empty;
            Editorial = editorial ?? throw new ArgumentNullException(nameof(editorial));
        }
        public ContentSourceId Id { get; }
        public string Institution { get; }
        public string Author { get; }
        public string Title { get; }
        public string Reference { get; }
        public string ConsultedOn { get; }
        public string Reviewer { get; }
        public EditorialMetadata Editorial { get; }
    }
}
