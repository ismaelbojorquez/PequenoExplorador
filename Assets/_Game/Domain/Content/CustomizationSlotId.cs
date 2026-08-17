using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct CustomizationSlotId : IEquatable<CustomizationSlotId>
    {
        private readonly ContentIdValue _value;
        private CustomizationSlotId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out CustomizationSlotId id)
        { bool ok = ContentIdValue.TryCreate(raw, "customization-slot", out ContentIdValue value); id = ok ? new CustomizationSlotId(value) : default; return ok; }
        public static CustomizationSlotId Parse(string raw) => TryParse(raw, out CustomizationSlotId id)
            ? id
            : throw new FormatException("Customization slot ID must be namespaced as customization-slot.<scope>.");
        public bool Equals(CustomizationSlotId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is CustomizationSlotId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
        public static bool operator ==(CustomizationSlotId left, CustomizationSlotId right) => left.Equals(right);
        public static bool operator !=(CustomizationSlotId left, CustomizationSlotId right) => !left.Equals(right);
    }
}
