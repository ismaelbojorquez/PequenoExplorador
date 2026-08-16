using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class SaveEnvelopeDto
    {
        [SerializeField] private int schemaVersion;
        [SerializeField] private string checksum;
        [SerializeField] private string payload;

        public int SchemaVersion => schemaVersion;
        public string Checksum => checksum;
        public string Payload => payload;

        public static SaveEnvelopeDto Create(int version, string checksumValue, string payloadValue)
        {
            return new SaveEnvelopeDto
            {
                schemaVersion = version,
                checksum = checksumValue,
                payload = payloadValue
            };
        }
    }
}
