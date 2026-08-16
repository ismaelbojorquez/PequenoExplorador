using System;

namespace PequenoExplorador.Application.Explorer
{
    public readonly struct WorldPosition : IEquatable<WorldPosition>
    {
        public WorldPosition(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public bool Equals(WorldPosition other) =>
            X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        public override bool Equals(object obj) => obj is WorldPosition other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);
    }
}
