using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class PhotoProgressV6Dto
    {
        [SerializeField] private string discoveryId;
        [SerializeField] private string fileReference;
        [SerializeField] private int scorePermille;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private int byteLength;

        public string DiscoveryId => discoveryId;
        public string FileReference => fileReference;
        public int ScorePermille => scorePermille;
        public int Width => width;
        public int Height => height;
        public int ByteLength => byteLength;
        public static PhotoProgressV6Dto Create(string id, string reference, int score, int pixelWidth, int pixelHeight, int bytes) =>
            new PhotoProgressV6Dto { discoveryId = id, fileReference = reference, scorePermille = score,
                width = pixelWidth, height = pixelHeight, byteLength = bytes };
    }
}
