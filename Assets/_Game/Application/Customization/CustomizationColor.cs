using System;

namespace PequenoExplorador.Application.Customization
{
    public readonly struct CustomizationColor : IEquatable<CustomizationColor>
    {
        public CustomizationColor(byte red, byte green, byte blue, byte alpha = byte.MaxValue)
        { Red = red; Green = green; Blue = blue; Alpha = alpha; }
        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }
        public byte Alpha { get; }
        public bool Equals(CustomizationColor other) => Red == other.Red && Green == other.Green && Blue == other.Blue && Alpha == other.Alpha;
        public override bool Equals(object obj) => obj is CustomizationColor other && Equals(other);
        public override int GetHashCode() => (((Red * 397) ^ Green) * 397 ^ Blue) * 397 ^ Alpha;
    }
}
