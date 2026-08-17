using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class PlayerProgressV6Dto
    {
        [SerializeField] private string appVersion;
        [SerializeField] private int stars;
        [SerializeField] private string[] worldIds;
        [SerializeField] private DiscoveryProgressV4Dto[] discoveries;
        [SerializeField] private string[] processedDiscoveryGrantIds;
        [SerializeField] private PhotoProgressV6Dto[] photos;
        [SerializeField] private string[] completedMissionIds;
        [SerializeField] private PlayerPreferencesV3Dto settings;
        [SerializeField] private SaveMetadataV1Dto metadata;

        public string AppVersion => appVersion;
        public int Stars => stars;
        public string[] WorldIds => worldIds;
        public DiscoveryProgressV4Dto[] Discoveries => discoveries;
        public string[] ProcessedDiscoveryGrantIds => processedDiscoveryGrantIds;
        public PhotoProgressV6Dto[] Photos => photos;
        public string[] CompletedMissionIds => completedMissionIds;
        public PlayerPreferencesV3Dto Settings => settings;
        public SaveMetadataV1Dto Metadata => metadata;

        public static PlayerProgressV6Dto Create(string version, int starCount, string[] worlds,
            DiscoveryProgressV4Dto[] discoveryProgress, string[] processedGrants, PhotoProgressV6Dto[] photoProgress,
            string[] missions, PlayerPreferencesV3Dto preferences, SaveMetadataV1Dto technicalMetadata) =>
            new PlayerProgressV6Dto { appVersion = version, stars = starCount, worldIds = worlds,
                discoveries = discoveryProgress, processedDiscoveryGrantIds = processedGrants, photos = photoProgress,
                completedMissionIds = missions, settings = preferences, metadata = technicalMetadata };
    }
}
