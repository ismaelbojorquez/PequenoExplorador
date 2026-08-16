using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    [Serializable]
    internal sealed class PlayerProgressV1Dto
    {
        [SerializeField] private string appVersion;
        [SerializeField] private int stars;
        [SerializeField] private string[] worldIds;
        [SerializeField] private string[] discoveryIds;
        [SerializeField] private string[] completedMissionIds;
        [SerializeField] private PlayerPreferencesV1Dto settings;
        [SerializeField] private SaveMetadataV1Dto metadata;

        public string AppVersion => appVersion;
        public int Stars => stars;
        public string[] WorldIds => worldIds;
        public string[] DiscoveryIds => discoveryIds;
        public string[] CompletedMissionIds => completedMissionIds;
        public PlayerPreferencesV1Dto Settings => settings;
        public SaveMetadataV1Dto Metadata => metadata;

        public static PlayerProgressV1Dto Create(
            string version,
            int starCount,
            string[] worlds,
            string[] discoveries,
            string[] missions,
            PlayerPreferencesV1Dto preferences,
            SaveMetadataV1Dto technicalMetadata)
        {
            return new PlayerProgressV1Dto
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
