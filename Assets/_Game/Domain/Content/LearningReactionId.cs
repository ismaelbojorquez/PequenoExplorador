using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct LearningReactionId : IEquatable<LearningReactionId>
    {
        private readonly ContentIdValue _value;
        private LearningReactionId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out LearningReactionId id)
        {
            bool ok = ContentIdValue.TryCreate(raw, "learning-reaction", out ContentIdValue value);
            id = ok ? new LearningReactionId(value) : default;
            return ok;
        }
        public static LearningReactionId Parse(string raw) => TryParse(raw, out LearningReactionId id)
            ? id
            : throw new FormatException("Learning reaction ID must be namespaced as learning-reaction.<scope>.");
        public bool Equals(LearningReactionId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is LearningReactionId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
