using System;

namespace PequenoExplorador.Application.Input
{
    public readonly struct ScreenPoint : IEquatable<ScreenPoint>
    {
        public ScreenPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; }
        public float Y { get; }

        public float DistanceTo(ScreenPoint other)
        {
            float x = X - other.X;
            float y = Y - other.Y;
            return (float)Math.Sqrt((x * x) + (y * y));
        }

        public static ScreenPoint operator -(ScreenPoint left, ScreenPoint right) =>
            new ScreenPoint(left.X - right.X, left.Y - right.Y);

        public bool Equals(ScreenPoint other) => X.Equals(other.X) && Y.Equals(other.Y);
        public override bool Equals(object obj) => obj is ScreenPoint other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}
