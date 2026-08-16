using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct SpawnPointId : IEquatable<SpawnPointId>
    {
        private readonly ContentIdValue _value;
        private SpawnPointId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out SpawnPointId id)
        {
            bool valid = ContentIdValue.TryCreate(raw, "spawn", out ContentIdValue value);
            id = valid ? new SpawnPointId(value) : default;
            return valid;
        }
        public static SpawnPointId Parse(string raw) => TryParse(raw, out SpawnPointId id)
            ? id : throw new FormatException("Spawn point ID must be namespaced as spawn.<world>.<slug>.");
        public bool Equals(SpawnPointId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is SpawnPointId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
        public static bool operator ==(SpawnPointId left, SpawnPointId right) => left.Equals(right);
        public static bool operator !=(SpawnPointId left, SpawnPointId right) => !left.Equals(right);
    }
}
