using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class V5ToV6PhotoProgressMigration : ISaveMigration
    {
        public int FromVersion => 5;
        public int ToVersion => 6;
        public string Migrate(string sourcePayload)
        {
            PlayerProgressV5Dto source;
            try { source = JsonUtility.FromJson<PlayerProgressV5Dto>(sourcePayload); }
            catch (Exception exception) { throw new SaveDataException("SaveMigrationV5Invalid", exception); }
            if (source == null || string.IsNullOrWhiteSpace(source.AppVersion) || source.Stars < 0 ||
                source.WorldIds == null || source.Discoveries == null || source.ProcessedDiscoveryGrantIds == null ||
                source.CompletedMissionIds == null || source.Settings == null || source.Metadata == null)
                throw new SaveDataException("SaveMigrationV5Invalid");
            return JsonUtility.ToJson(PlayerProgressV6Dto.Create(source.AppVersion, source.Stars, source.WorldIds,
                source.Discoveries, source.ProcessedDiscoveryGrantIds, Array.Empty<PhotoProgressV6Dto>(),
                source.CompletedMissionIds, source.Settings, source.Metadata), false);
        }
    }
}
