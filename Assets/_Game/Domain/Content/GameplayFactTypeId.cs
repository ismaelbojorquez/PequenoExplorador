using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct GameplayFactTypeId : IEquatable<GameplayFactTypeId>
    {
        private readonly ContentIdValue _value;
        private GameplayFactTypeId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out GameplayFactTypeId id)
        {
            bool ok = ContentIdValue.TryCreate(raw, "gameplay-fact-type", out ContentIdValue value);
            id = ok ? new GameplayFactTypeId(value) : default;
            return ok;
        }
        public static GameplayFactTypeId Parse(string raw) => TryParse(raw, out GameplayFactTypeId id)
            ? id
            : throw new FormatException("Gameplay fact type ID must be namespaced as gameplay-fact-type.<scope>.");
        public bool Equals(GameplayFactTypeId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is GameplayFactTypeId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
