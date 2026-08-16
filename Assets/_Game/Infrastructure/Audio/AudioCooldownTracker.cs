using System;
using System.Collections.Generic;
using PequenoExplorador.Application.Audio;

namespace PequenoExplorador.Infrastructure.Audio
{
    internal sealed class AudioCooldownTracker
    {
        private readonly Func<double> _now;
        private readonly Dictionary<AudioCueId, double> _lastPlayed = new Dictionary<AudioCueId, double>();

        public AudioCooldownTracker(Func<double> now)
        {
            _now = now ?? throw new ArgumentNullException(nameof(now));
        }

        public bool TryConsume(AudioCueId cueId, float cooldownSeconds)
        {
            double current = _now();
            if (_lastPlayed.TryGetValue(cueId, out double previous) && current - previous < cooldownSeconds)
            {
                return false;
            }

            _lastPlayed[cueId] = current;
            return true;
        }

        public void Clear() => _lastPlayed.Clear();
    }
}
