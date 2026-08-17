using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct CosmeticId : IEquatable<CosmeticId>
    {
        private readonly ContentIdValue _value;
        private CosmeticId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out CosmeticId id)
        { bool ok = ContentIdValue.TryCreate(raw, "cosmetic", out ContentIdValue value); id = ok ? new CosmeticId(value) : default; return ok; }
        public static CosmeticId Parse(string raw) => TryParse(raw, out CosmeticId id)
            ? id
            : throw new FormatException("Cosmetic ID must be namespaced as cosmetic.<scope>.");
        public bool Equals(CosmeticId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is CosmeticId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
        public static bool operator ==(CosmeticId left, CosmeticId right) => left.Equals(right);
        public static bool operator !=(CosmeticId left, CosmeticId right) => !left.Equals(right);
    }
}
