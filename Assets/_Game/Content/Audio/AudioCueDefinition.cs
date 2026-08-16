using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using UnityEngine;

namespace PequenoExplorador.Content.Audio
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Audio Cue", fileName = "AudioCue")]
    public sealed class AudioCueDefinition : ScriptableObject
    {
        [SerializeField] private string _cueId;
        [SerializeField] private AudioCueCategory _category;
        [SerializeField] private AudioBus _bus;
        [SerializeField] private AudioPriority _priority = AudioPriority.Normal;
        [SerializeField, Min(0f)] private float _cooldownSeconds;
        [SerializeField, Range(0f, 1f)] private float _gain = 0.5f;
        [SerializeField] private bool _loop;
        [SerializeField] private string _subtitleTable;
        [SerializeField] private string _subtitleKey;
        [SerializeField] private AudioClip _spanishClip;
        [SerializeField] private AudioClip _englishClip;
        [SerializeField] private string _spanishAddress;
        [SerializeField] private string _englishAddress;
        [SerializeField] private bool _placeholder = true;
        [SerializeField] private string _placeholderId;
        [SerializeField] private string _releaseState = "ReleaseBlocked";

        public AudioCueId CueId => new AudioCueId(_cueId);
        public string RawCueId => _cueId;
        public AudioCueCategory Category => _category;
        public AudioBus Bus => _bus;
        public AudioPriority Priority => _priority;
        public float CooldownSeconds => _cooldownSeconds;
        public float Gain => _gain;
        public bool Loop => _loop;
        public bool HasSubtitle => !string.IsNullOrWhiteSpace(_subtitleTable) && !string.IsNullOrWhiteSpace(_subtitleKey);
        public LocalizedKey SubtitleKey => HasSubtitle ? new LocalizedKey(_subtitleTable, _subtitleKey) : default;
        public AudioClip SpanishClip => _spanishClip;
        public AudioClip EnglishClip => _englishClip;
        public string SpanishAddress => _spanishAddress;
        public string EnglishAddress => _englishAddress;
        public bool IsPlaceholder => _placeholder;
        public string PlaceholderId => _placeholderId;
        public string ReleaseState => _releaseState;

        public AudioClip GetClip(string localeCode)
        {
            return localeCode == LocaleCode.English && _englishClip != null
                ? _englishClip
                : _spanishClip;
        }
    }
}
