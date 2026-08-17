using System;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Domain.Economy
{
    public readonly struct EconomyTransactionId : IEquatable<EconomyTransactionId>
    {
        private readonly ContentIdValue _value;
        private EconomyTransactionId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out EconomyTransactionId id)
        {
            bool valid = ContentIdValue.TryCreate(raw, "economy-tx", out ContentIdValue value);
            id = valid ? new EconomyTransactionId(value) : default;
            return valid;
        }
        public static EconomyTransactionId Parse(string raw) => TryParse(raw, out EconomyTransactionId id)
            ? id : throw new FormatException("Economy transaction ID must be namespaced as economy-tx.<scope>.");
        public bool Equals(EconomyTransactionId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is EconomyTransactionId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
