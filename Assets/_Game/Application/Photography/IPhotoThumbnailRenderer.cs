using System.Threading;
using System.Threading.Tasks;

namespace PequenoExplorador.Application.Photography
{
    public interface IPhotoThumbnailRenderer
    {
        Task<PhotoThumbnail> CaptureAsync(CancellationToken cancellationToken);
    }
}
