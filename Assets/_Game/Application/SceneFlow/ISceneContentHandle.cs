namespace PequenoExplorador.Application.SceneFlow
{
    public interface ISceneContentHandle
    {
        SceneContentId ContentId { get; }
        bool IsReleased { get; }
    }
}
