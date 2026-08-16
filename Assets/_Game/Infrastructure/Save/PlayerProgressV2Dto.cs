using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class PlayerProgressV2Dto
    {
        [SerializeField] private string appVersion;
        [SerializeField] private int stars;
        [SerializeField] private string[] worldIds;
        [SerializeField] private string[] discoveryIds;
        [SerializeField] private string[] completedMissionIds;
        [SerializeField] private PlayerPreferencesV2Dto settings;
        [SerializeField] private SaveMetadataV1Dto metadata;

        public string AppVersion => appVersion;
        public int Stars => stars;
        public string[] WorldIds => worldIds;
        public string[] DiscoveryIds => discoveryIds;
        public string[] CompletedMissionIds => completedMissionIds;
        public PlayerPreferencesV2Dto Settings => settings;
        public SaveMetadataV1Dto Metadata => metadata;

        public static PlayerProgressV2Dto Create(
            string version,
            int starCount,
            string[] worlds,
            string[] discoveries,
            string[] missions,
            PlayerPreferencesV2Dto preferences,
            SaveMetadataV1Dto technicalMetadata)
        {
            return new PlayerProgressV2Dto
            {
                appVersion = version,
                stars = starCount,
                worldIds = worlds,
                discoveryIds = discoveries,
                completedMissionIds = missions,
                settings = preferences,
                metadata = technicalMetadata
            };
        }
    }
}
