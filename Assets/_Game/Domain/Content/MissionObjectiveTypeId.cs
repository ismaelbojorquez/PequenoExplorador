using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct MissionObjectiveTypeId : IEquatable<MissionObjectiveTypeId>
    {
        private readonly ContentIdValue _value;
        private MissionObjectiveTypeId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out MissionObjectiveTypeId id)
        {
            bool ok = ContentIdValue.TryCreate(raw, "mission-objective-type", out ContentIdValue value);
            id = ok ? new MissionObjectiveTypeId(value) : default;
            return ok;
        }
        public static MissionObjectiveTypeId Parse(string raw) => TryParse(raw, out MissionObjectiveTypeId id)
            ? id
            : throw new FormatException("Mission objective type ID must be namespaced as mission-objective-type.<scope>.");
        public bool Equals(MissionObjectiveTypeId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is MissionObjectiveTypeId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
