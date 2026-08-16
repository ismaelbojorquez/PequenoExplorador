using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct VisualAssetId : IEquatable<VisualAssetId>
    {
        private readonly ContentIdValue _value;
        private VisualAssetId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out VisualAssetId id) { bool ok = ContentIdValue.TryCreate(raw, "visual", out ContentIdValue value); id = ok ? new VisualAssetId(value) : default; return ok; }
        public static VisualAssetId Parse(string raw) => TryParse(raw, out VisualAssetId id) ? id : throw new FormatException("Visual asset ID must be namespaced as visual.<scope>.");
        public bool Equals(VisualAssetId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is VisualAssetId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
