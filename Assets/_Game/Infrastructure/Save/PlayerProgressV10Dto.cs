using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class PlayerProgressV10Dto
    {
        [SerializeField] private string appVersion; [SerializeField] private int stars; [SerializeField] private string[] worldIds;
        [SerializeField] private DiscoveryProgressV4Dto[] discoveries; [SerializeField] private string[] processedDiscoveryGrantIds;
        [SerializeField] private PhotoProgressV6Dto[] photos; [SerializeField] private string[] completedMissionIds;
        [SerializeField] private PlayerPreferencesV3Dto settings; [SerializeField] private string[] processedEconomyTransactionIds;
        [SerializeField] private EconomyLedgerEntryV7Dto[] economyLedger; [SerializeField] private MissionProgressV8Dto[] missions;
        [SerializeField] private string[] processedMissionFactIds; [SerializeField] private long lastMissionFactSequence;
        [SerializeField] private LearningSessionV9Dto[] learningSessions; [SerializeField] private LearningConceptDailyV9Dto[] learningConcepts;
        [SerializeField] private string[] unlockedCampUpgradeIds;
        [SerializeField] private SaveMetadataV1Dto metadata;
        public string AppVersion => appVersion; public int Stars => stars; public string[] WorldIds => worldIds;
        public DiscoveryProgressV4Dto[] Discoveries => discoveries; public string[] ProcessedDiscoveryGrantIds => processedDiscoveryGrantIds;
        public PhotoProgressV6Dto[] Photos => photos; public string[] CompletedMissionIds => completedMissionIds; public PlayerPreferencesV3Dto Settings => settings;
        public string[] ProcessedEconomyTransactionIds => processedEconomyTransactionIds; public EconomyLedgerEntryV7Dto[] EconomyLedger => economyLedger;
        public MissionProgressV8Dto[] Missions => missions; public string[] ProcessedMissionFactIds => processedMissionFactIds; public long LastMissionFactSequence => lastMissionFactSequence;
        public LearningSessionV9Dto[] LearningSessions => learningSessions; public LearningConceptDailyV9Dto[] LearningConcepts => learningConcepts;
        public string[] UnlockedCampUpgradeIds => unlockedCampUpgradeIds; public SaveMetadataV1Dto Metadata => metadata;
        public static PlayerProgressV10Dto Create(string version, int starCount, string[] worlds, DiscoveryProgressV4Dto[] discoveryProgress,
            string[] discoveryGrants, PhotoProgressV6Dto[] photoProgress, string[] completedMissions, PlayerPreferencesV3Dto preferences,
            string[] economyTransactions, EconomyLedgerEntryV7Dto[] ledger, MissionProgressV8Dto[] missionProgress,
            string[] missionFacts, long missionFactSequence, LearningSessionV9Dto[] sessions, LearningConceptDailyV9Dto[] concepts,
            string[] campUpgradeIds, SaveMetadataV1Dto technicalMetadata) => new PlayerProgressV10Dto
            { appVersion = version, stars = starCount, worldIds = worlds, discoveries = discoveryProgress, processedDiscoveryGrantIds = discoveryGrants,
              photos = photoProgress, completedMissionIds = completedMissions, settings = preferences, processedEconomyTransactionIds = economyTransactions,
              economyLedger = ledger, missions = missionProgress, processedMissionFactIds = missionFacts, lastMissionFactSequence = missionFactSequence,
              learningSessions = sessions, learningConcepts = concepts, unlockedCampUpgradeIds = campUpgradeIds, metadata = technicalMetadata };
    }
}
