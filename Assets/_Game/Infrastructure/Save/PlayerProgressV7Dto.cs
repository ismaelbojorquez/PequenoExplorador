using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class PlayerProgressV7Dto
    {
        [SerializeField] private string appVersion;
        [SerializeField] private int stars;
        [SerializeField] private string[] worldIds;
        [SerializeField] private DiscoveryProgressV4Dto[] discoveries;
        [SerializeField] private string[] processedDiscoveryGrantIds;
        [SerializeField] private PhotoProgressV6Dto[] photos;
        [SerializeField] private string[] completedMissionIds;
        [SerializeField] private PlayerPreferencesV3Dto settings;
        [SerializeField] private string[] processedEconomyTransactionIds;
        [SerializeField] private EconomyLedgerEntryV7Dto[] economyLedger;
        [SerializeField] private SaveMetadataV1Dto metadata;
        public string AppVersion => appVersion; public int Stars => stars; public string[] WorldIds => worldIds;
        public DiscoveryProgressV4Dto[] Discoveries => discoveries; public string[] ProcessedDiscoveryGrantIds => processedDiscoveryGrantIds;
        public PhotoProgressV6Dto[] Photos => photos; public string[] CompletedMissionIds => completedMissionIds;
        public PlayerPreferencesV3Dto Settings => settings; public string[] ProcessedEconomyTransactionIds => processedEconomyTransactionIds;
        public EconomyLedgerEntryV7Dto[] EconomyLedger => economyLedger; public SaveMetadataV1Dto Metadata => metadata;
        public static PlayerProgressV7Dto Create(string version, int starCount, string[] worlds, DiscoveryProgressV4Dto[] discoveryProgress,
            string[] discoveryGrants, PhotoProgressV6Dto[] photoProgress, string[] missions, PlayerPreferencesV3Dto preferences,
            string[] economyTransactions, EconomyLedgerEntryV7Dto[] ledger, SaveMetadataV1Dto technicalMetadata) =>
            new PlayerProgressV7Dto { appVersion = version, stars = starCount, worldIds = worlds, discoveries = discoveryProgress,
                processedDiscoveryGrantIds = discoveryGrants, photos = photoProgress, completedMissionIds = missions, settings = preferences,
                processedEconomyTransactionIds = economyTransactions, economyLedger = ledger, metadata = technicalMetadata };
    }
}
