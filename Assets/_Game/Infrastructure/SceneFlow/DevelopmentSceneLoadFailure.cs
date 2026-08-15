#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
using System.Threading;
using PequenoExplorador.Application.SceneFlow;

namespace PequenoExplorador.Infrastructure.SceneFlow
{
    public sealed class DevelopmentSceneLoadFailure
    {
        private int _remaining;

        public void FailNextLoad()
        {
            Interlocked.Exchange(ref _remaining, 1);
        }

        internal bool Consume(SceneContentId contentId)
        {
            return Interlocked.Exchange(ref _remaining, 0) == 1;
        }
    }
}
#endif
