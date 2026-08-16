namespace PequenoExplorador.Application.Input
{
    public readonly struct InputIntent
    {
        public InputIntent(
            InputMapId map,
            InputGestureKind kind,
            int pointerId,
            ScreenPoint position,
            ScreenPoint delta,
            float value = 0f)
        {
            Map = map;
            Kind = kind;
            PointerId = pointerId;
            Position = position;
            Delta = delta;
            Value = value;
        }

        public InputMapId Map { get; }
        public InputGestureKind Kind { get; }
        public int PointerId { get; }
        public ScreenPoint Position { get; }
        public ScreenPoint Delta { get; }
        public float Value { get; }
    }
}
