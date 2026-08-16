namespace PequenoExplorador.Application.Accessibility
{
    public readonly struct DeviceAspectPreset
    {
        public DeviceAspectPreset(string id, int width, int height, float leftInset, float rightInset)
        {
            Id = id;
            Width = width;
            Height = height;
            LeftInset = leftInset;
            RightInset = rightInset;
        }

        public string Id { get; }
        public int Width { get; }
        public int Height { get; }
        public float LeftInset { get; }
        public float RightInset { get; }
        public float Ratio => (float)Width / Height;
    }

    public static class DeviceAspectPresets
    {
        public static readonly DeviceAspectPreset[] Landscape =
        {
            new DeviceAspectPreset("tablet-4-3", 2048, 1536, 0f, 0f),
            new DeviceAspectPreset("phone-16-9", 1920, 1080, 0f, 0f),
            new DeviceAspectPreset("phone-20-9", 2400, 1080, 80f / 2400f, 80f / 2400f),
            new DeviceAspectPreset("tablet-16-10", 2560, 1600, 0f, 0f)
        };
    }
}
