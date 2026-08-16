using System;

namespace PequenoExplorador.Application.SceneFlow
{
    public readonly struct SceneContentId : IEquatable<SceneContentId>
    {
        private SceneContentId(string value) => Value = value;
        public string Value { get; }
        public bool IsValid => !string.IsNullOrEmpty(Value);

        public static bool TryParse(string raw, out SceneContentId id)
        {
            id = default;
            if (string.IsNullOrWhiteSpace(raw) || !raw.StartsWith("scene/", StringComparison.Ordinal) || raw.Length <= 6)
                return false;
            for (int index = 0; index < raw.Length; index++)
            {
                char character = raw[index];
                bool valid = character >= 'a' && character <= 'z' || character >= '0' && character <= '9' ||
                             character == '/' || character == '-' || character == '.';
                if (!valid) return false;
            }
            id = new SceneContentId(raw);
            return true;
        }

        public static SceneContentId Parse(string raw) => TryParse(raw, out SceneContentId id)
            ? id : throw new FormatException("Scene content ID must be a lowercase semantic address under scene/.");
        public bool Equals(SceneContentId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is SceneContentId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(SceneContentId left, SceneContentId right) => left.Equals(right);
        public static bool operator !=(SceneContentId left, SceneContentId right) => !left.Equals(right);
    }
}
