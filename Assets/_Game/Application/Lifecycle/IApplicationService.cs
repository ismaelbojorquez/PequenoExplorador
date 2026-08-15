using System.Threading;
using System.Threading.Tasks;

namespace PequenoExplorador.Application.Lifecycle
{
    public interface IApplicationService
    {
        string ServiceId { get; }

        Task InitializeAsync(CancellationToken cancellationToken);

        void Shutdown();
    }
}
