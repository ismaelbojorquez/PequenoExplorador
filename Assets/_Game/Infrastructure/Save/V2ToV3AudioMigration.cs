using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class V2ToV3AudioMigration : ISaveMigration
    {
        public int FromVersion => 2;
        public int ToVersion => 3;

        public string Migrate(string sourcePayload)
        {
            PlayerProgressV2Dto source;
            try
            {
                source = JsonUtility.FromJson<PlayerProgressV2Dto>(sourcePayload);
            }
            catch (Exception exception)
            {
                throw new SaveDataException("SaveMigrationV2Invalid", exception);
            }

            if (source == null || string.IsNullOrWhiteSpace(source.AppVersion) || source.Stars < 0 ||
                source.WorldIds == null || source.DiscoveryIds == null || source.CompletedMissionIds == null ||
                source.Settings == null || source.Metadata == null)
            {
                throw new SaveDataException("SaveMigrationV2Invalid");
            }

            float music = source.Settings.MusicEnabled ? 0.65f : 0f;
            PlayerProgressV3Dto migrated = PlayerProgressV3Dto.Create(
                source.AppVersion,
                source.Stars,
                source.WorldIds,
                source.DiscoveryIds,
                source.CompletedMissionIds,
                PlayerPreferencesV3Dto.Create(
                    source.Settings.GuidanceMode,
                    source.Settings.LocaleCode,
                    0.85f,
                    music,
                    music,
                    source.Settings.SoundEffectsEnabled ? 0.75f : 0f,
                    source.Settings.NarrationEnabled ? 0.85f : 0f,
                    true),
                SaveMetadataV1Dto.Create(source.Metadata.SaveSequence));
            return JsonUtility.ToJson(migrated, false);
        }
    }
}
