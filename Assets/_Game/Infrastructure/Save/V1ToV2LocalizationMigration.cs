using System;
using PequenoExplorador.Application.Localization;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class V1ToV2LocalizationMigration : ISaveMigration
    {
        public int FromVersion => 1;
        public int ToVersion => 2;

        public string Migrate(string sourcePayload)
        {
            PlayerProgressV1Dto source;
            try
            {
                source = JsonUtility.FromJson<PlayerProgressV1Dto>(sourcePayload);
            }
            catch (Exception exception)
            {
                throw new SaveDataException("SaveMigrationV1Invalid", exception);
            }

            if (source == null || string.IsNullOrWhiteSpace(source.AppVersion) || source.Stars < 0 ||
                source.WorldIds == null || source.DiscoveryIds == null || source.CompletedMissionIds == null ||
                source.Settings == null || source.Metadata == null)
            {
                throw new SaveDataException("SaveMigrationV1Invalid");
            }

            PlayerProgressV2Dto migrated = PlayerProgressV2Dto.Create(
                source.AppVersion,
                source.Stars,
                source.WorldIds,
                source.DiscoveryIds,
                source.CompletedMissionIds,
                PlayerPreferencesV2Dto.Create(
                    source.Settings.GuidanceMode,
                    source.Settings.MusicEnabled,
                    source.Settings.SoundEffectsEnabled,
                    source.Settings.NarrationEnabled,
                    LocaleCode.Spanish),
                SaveMetadataV1Dto.Create(source.Metadata.SaveSequence));
            return JsonUtility.ToJson(migrated, false);
        }
    }
}
