using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct DiscoveryId : IEquatable<DiscoveryId>
    {
        private readonly ContentIdValue _value;
        private DiscoveryId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out DiscoveryId id)
        {
            bool valid = ContentIdValue.TryCreate(raw, "discovery", out ContentIdValue value);
            id = valid ? new DiscoveryId(value) : default;
            return valid;
        }
        public static DiscoveryId Parse(string raw) => TryParse(raw, out DiscoveryId id)
            ? id : throw new FormatException("Discovery ID must be namespaced as discovery.<scope> using lowercase letters, digits, dots and hyphens.");
        public bool Equals(DiscoveryId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is DiscoveryId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
        public static bool operator ==(DiscoveryId left, DiscoveryId right) => left.Equals(right);
        public static bool operator !=(DiscoveryId left, DiscoveryId right) => !left.Equals(right);
    }
}
