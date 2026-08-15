using System;
using PequenoExplorador.Application.Logging;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Logging
{
    public sealed class UnityStructuredLogger : IAppLogger
    {
        public void Write(AppLogEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            string line =
                $"PE_LOG level={entry.Level} subsystem={Sanitize(entry.Subsystem)} event={Sanitize(entry.EventId)} detail={Sanitize(entry.Detail)}";
            switch (entry.Level)
            {
                case AppLogLevel.Warning:
                    Debug.LogWarning(line);
                    break;
                case AppLogLevel.Error:
                    Debug.LogError(line);
                    break;
                default:
                    Debug.Log(line);
                    break;
            }
        }

        private static string Sanitize(string value)
        {
            return value.Replace(' ', '_').Replace('\n', '_').Replace('\r', '_');
        }
    }
}
