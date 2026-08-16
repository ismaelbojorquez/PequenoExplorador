using System;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Interaction
{
    public sealed class InteractionDefinition
    {
        public InteractionDefinition(
            InteractionId id,
            LocalizedKey displayName,
            LocalizedKey prompt,
            LocalizedKey unavailable,
            AudioCueId promptAudioCue,
            AudioCueId unavailableAudioCue,
            float interactionRange,
            float cooldownSeconds,
            int priority,
            DiscoveryId directDiscoveryId,
            EditorialMetadata editorial)
        {
            if (!id.IsValid) throw new ArgumentException("Interaction ID is invalid.", nameof(id));
            if (float.IsNaN(interactionRange) || interactionRange < 0.5f || interactionRange > 4f)
                throw new ArgumentOutOfRangeException(nameof(interactionRange));
            if (float.IsNaN(cooldownSeconds) || cooldownSeconds < 0f || cooldownSeconds > 30f)
                throw new ArgumentOutOfRangeException(nameof(cooldownSeconds));
            if (priority < 0 || priority > 100) throw new ArgumentOutOfRangeException(nameof(priority));

            Id = id;
            DisplayName = displayName;
            Prompt = prompt;
            Unavailable = unavailable;
            PromptAudioCue = promptAudioCue;
            UnavailableAudioCue = unavailableAudioCue;
            InteractionRange = interactionRange;
            CooldownSeconds = cooldownSeconds;
            Priority = priority;
            DirectDiscoveryId = directDiscoveryId;
            Editorial = editorial ?? throw new ArgumentNullException(nameof(editorial));
        }

        public InteractionId Id { get; }
        public LocalizedKey DisplayName { get; }
        public LocalizedKey Prompt { get; }
        public LocalizedKey Unavailable { get; }
        public AudioCueId PromptAudioCue { get; }
        public AudioCueId UnavailableAudioCue { get; }
        public float InteractionRange { get; }
        public float CooldownSeconds { get; }
        public int Priority { get; }
        public DiscoveryId DirectDiscoveryId { get; }
        public bool HasDirectDiscovery => DirectDiscoveryId.IsValid;
        public EditorialMetadata Editorial { get; }
    }
}
