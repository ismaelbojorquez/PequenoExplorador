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
            PlayerProgressV5Dto payload = PlayerProgressV5Dto.Create(
                appVersion,
                progress.Stars,
                progress.WorldIds.ToArray(),
                progress.Discoveries.Select(item => DiscoveryProgressV4Dto.Create(
                    item.Id.Value,
                    item.Count,
                    item.FirstObservedLocalDate)).ToArray(),
                progress.ProcessedDiscoveryGrantIds.ToArray(),
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
            PlayerProgressV5Dto dto;
            try
            {
                dto = JsonUtility.FromJson<PlayerProgressV5Dto>(payload);
            }
            catch (Exception exception)
            {
                throw new SaveDataException("SavePayloadInvalid", exception);
            }

            if (dto == null || string.IsNullOrWhiteSpace(dto.AppVersion) || dto.Stars < 0 ||
                dto.WorldIds == null || dto.Discoveries == null || dto.ProcessedDiscoveryGrantIds == null ||
                dto.CompletedMissionIds == null ||
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
                var progress = new PlayerProgress(
                    dto.Stars,
                    dto.WorldIds,
                    discoveries,
                    dto.ProcessedDiscoveryGrantIds,
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
