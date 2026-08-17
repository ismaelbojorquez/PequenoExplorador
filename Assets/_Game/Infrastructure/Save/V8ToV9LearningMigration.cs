using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class V8ToV9LearningMigration : ISaveMigration
    {
        public int FromVersion => 8; public int ToVersion => 9;
        public string Migrate(string sourcePayload)
        {
            PlayerProgressV8Dto source;
            try { source = JsonUtility.FromJson<PlayerProgressV8Dto>(sourcePayload); }
            catch (Exception exception) { throw new SaveDataException("SaveMigrationV8Invalid", exception); }
            if (source == null || source.Metadata == null) throw new SaveDataException("SaveMigrationV8Invalid");
            return JsonUtility.ToJson(PlayerProgressV9Dto.Create(source.AppVersion, source.Stars, source.WorldIds,
                source.Discoveries, source.ProcessedDiscoveryGrantIds, source.Photos, source.CompletedMissionIds, source.Settings,
                source.ProcessedEconomyTransactionIds, source.EconomyLedger, source.Missions, source.ProcessedMissionFactIds,
                source.LastMissionFactSequence, Array.Empty<LearningSessionV9Dto>(), Array.Empty<LearningConceptDailyV9Dto>(), source.Metadata));
        }
    }
}
