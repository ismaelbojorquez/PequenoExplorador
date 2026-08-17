using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Infrastructure.Save;
using PequenoExplorador.Tests.EditMode.Fixtures;
using UnityEngine;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class SaveServiceTests
    {
        [Test]
        public async Task FirstRunCreatesDefaultSchemaV6()
        {
            var store = new InMemoryFileStore();
            LocalSaveService service = CreateService(store);

            await service.InitializeAsync(CancellationToken.None);
            SaveEnvelopeData envelope = new UnityJsonSaveSerializer().DeserializeEnvelope(store.Primary);

            Assert.That(service.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.DefaultCreated));
            Assert.That(service.Current.Stars, Is.Zero);
            Assert.That(service.Current.WorldIds, Is.Empty);
            Assert.That(service.Current.DiscoveryIds, Is.Empty);
            Assert.That(service.Current.CompletedMissionIds, Is.Empty);
            Assert.That(envelope.SchemaVersion, Is.EqualTo(6));
            Assert.That(service.Current.Photos, Is.Empty);
            Assert.That(service.Current.Preferences.Language, Is.EqualTo(LanguagePreference.Spanish));
        }

        [Test]
        public async Task RoundTripRestoresProgressAndSettings()
        {
            var store = new InMemoryFileStore();
            LocalSaveService writer = CreateService(store);
            await writer.InitializeAsync(CancellationToken.None);
            var expected = new PlayerProgress(
                7,
                new[] { "world.jungle" },
                new[] { "discovery.test" },
                new[] { "mission.test" },
                new PlayerPreferences(GuidanceMode.MoreGuidance, false, true, false))
                .WithPhotos(new[]
                {
                    new PhotoProgress(PequenoExplorador.Domain.Content.DiscoveryId.Parse("discovery.test"),
                        "discovery_test.png", 777, 384, 216, 12345)
                });

            Assert.That((await writer.SaveAsync(expected, CancellationToken.None)).IsSuccess, Is.True);
            LocalSaveService reader = CreateService(store);
            await reader.InitializeAsync(CancellationToken.None);

            AssertProgress(reader.Current, expected);
            Assert.That(reader.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.Loaded));
        }

        [TestCase(FileStoreFailurePoint.WriteTemporary)]
        [TestCase(FileStoreFailurePoint.FlushTemporary)]
        [TestCase(FileStoreFailurePoint.CommitTemporary)]
        public async Task FailureBeforeAtomicCommitPreservesPrimaryAndBackup(FileStoreFailurePoint point)
        {
            var store = new InMemoryFileStore();
            LocalSaveService service = CreateService(store);
            await service.InitializeAsync(CancellationToken.None);
            string primaryBefore = store.Primary;
            string backupBefore = store.Backup;
            store.FailurePoint = point;

            SaveOperationResult result = await service.SaveAsync(
                service.Current.WithStars(99),
                CancellationToken.None);

            Assert.That(result.Status, Is.EqualTo(SaveOperationStatus.Failed));
            Assert.That(store.Primary, Is.EqualTo(primaryBefore));
            Assert.That(store.Backup, Is.EqualTo(backupBefore));
            Assert.That(service.Current.Stars, Is.Zero);
        }

        [Test]
        public async Task TruncatedPrimaryRecoversBackupWithoutReplacingBackupWithCorruptData()
        {
            var store = new InMemoryFileStore();
            LocalSaveService writer = CreateService(store);
            await writer.InitializeAsync(CancellationToken.None);
            await writer.SaveAsync(writer.Current.WithStars(5), CancellationToken.None);
            string validBackup = store.Backup;
            store.SeedPrimary(store.Primary.Substring(0, store.Primary.Length / 2));

            LocalSaveService reader = CreateService(store);
            await reader.InitializeAsync(CancellationToken.None);

            Assert.That(reader.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.RecoveredBackup));
            Assert.That(reader.LastLoadResult.UserNotice, Is.EqualTo(SaveUserNotice.ProgressRecovered));
            Assert.That(reader.Current.Stars, Is.Zero, "Backup contains the previous committed checkpoint.");
            Assert.That(store.Backup, Is.EqualTo(validBackup));
            Assert.That(store.Primary, Is.Not.EqualTo(validBackup), "Repair advances technical save sequence.");
        }

        [Test]
        public async Task ChecksumMismatchRecoversBackup()
        {
            var store = new InMemoryFileStore();
            LocalSaveService writer = CreateService(store);
            await writer.InitializeAsync(CancellationToken.None);
            await writer.SaveAsync(writer.Current.WithStars(5), CancellationToken.None);
            store.SeedPrimary(store.Primary.Replace("\\\"stars\\\":5", "\\\"stars\\\":6"));

            LocalSaveService reader = CreateService(store);
            await reader.InitializeAsync(CancellationToken.None);

            Assert.That(reader.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.RecoveredBackup));
            Assert.That(reader.Current.Stars, Is.Zero);
        }

        [Test]
        public void InvalidPrimaryWithoutBackupFailsRecoverablyAndPreservesEvidence()
        {
            var store = new InMemoryFileStore();
            const string invalid = "{truncated";
            store.SeedPrimary(invalid);
            LocalSaveService service = CreateService(store);

            Assert.ThrowsAsync<SaveDataException>(async () =>
                await service.InitializeAsync(CancellationToken.None));
            Assert.That(store.Primary, Is.EqualTo(invalid));
            Assert.That(store.Backup, Is.Null);
        }

        [Test]
        public async Task LegacyV0MigratesStepByStepAndPreservesOriginalAsBackup()
        {
            var store = new InMemoryFileStore();
            var serializer = new UnityJsonSaveSerializer();
            string legacy = serializer.SerializeEnvelope(
                0,
                "{\"appVersion\":\"0.0.1\",\"stars\":9}");
            store.SeedPrimary(legacy);
            LocalSaveService service = CreateService(store);

            await service.InitializeAsync(CancellationToken.None);
            SaveEnvelopeData current = serializer.DeserializeEnvelope(store.Primary);

            Assert.That(service.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.Migrated));
            Assert.That(service.LastLoadResult.SourceSchemaVersion, Is.Zero);
            Assert.That(service.Current.Stars, Is.EqualTo(9));
            Assert.That(current.SchemaVersion, Is.EqualTo(6));
            Assert.That(store.Backup, Is.EqualTo(legacy));
        }

        [Test]
        public async Task FutureVersionIsReadOnlyAndNeverOverwritten()
        {
            var store = new InMemoryFileStore();
            string future = new UnityJsonSaveSerializer().SerializeEnvelope(99, "{\"future\":true}");
            store.SeedPrimary(future);
            LocalSaveService service = CreateService(store);

            await service.InitializeAsync(CancellationToken.None);
            SaveOperationResult attempted = await service.SaveAsync(
                PlayerProgress.CreateDefault().WithStars(3),
                CancellationToken.None);

            Assert.That(service.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.FutureVersion));
            Assert.That(service.IsReadOnly, Is.True);
            Assert.That(attempted.Status, Is.EqualTo(SaveOperationStatus.BlockedByFutureVersion));
            Assert.That(store.Primary, Is.EqualTo(future));
            Assert.That(store.Backup, Is.Null);
        }

        [Test]
        public async Task SchemaV2MigratesAudioDefaultsWithoutChangingLocaleOrProgress()
        {
            var store = new InMemoryFileStore();
            var serializer = new UnityJsonSaveSerializer();
            PlayerProgressV2Dto v2 = PlayerProgressV2Dto.Create(
                "0.1.0-test", 6, new[] { "world.jungle" }, Array.Empty<string>(), Array.Empty<string>(),
                PlayerPreferencesV2Dto.Create((int)GuidanceMode.MoreGuidance, false, true, false, "en"),
                SaveMetadataV1Dto.Create(4));
            store.SeedPrimary(serializer.SerializeEnvelope(2, JsonUtility.ToJson(v2, false)));

            LocalSaveService service = CreateService(store);
            await service.InitializeAsync(CancellationToken.None);

            Assert.That(service.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.Migrated));
            Assert.That(service.Current.Stars, Is.EqualTo(6));
            Assert.That(service.Current.Preferences.Language, Is.EqualTo(LanguagePreference.English));
            Assert.That(service.Current.Preferences.MusicVolume, Is.Zero);
            Assert.That(service.Current.Preferences.AmbienceVolume, Is.Zero);
            Assert.That(service.Current.Preferences.EffectsVolume, Is.EqualTo(.75f));
            Assert.That(service.Current.Preferences.VoiceVolume, Is.Zero);
            Assert.That(service.Current.Preferences.SubtitlesEnabled, Is.True);
            Assert.That(serializer.DeserializeEnvelope(store.Primary).SchemaVersion, Is.EqualTo(6));
        }

        [Test]
        public async Task SchemaV3MigratesDiscoveryIdsToCountedRecordsWithoutInventingHistory()
        {
            var store = new InMemoryFileStore();
            var serializer = new UnityJsonSaveSerializer();
            PlayerProgressV3Dto v3 = PlayerProgressV3Dto.Create(
                "0.1.0-test",
                2,
                new[] { "world.jungle" },
                new[] { "discovery.jungle.legacy" },
                Array.Empty<string>(),
                PlayerPreferencesV3Dto.Create(
                    (int)GuidanceMode.Standard,
                    "es",
                    .85f,
                    .65f,
                    .65f,
                    .75f,
                    .85f,
                    true),
                SaveMetadataV1Dto.Create(5));
            string original = serializer.SerializeEnvelope(3, JsonUtility.ToJson(v3, false));
            store.SeedPrimary(original);

            LocalSaveService service = CreateService(store);
            await service.InitializeAsync(CancellationToken.None);

            Assert.That(service.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.Migrated));
            Assert.That(service.LastLoadResult.SourceSchemaVersion, Is.EqualTo(3));
            Assert.That(service.Current.Discoveries.Count, Is.EqualTo(1));
            Assert.That(service.Current.Discoveries[0].Id.Value, Is.EqualTo("discovery.jungle.legacy"));
            Assert.That(service.Current.Discoveries[0].Count, Is.EqualTo(1));
            Assert.That(service.Current.Discoveries[0].FirstObservedLocalDate, Is.Empty);
            Assert.That(service.Current.ProcessedDiscoveryGrantIds, Is.Empty);
            Assert.That(serializer.DeserializeEnvelope(store.Primary).SchemaVersion, Is.EqualTo(6));
            Assert.That(store.Backup, Is.EqualTo(original));
        }

        [Test]
        public async Task SchemaV4MigratesRetiredToucanIdMergesProgressAndNormalizesGrants()
        {
            var store = new InMemoryFileStore();
            var serializer = new UnityJsonSaveSerializer();
            PlayerProgressV4Dto v4 = PlayerProgressV4Dto.Create(
                "0.1.0-test",
                3,
                new[] { "world.jungle" },
                new[]
                {
                    DiscoveryProgressV4Dto.Create(V4ToV5ToucanDiscoveryMigration.RetiredDiscoveryId, 2, "2026-08-16"),
                    DiscoveryProgressV4Dto.Create(V4ToV5ToucanDiscoveryMigration.CurrentDiscoveryId, 3, "2026-08-15"),
                    DiscoveryProgressV4Dto.Create("discovery.jungle.unrelated", 1, string.Empty)
                },
                new[]
                {
                    "grant.interaction.10." + V4ToV5ToucanDiscoveryMigration.RetiredDiscoveryId,
                    "grant.interaction.10." + V4ToV5ToucanDiscoveryMigration.CurrentDiscoveryId,
                    "grant.interaction.11.discovery.jungle.unrelated"
                },
                Array.Empty<string>(),
                PlayerPreferencesV3Dto.Create((int)GuidanceMode.Standard, "es", 1f, .7f, .7f, .8f, .9f, true),
                SaveMetadataV1Dto.Create(6));
            string original = serializer.SerializeEnvelope(4, JsonUtility.ToJson(v4, false));
            store.SeedPrimary(original);

            LocalSaveService service = CreateService(store);
            await service.InitializeAsync(CancellationToken.None);

            Assert.That(service.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.Migrated));
            Assert.That(service.LastLoadResult.SourceSchemaVersion, Is.EqualTo(4));
            DiscoveryProgress toucan = service.Current.Discoveries.Single(item =>
                item.Id.Value == V4ToV5ToucanDiscoveryMigration.CurrentDiscoveryId);
            Assert.That(toucan.Count, Is.EqualTo(5));
            Assert.That(toucan.FirstObservedLocalDate, Is.EqualTo("2026-08-15"));
            Assert.That(service.Current.Discoveries.Any(item =>
                item.Id.Value == V4ToV5ToucanDiscoveryMigration.RetiredDiscoveryId), Is.False);
            Assert.That(service.Current.ProcessedDiscoveryGrantIds.Count(value => value.EndsWith(
                V4ToV5ToucanDiscoveryMigration.CurrentDiscoveryId, StringComparison.Ordinal)), Is.EqualTo(1));
            Assert.That(service.Current.ProcessedDiscoveryGrantIds, Does.Contain("grant.interaction.11.discovery.jungle.unrelated"));
            Assert.That(serializer.DeserializeEnvelope(store.Primary).SchemaVersion, Is.EqualTo(6));
            Assert.That(store.Backup, Is.EqualTo(original));
        }

        [Test]
        public async Task SchemaV5MigratesToEmptyPhotoMetadataWithoutInventingCapture()
        {
            var store = new InMemoryFileStore();
            var serializer = new UnityJsonSaveSerializer();
            PlayerProgressV5Dto v5 = PlayerProgressV5Dto.Create(
                "0.1.0-test",
                4,
                new[] { "world.jungle" },
                new[] { DiscoveryProgressV4Dto.Create(V4ToV5ToucanDiscoveryMigration.CurrentDiscoveryId, 2, "2026-08-16") },
                new[] { "grant.interaction.1." + V4ToV5ToucanDiscoveryMigration.CurrentDiscoveryId },
                Array.Empty<string>(),
                PlayerPreferencesV3Dto.Create((int)GuidanceMode.Standard, "es", 1f, .7f, .7f, .8f, .9f, true),
                SaveMetadataV1Dto.Create(7));
            string original = serializer.SerializeEnvelope(5, JsonUtility.ToJson(v5, false));
            store.SeedPrimary(original);

            LocalSaveService service = CreateService(store);
            await service.InitializeAsync(CancellationToken.None);

            Assert.That(service.LastLoadResult.Status, Is.EqualTo(SaveLoadStatus.Migrated));
            Assert.That(service.LastLoadResult.SourceSchemaVersion, Is.EqualTo(5));
            Assert.That(service.Current.Stars, Is.EqualTo(4));
            Assert.That(service.Current.Discoveries.Single().Count, Is.EqualTo(2));
            Assert.That(service.Current.Photos, Is.Empty, "Migration cannot invent a rendered thumbnail.");
            Assert.That(serializer.DeserializeEnvelope(store.Primary).SchemaVersion, Is.EqualTo(6));
            Assert.That(store.Backup, Is.EqualTo(original));
        }

        [Test]
        public async Task MissingMigrationFailsWithoutRewritingLegacyPrimary()
        {
            var store = new InMemoryFileStore();
            var serializer = new UnityJsonSaveSerializer();
            string legacy = serializer.SerializeEnvelope(
                0,
                "{\"appVersion\":\"0.0.1\",\"stars\":1}");
            store.SeedPrimary(legacy);
            var service = new LocalSaveService(
                store,
                "0.1.0-test",
                new RecordingLogger(),
                Array.Empty<ISaveMigration>());

            Assert.ThrowsAsync<SaveDataException>(async () =>
                await service.InitializeAsync(CancellationToken.None));
            Assert.That(store.Primary, Is.EqualTo(legacy));
            Assert.That(store.Backup, Is.Null);
        }

        [Test]
        public void CancellationBeforeCommitPreservesLastCommittedSave()
        {
            var store = new InMemoryFileStore();
            LocalSaveService service = CreateService(store);
            service.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
            string primaryBefore = store.Primary;
            using var cancellation = new CancellationTokenSource();
            store.BeforeFlush = cancellation.Cancel;

            Assert.CatchAsync<OperationCanceledException>(async () =>
                await service.SaveAsync(service.Current.WithStars(4), cancellation.Token));
            Assert.That(store.Primary, Is.EqualTo(primaryBefore));
            Assert.That(store.Backup, Is.Null);
        }

        [Test]
        public void SerializationIsDeterministicAndContainsNoPersonalOrRuntimeConfigurationFields()
        {
            var serializer = new UnityJsonSaveSerializer();
            PlayerProgress progress = PlayerProgress.CreateDefault().WithStars(2);

            string first = serializer.Serialize(progress, "0.1.0-test", 4);
            string second = serializer.Serialize(progress, "0.1.0-test", 4);

            Assert.That(first, Is.EqualTo(second));
            Assert.That(first, Does.Not.Contain("name"));
            Assert.That(first, Does.Not.Contain("birth"));
            Assert.That(first, Does.Not.Contain("age"));
            Assert.That(first, Does.Not.Contain("device"));
            Assert.That(first, Does.Not.Contain("location"));
            Assert.That(first, Does.Not.Contain("buildProfile"));
            Assert.That(first, Does.Not.Contain("featureFlag"));
            Assert.That(first, Does.Not.Contain("randomSeed"));
            Assert.That(first, Does.Not.Contain("sceneTransitionTimeout"));
            Assert.That(first, Does.Not.Contain("autosaveDebounce"));
            Assert.That(first, Does.Not.Contain("pngBytes"));
        }

        [Test]
        public async Task ResetCreatesFreshDefaultAndRemovesOldBackup()
        {
            var store = new InMemoryFileStore();
            LocalSaveService service = CreateService(store);
            await service.InitializeAsync(CancellationToken.None);
            await service.SaveAsync(service.Current.WithStars(8), CancellationToken.None);

            SaveOperationResult reset = await service.ResetAsync(CancellationToken.None);

            Assert.That(reset.IsSuccess, Is.True);
            Assert.That(service.Current.Stars, Is.Zero);
            Assert.That(store.Primary, Is.Not.Null);
            Assert.That(store.Backup, Is.Null);
        }

        [Test]
        public async Task LocalFileStoreUsesOnlyKnownPortableNamesAndSupportsRepeatedReplace()
        {
            string parent = Path.Combine(Path.GetTempPath(), "PequenoExploradorSaveTests");
            string directory = Path.Combine(parent, Guid.NewGuid().ToString("N"));
            try
            {
                using var store = new LocalFileStore(directory);
                LocalSaveService service = CreateService(store);
                await service.InitializeAsync(CancellationToken.None);
                await service.SaveAsync(service.Current.WithStars(1), CancellationToken.None);
                await service.SaveAsync(service.Current.WithStars(2), CancellationToken.None);

                Assert.That(File.Exists(Path.Combine(directory, LocalFileStore.PrimaryFileName)), Is.True);
                Assert.That(File.Exists(Path.Combine(directory, LocalFileStore.BackupFileName)), Is.True);
                Assert.That(File.Exists(Path.Combine(directory, LocalFileStore.TemporaryFileName)), Is.False);
            }
            finally
            {
                string fullParent = Path.GetFullPath(parent) + Path.DirectorySeparatorChar;
                string fullDirectory = Path.GetFullPath(directory);
                if (fullDirectory.StartsWith(fullParent, StringComparison.Ordinal) && Directory.Exists(fullDirectory))
                {
                    Directory.Delete(fullDirectory, true);
                }
            }
        }

        private static LocalSaveService CreateService(IFileStore store)
        {
            return new LocalSaveService(
                store,
                "0.1.0-test",
                new RecordingLogger(),
                new ISaveMigration[]
                {
                    new LegacyV0ToV1Migration(),
                    new V1ToV2LocalizationMigration(),
                    new V2ToV3AudioMigration(),
                    new V3ToV4DiscoveryMigration(),
                    new V4ToV5ToucanDiscoveryMigration(),
                    new V5ToV6PhotoProgressMigration()
                });
        }

        private static void AssertProgress(PlayerProgress actual, PlayerProgress expected)
        {
            Assert.That(actual.Stars, Is.EqualTo(expected.Stars));
            Assert.That(actual.WorldIds, Is.EqualTo(expected.WorldIds));
            Assert.That(actual.DiscoveryIds, Is.EqualTo(expected.DiscoveryIds));
            Assert.That(actual.CompletedMissionIds, Is.EqualTo(expected.CompletedMissionIds));
            Assert.That(actual.Photos.Select(item => new { Id = item.DiscoveryId.Value, item.FileReference,
                    item.ScorePermille, item.Width, item.Height, item.ByteLength }),
                Is.EqualTo(expected.Photos.Select(item => new { Id = item.DiscoveryId.Value, item.FileReference,
                    item.ScorePermille, item.Width, item.Height, item.ByteLength })));
            Assert.That(actual.Preferences.GuidanceMode, Is.EqualTo(expected.Preferences.GuidanceMode));
            Assert.That(actual.Preferences.MusicEnabled, Is.EqualTo(expected.Preferences.MusicEnabled));
            Assert.That(actual.Preferences.SoundEffectsEnabled, Is.EqualTo(expected.Preferences.SoundEffectsEnabled));
            Assert.That(actual.Preferences.NarrationEnabled, Is.EqualTo(expected.Preferences.NarrationEnabled));
            Assert.That(actual.Preferences.Language, Is.EqualTo(expected.Preferences.Language));
            Assert.That(actual.Preferences.MasterVolume, Is.EqualTo(expected.Preferences.MasterVolume));
            Assert.That(actual.Preferences.MusicVolume, Is.EqualTo(expected.Preferences.MusicVolume));
            Assert.That(actual.Preferences.AmbienceVolume, Is.EqualTo(expected.Preferences.AmbienceVolume));
            Assert.That(actual.Preferences.EffectsVolume, Is.EqualTo(expected.Preferences.EffectsVolume));
            Assert.That(actual.Preferences.VoiceVolume, Is.EqualTo(expected.Preferences.VoiceVolume));
            Assert.That(actual.Preferences.SubtitlesEnabled, Is.EqualTo(expected.Preferences.SubtitlesEnabled));
        }
    }
}
