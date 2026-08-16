using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct TagId : IEquatable<TagId>
    {
        private readonly ContentIdValue _value;
        private TagId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out TagId id) { bool ok = ContentIdValue.TryCreate(raw, "tag", out ContentIdValue value); id = ok ? new TagId(value) : default; return ok; }
        public static TagId Parse(string raw) => TryParse(raw, out TagId id) ? id : throw new FormatException("Tag ID must be namespaced as tag.<scope>.");
        public bool Equals(TagId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is TagId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
        public static bool operator ==(TagId left, TagId right) => left.Equals(right);
        public static bool operator !=(TagId left, TagId right) => !left.Equals(right);
    }
}
