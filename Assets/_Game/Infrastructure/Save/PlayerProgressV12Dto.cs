using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class TutorialProgressV12Dto
    {
        [SerializeField] private string tutorialId;
        [SerializeField] private int contentVersion;
        [SerializeField] private int stepIndex;
        [SerializeField] private int status;
        public string TutorialId => tutorialId; public int ContentVersion => contentVersion;
        public int StepIndex => stepIndex; public int Status => status;
        public static TutorialProgressV12Dto Create(string id, int version, int step, int progressStatus) =>
            new TutorialProgressV12Dto { tutorialId = id, contentVersion = version, stepIndex = step, status = progressStatus };
    }

    [Serializable]
    internal sealed class PlayerProgressV12Dto
    {
        [SerializeField] private string appVersion; [SerializeField] private int stars; [SerializeField] private string[] worldIds;
        [SerializeField] private DiscoveryProgressV4Dto[] discoveries; [SerializeField] private string[] processedDiscoveryGrantIds;
        [SerializeField] private PhotoProgressV6Dto[] photos; [SerializeField] private string[] completedMissionIds;
        [SerializeField] private PlayerPreferencesV3Dto settings; [SerializeField] private string[] processedEconomyTransactionIds;
        [SerializeField] private EconomyLedgerEntryV7Dto[] economyLedger; [SerializeField] private MissionProgressV8Dto[] missions;
        [SerializeField] private string[] processedMissionFactIds; [SerializeField] private long lastMissionFactSequence;
        [SerializeField] private LearningSessionV9Dto[] learningSessions; [SerializeField] private LearningConceptDailyV9Dto[] learningConcepts;
        [SerializeField] private string[] unlockedCampUpgradeIds; [SerializeField] private string[] unlockedCosmeticIds;
        [SerializeField] private EquippedCosmeticV11Dto[] equippedCosmetics; [SerializeField] private TutorialProgressV12Dto tutorial;
        [SerializeField] private SaveMetadataV1Dto metadata;
        public string AppVersion => appVersion; public int Stars => stars; public string[] WorldIds => worldIds;
        public DiscoveryProgressV4Dto[] Discoveries => discoveries; public string[] ProcessedDiscoveryGrantIds => processedDiscoveryGrantIds;
        public PhotoProgressV6Dto[] Photos => photos; public string[] CompletedMissionIds => completedMissionIds; public PlayerPreferencesV3Dto Settings => settings;
        public string[] ProcessedEconomyTransactionIds => processedEconomyTransactionIds; public EconomyLedgerEntryV7Dto[] EconomyLedger => economyLedger;
        public MissionProgressV8Dto[] Missions => missions; public string[] ProcessedMissionFactIds => processedMissionFactIds; public long LastMissionFactSequence => lastMissionFactSequence;
        public LearningSessionV9Dto[] LearningSessions => learningSessions; public LearningConceptDailyV9Dto[] LearningConcepts => learningConcepts;
        public string[] UnlockedCampUpgradeIds => unlockedCampUpgradeIds; public string[] UnlockedCosmeticIds => unlockedCosmeticIds;
        public EquippedCosmeticV11Dto[] EquippedCosmetics => equippedCosmetics; public TutorialProgressV12Dto Tutorial => tutorial; public SaveMetadataV1Dto Metadata => metadata;
        public static PlayerProgressV12Dto Create(string version, int starCount, string[] worlds, DiscoveryProgressV4Dto[] discoveryProgress,
            string[] discoveryGrants, PhotoProgressV6Dto[] photoProgress, string[] completedMissions, PlayerPreferencesV3Dto preferences,
            string[] economyTransactions, EconomyLedgerEntryV7Dto[] ledger, MissionProgressV8Dto[] missionProgress,
            string[] missionFacts, long missionFactSequence, LearningSessionV9Dto[] sessions, LearningConceptDailyV9Dto[] concepts,
            string[] campUpgradeIds, string[] cosmeticIds, EquippedCosmeticV11Dto[] equipped, TutorialProgressV12Dto tutorialProgress,
            SaveMetadataV1Dto technicalMetadata) => new PlayerProgressV12Dto
            { appVersion = version, stars = starCount, worldIds = worlds, discoveries = discoveryProgress, processedDiscoveryGrantIds = discoveryGrants,
              photos = photoProgress, completedMissionIds = completedMissions, settings = preferences, processedEconomyTransactionIds = economyTransactions,
              economyLedger = ledger, missions = missionProgress, processedMissionFactIds = missionFacts, lastMissionFactSequence = missionFactSequence,
              learningSessions = sessions, learningConcepts = concepts, unlockedCampUpgradeIds = campUpgradeIds, unlockedCosmeticIds = cosmeticIds,
              equippedCosmetics = equipped, tutorial = tutorialProgress, metadata = technicalMetadata };
    }
}
