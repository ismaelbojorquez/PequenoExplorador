using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct GameplayFactId : IEquatable<GameplayFactId>
    {
        private readonly ContentIdValue _value;
        private GameplayFactId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out GameplayFactId id)
        {
            bool ok = ContentIdValue.TryCreate(raw, "gameplay-fact", out ContentIdValue value);
            id = ok ? new GameplayFactId(value) : default;
            return ok;
        }
        public static GameplayFactId Parse(string raw) => TryParse(raw, out GameplayFactId id)
            ? id
            : throw new FormatException("Gameplay fact ID must be namespaced as gameplay-fact.<scope>.");
        public bool Equals(GameplayFactId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is GameplayFactId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
