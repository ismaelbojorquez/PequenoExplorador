using PequenoExplorador.Application.Localization;

namespace PequenoExplorador.Application.Audio
{
    public readonly struct SubtitleModel
    {
        public SubtitleModel(AudioCueId cueId, LocalizedKey textKey, bool visible)
        {
            CueId = cueId;
            TextKey = textKey;
            Visible = visible;
        }

        public AudioCueId CueId { get; }
        public LocalizedKey TextKey { get; }
        public bool Visible { get; }

        public static SubtitleModel Hidden => new SubtitleModel(default, default, false);
    }
}
