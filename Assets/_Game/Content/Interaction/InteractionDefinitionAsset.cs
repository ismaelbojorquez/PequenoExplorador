using PequenoExplorador.Content.Data;
using UnityEngine;

namespace PequenoExplorador.Content.Interaction
{
    [CreateAssetMenu(
        menuName = "Pequeño Explorador/Content/Interaction Definition",
        fileName = "PH_Interaction")]
    public sealed class InteractionDefinitionAsset : ContentDefinitionAsset
    {
        [SerializeField] private string _displayNameTable = "Content";
        [SerializeField] private string _displayNameKey;
        [SerializeField] private string _promptTable = "UI";
        [SerializeField] private string _promptKey = "ui.interaction.action";
        [SerializeField] private string _unavailableTable = "UI";
        [SerializeField] private string _unavailableKey = "ui.interaction.unavailable";
        [SerializeField] private string _promptAudioCueId = "audio.voice.instruction.explore";
        [SerializeField] private string _unavailableAudioCueId = "audio.feedback.retry";
        [SerializeField, Range(0.5f, 4f)] private float _interactionRange = 1.35f;
        [SerializeField, Range(0f, 30f)] private float _cooldownSeconds = 1.5f;
        [SerializeField, Range(0, 100)] private int _priority = 50;
        [SerializeField] private string _directDiscoveryId;

        public string DisplayNameTable => _displayNameTable;
        public string DisplayNameKey => _displayNameKey;
        public string PromptTable => _promptTable;
        public string PromptKey => _promptKey;
        public string UnavailableTable => _unavailableTable;
        public string UnavailableKey => _unavailableKey;
        public string PromptAudioCueId => _promptAudioCueId;
        public string UnavailableAudioCueId => _unavailableAudioCueId;
        public float InteractionRange => _interactionRange;
        public float CooldownSeconds => _cooldownSeconds;
        public int Priority => _priority;
        public string DirectDiscoveryId => _directDiscoveryId;
    }
}
