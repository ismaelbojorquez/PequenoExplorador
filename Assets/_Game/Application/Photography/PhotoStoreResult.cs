using System;

namespace PequenoExplorador.Application.Photography
{
    public sealed class PhotoStoreResult
    {
        public PhotoStoreResult(string fileReference, int byteLength)
        {
            FileReference = string.IsNullOrWhiteSpace(fileReference)
                ? throw new ArgumentException("Photo reference is required.", nameof(fileReference))
                : fileReference;
            ByteLength = byteLength;
        }
        public string FileReference { get; }
        public int ByteLength { get; }
    }
}
