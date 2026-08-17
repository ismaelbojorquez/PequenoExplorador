using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class V7ToV8MissionMigration : ISaveMigration
    {
        public int FromVersion => 7;
        public int ToVersion => 8;
        public string Migrate(string sourcePayload)
        {
            PlayerProgressV7Dto source;
            try { source = JsonUtility.FromJson<PlayerProgressV7Dto>(sourcePayload); }
            catch (Exception exception) { throw new SaveDataException("SaveMigrationV7Invalid", exception); }
            if (source == null || string.IsNullOrWhiteSpace(source.AppVersion) || source.Stars < 0 || source.WorldIds == null ||
                source.Discoveries == null || source.ProcessedDiscoveryGrantIds == null || source.Photos == null ||
                source.CompletedMissionIds == null || source.Settings == null || source.ProcessedEconomyTransactionIds == null ||
                source.EconomyLedger == null || source.Metadata == null)
                throw new SaveDataException("SaveMigrationV7Invalid");
            return JsonUtility.ToJson(PlayerProgressV8Dto.Create(source.AppVersion, source.Stars, source.WorldIds,
                source.Discoveries, source.ProcessedDiscoveryGrantIds, source.Photos, source.CompletedMissionIds, source.Settings,
                source.ProcessedEconomyTransactionIds, source.EconomyLedger, Array.Empty<MissionProgressV8Dto>(),
                Array.Empty<string>(), 0, source.Metadata), false);
        }
    }
}
