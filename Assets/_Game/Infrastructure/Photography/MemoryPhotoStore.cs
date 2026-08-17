using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Infrastructure.Photography
{
    public sealed class MemoryPhotoStore : IPhotoStore
    {
        private readonly Dictionary<string, byte[]> _photos = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        public string ServiceId => "Photos";
        public Task InitializeAsync(CancellationToken cancellationToken) { cancellationToken.ThrowIfCancellationRequested(); return Task.CompletedTask; }
        public void Shutdown() { }
        public Task<PhotoStoreResult> SaveAsync(DiscoveryId discoveryId, int scorePermille, PhotoThumbnail thumbnail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string reference = LocalPhotoStore.SafeFileName(discoveryId, scorePermille);
            _photos[reference] = (byte[])thumbnail.PngBytes.Clone();
            return Task.FromResult(new PhotoStoreResult(reference, thumbnail.PngBytes.Length));
        }
        public Task<PhotoLoadResult> LoadAsync(string fileReference, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_photos.TryGetValue(fileReference ?? string.Empty, out byte[] bytes)
                ? PhotoLoadResult.Loaded((byte[])bytes.Clone())
                : PhotoLoadResult.Missing());
        }
        public Task DeleteAllAsync(CancellationToken cancellationToken) { cancellationToken.ThrowIfCancellationRequested(); _photos.Clear(); return Task.CompletedTask; }
    }
}
