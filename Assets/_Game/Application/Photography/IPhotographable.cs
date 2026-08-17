namespace PequenoExplorador.Application.Photography
{
    public interface IPhotographable
    {
        PhotoTarget Target { get; }
        bool IsAlive { get; }
        PhotoFrameSample Sample();
    }
}
