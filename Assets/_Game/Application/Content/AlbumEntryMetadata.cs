using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Content
{
    public sealed class AlbumEntryMetadata
    {
        public AlbumEntryMetadata(
            EducationalFactId habitatFactId,
            EducationalFactId dietFactId,
            EducationalFactId sizeFactId,
            EducationalFactId curiosityFactId,
            EducationalFactId soundFactId,
            bool hasPlayableAudio)
        {
            HabitatFactId = habitatFactId;
            DietFactId = dietFactId;
            SizeFactId = sizeFactId;
            CuriosityFactId = curiosityFactId;
            SoundFactId = soundFactId;
            HasPlayableAudio = hasPlayableAudio;
        }

        public EducationalFactId HabitatFactId { get; }
        public EducationalFactId DietFactId { get; }
        public EducationalFactId SizeFactId { get; }
        public EducationalFactId CuriosityFactId { get; }
        public EducationalFactId SoundFactId { get; }
        public bool HasPlayableAudio { get; }

        public static AlbumEntryMetadata Empty { get; } = new AlbumEntryMetadata(
            default, default, default, default, default, false);

        public bool References(EducationalFactId id) =>
            id.IsValid &&
            (HabitatFactId.Equals(id) || DietFactId.Equals(id) || SizeFactId.Equals(id) ||
             CuriosityFactId.Equals(id) || SoundFactId.Equals(id));
    }
}
