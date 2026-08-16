using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct WorldRequirementId : IEquatable<WorldRequirementId>
    {
        private readonly ContentIdValue _value;
        private WorldRequirementId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out WorldRequirementId id)
        {
            bool valid = ContentIdValue.TryCreate(raw, "requirement", out ContentIdValue value);
            id = valid ? new WorldRequirementId(value) : default;
            return valid;
        }
        public static WorldRequirementId Parse(string raw) => TryParse(raw, out WorldRequirementId id)
            ? id : throw new FormatException("World requirement ID must be namespaced as requirement.<scope>.");
        public bool Equals(WorldRequirementId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is WorldRequirementId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
        public static bool operator ==(WorldRequirementId left, WorldRequirementId right) => left.Equals(right);
        public static bool operator !=(WorldRequirementId left, WorldRequirementId right) => !left.Equals(right);
    }
}
