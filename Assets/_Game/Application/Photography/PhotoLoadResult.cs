using System;

namespace PequenoExplorador.Application.Photography
{
    public sealed class PhotoLoadResult
    {
        private PhotoLoadResult(PhotoLoadStatus status, byte[] pngBytes)
        {
            Status = status;
            PngBytes = pngBytes ?? Array.Empty<byte>();
        }

        public PhotoLoadStatus Status { get; }
        public byte[] PngBytes { get; }
        public bool IsLoaded => Status == PhotoLoadStatus.Loaded && PngBytes.Length > 0;
        public static PhotoLoadResult Loaded(byte[] bytes) => new PhotoLoadResult(PhotoLoadStatus.Loaded, bytes);
        public static PhotoLoadResult Missing() => new PhotoLoadResult(PhotoLoadStatus.Missing, Array.Empty<byte>());
        public static PhotoLoadResult Invalid() => new PhotoLoadResult(PhotoLoadStatus.Invalid, Array.Empty<byte>());
    }
}
