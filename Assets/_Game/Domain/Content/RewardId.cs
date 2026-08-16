using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct RewardId : IEquatable<RewardId>
    {
        private readonly ContentIdValue _value;
        private RewardId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out RewardId id) { bool ok = ContentIdValue.TryCreate(raw, "reward", out ContentIdValue value); id = ok ? new RewardId(value) : default; return ok; }
        public static RewardId Parse(string raw) => TryParse(raw, out RewardId id) ? id : throw new FormatException("Reward ID must be namespaced as reward.<scope>.");
        public bool Equals(RewardId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is RewardId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
