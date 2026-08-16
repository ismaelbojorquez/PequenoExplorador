using System;
using System.Linq;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class V3ToV4DiscoveryMigration : ISaveMigration
    {
        public int FromVersion => 3;
        public int ToVersion => 4;

        public string Migrate(string sourcePayload)
        {
            PlayerProgressV3Dto source;
            try
            {
                source = JsonUtility.FromJson<PlayerProgressV3Dto>(sourcePayload);
            }
            catch (Exception exception)
            {
                throw new SaveDataException("SaveMigrationV3Invalid", exception);
            }

            if (source == null || string.IsNullOrWhiteSpace(source.AppVersion) || source.Stars < 0 ||
                source.WorldIds == null || source.DiscoveryIds == null || source.CompletedMissionIds == null ||
                source.Settings == null || source.Metadata == null)
                throw new SaveDataException("SaveMigrationV3Invalid");

            DiscoveryProgressV4Dto[] discoveries = source.DiscoveryIds
                .Select(id => DiscoveryProgressV4Dto.Create(id, 1, string.Empty))
                .ToArray();
            PlayerProgressV4Dto migrated = PlayerProgressV4Dto.Create(
                source.AppVersion,
                source.Stars,
                source.WorldIds,
                discoveries,
                Array.Empty<string>(),
                source.CompletedMissionIds,
                source.Settings,
                source.Metadata);
            return JsonUtility.ToJson(migrated, false);
        }
    }
}
