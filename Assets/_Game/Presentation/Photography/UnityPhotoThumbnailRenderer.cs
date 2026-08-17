using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Photography;
using UnityEngine;

namespace PequenoExplorador.Presentation.Photography
{
    public sealed class UnityPhotoThumbnailRenderer : IPhotoThumbnailRenderer
    {
        public const int DefaultWidth = 384;
        public const int DefaultHeight = 216;
        private readonly Camera _camera;
        private readonly int _width;
        private readonly int _height;
        private readonly RenderTextureFormat _format;
        public static int ActiveTemporaryResources { get; private set; }
        public static long LastEstimatedPeakBytes { get; private set; }
        public UnityPhotoThumbnailRenderer(Camera camera, int width = DefaultWidth, int height = DefaultHeight,
            RenderTextureFormat format = RenderTextureFormat.ARGB32)
        {
            _camera = camera != null ? camera : throw new ArgumentNullException(nameof(camera));
            if (width < 64 || width > 512 || height < 64 || height > 512) throw new ArgumentOutOfRangeException(nameof(width));
            _width = width; _height = height; _format = format;
        }
        public Task<PhotoThumbnail> CaptureAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            RenderTexture temporary = null;
            Texture2D texture = null;
            RenderTexture previousActive = RenderTexture.active;
            RenderTexture previousTarget = _camera.targetTexture;
            try
            {
                temporary = RenderTexture.GetTemporary(_width, _height, 24, _format, RenderTextureReadWrite.sRGB);
                temporary.filterMode = FilterMode.Bilinear;
                ActiveTemporaryResources++;
                _camera.targetTexture = temporary;
                _camera.Render();
                RenderTexture.active = temporary;
                texture = new Texture2D(_width, _height, TextureFormat.RGB24, false, false);
                texture.ReadPixels(new Rect(0f, 0f, _width, _height), 0, 0, false);
                texture.Apply(false, false);
                cancellationToken.ThrowIfCancellationRequested();
                byte[] png = texture.EncodeToPNG();
                LastEstimatedPeakBytes = (long)_width * _height * 7L + png.Length;
                return Task.FromResult(new PhotoThumbnail(png, _width, _height));
            }
            finally
            {
                _camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                if (temporary != null) { RenderTexture.ReleaseTemporary(temporary); ActiveTemporaryResources--; }
                if (texture != null)
                {
                    if (UnityEngine.Application.isPlaying) UnityEngine.Object.Destroy(texture);
                    else UnityEngine.Object.DestroyImmediate(texture);
                }
            }
        }
    }
}
