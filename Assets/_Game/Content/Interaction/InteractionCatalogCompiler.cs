using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Content.Interaction
{
    public static class InteractionCatalogCompiler
    {
        private static readonly string[] RequiredFixtureIds =
        {
            "interaction.fixture.animal",
            "interaction.fixture.plant",
            "interaction.fixture.object"
        };

        public static bool TryCompile(
            InteractionCatalogAsset source,
            ContentValidationMode mode,
            IContentReferenceResolver resolver,
            out InteractionCatalog catalog,
            out IReadOnlyList<string> violations)
        {
            var errors = new List<string>();
            var definitions = new List<InteractionDefinition>();
            var ids = new HashSet<InteractionId>();
            catalog = null;
            if (source == null)
            {
                violations = new[] { "INTERACTION001 missing InteractionCatalogAsset; create and wire the canonical catalog." };
                return false;
            }

            int slot = 0;
            foreach (InteractionDefinitionAsset asset in source.Definitions)
            {
                if (asset == null)
                {
                    errors.Add($"INTERACTION002 missing definition in slot {slot}; assign or remove the slot.");
                    slot++;
                    continue;
                }
                slot++;
                string path = resolver?.Describe(asset) ?? asset.name;
                try
                {
                    InteractionId id = InteractionId.Parse(asset.RawId);
                    if (!ids.Add(id))
                    {
                        errors.Add($"INTERACTION003 duplicate ID '{id}' at {path}; keep one stable definition.");
                        continue;
                    }
                    var display = new LocalizedKey(asset.DisplayNameTable, asset.DisplayNameKey);
                    var prompt = new LocalizedKey(asset.PromptTable, asset.PromptKey);
                    var unavailable = new LocalizedKey(asset.UnavailableTable, asset.UnavailableKey);
                    var promptAudio = new AudioCueId(asset.PromptAudioCueId);
                    var unavailableAudio = new AudioCueId(asset.UnavailableAudioCueId);
                    ValidateReference(resolver, display, path, errors);
                    ValidateReference(resolver, prompt, path, errors);
                    ValidateReference(resolver, unavailable, path, errors);
                    ValidateReference(resolver, promptAudio, path, errors);
                    ValidateReference(resolver, unavailableAudio, path, errors);
                    if (asset.Editorial == null)
                    {
                        errors.Add($"INTERACTION004 missing editorial metadata at {path}; assign owner/watermark.");
                        continue;
                    }
                    EditorialMetadata editorial = asset.Editorial.ToRuntime();
                    DiscoveryId directDiscovery = string.IsNullOrWhiteSpace(asset.DirectDiscoveryId)
                        ? default
                        : DiscoveryId.Parse(asset.DirectDiscoveryId);
                    if (directDiscovery.IsValid && resolver != null && !resolver.HasDiscovery(directDiscovery))
                        errors.Add($"INTERACTION011 missing discovery '{directDiscovery}' referenced by {path}; add it to the canonical content catalog or clear the optional action.");
                    if (mode == ContentValidationMode.Release && !editorial.IsReleaseApproved)
                        errors.Add($"INTERACTION005 Release rejects {editorial.State} or placeholder '{id}' at {path}; replace and approve it.");
                    definitions.Add(new InteractionDefinition(
                        id,
                        display,
                        prompt,
                        unavailable,
                        promptAudio,
                        unavailableAudio,
                        asset.InteractionRange,
                        asset.CooldownSeconds,
                        asset.Priority,
                        directDiscovery,
                        editorial));
                }
                catch (Exception exception) when (
                    exception is FormatException ||
                    exception is ArgumentException ||
                    exception is ArgumentOutOfRangeException)
                {
                    errors.Add($"INTERACTION006 invalid definition at {path}: {exception.Message}");
                }
            }

            foreach (string required in RequiredFixtureIds)
            {
                if (!ids.Contains(InteractionId.Parse(required)))
                    errors.Add($"INTERACTION007 Prompt 17 baseline is missing required neutral fixture '{required}'.");
            }
            if (errors.Count == 0)
            {
                try { catalog = new InteractionCatalog(definitions.OrderBy(item => item.Id.Value, StringComparer.Ordinal)); }
                catch (ArgumentException exception) { errors.Add("INTERACTION008 catalog index failed: " + exception.Message); }
            }
            violations = new ReadOnlyCollection<string>(errors);
            return errors.Count == 0;
        }

        private static void ValidateReference(
            IContentReferenceResolver resolver,
            LocalizedKey key,
            string path,
            ICollection<string> errors)
        {
            if (resolver != null && !resolver.HasLocalization(key))
                errors.Add($"INTERACTION009 missing ES/EN localization '{key}' referenced by {path}.");
        }

        private static void ValidateReference(
            IContentReferenceResolver resolver,
            AudioCueId cue,
            string path,
            ICollection<string> errors)
        {
            if (resolver != null && !resolver.HasAudioCue(cue))
                errors.Add($"INTERACTION010 missing audio cue '{cue}' referenced by {path}.");
        }
    }
}
