using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct WorldId : IEquatable<WorldId>
    {
        private readonly ContentIdValue _value;
        private WorldId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out WorldId id) { bool ok = ContentIdValue.TryCreate(raw, "world", out ContentIdValue value); id = ok ? new WorldId(value) : default; return ok; }
        public static WorldId Parse(string raw) => TryParse(raw, out WorldId id) ? id : throw new FormatException("World ID must be namespaced as world.<scope>.");
        public bool Equals(WorldId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is WorldId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
