using System;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Content
{
    public sealed class TagDefinition
    {
        public TagDefinition(TagId id, EditorialMetadata editorial)
        {
            if (!id.IsValid) throw new ArgumentException("Tag ID is invalid.", nameof(id));
            Id = id;
            Editorial = editorial ?? throw new ArgumentNullException(nameof(editorial));
        }
        public TagId Id { get; }
        public EditorialMetadata Editorial { get; }
    }
}
