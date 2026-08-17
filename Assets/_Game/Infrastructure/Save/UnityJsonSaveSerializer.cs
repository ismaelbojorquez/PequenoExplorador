using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Save
{
    internal sealed class UnityJsonSaveSerializer
    {
        public string Serialize(PlayerProgress progress, string appVersion, int saveSequence)
        {
            if (progress == null)
            {
                throw new ArgumentNullException(nameof(progress));
            }

            if (string.IsNullOrWhiteSpace(appVersion))
            {
                throw new ArgumentException("App version is required.", nameof(appVersion));
            }

            PlayerPreferences preferences = progress.Preferences;
            PlayerProgressV9Dto payload = PlayerProgressV9Dto.Create(
                appVersion,
                progress.Stars,
                progress.WorldIds.ToArray(),
                progress.Discoveries.Select(item => DiscoveryProgressV4Dto.Create(
                    item.Id.Value,
                    item.Count,
                    item.FirstObservedLocalDate)).ToArray(),
                progress.ProcessedDiscoveryGrantIds.ToArray(),
                progress.Photos.Select(item => PhotoProgressV6Dto.Create(
                    item.DiscoveryId.Value, item.FileReference, item.ScorePermille,
                    item.Width, item.Height, item.ByteLength)).ToArray(),
                progress.CompletedMissionIds.ToArray(),
                PlayerPreferencesV3Dto.Create(
                    (int)preferences.GuidanceMode,
                    ToLocaleCode(preferences.Language),
                    preferences.MasterVolume,
                    preferences.MusicVolume,
                    preferences.AmbienceVolume,
                    preferences.EffectsVolume,
                    preferences.VoiceVolume,
                    preferences.SubtitlesEnabled),
                progress.ProcessedEconomyTransactionIds.ToArray(),
                progress.EconomyLedger.Select(item => EconomyLedgerEntryV7Dto.Create(
                    item.TransactionId.Value, (int)item.Kind, item.RewardId.Value,
                    item.Amount.Value, item.BalanceAfter.Value)).ToArray(),
                progress.Missions.Select(item => MissionProgressV8Dto.Create(
                    item.Id.Value, (int)item.Status, item.ActivationSequence,
                    item.Objectives.Select(objective => MissionObjectiveProgressV8Dto.Create(
                        objective.Id.Value, objective.Count)).ToArray())).ToArray(),
                progress.ProcessedMissionFactIds.ToArray(),
                progress.LastMissionFactSequence,
                progress.LearningSessions.Select(item => LearningSessionV9Dto.Create(
                    item.ActivityId.Value, (int)item.Status, item.Attempts, item.HintLevel)).ToArray(),
                progress.LearningConcepts.Select(item => LearningConceptDailyV9Dto.Create(
                    item.ConceptId.Value, item.LocalDate, item.SeenCount, item.CompletedCount)).ToArray(),
                SaveMetadataV1Dto.Create(saveSequence));
            string payloadJson = JsonUtility.ToJson(payload, false);
            return SerializeEnvelope(LocalSaveService.CurrentSchemaVersion, payloadJson);
        }

        public string SerializeEnvelope(int schemaVersion, string payload)
        {
            if (schemaVersion < 0 || payload == null)
            {
                throw new ArgumentOutOfRangeException(nameof(schemaVersion));
            }

            SaveEnvelopeDto envelope = SaveEnvelopeDto.Create(
                schemaVersion,
                ComputeChecksum(payload),
                payload);
            return JsonUtility.ToJson(envelope, false);
        }

        public SaveEnvelopeData DeserializeEnvelope(string serialized)
        {
            if (string.IsNullOrWhiteSpace(serialized))
            {
                throw new SaveDataException("SaveEmpty");
            }

            SaveEnvelopeDto envelope;
            try
            {
                envelope = JsonUtility.FromJson<SaveEnvelopeDto>(serialized);
            }
            catch (Exception exception)
            {
                throw new SaveDataException("SaveEnvelopeInvalid", exception);
            }

            if (envelope == null || envelope.SchemaVersion < 0 ||
                string.IsNullOrWhiteSpace(envelope.Checksum) || envelope.Payload == null)
            {
                throw new SaveDataException("SaveEnvelopeInvalid");
            }

            return new SaveEnvelopeData(envelope.SchemaVersion, envelope.Checksum, envelope.Payload);
        }

        public void ValidateChecksum(SaveEnvelopeData envelope)
        {
            string actual = ComputeChecksum(envelope.Payload);
            if (!string.Equals(actual, envelope.Checksum, StringComparison.OrdinalIgnoreCase))
            {
                throw new SaveDataException("SaveChecksumMismatch");
            }
        }

        public DecodedSaveData DeserializeCurrentPayload(string payload)
        {
            PlayerProgressV9Dto dto;
            try
            {
                dto = JsonUtility.FromJson<PlayerProgressV9Dto>(payload);
            }
            catch (Exception exception)
            {
                throw new SaveDataException("SavePayloadInvalid", exception);
            }

            if (dto == null || string.IsNullOrWhiteSpace(dto.AppVersion) || dto.Stars < 0 ||
                dto.WorldIds == null || dto.Discoveries == null || dto.ProcessedDiscoveryGrantIds == null || dto.Photos == null ||
                dto.CompletedMissionIds == null || dto.ProcessedEconomyTransactionIds == null || dto.EconomyLedger == null ||
                dto.Missions == null || dto.ProcessedMissionFactIds == null || dto.LastMissionFactSequence < 0 ||
                dto.LearningSessions == null || dto.LearningConcepts == null ||
                dto.Settings == null || dto.Metadata == null || dto.Metadata.SaveSequence < 0 ||
                !Enum.IsDefined(typeof(GuidanceMode), dto.Settings.GuidanceMode) ||
                !LocaleCode.IsSupported(dto.Settings.LocaleCode, includePseudo: false) ||
                !IsVolume(dto.Settings.MasterVolume) || !IsVolume(dto.Settings.MusicVolume) ||
                !IsVolume(dto.Settings.AmbienceVolume) || !IsVolume(dto.Settings.EffectsVolume) ||
                !IsVolume(dto.Settings.VoiceVolume))
            {
                throw new SaveDataException("SavePayloadInvalid");
            }

            try
            {
                var preferences = new PlayerPreferences(
                    (GuidanceMode)dto.Settings.GuidanceMode,
                    dto.Settings.MusicVolume > 0f,
                    dto.Settings.EffectsVolume > 0f,
                    dto.Settings.VoiceVolume > 0f,
                    ToLanguagePreference(dto.Settings.LocaleCode),
                    dto.Settings.MasterVolume,
                    dto.Settings.MusicVolume,
                    dto.Settings.AmbienceVolume,
                    dto.Settings.EffectsVolume,
                    dto.Settings.VoiceVolume,
                    dto.Settings.SubtitlesEnabled);
                var discoveries = dto.Discoveries.Select(item =>
                {
                    if (item == null) throw new SaveDataException("SavePayloadInvalid");
                    return new DiscoveryProgress(
                        PequenoExplorador.Domain.Content.DiscoveryId.Parse(item.Id),
                        item.Count,
                        item.FirstObservedLocalDate);
                }).ToArray();
                var photos = dto.Photos.Select(item =>
                {
                    if (item == null) throw new SaveDataException("SavePayloadInvalid");
                    return new PhotoProgress(
                        PequenoExplorador.Domain.Content.DiscoveryId.Parse(item.DiscoveryId),
                        item.FileReference,
                        item.ScorePermille,
                        item.Width,
                        item.Height,
                        item.ByteLength);
                }).ToArray();
                var ledger = dto.EconomyLedger.Select(item =>
                {
                    if (item == null || item.Amount < 0 || item.BalanceAfter < 0 ||
                        !Enum.IsDefined(typeof(EconomyTransactionKind), item.Kind))
                        throw new SaveDataException("SavePayloadInvalid");
                    return new EconomyLedgerEntry(EconomyTransactionId.Parse(item.TransactionId),
                        (EconomyTransactionKind)item.Kind, RewardId.Parse(item.RewardId),
                        new ExplorerStars(item.Amount), new ExplorerStars(item.BalanceAfter));
                }).ToArray();
                var missions = dto.Missions.Select(item =>
                {
                    if (item == null || item.Objectives == null || item.ActivationSequence < 0 ||
                        !Enum.IsDefined(typeof(MissionProgressStatus), item.Status))
                        throw new SaveDataException("SavePayloadInvalid");
                    return new MissionProgress(MissionId.Parse(item.Id), (MissionProgressStatus)item.Status,
                        item.ActivationSequence, item.Objectives.Select(objective =>
                        {
                            if (objective == null || objective.Count < 0) throw new SaveDataException("SavePayloadInvalid");
                            return new MissionObjectiveProgress(MissionObjectiveId.Parse(objective.Id), objective.Count);
                        }));
                }).ToArray();
                var learningSessions = dto.LearningSessions.Select(item =>
                {
                    if (item == null || item.Attempts < 0 || item.HintLevel < 0 ||
                        !Enum.IsDefined(typeof(LearningSessionStatus), item.Status)) throw new SaveDataException("SavePayloadInvalid");
                    return new LearningSession(ActivityId.Parse(item.ActivityId), (LearningSessionStatus)item.Status,
                        item.Attempts, item.HintLevel);
                }).ToArray();
                var learningConcepts = dto.LearningConcepts.Select(item =>
                {
                    if (item == null) throw new SaveDataException("SavePayloadInvalid");
                    return new LearningConceptDailyProgress(LearningConceptId.Parse(item.ConceptId), item.LocalDate,
                        item.SeenCount, item.CompletedCount);
                }).ToArray();
                var progress = new PlayerProgress(
                    dto.Stars,
                    dto.WorldIds,
                    discoveries,
                    dto.ProcessedDiscoveryGrantIds,
                    photos,
                    dto.CompletedMissionIds,
                    preferences,
                    dto.ProcessedEconomyTransactionIds,
                    ledger,
                    missions,
                    dto.ProcessedMissionFactIds,
                    dto.LastMissionFactSequence,
                    learningSessions,
                    learningConcepts);
                return new DecodedSaveData(progress, dto.Metadata.SaveSequence);
            }
            catch (Exception exception)
            {
                throw new SaveDataException("SavePayloadInvalid", exception);
            }
        }

        private static string ComputeChecksum(string payload)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] digest = sha256.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var builder = new StringBuilder(digest.Length * 2);
            foreach (byte value in digest)
            {
                builder.Append(value.ToString("x2"));
            }

            return builder.ToString();
        }

        private static bool IsVolume(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && value >= 0f && value <= 1f;
        }

        private static string ToLocaleCode(LanguagePreference language)
        {
            return language == LanguagePreference.English
                ? LocaleCode.English
                : LocaleCode.Spanish;
        }

        private static LanguagePreference ToLanguagePreference(string localeCode)
        {
            return string.Equals(localeCode, LocaleCode.English, StringComparison.Ordinal)
                ? LanguagePreference.English
                : LanguagePreference.Spanish;
        }
    }
}
