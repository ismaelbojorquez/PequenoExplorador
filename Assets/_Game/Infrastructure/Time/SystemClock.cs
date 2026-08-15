using System;
using PequenoExplorador.Application.Services;

namespace PequenoExplorador.Infrastructure.Time
{
    public sealed class SystemClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
