using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Content.Data
{
    public static class ContentCatalogCompiler
    {
        public static bool TryCompile(
            ContentCatalogAsset source,
            ContentValidationMode mode,
            IContentReferenceResolver resolver,
            out ContentCatalog catalog,
            out IReadOnlyList<string> violations)
        {
            var errors = new List<string>();
            catalog = null;
            if (source == null)
            {
                violations = new[] { "DATA001 missing ContentCatalogAsset; create and wire the canonical catalog." };
                return false;
            }

            ValidateSlots(source.Categories, "category", errors);
            ValidateSlots(source.Tags, "tag", errors);
            ValidateSlots(source.Sources, "source", errors);
            ValidateSlots(source.Facts, "fact", errors);
            ValidateSlots(source.Discoveries, "discovery", errors);

            CategoryDefinition[] categories = MapUnique(
                source.Categories, asset => CategoryId.Parse(asset.RawId),
                (asset, id) => new CategoryDefinition(id, MapEditorial(asset, mode, resolver, errors)),
                "category", resolver, errors);
            TagDefinition[] tags = MapUnique(
                source.Tags, asset => TagId.Parse(asset.RawId),
                (asset, id) => new TagDefinition(id, MapEditorial(asset, mode, resolver, errors)),
                "tag", resolver, errors);
            ContentSourceRecord[] sources = MapUnique(
                source.Sources, asset => ContentSourceId.Parse(asset.RawId),
                (asset, id) => new ContentSourceRecord(
                    id, asset.Institution, asset.Author, asset.Title, asset.Reference,
                    asset.ConsultedOn, asset.Reviewer, MapEditorial(asset, mode, resolver, errors)),
                "source", resolver, errors);
            ValidateSourceDetails(source.Sources, resolver, errors);

            var sourceIds = new HashSet<ContentSourceId>(sources.Select(item => item.Id));
            EducationalFactDefinition[] facts = MapFacts(source.Facts, sourceIds, mode, resolver, errors);
            var categoryIds = new HashSet<CategoryId>(categories.Select(item => item.Id));
            var tagIds = new HashSet<TagId>(tags.Select(item => item.Id));
            var factIds = new HashSet<EducationalFactId>(facts.Select(item => item.Id));
            DiscoveryDefinition[] discoveries = MapDiscoveries(
                source.Discoveries, categoryIds, tagIds, factIds, mode, resolver, errors);
            DiscoveryIdAlias[] aliases = MapAliases(source.DiscoveryAliases, discoveries, resolver, errors);

            if (errors.Count == 0)
            {
                try { catalog = new ContentCatalog(ContentCatalogId.Parse(source.RawId), categories, tags, sources, facts, discoveries, aliases); }
                catch (ArgumentException exception) { errors.Add("DATA002 catalog index failed: " + exception.Message); }
            }

            violations = new ReadOnlyCollection<string>(errors);
            return errors.Count == 0;
        }

        private static TModel[] MapUnique<TAsset, TId, TModel>(
            IEnumerable<TAsset> assets,
            Func<TAsset, TId> parse,
            Func<TAsset, TId, TModel> map,
            string kind,
            IContentReferenceResolver resolver,
            ICollection<string> errors)
            where TAsset : ContentDefinitionAsset
        {
            var result = new List<TModel>();
            var ids = new HashSet<TId>();
            foreach (TAsset asset in Ordered(assets))
            {
                string path = Describe(asset, resolver);
                try
                {
                    TId id = parse(asset);
                    if (!ids.Add(id)) { errors.Add($"DATA003 duplicate {kind} ID '{asset.RawId}' at {path}; assign a unique stable ID or alias."); continue; }
                    result.Add(map(asset, id));
                }
                catch (Exception exception) when (exception is FormatException || exception is ArgumentException)
                {
                    errors.Add($"DATA004 invalid {kind} at {path}: {exception.Message}");
                }
            }
            return result.ToArray();
        }

        private static EducationalFactDefinition[] MapFacts(
            IEnumerable<EducationalFactDefinitionAsset> assets,
            ISet<ContentSourceId> knownSources,
            ContentValidationMode mode,
            IContentReferenceResolver resolver,
            ICollection<string> errors)
        {
            var result = new List<EducationalFactDefinition>();
            var ids = new HashSet<EducationalFactId>();
            foreach (EducationalFactDefinitionAsset asset in Ordered(assets))
            {
                string path = Describe(asset, resolver);
                try
                {
                    EducationalFactId id = EducationalFactId.Parse(asset.RawId);
                    if (!ids.Add(id)) { errors.Add($"DATA005 duplicate fact ID '{id}' at {path}; keep one definition."); continue; }
                    var sourceIds = new List<ContentSourceId>();
                    foreach (ContentSourceRecordAsset source in asset.Sources)
                    {
                        if (source == null) { errors.Add($"DATA006 missing source reference at {path}; assign a ContentSourceRecord."); continue; }
                        ContentSourceId sourceId = ContentSourceId.Parse(source.RawId);
                        if (!knownSources.Contains(sourceId)) errors.Add($"DATA007 source '{sourceId}' referenced by {path} is absent from the catalog.");
                        sourceIds.Add(sourceId);
                    }
                    var key = new LocalizedKey(asset.ChildCopyTable, asset.ChildCopyKey);
                    if (resolver != null && !resolver.HasLocalization(key)) errors.Add($"DATA008 missing localization '{key}' referenced by {path}; add ES/EN entries.");
                    EditorialMetadata editorial = MapEditorial(asset, mode, resolver, errors);
                    if (editorial.State >= EditorialState.Sourced &&
                        (sourceIds.Count == 0 || string.IsNullOrWhiteSpace(asset.ClaimForReview)))
                        errors.Add($"DATA009 sourced/reviewed fact '{id}' at {path} requires an atomic claim and at least one source record.");
                    result.Add(new EducationalFactDefinition(id, key, asset.ClaimForReview, sourceIds, editorial));
                }
                catch (Exception exception) when (exception is FormatException || exception is ArgumentException)
                {
                    errors.Add($"DATA010 invalid fact at {path}: {exception.Message}");
                }
            }
            return result.ToArray();
        }

        private static DiscoveryDefinition[] MapDiscoveries(
            IEnumerable<DiscoveryDefinitionAsset> assets,
            ISet<CategoryId> categories,
            ISet<TagId> tags,
            ISet<EducationalFactId> facts,
            ContentValidationMode mode,
            IContentReferenceResolver resolver,
            ICollection<string> errors)
        {
            var result = new List<DiscoveryDefinition>();
            var ids = new HashSet<DiscoveryId>();
            foreach (DiscoveryDefinitionAsset asset in Ordered(assets))
            {
                string path = Describe(asset, resolver);
                try
                {
                    DiscoveryId id = DiscoveryId.Parse(asset.RawId);
                    if (!ids.Add(id)) { errors.Add($"DATA011 duplicate discovery ID '{id}' at {path}; keep one and register an alias for retired IDs."); continue; }
                    if (asset.Category == null) { errors.Add($"DATA012 missing category reference at {path}; assign a catalog category."); continue; }
                    CategoryId category = CategoryId.Parse(asset.Category.RawId);
                    if (!categories.Contains(category)) errors.Add($"DATA013 category '{category}' referenced by {path} is absent from the catalog.");
                    TagId[] tagIds = ParseReferences(asset.Tags, TagId.Parse, tags, "tag", path, errors);
                    EducationalFactId[] factIds = ParseReferences(asset.Facts, EducationalFactId.Parse, facts, "fact", path, errors);
                    var displayName = new LocalizedKey(asset.DisplayNameTable, asset.DisplayNameKey);
                    var audio = new AudioCueId(asset.NameAudioCueId);
                    VisualAssetId visual = VisualAssetId.Parse(asset.VisualAssetId);
                    if (resolver != null && !resolver.HasLocalization(displayName)) errors.Add($"DATA014 missing localization '{displayName}' referenced by {path}; add ES/EN entries.");
                    if (resolver != null && !resolver.HasAudioCue(audio)) errors.Add($"DATA015 missing audio cue '{audio}' referenced by {path}; add it to AudioCueCatalog or choose an existing cue.");
                    if (asset.VisualAsset == null || resolver != null && !resolver.HasVisualAsset(visual, asset.VisualAsset))
                        errors.Add($"DATA016 missing visual asset '{visual}' referenced by {path}; assign a local asset with provenance metadata.");
                    EditorialMetadata editorial = MapEditorial(asset, mode, resolver, errors);
                    if (editorial.State == EditorialState.Approved && factIds.Length == 0)
                        errors.Add($"DATA027 Approved discovery '{id}' at {path} requires at least one approved fact reference.");
                    result.Add(new DiscoveryDefinition(
                        id, WorldId.Parse(asset.WorldId), category, tagIds, factIds, displayName, audio, visual,
                        editorial));
                }
                catch (Exception exception) when (exception is FormatException || exception is ArgumentException)
                {
                    errors.Add($"DATA017 invalid discovery at {path}: {exception.Message}");
                }
            }
            return result.ToArray();
        }

        private static TId[] ParseReferences<TAsset, TId>(
            IEnumerable<TAsset> assets, Func<string, TId> parse, ISet<TId> known,
            string kind, string ownerPath, ICollection<string> errors) where TAsset : ContentDefinitionAsset
        {
            var result = new List<TId>();
            foreach (TAsset asset in assets ?? Array.Empty<TAsset>())
            {
                if (asset == null) { errors.Add($"DATA018 missing {kind} reference at {ownerPath}; assign or remove the empty slot."); continue; }
                TId id = parse(asset.RawId);
                if (!known.Contains(id)) errors.Add($"DATA019 {kind} '{id}' referenced by {ownerPath} is absent from the catalog.");
                result.Add(id);
            }
            if (result.Distinct().Count() != result.Count) errors.Add($"DATA020 duplicate {kind} reference at {ownerPath}; keep one reference.");
            return result.ToArray();
        }

        private static DiscoveryIdAlias[] MapAliases(
            IEnumerable<DiscoveryAliasAsset> assets,
            IEnumerable<DiscoveryDefinition> discoveries,
            IContentReferenceResolver resolver,
            ICollection<string> errors)
        {
            var known = new HashSet<DiscoveryId>(discoveries.Select(item => item.Id));
            var previous = new HashSet<DiscoveryId>();
            var result = new List<DiscoveryIdAlias>();
            foreach (DiscoveryAliasAsset alias in assets ?? Array.Empty<DiscoveryAliasAsset>())
            {
                if (alias == null || alias.Current == null) { errors.Add("DATA021 alias has no target; assign a current discovery or remove it."); continue; }
                try
                {
                    DiscoveryId oldId = DiscoveryId.Parse(alias.PreviousId);
                    DiscoveryId currentId = DiscoveryId.Parse(alias.Current.RawId);
                    if (!known.Contains(currentId)) errors.Add($"DATA022 alias '{oldId}' targets discovery '{currentId}' absent from catalog.");
                    else if (known.Contains(oldId) || !previous.Add(oldId)) errors.Add($"DATA023 alias '{oldId}' collides with a current or previous ID; use a unique retired ID.");
                    else result.Add(new DiscoveryIdAlias(oldId, currentId));
                }
                catch (Exception exception) when (exception is FormatException || exception is ArgumentException)
                {
                    errors.Add("DATA024 invalid discovery alias: " + exception.Message);
                }
            }
            return result.OrderBy(item => item.Previous.Value, StringComparer.Ordinal).ToArray();
        }

        private static EditorialMetadata MapEditorial(
            ContentDefinitionAsset asset,
            ContentValidationMode mode,
            IContentReferenceResolver resolver,
            ICollection<string> errors)
        {
            string path = Describe(asset, resolver);
            if (asset.Editorial == null)
            {
                errors.Add($"DATA026 missing editorial metadata at {path}; assign state, owner and Development watermark.");
                return new EditorialMetadata(EditorialState.Draft, true, "Missing owner", "BORRADOR · PH_");
            }
            EditorialMetadata metadata = asset.Editorial.ToRuntime();
            if (metadata.State == EditorialState.Rejected)
                errors.Add($"DATA028 rejected content '{asset.RawId}' remains in catalog at {path}; remove it or replace references.");
            if (mode == ContentValidationMode.Release && !metadata.IsReleaseApproved)
                errors.Add($"DATA025 Release rejects {metadata.State} or placeholder content '{asset.RawId}' at {path}; obtain per-item human approval and replace PH_ assets.");
            return metadata;
        }

        private static void ValidateSourceDetails(
            IEnumerable<ContentSourceRecordAsset> sources,
            IContentReferenceResolver resolver,
            ICollection<string> errors)
        {
            foreach (ContentSourceRecordAsset source in Ordered(sources))
            {
                EditorialState state = source.Editorial?.State ?? EditorialState.Draft;
                if (state < EditorialState.Sourced || state == EditorialState.Rejected) continue;
                string path = Describe(source, resolver);
                if (string.IsNullOrWhiteSpace(source.Institution) || string.IsNullOrWhiteSpace(source.Title) ||
                    string.IsNullOrWhiteSpace(source.Reference) || string.IsNullOrWhiteSpace(source.ConsultedOn))
                    errors.Add($"DATA029 sourced record '{source.RawId}' at {path} requires institution, title, reference and consultation date.");
                if (state >= EditorialState.Reviewed && string.IsNullOrWhiteSpace(source.Reviewer))
                    errors.Add($"DATA030 reviewed/approved record '{source.RawId}' at {path} requires a human reviewer.");
            }
        }

        private static IEnumerable<TAsset> Ordered<TAsset>(IEnumerable<TAsset> assets) where TAsset : ContentDefinitionAsset =>
            (assets ?? Array.Empty<TAsset>()).Where(asset => asset != null)
            .OrderBy(asset => asset.RawId, StringComparer.Ordinal).ThenBy(asset => asset.name, StringComparer.Ordinal);

        private static void ValidateSlots<TAsset>(
            IEnumerable<TAsset> assets,
            string kind,
            ICollection<string> errors) where TAsset : ContentDefinitionAsset
        {
            int index = 0;
            foreach (TAsset asset in assets ?? Array.Empty<TAsset>())
            {
                if (asset == null) errors.Add($"DATA031 missing {kind} asset in catalog slot {index}; assign a definition or remove the empty slot.");
                index++;
            }
        }

        private static string Describe(UnityEngine.Object asset, IContentReferenceResolver resolver) =>
            resolver?.Describe(asset) ?? (asset == null ? "<missing asset>" : asset.name);
    }
}
