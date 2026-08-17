using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Album
{
    public sealed class AlbumCategoryViewModel
    {
        public AlbumCategoryViewModel(CategoryId id, LocalizedKey displayName, int discovered, int total)
        {
            Id = id;
            DisplayName = displayName;
            Discovered = discovered;
            Total = total;
        }

        public CategoryId Id { get; }
        public LocalizedKey DisplayName { get; }
        public int Discovered { get; }
        public int Total { get; }
    }
}
