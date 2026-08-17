using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class V10ToV11CustomizationMigration : ISaveMigration
    {
        public int FromVersion => 10;
        public int ToVersion => 11;
        public string Migrate(string sourcePayload)
        {
            PlayerProgressV10Dto source;
            try { source = JsonUtility.FromJson<PlayerProgressV10Dto>(sourcePayload); }
            catch (Exception exception) { throw new SaveDataException("SaveMigrationV10Invalid", exception); }
            if (source == null || string.IsNullOrWhiteSpace(source.AppVersion) || source.Stars < 0 || source.WorldIds == null ||
                source.Discoveries == null || source.ProcessedDiscoveryGrantIds == null || source.Photos == null ||
                source.CompletedMissionIds == null || source.Settings == null || source.ProcessedEconomyTransactionIds == null ||
                source.EconomyLedger == null || source.Missions == null || source.ProcessedMissionFactIds == null ||
                source.LearningSessions == null || source.LearningConcepts == null || source.UnlockedCampUpgradeIds == null || source.Metadata == null)
                throw new SaveDataException("SaveMigrationV10Invalid");
            return JsonUtility.ToJson(PlayerProgressV11Dto.Create(source.AppVersion, source.Stars, source.WorldIds,
                source.Discoveries, source.ProcessedDiscoveryGrantIds, source.Photos, source.CompletedMissionIds,
                source.Settings, source.ProcessedEconomyTransactionIds, source.EconomyLedger, source.Missions,
                source.ProcessedMissionFactIds, source.LastMissionFactSequence, source.LearningSessions,
                source.LearningConcepts, source.UnlockedCampUpgradeIds, Array.Empty<string>(),
                Array.Empty<EquippedCosmeticV11Dto>(), source.Metadata), false);
        }
    }
}
