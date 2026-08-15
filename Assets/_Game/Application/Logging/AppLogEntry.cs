using System;

namespace PequenoExplorador.Application.Logging
{
    public enum AppLogLevel
    {
        Info,
        Warning,
        Error
    }

    public sealed class AppLogEntry
    {
        public AppLogEntry(AppLogLevel level, string subsystem, string eventId, string detail)
        {
            Level = level;
            Subsystem = RequireTechnicalValue(subsystem, nameof(subsystem));
            EventId = RequireTechnicalValue(eventId, nameof(eventId));
            Detail = RequireTechnicalValue(detail, nameof(detail));
        }

        public AppLogLevel Level { get; }
        public string Subsystem { get; }
        public string EventId { get; }
        public string Detail { get; }

        private static string RequireTechnicalValue(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Structured log values must not be empty.", parameterName);
            }

            return value;
        }
    }
}
