using System;

namespace PequenoExplorador.Application.Accessibility
{
    public readonly struct SafeAreaSnapshot : IEquatable<SafeAreaSnapshot>
    {
        public SafeAreaSnapshot(
            int screenWidth,
            int screenHeight,
            float left,
            float bottom,
            float right,
            float top,
            DisplayOrientation orientation)
        {
            if (screenWidth <= 0 || screenHeight <= 0) throw new ArgumentOutOfRangeException(nameof(screenWidth));
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            Left = Clamp(left);
            Bottom = Clamp(bottom);
            Right = Clamp(right);
            Top = Clamp(top);
            Orientation = orientation;
        }

        public int ScreenWidth { get; }
        public int ScreenHeight { get; }
        public float Left { get; }
        public float Bottom { get; }
        public float Right { get; }
        public float Top { get; }
        public DisplayOrientation Orientation { get; }
        public float Width => Math.Max(0f, 1f - Left - Right);
        public float Height => Math.Max(0f, 1f - Bottom - Top);

        public bool Equals(SafeAreaSnapshot other) =>
            ScreenWidth == other.ScreenWidth && ScreenHeight == other.ScreenHeight &&
            Left.Equals(other.Left) && Bottom.Equals(other.Bottom) && Right.Equals(other.Right) && Top.Equals(other.Top) &&
            Orientation == other.Orientation;
        public override bool Equals(object obj) => obj is SafeAreaSnapshot other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(ScreenWidth, ScreenHeight, Left, Bottom, Right, Top, Orientation);
        private static float Clamp(float value) => Math.Max(0f, Math.Min(1f, value));
    }
}
