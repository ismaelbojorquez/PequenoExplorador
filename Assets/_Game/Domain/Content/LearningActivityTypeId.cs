using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct LearningActivityTypeId : IEquatable<LearningActivityTypeId>
    {
        private readonly ContentIdValue _value;
        private LearningActivityTypeId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out LearningActivityTypeId id) { bool ok = ContentIdValue.TryCreate(raw, "activity-type", out ContentIdValue value); id = ok ? new LearningActivityTypeId(value) : default; return ok; }
        public static LearningActivityTypeId Parse(string raw) => TryParse(raw, out LearningActivityTypeId id) ? id : throw new FormatException("Activity type ID must be namespaced as activity-type.<scope>.");
        public bool Equals(LearningActivityTypeId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is LearningActivityTypeId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
