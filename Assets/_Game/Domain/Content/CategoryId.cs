using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct CategoryId : IEquatable<CategoryId>
    {
        private readonly ContentIdValue _value;
        private CategoryId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out CategoryId id) { bool ok = ContentIdValue.TryCreate(raw, "category", out ContentIdValue value); id = ok ? new CategoryId(value) : default; return ok; }
        public static CategoryId Parse(string raw) => TryParse(raw, out CategoryId id) ? id : throw new FormatException("Category ID must be namespaced as category.<scope>.");
        public bool Equals(CategoryId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is CategoryId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
        public static bool operator ==(CategoryId left, CategoryId right) => left.Equals(right);
        public static bool operator !=(CategoryId left, CategoryId right) => !left.Equals(right);
    }
}
