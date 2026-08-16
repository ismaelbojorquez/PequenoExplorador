namespace PequenoExplorador.Application.Input
{
    public readonly struct InputMapPolicy
    {
        private InputMapPolicy(bool tap, bool hold, bool drag, bool pinch)
        {
            AllowTap = tap;
            AllowHold = hold;
            AllowDrag = drag;
            AllowPinch = pinch;
        }

        public bool AllowTap { get; }
        public bool AllowHold { get; }
        public bool AllowDrag { get; }
        public bool AllowPinch { get; }

        public static InputMapPolicy For(InputMapId map)
        {
            switch (map)
            {
                case InputMapId.UI: return new InputMapPolicy(true, true, true, false);
                case InputMapId.Explorer: return new InputMapPolicy(true, false, false, false);
                case InputMapId.Photography: return new InputMapPolicy(true, false, true, true);
                case InputMapId.Parents: return new InputMapPolicy(true, false, true, false);
                case InputMapId.Debug: return new InputMapPolicy(true, true, true, true);
                default: return new InputMapPolicy(false, false, false, false);
            }
        }
    }
}
