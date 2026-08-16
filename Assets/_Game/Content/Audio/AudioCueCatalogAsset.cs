using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace PequenoExplorador.Content.Audio
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Audio Cue Catalog", fileName = "AudioCueCatalog")]
    public sealed class AudioCueCatalogAsset : ScriptableObject
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private AudioMixerGroup _master;
        [SerializeField] private AudioMixerGroup _music;
        [SerializeField] private AudioMixerGroup _ambience;
        [SerializeField] private AudioMixerGroup _effects;
        [SerializeField] private AudioMixerGroup _voice;
        [SerializeField] private AudioCueDefinition[] _cues = Array.Empty<AudioCueDefinition>();

        public AudioMixer Mixer => _mixer;
        public AudioMixerGroup Master => _master;
        public AudioMixerGroup Music => _music;
        public AudioMixerGroup Ambience => _ambience;
        public AudioMixerGroup Effects => _effects;
        public AudioMixerGroup Voice => _voice;
        public IReadOnlyList<AudioCueDefinition> Cues => _cues ?? Array.Empty<AudioCueDefinition>();

        public AudioCueDefinition GetRequired(string cueId)
        {
            return Cues.Single(cue => cue != null && string.Equals(cue.RawCueId, cueId, StringComparison.Ordinal));
        }
    }
}
