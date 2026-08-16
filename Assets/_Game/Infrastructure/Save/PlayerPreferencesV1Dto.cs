using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class PlayerPreferencesV1Dto
    {
        [SerializeField] private int guidanceMode;
        [SerializeField] private bool musicEnabled;
        [SerializeField] private bool soundEffectsEnabled;
        [SerializeField] private bool narrationEnabled;

        public int GuidanceMode => guidanceMode;
        public bool MusicEnabled => musicEnabled;
        public bool SoundEffectsEnabled => soundEffectsEnabled;
        public bool NarrationEnabled => narrationEnabled;

        public static PlayerPreferencesV1Dto Create(
            int mode,
            bool music,
            bool soundEffects,
            bool narration)
        {
            return new PlayerPreferencesV1Dto
            {
                guidanceMode = mode,
                musicEnabled = music,
                soundEffectsEnabled = soundEffects,
                narrationEnabled = narration
            };
        }
    }
}
