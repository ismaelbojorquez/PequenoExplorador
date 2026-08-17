using PequenoExplorador.Application.Localization;

namespace PequenoExplorador.Application.Album
{
    public readonly struct AlbumFactViewModel
    {
        public AlbumFactViewModel(AlbumFactField field, LocalizedKey value, bool hasApprovedValue)
        {
            Field = field;
            Value = value;
            HasApprovedValue = hasApprovedValue;
        }

        public AlbumFactField Field { get; }
        public LocalizedKey Value { get; }
        public bool HasApprovedValue { get; }
    }
}
