using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Domain.Progress
{
    public sealed class PlayerProgress
    {
        private readonly string[] _worldIds;
        private readonly DiscoveryProgress[] _discoveries;
        private readonly string[] _discoveryIds;
        private readonly string[] _processedDiscoveryGrantIds;
        private readonly PhotoProgress[] _photos;
        private readonly string[] _completedMissionIds;

        public PlayerProgress(
            int stars,
            IEnumerable<string> worldIds,
            IEnumerable<string> discoveryIds,
            IEnumerable<string> completedMissionIds,
            PlayerPreferences preferences)
            : this(
                stars,
                worldIds,
                ConvertLegacyDiscoveries(discoveryIds),
                Array.Empty<string>(),
                Array.Empty<PhotoProgress>(),
                completedMissionIds,
                preferences)
        {
        }

        public PlayerProgress(
            int stars,
            IEnumerable<string> worldIds,
            IEnumerable<DiscoveryProgress> discoveries,
            IEnumerable<string> processedDiscoveryGrantIds,
            IEnumerable<string> completedMissionIds,
            PlayerPreferences preferences)
            : this(stars, worldIds, discoveries, processedDiscoveryGrantIds, Array.Empty<PhotoProgress>(), completedMissionIds, preferences)
        {
        }

        public PlayerProgress(
            int stars,
            IEnumerable<string> worldIds,
            IEnumerable<DiscoveryProgress> discoveries,
            IEnumerable<string> processedDiscoveryGrantIds,
            IEnumerable<PhotoProgress> photos,
            IEnumerable<string> completedMissionIds,
            PlayerPreferences preferences)
        {
            if (stars < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stars), "Stars cannot be negative.");
            }

            Stars = stars;
            _worldIds = CopyAndValidateIds(worldIds, nameof(worldIds));
            _discoveries = CopyAndValidateDiscoveries(discoveries);
            _discoveryIds = _discoveries.Select(item => item.Id.Value).ToArray();
            _processedDiscoveryGrantIds = CopyAndValidateGrantIds(processedDiscoveryGrantIds);
            _photos = CopyAndValidatePhotos(photos);
            _completedMissionIds = CopyAndValidateIds(completedMissionIds, nameof(completedMissionIds));
            Preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        }

        public int Stars { get; }
        public IReadOnlyList<string> WorldIds => _worldIds;
        public IReadOnlyList<string> DiscoveryIds => _discoveryIds;
        public IReadOnlyList<DiscoveryProgress> Discoveries => _discoveries;
        public IReadOnlyList<string> ProcessedDiscoveryGrantIds => _processedDiscoveryGrantIds;
        public IReadOnlyList<PhotoProgress> Photos => _photos;
        public IReadOnlyList<string> CompletedMissionIds => _completedMissionIds;
        public PlayerPreferences Preferences { get; }

        public static PlayerProgress CreateDefault()
        {
            return new PlayerProgress(
                0,
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                PlayerPreferences.CreateDefault());
        }

        public PlayerProgress WithStars(int stars)
        {
            return new PlayerProgress(
                stars,
                _worldIds,
                _discoveries,
                _processedDiscoveryGrantIds,
                _photos,
                _completedMissionIds,
                Preferences);
        }

        public PlayerProgress WithPreferences(PlayerPreferences preferences)
        {
            return new PlayerProgress(
                Stars,
                _worldIds,
                _discoveries,
                _processedDiscoveryGrantIds,
                _photos,
                _completedMissionIds,
                preferences);
        }

        public PlayerProgress WithDiscoveryState(
            IEnumerable<DiscoveryProgress> discoveries,
            IEnumerable<string> processedDiscoveryGrantIds)
        {
            return new PlayerProgress(
                Stars,
                _worldIds,
                discoveries,
                processedDiscoveryGrantIds,
                _photos,
                _completedMissionIds,
                Preferences);
        }

        public PlayerProgress WithPhotos(IEnumerable<PhotoProgress> photos)
        {
            return new PlayerProgress(
                Stars,
                _worldIds,
                _discoveries,
                _processedDiscoveryGrantIds,
                photos,
                _completedMissionIds,
                Preferences);
        }

        private static PhotoProgress[] CopyAndValidatePhotos(IEnumerable<PhotoProgress> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            PhotoProgress[] result = values.ToArray();
            if (result.Any(item => item == null))
                throw new ArgumentException("Photo progress cannot contain null entries.", nameof(values));
            if (result.Select(item => item.DiscoveryId).Distinct().Count() != result.Length)
                throw new ArgumentException("Photo progress discovery IDs must be unique.", nameof(values));
            return result.OrderBy(item => item.DiscoveryId.Value, StringComparer.Ordinal).ToArray();
        }

        private static DiscoveryProgress[] CopyAndValidateDiscoveries(
            IEnumerable<DiscoveryProgress> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            DiscoveryProgress[] result = values.ToArray();
            if (result.Any(item => item == null))
                throw new ArgumentException("Discovery progress cannot contain null entries.", nameof(values));
            if (result.Select(item => item.Id).Distinct().Count() != result.Length)
                throw new ArgumentException("Discovery progress IDs must be unique.", nameof(values));
            return result.OrderBy(item => item.Id.Value, StringComparer.Ordinal).ToArray();
        }

        private static DiscoveryProgress[] ConvertLegacyDiscoveries(IEnumerable<string> values)
        {
            return CopyAndValidateIds(values, nameof(values))
                .Select(value => new DiscoveryProgress(DiscoveryId.Parse(value), 1, string.Empty))
                .ToArray();
        }

        private static string[] CopyAndValidateGrantIds(IEnumerable<string> values)
        {
            string[] result = CopyAndValidateIds(values, nameof(values));
            if (result.Any(value => !DiscoveryGrantId.TryParse(value, out _)))
                throw new ArgumentException("Discovery grant IDs are invalid.", nameof(values));
            return result.OrderBy(value => value, StringComparer.Ordinal).ToArray();
        }

        private static string[] CopyAndValidateIds(IEnumerable<string> values, string parameterName)
        {
            if (values == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            string[] result = values.ToArray();
            if (result.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("Progress IDs cannot be empty.", parameterName);
            }

            if (result.Distinct(StringComparer.Ordinal).Count() != result.Length)
            {
                throw new ArgumentException("Progress IDs must be unique.", parameterName);
            }

            return result;
        }
    }
}
