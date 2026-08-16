using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct CheckpointId : IEquatable<CheckpointId>
    {
        private readonly ContentIdValue _value;
        private CheckpointId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out CheckpointId id)
        {
            bool valid = ContentIdValue.TryCreate(raw, "checkpoint", out ContentIdValue value);
            id = valid ? new CheckpointId(value) : default;
            return valid;
        }
        public static CheckpointId Parse(string raw) => TryParse(raw, out CheckpointId id)
            ? id : throw new FormatException("Checkpoint ID must be namespaced as checkpoint.<scope>.");
        public bool Equals(CheckpointId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is CheckpointId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
        public static bool operator ==(CheckpointId left, CheckpointId right) => left.Equals(right);
        public static bool operator !=(CheckpointId left, CheckpointId right) => !left.Equals(right);
    }
}
