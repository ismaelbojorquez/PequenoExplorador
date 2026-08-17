using System;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Domain.Progress
{
    public sealed class PhotoProgress
    {
        public PhotoProgress(
            DiscoveryId discoveryId,
            string fileReference,
            int scorePermille,
            int width,
            int height,
            int byteLength)
        {
            if (!discoveryId.IsValid) throw new ArgumentException("Discovery ID is invalid.", nameof(discoveryId));
            if (!IsSafeReference(fileReference)) throw new ArgumentException("Photo reference is invalid.", nameof(fileReference));
            if (scorePermille < 0 || scorePermille > 1000) throw new ArgumentOutOfRangeException(nameof(scorePermille));
            if (width < 1 || width > 512 || height < 1 || height > 512) throw new ArgumentOutOfRangeException(nameof(width));
            if (byteLength < 1 || byteLength > 524288) throw new ArgumentOutOfRangeException(nameof(byteLength));
            DiscoveryId = discoveryId;
            FileReference = fileReference;
            ScorePermille = scorePermille;
            Width = width;
            Height = height;
            ByteLength = byteLength;
        }

        public DiscoveryId DiscoveryId { get; }
        public string FileReference { get; }
        public int ScorePermille { get; }
        public int Width { get; }
        public int Height { get; }
        public int ByteLength { get; }

        private static bool IsSafeReference(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 128 ||
                !value.EndsWith(".png", StringComparison.Ordinal) || value.Contains("..")) return false;
            foreach (char character in value)
            {
                if (!(character >= 'a' && character <= 'z') &&
                    !(character >= '0' && character <= '9') &&
                    character != '_' && character != '-' && character != '.') return false;
            }
            return true;
        }
    }
}
