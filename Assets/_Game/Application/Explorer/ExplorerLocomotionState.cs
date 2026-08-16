namespace PequenoExplorador.Application.Explorer
{
    public enum ExplorerLocomotionState
    {
        Idle = 0,
        PathPending = 1,
        Moving = 2,
        Arrived = 3,
        InvalidDestination = 4,
        Suspended = 5
    }
}
