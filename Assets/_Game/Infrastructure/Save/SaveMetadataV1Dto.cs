using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class SaveMetadataV1Dto
    {
        [SerializeField] private int saveSequence;

        public int SaveSequence => saveSequence;

        public static SaveMetadataV1Dto Create(int sequence)
        {
            return new SaveMetadataV1Dto { saveSequence = sequence };
        }
    }
}
