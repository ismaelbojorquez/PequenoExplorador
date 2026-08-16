namespace PequenoExplorador.Infrastructure.Save
{
    internal sealed class SaveEnvelopeData
    {
        public SaveEnvelopeData(int schemaVersion, string checksum, string payload)
        {
            SchemaVersion = schemaVersion;
            Checksum = checksum;
            Payload = payload;
        }

        public int SchemaVersion { get; }
        public string Checksum { get; }
        public string Payload { get; }
    }
}
