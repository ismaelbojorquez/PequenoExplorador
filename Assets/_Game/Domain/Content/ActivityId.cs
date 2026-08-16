using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct ActivityId : IEquatable<ActivityId>
    {
        private readonly ContentIdValue _value;
        private ActivityId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out ActivityId id) { bool ok = ContentIdValue.TryCreate(raw, "activity", out ContentIdValue value); id = ok ? new ActivityId(value) : default; return ok; }
        public static ActivityId Parse(string raw) => TryParse(raw, out ActivityId id) ? id : throw new FormatException("Activity ID must be namespaced as activity.<scope>.");
        public bool Equals(ActivityId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is ActivityId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
