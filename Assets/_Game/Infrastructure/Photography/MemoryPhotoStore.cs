using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Infrastructure.Photography
{
    public sealed class MemoryPhotoStore : IPhotoStore
    {
        public string ServiceId => "Photos";
        public Task InitializeAsync(CancellationToken cancellationToken) { cancellationToken.ThrowIfCancellationRequested(); return Task.CompletedTask; }
        public void Shutdown() { }
        public Task<PhotoStoreResult> SaveAsync(DiscoveryId discoveryId, int scorePermille, PhotoThumbnail thumbnail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(new PhotoStoreResult(LocalPhotoStore.SafeFileName(discoveryId, scorePermille), thumbnail.PngBytes.Length));
        }
        public Task DeleteAllAsync(CancellationToken cancellationToken) { cancellationToken.ThrowIfCancellationRequested(); return Task.CompletedTask; }
    }
}
