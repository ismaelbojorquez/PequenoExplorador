using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct CampUpgradeId : IEquatable<CampUpgradeId>
    {
        private readonly ContentIdValue _value;
        private CampUpgradeId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out CampUpgradeId id)
        { bool ok = ContentIdValue.TryCreate(raw, "camp-upgrade", out ContentIdValue value); id = ok ? new CampUpgradeId(value) : default; return ok; }
        public static CampUpgradeId Parse(string raw) => TryParse(raw, out CampUpgradeId id)
            ? id
            : throw new FormatException("Camp upgrade ID must be namespaced as camp-upgrade.<scope>.");
        public bool Equals(CampUpgradeId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is CampUpgradeId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
