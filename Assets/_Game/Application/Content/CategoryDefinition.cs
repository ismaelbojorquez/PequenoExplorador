using System;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Content
{
    public sealed class CategoryDefinition
    {
        public CategoryDefinition(CategoryId id, EditorialMetadata editorial)
        {
            if (!id.IsValid) throw new ArgumentException("Category ID is invalid.", nameof(id));
            Id = id;
            Editorial = editorial ?? throw new ArgumentNullException(nameof(editorial));
        }
        public CategoryId Id { get; }
        public EditorialMetadata Editorial { get; }
    }
}
