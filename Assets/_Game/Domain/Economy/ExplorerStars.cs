using System;

namespace PequenoExplorador.Domain.Economy
{
    public readonly struct ExplorerStars : IEquatable<ExplorerStars>, IComparable<ExplorerStars>
    {
        public ExplorerStars(int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Explorer Stars cannot be negative.");
            Value = value;
        }

        public int Value { get; }
        public bool CanSpend(ExplorerStars amount) => Value >= amount.Value;
        public bool TryAdd(ExplorerStars amount, out ExplorerStars result)
        {
            long sum = (long)Value + amount.Value;
            if (sum > int.MaxValue) { result = default; return false; }
            result = new ExplorerStars((int)sum);
            return true;
        }
        public bool TrySpend(ExplorerStars amount, out ExplorerStars result)
        {
            if (!CanSpend(amount)) { result = this; return false; }
            result = new ExplorerStars(Value - amount.Value);
            return true;
        }
        public bool Equals(ExplorerStars other) => Value == other.Value;
        public override bool Equals(object obj) => obj is ExplorerStars other && Equals(other);
        public override int GetHashCode() => Value;
        public int CompareTo(ExplorerStars other) => Value.CompareTo(other.Value);
        public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}
