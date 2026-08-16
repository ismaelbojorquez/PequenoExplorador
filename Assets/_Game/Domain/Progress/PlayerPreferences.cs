using System;

namespace PequenoExplorador.Domain.Progress
{
    public sealed class PlayerPreferences
    {
        public PlayerPreferences(
            GuidanceMode guidanceMode,
            bool musicEnabled,
            bool soundEffectsEnabled,
            bool narrationEnabled,
            LanguagePreference language = LanguagePreference.Spanish)
        {
            if (!Enum.IsDefined(typeof(GuidanceMode), guidanceMode))
            {
                throw new ArgumentOutOfRangeException(nameof(guidanceMode));
            }

            if (!Enum.IsDefined(typeof(LanguagePreference), language))
            {
                throw new ArgumentOutOfRangeException(nameof(language));
            }

            GuidanceMode = guidanceMode;
            MusicEnabled = musicEnabled;
            SoundEffectsEnabled = soundEffectsEnabled;
            NarrationEnabled = narrationEnabled;
            Language = language;
        }

        public GuidanceMode GuidanceMode { get; }
        public bool MusicEnabled { get; }
        public bool SoundEffectsEnabled { get; }
        public bool NarrationEnabled { get; }
        public LanguagePreference Language { get; }

        public static PlayerPreferences CreateDefault()
        {
            return new PlayerPreferences(GuidanceMode.Standard, true, true, true);
        }

        public PlayerPreferences WithLanguage(LanguagePreference language)
        {
            return new PlayerPreferences(
                GuidanceMode,
                MusicEnabled,
                SoundEffectsEnabled,
                NarrationEnabled,
                language);
        }
    }
}
