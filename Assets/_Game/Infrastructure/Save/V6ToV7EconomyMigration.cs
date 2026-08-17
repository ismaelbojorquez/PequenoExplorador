using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class V6ToV7EconomyMigration : ISaveMigration
    {
        public int FromVersion => 6;
        public int ToVersion => 7;
        public string Migrate(string sourcePayload)
        {
            PlayerProgressV6Dto source;
            try { source = JsonUtility.FromJson<PlayerProgressV6Dto>(sourcePayload); }
            catch (Exception exception) { throw new SaveDataException("SaveMigrationV6Invalid", exception); }
            if (source == null || string.IsNullOrWhiteSpace(source.AppVersion) || source.Stars < 0 || source.WorldIds == null ||
                source.Discoveries == null || source.ProcessedDiscoveryGrantIds == null || source.Photos == null ||
                source.CompletedMissionIds == null || source.Settings == null || source.Metadata == null)
                throw new SaveDataException("SaveMigrationV6Invalid");
            return JsonUtility.ToJson(PlayerProgressV7Dto.Create(source.AppVersion, source.Stars, source.WorldIds,
                source.Discoveries, source.ProcessedDiscoveryGrantIds, source.Photos, source.CompletedMissionIds, source.Settings,
                Array.Empty<string>(), Array.Empty<EconomyLedgerEntryV7Dto>(), source.Metadata), false);
        }
    }
}
