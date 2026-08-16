using System;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Audio
{
    public sealed class UnityAudioCue
    {
        public UnityAudioCue(
            AudioCueId id,
            AudioCueCategory category,
            AudioBus bus,
            AudioPriority priority,
            float cooldownSeconds,
            float gain,
            bool loop,
            LocalizedKey subtitleKey,
            bool hasSubtitle,
            AudioClip spanishClip,
            AudioClip englishClip,
            bool placeholder)
        {
            if (cooldownSeconds < 0f || gain < 0f || gain > 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(cooldownSeconds));
            }

            Id = id;
            Category = category;
            Bus = bus;
            Priority = priority;
            CooldownSeconds = cooldownSeconds;
            Gain = gain;
            Loop = loop;
            SubtitleKey = subtitleKey;
            HasSubtitle = hasSubtitle;
            SpanishClip = spanishClip;
            EnglishClip = englishClip;
            IsPlaceholder = placeholder;
        }

        public AudioCueId Id { get; }
        public AudioCueCategory Category { get; }
        public AudioBus Bus { get; }
        public AudioPriority Priority { get; }
        public float CooldownSeconds { get; }
        public float Gain { get; }
        public bool Loop { get; }
        public LocalizedKey SubtitleKey { get; }
        public bool HasSubtitle { get; }
        public AudioClip SpanishClip { get; }
        public AudioClip EnglishClip { get; }
        public bool IsPlaceholder { get; }

        public AudioClip ClipFor(string localeCode)
        {
            return localeCode == LocaleCode.English && EnglishClip != null ? EnglishClip : SpanishClip;
        }
    }
}
