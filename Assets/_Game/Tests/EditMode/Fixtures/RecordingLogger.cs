using System.Collections.Generic;
using PequenoExplorador.Application.Logging;

namespace PequenoExplorador.Tests.EditMode.Fixtures
{
    internal sealed class RecordingLogger : IAppLogger
    {
        public List<AppLogEntry> Entries { get; } = new List<AppLogEntry>();

        public void Write(AppLogEntry entry)
        {
            Entries.Add(entry);
        }
    }
}
