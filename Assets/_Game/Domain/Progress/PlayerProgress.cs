using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;

namespace PequenoExplorador.Domain.Progress
{
    public sealed class PlayerProgress
    {
        public const int EconomyLedgerMaximumEntries = 32;
        private readonly string[] _worldIds;
        private readonly DiscoveryProgress[] _discoveries;
        private readonly string[] _discoveryIds;
        private readonly string[] _processedDiscoveryGrantIds;
        private readonly PhotoProgress[] _photos;
        private readonly string[] _completedMissionIds;
        private readonly string[] _processedEconomyTransactionIds;
        private readonly EconomyLedgerEntry[] _economyLedger;
        private readonly MissionProgress[] _missions;
        private readonly string[] _processedMissionFactIds;

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
            : this(stars, worldIds, discoveries, processedDiscoveryGrantIds, photos, completedMissionIds,
                preferences, Array.Empty<string>(), Array.Empty<EconomyLedgerEntry>())
        {
        }

        public PlayerProgress(
            int stars,
            IEnumerable<string> worldIds,
            IEnumerable<DiscoveryProgress> discoveries,
            IEnumerable<string> processedDiscoveryGrantIds,
            IEnumerable<PhotoProgress> photos,
            IEnumerable<string> completedMissionIds,
            PlayerPreferences preferences,
            IEnumerable<string> processedEconomyTransactionIds,
            IEnumerable<EconomyLedgerEntry> economyLedger,
            IEnumerable<MissionProgress> missions = null,
            IEnumerable<string> processedMissionFactIds = null,
            long lastMissionFactSequence = 0)
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
            _processedEconomyTransactionIds = CopyAndValidateEconomyTransactionIds(processedEconomyTransactionIds);
            _economyLedger = CopyAndValidateEconomyLedger(economyLedger);
            if (_economyLedger.Any(item => !_processedEconomyTransactionIds.Contains(item.TransactionId.Value, StringComparer.Ordinal)))
                throw new ArgumentException("Economy ledger entries must reference processed transaction IDs.", nameof(economyLedger));
            Preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
            _missions = CopyAndValidateMissions(missions ?? Array.Empty<MissionProgress>());
            _processedMissionFactIds = CopyAndValidateMissionFactIds(processedMissionFactIds ?? Array.Empty<string>());
            if (lastMissionFactSequence < 0) throw new ArgumentOutOfRangeException(nameof(lastMissionFactSequence));
            LastMissionFactSequence = lastMissionFactSequence;
            if (_missions.Where(item => item.IsCompleted).Any(item => !_completedMissionIds.Contains(item.Id.Value, StringComparer.Ordinal)))
                throw new ArgumentException("Completed mission progress must appear in completed mission IDs.", nameof(missions));
        }

        public int Stars { get; }
        public IReadOnlyList<string> WorldIds => _worldIds;
        public IReadOnlyList<string> DiscoveryIds => _discoveryIds;
        public IReadOnlyList<DiscoveryProgress> Discoveries => _discoveries;
        public IReadOnlyList<string> ProcessedDiscoveryGrantIds => _processedDiscoveryGrantIds;
        public IReadOnlyList<PhotoProgress> Photos => _photos;
        public IReadOnlyList<string> CompletedMissionIds => _completedMissionIds;
        public PlayerPreferences Preferences { get; }
        public ExplorerStars Wallet => new ExplorerStars(Stars);
        public IReadOnlyList<string> ProcessedEconomyTransactionIds => _processedEconomyTransactionIds;
        public IReadOnlyList<EconomyLedgerEntry> EconomyLedger => _economyLedger;
        public IReadOnlyList<MissionProgress> Missions => _missions;
        public IReadOnlyList<string> ProcessedMissionFactIds => _processedMissionFactIds;
        public long LastMissionFactSequence { get; }

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
                Preferences,
                _processedEconomyTransactionIds,
                _economyLedger,
                _missions,
                _processedMissionFactIds,
                LastMissionFactSequence);
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
                preferences,
                _processedEconomyTransactionIds,
                _economyLedger,
                _missions,
                _processedMissionFactIds,
                LastMissionFactSequence);
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
                Preferences,
                _processedEconomyTransactionIds,
                _economyLedger,
                _missions,
                _processedMissionFactIds,
                LastMissionFactSequence);
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
                Preferences,
                _processedEconomyTransactionIds,
                _economyLedger,
                _missions,
                _processedMissionFactIds,
                LastMissionFactSequence);
        }

        public PlayerProgress WithEconomy(ExplorerStars balance, IEnumerable<string> processedTransactionIds,
            IEnumerable<EconomyLedgerEntry> ledger) => new PlayerProgress(
            balance.Value, _worldIds, _discoveries, _processedDiscoveryGrantIds, _photos,
            _completedMissionIds, Preferences, processedTransactionIds, ledger,
            _missions, _processedMissionFactIds, LastMissionFactSequence);

        public PlayerProgress WithMissionState(IEnumerable<MissionProgress> missions,
            IEnumerable<string> completedMissionIds, IEnumerable<string> processedFactIds, long lastFactSequence) =>
            new PlayerProgress(Stars, _worldIds, _discoveries, _processedDiscoveryGrantIds, _photos,
                completedMissionIds, Preferences, _processedEconomyTransactionIds, _economyLedger,
                missions, processedFactIds, lastFactSequence);

        private static MissionProgress[] CopyAndValidateMissions(IEnumerable<MissionProgress> values)
        {
            MissionProgress[] result = (values ?? throw new ArgumentNullException(nameof(values))).ToArray();
            if (result.Any(item => item == null)) throw new ArgumentException("Mission progress cannot contain null.", nameof(values));
            if (result.Select(item => item.Id).Distinct().Count() != result.Length)
                throw new ArgumentException("Mission progress IDs must be unique.", nameof(values));
            return result.OrderBy(item => item.Id.Value, StringComparer.Ordinal).ToArray();
        }

        private static string[] CopyAndValidateMissionFactIds(IEnumerable<string> values)
        {
            string[] result = CopyAndValidateIds(values, nameof(values));
            if (result.Any(value => !GameplayFactId.TryParse(value, out _)))
                throw new ArgumentException("Mission fact IDs are invalid.", nameof(values));
            return result.OrderBy(value => value, StringComparer.Ordinal).ToArray();
        }

        private static string[] CopyAndValidateEconomyTransactionIds(IEnumerable<string> values)
        {
            string[] result = CopyAndValidateIds(values, nameof(values));
            if (result.Any(value => !EconomyTransactionId.TryParse(value, out _)))
                throw new ArgumentException("Economy transaction IDs are invalid.", nameof(values));
            return result.OrderBy(value => value, StringComparer.Ordinal).ToArray();
        }

        private static EconomyLedgerEntry[] CopyAndValidateEconomyLedger(IEnumerable<EconomyLedgerEntry> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            EconomyLedgerEntry[] result = values.ToArray();
            if (result.Any(item => item == null)) throw new ArgumentException("Economy ledger cannot contain null entries.", nameof(values));
            if (result.Length > EconomyLedgerMaximumEntries) throw new ArgumentException("Economy ledger exceeds its bounded capacity.", nameof(values));
            if (result.Select(item => item.TransactionId).Distinct().Count() != result.Length)
                throw new ArgumentException("Economy ledger transaction IDs must be unique.", nameof(values));
            return result;
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
