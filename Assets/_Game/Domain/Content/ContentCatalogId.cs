using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct ContentCatalogId : IEquatable<ContentCatalogId>
    {
        private readonly ContentIdValue _value;
        private ContentCatalogId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out ContentCatalogId id)
        {
            bool valid = ContentIdValue.TryCreate(raw, "catalog", out ContentIdValue value);
            id = valid ? new ContentCatalogId(value) : default;
            return valid;
        }
        public static ContentCatalogId Parse(string raw) => TryParse(raw, out ContentCatalogId id)
            ? id : throw new FormatException("Content catalog ID must be namespaced as catalog.<scope>.");
        public bool Equals(ContentCatalogId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is ContentCatalogId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
        public static bool operator ==(ContentCatalogId left, ContentCatalogId right) => left.Equals(right);
        public static bool operator !=(ContentCatalogId left, ContentCatalogId right) => !left.Equals(right);
    }
}
