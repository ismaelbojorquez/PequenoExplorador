using System;

namespace PequenoExplorador.Application.Services
{
    public interface IClock
    {
        DateTimeOffset UtcNow { get; }
    }
}
