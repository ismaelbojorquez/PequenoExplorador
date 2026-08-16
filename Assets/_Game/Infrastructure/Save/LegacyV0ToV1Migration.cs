using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class LegacyV0ToV1Migration : ISaveMigration
    {
        public int FromVersion => 0;
        public int ToVersion => 1;

        public string Migrate(string sourcePayload)
        {
            LegacyProgressV0Dto legacy;
            try
            {
                legacy = JsonUtility.FromJson<LegacyProgressV0Dto>(sourcePayload);
            }
            catch (Exception exception)
            {
                throw new SaveDataException("SaveMigrationV0Invalid", exception);
            }

            if (legacy == null || string.IsNullOrWhiteSpace(legacy.AppVersion) || legacy.Stars < 0)
            {
                throw new SaveDataException("SaveMigrationV0Invalid");
            }

            PlayerProgressV1Dto migrated = PlayerProgressV1Dto.Create(
                legacy.AppVersion,
                legacy.Stars,
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                PlayerPreferencesV1Dto.Create(0, true, true, true),
                SaveMetadataV1Dto.Create(0));
            return JsonUtility.ToJson(migrated, false);
        }
    }
}
