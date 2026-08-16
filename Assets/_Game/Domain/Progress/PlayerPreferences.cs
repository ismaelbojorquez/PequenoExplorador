using System;

namespace PequenoExplorador.Domain.Progress
{
    public sealed class PlayerPreferences
    {
        public PlayerPreferences(
            GuidanceMode guidanceMode,
            bool musicEnabled,
            bool soundEffectsEnabled,
            bool narrationEnabled)
        {
            if (!Enum.IsDefined(typeof(GuidanceMode), guidanceMode))
            {
                throw new ArgumentOutOfRangeException(nameof(guidanceMode));
            }

            GuidanceMode = guidanceMode;
            MusicEnabled = musicEnabled;
            SoundEffectsEnabled = soundEffectsEnabled;
            NarrationEnabled = narrationEnabled;
        }

        public GuidanceMode GuidanceMode { get; }
        public bool MusicEnabled { get; }
        public bool SoundEffectsEnabled { get; }
        public bool NarrationEnabled { get; }

        public static PlayerPreferences CreateDefault()
        {
            return new PlayerPreferences(GuidanceMode.Standard, true, true, true);
        }
    }
}
