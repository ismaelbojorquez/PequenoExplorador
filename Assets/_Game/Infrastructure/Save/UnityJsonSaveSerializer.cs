using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Progress;
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
            PlayerProgressV2Dto payload = PlayerProgressV2Dto.Create(
                appVersion,
                progress.Stars,
                progress.WorldIds.ToArray(),
                progress.DiscoveryIds.ToArray(),
                progress.CompletedMissionIds.ToArray(),
                PlayerPreferencesV2Dto.Create(
                    (int)preferences.GuidanceMode,
                    preferences.MusicEnabled,
                    preferences.SoundEffectsEnabled,
                    preferences.NarrationEnabled,
                    ToLocaleCode(preferences.Language)),
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
            PlayerProgressV2Dto dto;
            try
            {
                dto = JsonUtility.FromJson<PlayerProgressV2Dto>(payload);
            }
            catch (Exception exception)
            {
                throw new SaveDataException("SavePayloadInvalid", exception);
            }

            if (dto == null || string.IsNullOrWhiteSpace(dto.AppVersion) || dto.Stars < 0 ||
                dto.WorldIds == null || dto.DiscoveryIds == null || dto.CompletedMissionIds == null ||
                dto.Settings == null || dto.Metadata == null || dto.Metadata.SaveSequence < 0 ||
                !Enum.IsDefined(typeof(GuidanceMode), dto.Settings.GuidanceMode) ||
                !LocaleCode.IsSupported(dto.Settings.LocaleCode, includePseudo: false))
            {
                throw new SaveDataException("SavePayloadInvalid");
            }

            try
            {
                var preferences = new PlayerPreferences(
                    (GuidanceMode)dto.Settings.GuidanceMode,
                    dto.Settings.MusicEnabled,
                    dto.Settings.SoundEffectsEnabled,
                    dto.Settings.NarrationEnabled,
                    ToLanguagePreference(dto.Settings.LocaleCode));
                var progress = new PlayerProgress(
                    dto.Stars,
                    dto.WorldIds,
                    dto.DiscoveryIds,
                    dto.CompletedMissionIds,
                    preferences);
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
