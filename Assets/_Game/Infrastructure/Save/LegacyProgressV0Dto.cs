using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class LegacyProgressV0Dto
    {
        [SerializeField] private string appVersion;
        [SerializeField] private int stars;

        public string AppVersion => appVersion;
        public int Stars => stars;
    }
}
