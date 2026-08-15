using System;
using PequenoExplorador.Application.Services;

namespace PequenoExplorador.Tests.EditMode.Fixtures
{
    internal sealed class ManualClock : IClock
    {
        public ManualClock(DateTimeOffset utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTimeOffset UtcNow { get; set; }
    }
}
