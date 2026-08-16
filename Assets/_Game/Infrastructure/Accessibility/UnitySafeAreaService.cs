using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Accessibility;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Accessibility
{
    public sealed class UnitySafeAreaService : ISafeAreaService
    {
        private bool _initialized;

        public string ServiceId => "SafeArea";
        public event Action<SafeAreaSnapshot> Changed;
        public SafeAreaSnapshot Current { get; private set; } =
            new SafeAreaSnapshot(1, 1, 0f, 0f, 0f, 0f, DisplayOrientation.Unknown);

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _initialized = true;
            Refresh();
            return Task.CompletedTask;
        }

        public void Tick()
        {
            if (_initialized) Refresh();
        }

        public void Shutdown()
        {
            _initialized = false;
            Changed = null;
        }

        private void Refresh()
        {
            int width = Math.Max(1, Screen.width);
            int height = Math.Max(1, Screen.height);
            Rect safe = Screen.safeArea;
            var next = new SafeAreaSnapshot(
                width,
                height,
                safe.xMin / width,
                safe.yMin / height,
                (width - safe.xMax) / width,
                (height - safe.yMax) / height,
                ResolveOrientation(Screen.orientation));
            if (next.Equals(Current)) return;
            Current = next;
            Changed?.Invoke(next);
        }

        private static DisplayOrientation ResolveOrientation(ScreenOrientation orientation)
        {
            switch (orientation)
            {
                case ScreenOrientation.LandscapeLeft: return DisplayOrientation.LandscapeLeft;
                case ScreenOrientation.LandscapeRight: return DisplayOrientation.LandscapeRight;
                default: return DisplayOrientation.Unknown;
            }
        }
    }
}
