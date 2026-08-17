using System;

namespace PequenoExplorador.Application.Photography
{
    public sealed class PhotoThumbnail
    {
        public PhotoThumbnail(byte[] pngBytes, int width, int height)
        {
            PngBytes = pngBytes ?? throw new ArgumentNullException(nameof(pngBytes));
            if (pngBytes.Length < 1 || pngBytes.Length > 524288) throw new ArgumentOutOfRangeException(nameof(pngBytes));
            if (width < 1 || width > 512 || height < 1 || height > 512) throw new ArgumentOutOfRangeException(nameof(width));
            Width = width;
            Height = height;
        }
        public byte[] PngBytes { get; }
        public int Width { get; }
        public int Height { get; }
    }
}
