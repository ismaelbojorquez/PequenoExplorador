using PequenoExplorador.Application.SceneFlow;

namespace PequenoExplorador.Infrastructure.SceneFlow
{
    public static class LocalSceneAddresses
    {
        public const string Camp = "scene/camp";
        public const string Jungle = "scene/jungle";
        public static SceneContentId CampId => SceneContentId.Parse(Camp);
    }
}
