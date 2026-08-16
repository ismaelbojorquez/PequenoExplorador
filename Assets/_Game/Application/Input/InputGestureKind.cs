namespace PequenoExplorador.Application.Input
{
    public enum InputGestureKind
    {
        None = 0,
        Tap = 1,
        PressAndHold = 2,
        DragStarted = 3,
        Dragged = 4,
        DragEnded = 5,
        Pinch = 6,
        Cancelled = 7,
        Back = 8,
        DebugToggle = 9
    }
}
