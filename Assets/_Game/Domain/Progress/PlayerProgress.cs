using System;
using System.Collections.Generic;
using System.Linq;

namespace PequenoExplorador.Domain.Progress
{
    public sealed class PlayerProgress
    {
        private readonly string[] _worldIds;
        private readonly string[] _discoveryIds;
        private readonly string[] _completedMissionIds;

        public PlayerProgress(
            int stars,
            IEnumerable<string> worldIds,
            IEnumerable<string> discoveryIds,
            IEnumerable<string> completedMissionIds,
            PlayerPreferences preferences)
        {
            if (stars < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stars), "Stars cannot be negative.");
            }

            Stars = stars;
            _worldIds = CopyAndValidateIds(worldIds, nameof(worldIds));
            _discoveryIds = CopyAndValidateIds(discoveryIds, nameof(discoveryIds));
            _completedMissionIds = CopyAndValidateIds(completedMissionIds, nameof(completedMissionIds));
            Preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        }

        public int Stars { get; }
        public IReadOnlyList<string> WorldIds => _worldIds;
        public IReadOnlyList<string> DiscoveryIds => _discoveryIds;
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
            return new PlayerProgress(stars, _worldIds, _discoveryIds, _completedMissionIds, Preferences);
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
