using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct LearningConceptId : IEquatable<LearningConceptId>
    {
        private readonly ContentIdValue _value;
        private LearningConceptId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out LearningConceptId id) { bool ok = ContentIdValue.TryCreate(raw, "concept", out ContentIdValue value); id = ok ? new LearningConceptId(value) : default; return ok; }
        public static LearningConceptId Parse(string raw) => TryParse(raw, out LearningConceptId id) ? id : throw new FormatException("Concept ID must be namespaced as concept.<scope>.");
        public bool Equals(LearningConceptId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is LearningConceptId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
