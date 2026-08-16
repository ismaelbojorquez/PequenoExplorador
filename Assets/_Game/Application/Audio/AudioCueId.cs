using System;

namespace PequenoExplorador.Application.Audio
{
    public readonly struct AudioCueId : IEquatable<AudioCueId>
    {
        public AudioCueId(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("audio.", StringComparison.Ordinal))
            {
                throw new ArgumentException("Audio cue ID must be a namespaced audio.* value.", nameof(value));
            }

            Value = value;
        }

        public string Value { get; }
        public bool Equals(AudioCueId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is AudioCueId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(AudioCueId left, AudioCueId right) => left.Equals(right);
        public static bool operator !=(AudioCueId left, AudioCueId right) => !left.Equals(right);
    }
}
