using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct MissionObjectiveId : IEquatable<MissionObjectiveId>
    {
        private readonly ContentIdValue _value;
        private MissionObjectiveId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out MissionObjectiveId id)
        {
            bool ok = ContentIdValue.TryCreate(raw, "mission-objective", out ContentIdValue value);
            id = ok ? new MissionObjectiveId(value) : default;
            return ok;
        }
        public static MissionObjectiveId Parse(string raw) => TryParse(raw, out MissionObjectiveId id)
            ? id
            : throw new FormatException("Mission objective ID must be namespaced as mission-objective.<scope>.");
        public bool Equals(MissionObjectiveId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is MissionObjectiveId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
