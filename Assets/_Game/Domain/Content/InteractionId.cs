using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct InteractionId : IEquatable<InteractionId>, IComparable<InteractionId>
    {
        private readonly ContentIdValue _value;

        private InteractionId(ContentIdValue value) => _value = value;

        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;

        public static bool TryParse(string raw, out InteractionId id)
        {
            bool valid = ContentIdValue.TryCreate(raw, "interaction", out ContentIdValue value);
            id = valid ? new InteractionId(value) : default;
            return valid;
        }

        public static InteractionId Parse(string raw) => TryParse(raw, out InteractionId id)
            ? id
            : throw new FormatException(
                "Interaction ID must be namespaced as interaction.<scope> using lowercase letters, digits, dots and hyphens.");

        public int CompareTo(InteractionId other) =>
            string.Compare(Value, other.Value, StringComparison.Ordinal);

        public bool Equals(InteractionId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is InteractionId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
        public static bool operator ==(InteractionId left, InteractionId right) => left.Equals(right);
        public static bool operator !=(InteractionId left, InteractionId right) => !left.Equals(right);
    }
}
