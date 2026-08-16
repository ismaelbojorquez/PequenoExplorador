using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Audio;

namespace PequenoExplorador.Infrastructure.Audio
{
    internal enum VoiceOfferResult
    {
        Start,
        Queue,
        Interrupt,
        Rejected
    }

    internal sealed class VoiceQueueScheduler
    {
        private readonly int _capacity;
        private readonly List<(UnityAudioCue Cue, long Sequence)> _pending = new List<(UnityAudioCue, long)>();
        private long _sequence;

        public VoiceQueueScheduler(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            _capacity = capacity;
        }

        public UnityAudioCue Current { get; private set; }
        public int PendingCount => _pending.Count;

        public VoiceOfferResult Offer(UnityAudioCue cue)
        {
            if (cue == null)
            {
                throw new ArgumentNullException(nameof(cue));
            }

            if (Current == null)
            {
                Current = cue;
                return VoiceOfferResult.Start;
            }

            if (cue.Priority > Current.Priority)
            {
                Current = cue;
                return VoiceOfferResult.Interrupt;
            }

            if (_pending.Count >= _capacity)
            {
                int lowestIndex = _pending
                    .Select((item, index) => (item, index))
                    .OrderBy(pair => pair.item.Cue.Priority)
                    .ThenByDescending(pair => pair.item.Sequence)
                    .First().index;
                if (_pending[lowestIndex].Cue.Priority > cue.Priority)
                {
                    return VoiceOfferResult.Rejected;
                }

                _pending.RemoveAt(lowestIndex);
            }

            _pending.Add((cue, _sequence++));
            return VoiceOfferResult.Queue;
        }

        public UnityAudioCue CompleteAndTakeNext()
        {
            if (_pending.Count == 0)
            {
                Current = null;
                return null;
            }

            var next = _pending
                .OrderByDescending(item => item.Cue.Priority)
                .ThenBy(item => item.Sequence)
                .First();
            _pending.Remove(next);
            Current = next.Cue;
            return Current;
        }

        public void Clear()
        {
            Current = null;
            _pending.Clear();
        }
    }
}
