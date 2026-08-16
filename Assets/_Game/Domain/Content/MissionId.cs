using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct MissionId : IEquatable<MissionId>
    {
        private readonly ContentIdValue _value;
        private MissionId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out MissionId id) { bool ok = ContentIdValue.TryCreate(raw, "mission", out ContentIdValue value); id = ok ? new MissionId(value) : default; return ok; }
        public static MissionId Parse(string raw) => TryParse(raw, out MissionId id) ? id : throw new FormatException("Mission ID must be namespaced as mission.<scope>.");
        public bool Equals(MissionId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is MissionId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
