namespace PequenoExplorador.Application.Explorer
{
    public interface IPathNavigator
    {
        bool IsAvailable { get; }
        bool IsPathPending { get; }
        bool HasPath { get; }
        float RemainingDistance { get; }
        float Speed { get; }
        bool TrySetDestination(WorldPosition destination);
        void Stop();
    }
}
