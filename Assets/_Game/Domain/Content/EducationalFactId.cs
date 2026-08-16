using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct EducationalFactId : IEquatable<EducationalFactId>
    {
        private readonly ContentIdValue _value;
        private EducationalFactId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out EducationalFactId id) { bool ok = ContentIdValue.TryCreate(raw, "fact", out ContentIdValue value); id = ok ? new EducationalFactId(value) : default; return ok; }
        public static EducationalFactId Parse(string raw) => TryParse(raw, out EducationalFactId id) ? id : throw new FormatException("Fact ID must be namespaced as fact.<scope>.");
        public bool Equals(EducationalFactId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is EducationalFactId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
        public static bool operator ==(EducationalFactId left, EducationalFactId right) => left.Equals(right);
        public static bool operator !=(EducationalFactId left, EducationalFactId right) => !left.Equals(right);
    }
}
