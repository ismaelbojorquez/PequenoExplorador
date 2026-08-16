using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class PlayerPreferencesV3Dto
    {
        [SerializeField] private int guidanceMode;
        [SerializeField] private string localeCode;
        [SerializeField] private float masterVolume;
        [SerializeField] private float musicVolume;
        [SerializeField] private float ambienceVolume;
        [SerializeField] private float effectsVolume;
        [SerializeField] private float voiceVolume;
        [SerializeField] private bool subtitlesEnabled;

        public int GuidanceMode => guidanceMode;
        public string LocaleCode => localeCode;
        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float AmbienceVolume => ambienceVolume;
        public float EffectsVolume => effectsVolume;
        public float VoiceVolume => voiceVolume;
        public bool SubtitlesEnabled => subtitlesEnabled;

        public static PlayerPreferencesV3Dto Create(
            int mode,
            string locale,
            float master,
            float music,
            float ambience,
            float effects,
            float voice,
            bool subtitles)
        {
            return new PlayerPreferencesV3Dto
            {
                guidanceMode = mode,
                localeCode = locale,
                masterVolume = master,
                musicVolume = music,
                ambienceVolume = ambience,
                effectsVolume = effects,
                voiceVolume = voice,
                subtitlesEnabled = subtitles
            };
        }
    }
}
