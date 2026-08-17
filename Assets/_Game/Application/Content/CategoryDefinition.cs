using System;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Content
{
    public sealed class CategoryDefinition
    {
        public CategoryDefinition(CategoryId id, LocalizedKey displayName, EditorialMetadata editorial)
        {
            if (!id.IsValid) throw new ArgumentException("Category ID is invalid.", nameof(id));
            Id = id;
            DisplayName = displayName;
            Editorial = editorial ?? throw new ArgumentNullException(nameof(editorial));
        }
        public CategoryId Id { get; }
        public LocalizedKey DisplayName { get; }
        public EditorialMetadata Editorial { get; }
    }
}
