using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class V11ToV12TutorialMigration : ISaveMigration
    {
        public int FromVersion => 11;
        public int ToVersion => 12;
        public string Migrate(string sourcePayload)
        {
            PlayerProgressV11Dto source;
            try { source = JsonUtility.FromJson<PlayerProgressV11Dto>(sourcePayload); }
            catch (Exception exception) { throw new SaveDataException("SaveMigrationV11Invalid", exception); }
            if (source == null || string.IsNullOrWhiteSpace(source.AppVersion) || source.Stars < 0 || source.WorldIds == null ||
                source.Discoveries == null || source.ProcessedDiscoveryGrantIds == null || source.Photos == null || source.CompletedMissionIds == null ||
                source.Settings == null || source.ProcessedEconomyTransactionIds == null || source.EconomyLedger == null || source.Missions == null ||
                source.ProcessedMissionFactIds == null || source.LearningSessions == null || source.LearningConcepts == null ||
                source.UnlockedCampUpgradeIds == null || source.UnlockedCosmeticIds == null || source.EquippedCosmetics == null || source.Metadata == null)
                throw new SaveDataException("SaveMigrationV11Invalid");
            return JsonUtility.ToJson(PlayerProgressV12Dto.Create(source.AppVersion, source.Stars, source.WorldIds,
                source.Discoveries, source.ProcessedDiscoveryGrantIds, source.Photos, source.CompletedMissionIds, source.Settings,
                source.ProcessedEconomyTransactionIds, source.EconomyLedger, source.Missions, source.ProcessedMissionFactIds,
                source.LastMissionFactSequence, source.LearningSessions, source.LearningConcepts, source.UnlockedCampUpgradeIds,
                source.UnlockedCosmeticIds, source.EquippedCosmetics,
                TutorialProgressV12Dto.Create("tutorial.vertical-slice", 0, 0, 0), source.Metadata), false);
        }
    }
}
