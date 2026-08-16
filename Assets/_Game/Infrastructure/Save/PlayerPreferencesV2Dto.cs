using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class PlayerPreferencesV2Dto
    {
        [SerializeField] private int guidanceMode;
        [SerializeField] private bool musicEnabled;
        [SerializeField] private bool soundEffectsEnabled;
        [SerializeField] private bool narrationEnabled;
        [SerializeField] private string localeCode;

        public int GuidanceMode => guidanceMode;
        public bool MusicEnabled => musicEnabled;
        public bool SoundEffectsEnabled => soundEffectsEnabled;
        public bool NarrationEnabled => narrationEnabled;
        public string LocaleCode => localeCode;

        public static PlayerPreferencesV2Dto Create(
            int mode,
            bool music,
            bool soundEffects,
            bool narration,
            string locale)
        {
            return new PlayerPreferencesV2Dto
            {
                guidanceMode = mode,
                musicEnabled = music,
                soundEffectsEnabled = soundEffects,
                narrationEnabled = narration,
                localeCode = locale
            };
        }
    }
}
