using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class DiscoveryProgressV4Dto
    {
        [SerializeField] private string id;
        [SerializeField] private int count;
        [SerializeField] private string firstObservedLocalDate;

        public string Id => id;
        public int Count => count;
        public string FirstObservedLocalDate => firstObservedLocalDate;

        public static DiscoveryProgressV4Dto Create(string value, int discoveryCount, string localDate) =>
            new DiscoveryProgressV4Dto
            {
                id = value,
                count = discoveryCount,
                firstObservedLocalDate = localDate ?? string.Empty
            };
    }

    [Serializable]
    internal sealed class PlayerProgressV4Dto
    {
        [SerializeField] private string appVersion;
        [SerializeField] private int stars;
        [SerializeField] private string[] worldIds;
        [SerializeField] private DiscoveryProgressV4Dto[] discoveries;
        [SerializeField] private string[] processedDiscoveryGrantIds;
        [SerializeField] private string[] completedMissionIds;
        [SerializeField] private PlayerPreferencesV3Dto settings;
        [SerializeField] private SaveMetadataV1Dto metadata;

        public string AppVersion => appVersion;
        public int Stars => stars;
        public string[] WorldIds => worldIds;
        public DiscoveryProgressV4Dto[] Discoveries => discoveries;
        public string[] ProcessedDiscoveryGrantIds => processedDiscoveryGrantIds;
        public string[] CompletedMissionIds => completedMissionIds;
        public PlayerPreferencesV3Dto Settings => settings;
        public SaveMetadataV1Dto Metadata => metadata;

        public static PlayerProgressV4Dto Create(
            string version,
            int starCount,
            string[] worlds,
            DiscoveryProgressV4Dto[] discoveryProgress,
            string[] processedGrants,
            string[] missions,
            PlayerPreferencesV3Dto preferences,
            SaveMetadataV1Dto technicalMetadata) =>
            new PlayerProgressV4Dto
            {
                appVersion = version,
                stars = starCount,
                worldIds = worlds,
                discoveries = discoveryProgress,
                processedDiscoveryGrantIds = processedGrants,
                completedMissionIds = missions,
                settings = preferences,
                metadata = technicalMetadata
            };
    }
}
