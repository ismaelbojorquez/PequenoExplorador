using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct LearningOptionId : IEquatable<LearningOptionId>
    {
        private readonly ContentIdValue _value;
        private LearningOptionId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out LearningOptionId id) { bool ok = ContentIdValue.TryCreate(raw, "activity-option", out ContentIdValue value); id = ok ? new LearningOptionId(value) : default; return ok; }
        public static LearningOptionId Parse(string raw) => TryParse(raw, out LearningOptionId id) ? id : throw new FormatException("Activity option ID must be namespaced as activity-option.<scope>.");
        public bool Equals(LearningOptionId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is LearningOptionId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
