using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    public sealed class V4ToV5ToucanDiscoveryMigration : ISaveMigration
    {
        public const string RetiredDiscoveryId = "discovery.jungle.placeholder";
        public const string CurrentDiscoveryId = "discovery.jungle.keel-billed-toucan";
        private const string RetiredGrantSuffix = "." + RetiredDiscoveryId;
        private const string CurrentGrantSuffix = "." + CurrentDiscoveryId;

        public int FromVersion => 4;
        public int ToVersion => 5;

        public string Migrate(string sourcePayload)
        {
            PlayerProgressV4Dto source;
            try
            {
                source = JsonUtility.FromJson<PlayerProgressV4Dto>(sourcePayload);
            }
            catch (Exception exception)
            {
                throw new SaveDataException("SaveMigrationV4Invalid", exception);
            }

            if (source == null || string.IsNullOrWhiteSpace(source.AppVersion) || source.Stars < 0 ||
                source.WorldIds == null || source.Discoveries == null || source.ProcessedDiscoveryGrantIds == null ||
                source.CompletedMissionIds == null || source.Settings == null || source.Metadata == null)
                throw new SaveDataException("SaveMigrationV4Invalid");

            DiscoveryProgressV4Dto[] discoveries;
            try
            {
                discoveries = NormalizeDiscoveries(source.Discoveries);
            }
            catch (Exception exception) when (exception is OverflowException || exception is ArgumentException)
            {
                throw new SaveDataException("SaveMigrationV4Invalid", exception);
            }

            string[] grants = source.ProcessedDiscoveryGrantIds
                .Select(NormalizeGrant)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
            PlayerProgressV5Dto migrated = PlayerProgressV5Dto.Create(
                source.AppVersion,
                source.Stars,
                source.WorldIds,
                discoveries,
                grants,
                source.CompletedMissionIds,
                source.Settings,
                source.Metadata);
            return JsonUtility.ToJson(migrated, false);
        }

        private static DiscoveryProgressV4Dto[] NormalizeDiscoveries(IEnumerable<DiscoveryProgressV4Dto> source)
        {
            var normalized = new Dictionary<string, DiscoveryAccumulator>(StringComparer.Ordinal);
            foreach (DiscoveryProgressV4Dto item in source)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Id) || item.Count < 1)
                    throw new ArgumentException("Invalid discovery record.");
                string id = string.Equals(item.Id, RetiredDiscoveryId, StringComparison.Ordinal)
                    ? CurrentDiscoveryId
                    : item.Id;
                if (!normalized.TryGetValue(id, out DiscoveryAccumulator current))
                    current = new DiscoveryAccumulator(0, string.Empty);
                normalized[id] = new DiscoveryAccumulator(
                    checked(current.Count + item.Count),
                    EarliestDate(current.FirstObservedLocalDate, item.FirstObservedLocalDate));
            }

            return normalized
                .OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .Select(pair => DiscoveryProgressV4Dto.Create(
                    pair.Key,
                    pair.Value.Count,
                    pair.Value.FirstObservedLocalDate))
                .ToArray();
        }

        private static string NormalizeGrant(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Invalid discovery grant ID.");
            return value.EndsWith(RetiredGrantSuffix, StringComparison.Ordinal)
                ? value.Substring(0, value.Length - RetiredGrantSuffix.Length) + CurrentGrantSuffix
                : value;
        }

        private static string EarliestDate(string first, string second)
        {
            if (string.IsNullOrEmpty(first)) return second ?? string.Empty;
            if (string.IsNullOrEmpty(second)) return first;
            return string.CompareOrdinal(first, second) <= 0 ? first : second;
        }

        private readonly struct DiscoveryAccumulator
        {
            public DiscoveryAccumulator(int count, string firstObservedLocalDate)
            {
                Count = count;
                FirstObservedLocalDate = firstObservedLocalDate;
            }
            public int Count { get; }
            public string FirstObservedLocalDate { get; }
        }
    }
}
