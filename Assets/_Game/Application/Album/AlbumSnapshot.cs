using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Album
{
    public sealed class AlbumSnapshot
    {
        private readonly IReadOnlyList<AlbumCategoryViewModel> _categories;
        private readonly IReadOnlyList<AlbumEntryViewModel> _entries;

        public AlbumSnapshot(
            WorldId worldId,
            CategoryId? selectedCategory,
            IEnumerable<AlbumCategoryViewModel> categories,
            IEnumerable<AlbumEntryViewModel> entries)
        {
            WorldId = worldId;
            SelectedCategory = selectedCategory;
            _categories = Array.AsReadOnly((categories ?? Array.Empty<AlbumCategoryViewModel>()).ToArray());
            _entries = Array.AsReadOnly((entries ?? Array.Empty<AlbumEntryViewModel>()).ToArray());
        }

        public WorldId WorldId { get; }
        public CategoryId? SelectedCategory { get; }
        public IReadOnlyList<AlbumCategoryViewModel> Categories => _categories;
        public IReadOnlyList<AlbumEntryViewModel> Entries => _entries;
        public int Discovered => _categories.Sum(item => item.Discovered);
        public int Total => _categories.Sum(item => item.Total);
    }
}
