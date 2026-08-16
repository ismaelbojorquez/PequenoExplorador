using System;

namespace PequenoExplorador.Application.Audio
{
    public sealed class AudioSettings : IEquatable<AudioSettings>
    {
        public AudioSettings(
            float master,
            float music,
            float ambience,
            float effects,
            float voice,
            bool subtitlesEnabled)
        {
            Master = Validate(master, nameof(master));
            Music = Validate(music, nameof(music));
            Ambience = Validate(ambience, nameof(ambience));
            Effects = Validate(effects, nameof(effects));
            Voice = Validate(voice, nameof(voice));
            SubtitlesEnabled = subtitlesEnabled;
        }

        public float Master { get; }
        public float Music { get; }
        public float Ambience { get; }
        public float Effects { get; }
        public float Voice { get; }
        public bool SubtitlesEnabled { get; }

        public static AudioSettings CreateDefault() => new AudioSettings(0.85f, 0.65f, 0.65f, 0.75f, 0.85f, true);

        public bool Equals(AudioSettings other)
        {
            return other != null && Master.Equals(other.Master) && Music.Equals(other.Music) &&
                   Ambience.Equals(other.Ambience) && Effects.Equals(other.Effects) &&
                   Voice.Equals(other.Voice) && SubtitlesEnabled == other.SubtitlesEnabled;
        }

        public override bool Equals(object obj) => Equals(obj as AudioSettings);
        public override int GetHashCode() => HashCode.Combine(Master, Music, Ambience, Effects, Voice, SubtitlesEnabled);

        private static float Validate(float value, string name)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value < 0f || value > 1f)
            {
                throw new ArgumentOutOfRangeException(name, "Audio volume must be normalized from 0 to 1.");
            }

            return value;
        }
    }
}
