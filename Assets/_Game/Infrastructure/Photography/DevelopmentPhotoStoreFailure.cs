using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Infrastructure.Photography
{
    public sealed class DevelopmentPhotoStoreFailure : IPhotoStore
    {
        private readonly IPhotoStore _inner;
        private int _failNext;
        public DevelopmentPhotoStoreFailure(IPhotoStore inner) => _inner = inner;
        public string ServiceId => _inner.ServiceId;
        public void FailNextSave() => Interlocked.Exchange(ref _failNext, 1);
        public Task InitializeAsync(CancellationToken cancellationToken) => _inner.InitializeAsync(cancellationToken);
        public void Shutdown() => _inner.Shutdown();
        public Task DeleteAllAsync(CancellationToken cancellationToken) => _inner.DeleteAllAsync(cancellationToken);
        public Task<PhotoStoreResult> SaveAsync(DiscoveryId discoveryId, int scorePermille, PhotoThumbnail thumbnail, CancellationToken cancellationToken)
        {
            if (Interlocked.Exchange(ref _failNext, 0) == 1) throw new IOException("InjectedPhotoStorageFailure");
            return _inner.SaveAsync(discoveryId, scorePermille, thumbnail, cancellationToken);
        }
    }
}
