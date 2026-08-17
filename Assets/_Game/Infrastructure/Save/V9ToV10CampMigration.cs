using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class V9ToV10CampMigration : ISaveMigration
    {
        public int FromVersion => 9;
        public int ToVersion => 10;

        public string Migrate(string sourcePayload)
        {
            PlayerProgressV9Dto source;
            try { source = JsonUtility.FromJson<PlayerProgressV9Dto>(sourcePayload); }
            catch (Exception exception) { throw new SaveDataException("SaveMigrationV9Invalid", exception); }
            if (source == null || string.IsNullOrWhiteSpace(source.AppVersion) || source.Stars < 0 || source.WorldIds == null ||
                source.Discoveries == null || source.ProcessedDiscoveryGrantIds == null || source.Photos == null ||
                source.CompletedMissionIds == null || source.Settings == null || source.ProcessedEconomyTransactionIds == null ||
                source.EconomyLedger == null || source.Missions == null || source.ProcessedMissionFactIds == null ||
                source.LearningSessions == null || source.LearningConcepts == null || source.Metadata == null)
                throw new SaveDataException("SaveMigrationV9Invalid");
            return JsonUtility.ToJson(PlayerProgressV10Dto.Create(source.AppVersion, source.Stars, source.WorldIds,
                source.Discoveries, source.ProcessedDiscoveryGrantIds, source.Photos, source.CompletedMissionIds,
                source.Settings, source.ProcessedEconomyTransactionIds, source.EconomyLedger, source.Missions,
                source.ProcessedMissionFactIds, source.LastMissionFactSequence, source.LearningSessions,
                source.LearningConcepts, Array.Empty<string>(), source.Metadata), false);
        }
    }
}
