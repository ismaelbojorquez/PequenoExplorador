using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct CosmeticCompatibilityTagId : IEquatable<CosmeticCompatibilityTagId>
    {
        private readonly ContentIdValue _value;
        private CosmeticCompatibilityTagId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out CosmeticCompatibilityTagId id)
        { bool ok = ContentIdValue.TryCreate(raw, "cosmetic-tag", out ContentIdValue value); id = ok ? new CosmeticCompatibilityTagId(value) : default; return ok; }
        public static CosmeticCompatibilityTagId Parse(string raw) => TryParse(raw, out CosmeticCompatibilityTagId id)
            ? id
            : throw new FormatException("Cosmetic compatibility tag must be namespaced as cosmetic-tag.<scope>.");
        public bool Equals(CosmeticCompatibilityTagId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is CosmeticCompatibilityTagId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
