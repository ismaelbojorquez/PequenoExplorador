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
            LanguagePreference language = LanguagePreference.Spanish,
            float masterVolume = 0.85f,
            float musicVolume = -1f,
            float ambienceVolume = -1f,
            float effectsVolume = -1f,
            float voiceVolume = -1f,
            bool subtitlesEnabled = true)
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
            MasterVolume = ValidateVolume(masterVolume, nameof(masterVolume));
            MusicVolume = ResolveLegacyVolume(musicVolume, musicEnabled, 0.65f, nameof(musicVolume));
            AmbienceVolume = ResolveLegacyVolume(ambienceVolume, musicEnabled, 0.65f, nameof(ambienceVolume));
            EffectsVolume = ResolveLegacyVolume(effectsVolume, soundEffectsEnabled, 0.75f, nameof(effectsVolume));
            VoiceVolume = ResolveLegacyVolume(voiceVolume, narrationEnabled, 0.85f, nameof(voiceVolume));
            SubtitlesEnabled = subtitlesEnabled;
        }

        public GuidanceMode GuidanceMode { get; }
        public bool MusicEnabled { get; }
        public bool SoundEffectsEnabled { get; }
        public bool NarrationEnabled { get; }
        public LanguagePreference Language { get; }
        public float MasterVolume { get; }
        public float MusicVolume { get; }
        public float AmbienceVolume { get; }
        public float EffectsVolume { get; }
        public float VoiceVolume { get; }
        public bool SubtitlesEnabled { get; }

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
                language,
                MasterVolume,
                MusicVolume,
                AmbienceVolume,
                EffectsVolume,
                VoiceVolume,
                SubtitlesEnabled);
        }

        public PlayerPreferences WithGuidanceMode(GuidanceMode guidanceMode)
        {
            return new PlayerPreferences(
                guidanceMode,
                MusicEnabled,
                SoundEffectsEnabled,
                NarrationEnabled,
                Language,
                MasterVolume,
                MusicVolume,
                AmbienceVolume,
                EffectsVolume,
                VoiceVolume,
                SubtitlesEnabled);
        }

        public PlayerPreferences WithAudioSettings(
            float master,
            float music,
            float ambience,
            float effects,
            float voice,
            bool subtitlesEnabled)
        {
            return new PlayerPreferences(
                GuidanceMode,
                music > 0f,
                effects > 0f,
                voice > 0f,
                Language,
                master,
                music,
                ambience,
                effects,
                voice,
                subtitlesEnabled);
        }

        private static float ResolveLegacyVolume(float value, bool enabled, float defaultValue, string name)
        {
            if (value < 0f)
            {
                return enabled ? defaultValue : 0f;
            }

            return ValidateVolume(value, name);
        }

        private static float ValidateVolume(float value, string name)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value < 0f || value > 1f)
            {
                throw new ArgumentOutOfRangeException(name, "Volume must be normalized from 0 to 1.");
            }

            return value;
        }
    }
}
