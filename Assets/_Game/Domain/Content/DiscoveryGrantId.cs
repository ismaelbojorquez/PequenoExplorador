using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct DiscoveryGrantId : IEquatable<DiscoveryGrantId>
    {
        private readonly ContentIdValue _value;

        private DiscoveryGrantId(ContentIdValue value) => _value = value;

        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;

        public static bool TryParse(string raw, out DiscoveryGrantId id)
        {
            bool valid = ContentIdValue.TryCreate(raw, "grant", out ContentIdValue value);
            id = valid ? new DiscoveryGrantId(value) : default;
            return valid;
        }

        public static DiscoveryGrantId Parse(string raw) => TryParse(raw, out DiscoveryGrantId id)
            ? id
            : throw new FormatException(
                "Discovery grant ID must be namespaced as grant.<source> using lowercase letters, digits, dots and hyphens.");

        public bool Equals(DiscoveryGrantId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is DiscoveryGrantId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
        public static bool operator ==(DiscoveryGrantId left, DiscoveryGrantId right) => left.Equals(right);
        public static bool operator !=(DiscoveryGrantId left, DiscoveryGrantId right) => !left.Equals(right);
    }
}
