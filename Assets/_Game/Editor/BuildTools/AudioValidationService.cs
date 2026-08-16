using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Content.Audio;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class AudioValidationService
    {
        private static readonly HashSet<string> ExpectedIds = new HashSet<string>(StringComparer.Ordinal)
        {
            "audio.music.camp", "audio.ambience.camp", "audio.feedback.confirm", "audio.feedback.retry",
            "audio.voice.instruction.explore", "audio.voice.name.jungle", "audio.voice.narration.welcome"
        };

        public static IReadOnlyList<string> Validate()
        {
            var violations = new List<string>();
            AudioCueCatalogAsset catalog = AssetDatabase.LoadAssetAtPath<AudioCueCatalogAsset>(AudioFoundationSetup.CatalogPath);
            if (catalog == null)
            {
                return new[] { "AUDIO001 missing canonical AudioCueCatalog" };
            }

            if (catalog.Mixer == null || catalog.Master == null || catalog.Music == null || catalog.Ambience == null || catalog.Effects == null || catalog.Voice == null)
            {
                violations.Add("AUDIO002 mixer and five bus references are required");
            }
            else
            {
                string[] groups = { catalog.Master.name, catalog.Music.name, catalog.Ambience.name, catalog.Effects.name, catalog.Voice.name };
                if (!groups.SequenceEqual(new[] { "Master", "Music", "Ambience", "Effects", "Voice" }))
                    violations.Add("AUDIO003 mixer groups must be Master/Music/Ambience/Effects/Voice");
            }

            AudioCueDefinition[] cues = catalog.Cues.Where(cue => cue != null).ToArray();
            if (cues.Length != 7 || !ExpectedIds.SetEquals(cues.Select(cue => cue.RawCueId)))
                violations.Add("AUDIO004 catalog must contain the seven stable baseline cue IDs exactly once");
            if (cues.Select(cue => cue.RawCueId).Distinct(StringComparer.Ordinal).Count() != cues.Length)
                violations.Add("AUDIO005 duplicate cue ID");

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            foreach (AudioCueDefinition cue in cues)
            {
                ValidateCue(cue, settings, violations);
            }

            string[] audioFiles = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/_Game" })
                .Select(AssetDatabase.GUIDToAssetPath).OrderBy(path => path, StringComparer.Ordinal).ToArray();
            if (audioFiles.Length != 10 || audioFiles.Any(path => !path.StartsWith(AudioFoundationSetup.ClipRoot + "/PH_", StringComparison.Ordinal)))
                violations.Add("AUDIO006 only ten PH_ baseline clips may exist under the audio placeholder root");

            return violations;
        }

        private static void ValidateCue(AudioCueDefinition cue, AddressableAssetSettings settings, ICollection<string> violations)
        {
            if (!cue.IsPlaceholder || string.IsNullOrWhiteSpace(cue.PlaceholderId) || !cue.PlaceholderId.StartsWith("PH_", StringComparison.Ordinal) || cue.ReleaseState != "ReleaseBlocked")
                violations.Add("AUDIO007 placeholder metadata/release block invalid: " + cue.RawCueId);
            if (cue.Gain <= 0f || cue.Gain > .4f || cue.CooldownSeconds < 0f)
                violations.Add("AUDIO008 gain/cooldown outside child-friendly baseline: " + cue.RawCueId);
            if ((cue.Category == AudioCueCategory.Music) != (cue.Bus == AudioBus.Music) ||
                (cue.Category == AudioCueCategory.Ambience) != (cue.Bus == AudioBus.Ambience) ||
                (IsVoice(cue.Category) != (cue.Bus == AudioBus.Voice)))
                violations.Add("AUDIO009 category/bus mismatch: " + cue.RawCueId);
            if (IsVoice(cue.Category) && !cue.HasSubtitle)
                violations.Add("AUDIO010 every voice cue requires a localized subtitle key: " + cue.RawCueId);
            if ((cue.Category == AudioCueCategory.Music || cue.Category == AudioCueCategory.Ambience) != cue.Loop)
                violations.Add("AUDIO011 only music/ambience baseline cues loop: " + cue.RawCueId);

            ValidateClip(cue.SpanishClip, cue.SpanishAddress, settings, cue.RawCueId + ":es", violations);
            ValidateClip(cue.EnglishClip, cue.EnglishAddress, settings, cue.RawCueId + ":en", violations);
        }

        private static void ValidateClip(AudioClip clip, string address, AddressableAssetSettings settings, string context, ICollection<string> violations)
        {
            if (clip == null)
            {
                violations.Add("AUDIO012 missing localized clip: " + context);
                return;
            }
            if (clip.frequency != 48000 || clip.channels != 1)
                violations.Add("AUDIO013 clip must import mono/48kHz: " + context);
            float[] samples = new float[Math.Min(clip.samples, 96000)];
            if (!clip.GetData(samples, 0) || samples.Any(sample => Mathf.Abs(sample) >= .95f))
                violations.Add("AUDIO014 unreadable or clipping clip: " + context);
            if (string.IsNullOrWhiteSpace(address))
            {
                violations.Add("AUDIO015 missing stable address: " + context);
                return;
            }

            string path = AssetDatabase.GetAssetPath(clip);
            AddressableAssetEntry entry = settings?.FindAssetEntry(AssetDatabase.AssetPathToGUID(path));
            if (entry == null || entry.address != address || !entry.labels.Contains(AudioFoundationSetup.AudioLabel) || !entry.labels.Contains(AudioFoundationSetup.PlaceholderLabel))
                violations.Add("AUDIO016 address/labels mismatch: " + context);
        }

        private static bool IsVoice(AudioCueCategory category) =>
            category == AudioCueCategory.VoiceName || category == AudioCueCategory.VoiceInstruction || category == AudioCueCategory.Narration;
    }
}
