using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Photography
{
    public interface IPhotoStore : IApplicationService
    {
        Task<PhotoStoreResult> SaveAsync(DiscoveryId discoveryId, int scorePermille, PhotoThumbnail thumbnail, CancellationToken cancellationToken);
        Task DeleteAllAsync(CancellationToken cancellationToken);
    }
}
