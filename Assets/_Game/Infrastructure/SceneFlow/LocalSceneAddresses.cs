using System;
using PequenoExplorador.Application.SceneFlow;

namespace PequenoExplorador.Infrastructure.SceneFlow
{
    public static class LocalSceneAddresses
    {
        public const string Camp = "scene/camp";
        public const string Jungle = "scene/jungle";

        public static string For(SceneContentId contentId)
        {
            switch (contentId)
            {
                case SceneContentId.Camp:
                    return Camp;
                case SceneContentId.Jungle:
                    return Jungle;
                default:
                    throw new ArgumentOutOfRangeException(nameof(contentId), contentId, null);
            }
        }
    }
}
