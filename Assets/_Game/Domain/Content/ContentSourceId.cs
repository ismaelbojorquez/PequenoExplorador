using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct ContentSourceId : IEquatable<ContentSourceId>
    {
        private readonly ContentIdValue _value;
        private ContentSourceId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out ContentSourceId id) { bool ok = ContentIdValue.TryCreate(raw, "source", out ContentIdValue value); id = ok ? new ContentSourceId(value) : default; return ok; }
        public static ContentSourceId Parse(string raw) => TryParse(raw, out ContentSourceId id) ? id : throw new FormatException("Source ID must be namespaced as source.<scope>.");
        public bool Equals(ContentSourceId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is ContentSourceId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
        public static bool operator ==(ContentSourceId left, ContentSourceId right) => left.Equals(right);
        public static bool operator !=(ContentSourceId left, ContentSourceId right) => !left.Equals(right);
    }
}
