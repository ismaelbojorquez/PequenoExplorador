using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Editor;
using PequenoExplorador.Editor.BuildTools;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class ContentCatalogTests
    {
        [Test]
        public void TypedIdsParseCompareAndRejectWrongNamespaces()
        {
            DiscoveryId first = DiscoveryId.Parse(ContentFoundationSetup.DiscoveryId);
            DiscoveryId second = DiscoveryId.Parse(ContentFoundationSetup.DiscoveryId);
            Assert.That(first, Is.EqualTo(second));
            Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
            Assert.That(DiscoveryId.TryParse("category.jungle.placeholder", out _), Is.False);
            Assert.That(TagId.TryParse("tag.Jungle.Invalid", out _), Is.False);
            Assert.That(CategoryId.Parse("category.nature.animal").Value, Is.EqualTo("category.nature.animal"));
        }

        [Test]
        public void CanonicalApprovedToucanMapsAndRetiredIdResolvesByAlias()
        {
            ContentCatalogAsset asset = AssetDatabase.LoadAssetAtPath<ContentCatalogAsset>(ContentFoundationSetup.CatalogPath);
            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.TryBuildRuntimeCatalog(ContentValidationMode.Development, out ContentCatalog catalog, out var errors), Is.True, string.Join("\n", errors));
            Assert.That(catalog.TryGetDiscovery(DiscoveryId.Parse(ContentFoundationSetup.DiscoveryId), out DiscoveryDefinition discovery), Is.True);
            Assert.That(discovery.Editorial.State, Is.EqualTo(EditorialState.Approved));
            Assert.That(discovery.Editorial.IsPlaceholder, Is.False);
            Assert.That(discovery.DisplayName, Is.EqualTo(LocalizationKeys.KeelBilledToucanName));
            Assert.That(catalog.TryResolveDiscovery(DiscoveryId.Parse(ContentFoundationSetup.RetiredDiscoveryId), out DiscoveryDefinition aliased), Is.True);
            Assert.That(aliased.Id, Is.EqualTo(discovery.Id));
            Assert.That(catalog.TryGetCategory(discovery.CategoryId, out _), Is.True);
            Assert.That(catalog.TryGetTag(discovery.TagIds.Single(), out _), Is.True);
            Assert.That(discovery.FactIds, Has.Count.EqualTo(7));
            Assert.That(catalog.TryGetFact(discovery.FactIds.First(), out EducationalFactDefinition fact), Is.True);
            Assert.That(fact.SourceIds, Is.Not.Empty);
            Assert.That(catalog.TryGetSource(fact.SourceIds.First(), out _), Is.True);
        }

        [Test]
        public void CatalogOrderIsDeterministicAndAliasResolvesRetiredId()
        {
            DiscoveryDefinition alpha = CreateDiscovery("discovery.jungle.alpha");
            DiscoveryDefinition beta = CreateDiscovery("discovery.jungle.beta");
            var alias = new DiscoveryIdAlias(DiscoveryId.Parse("discovery.jungle.retired-alpha"), alpha.Id);
            var catalog = new ContentCatalog(new[] { beta, alpha }, new[] { alias });
            Assert.That(catalog.Discoveries.Select(item => item.Id.Value), Is.EqualTo(new[] { alpha.Id.Value, beta.Id.Value }));
            Assert.That(catalog.TryResolveDiscovery(alias.Previous, out DiscoveryDefinition resolved), Is.True);
            Assert.That(resolved.Id, Is.EqualTo(alpha.Id));
        }

        [Test]
        public void CompilerReportsDuplicateAndMissingReferencesActionably()
        {
            ContentCatalogAsset original = AssetDatabase.LoadAssetAtPath<ContentCatalogAsset>(ContentFoundationSetup.CatalogPath);
            ContentCatalogAsset duplicateCatalog = Object.Instantiate(original);
            DiscoveryDefinitionAsset discovery = original.Discoveries.Single();
            SetObjectArray(new SerializedObject(duplicateCatalog), "_discoveries", discovery, discovery);
            Assert.That(ContentCatalogCompiler.TryCompile(duplicateCatalog, ContentValidationMode.Development, new AcceptAllResolver(), out _, out var duplicateErrors), Is.False);
            Assert.That(duplicateErrors.Any(error => error.Contains("DATA011") && error.Contains(ContentFoundationSetup.DiscoveryId)), Is.True);

            DiscoveryDefinitionAsset missingCategory = Object.Instantiate(discovery);
            var discoverySerialized = new SerializedObject(missingCategory);
            discoverySerialized.FindProperty("_category").objectReferenceValue = null;
            discoverySerialized.FindProperty("_visualAsset").objectReferenceValue = null;
            discoverySerialized.ApplyModifiedPropertiesWithoutUndo();
            ContentCatalogAsset brokenCatalog = Object.Instantiate(original);
            SetObjectArray(new SerializedObject(brokenCatalog), "_discoveries", missingCategory);
            Assert.That(ContentCatalogCompiler.TryCompile(brokenCatalog, ContentValidationMode.Development, new AcceptAllResolver(), out _, out var referenceErrors), Is.False);
            Assert.That(referenceErrors.Any(error => error.Contains("DATA012") && error.Contains(missingCategory.name)), Is.True);

            Object.DestroyImmediate(duplicateCatalog);
            Object.DestroyImmediate(missingCategory);
            Object.DestroyImmediate(brokenCatalog);
        }

        [Test]
        public void MissingLocalizationAudioAndVisualAreDetected()
        {
            ContentCatalogAsset original = AssetDatabase.LoadAssetAtPath<ContentCatalogAsset>(ContentFoundationSetup.CatalogPath);
            Assert.That(ContentCatalogCompiler.TryCompile(original, ContentValidationMode.Development, new RejectAllResolver(), out _, out var errors), Is.False);
            Assert.That(errors.Any(error => error.Contains("DATA008") || error.Contains("DATA014")), Is.True);
            Assert.That(errors.Any(error => error.Contains("DATA015")), Is.True);
            Assert.That(errors.Any(error => error.Contains("DATA016")), Is.True);
        }

        [Test]
        public void SourcedRecordWithoutTraceabilityIsRejected()
        {
            ContentCatalogAsset original = AssetDatabase.LoadAssetAtPath<ContentCatalogAsset>(ContentFoundationSetup.CatalogPath);
            ContentSourceRecordAsset source = Object.Instantiate(original.Sources.First());
            var sourceSerialized = new SerializedObject(source);
            SerializedProperty editorial = sourceSerialized.FindProperty("_editorial");
            editorial.FindPropertyRelative("_state").enumValueIndex = (int)EditorialState.Approved;
            editorial.FindPropertyRelative("_isPlaceholder").boolValue = false;
            editorial.FindPropertyRelative("_developmentWatermark").stringValue = string.Empty;
            sourceSerialized.FindProperty("_institution").stringValue = string.Empty;
            sourceSerialized.ApplyModifiedPropertiesWithoutUndo();
            ContentCatalogAsset catalog = Object.Instantiate(original);
            SetObjectArray(new SerializedObject(catalog), "_sources", source);
            Assert.That(ContentCatalogCompiler.TryCompile(catalog, ContentValidationMode.Development, new AcceptAllResolver(), out _, out var errors), Is.False);
            Assert.That(errors.Any(error => error.Contains("DATA029") && error.Contains(source.name)), Is.True);
            Object.DestroyImmediate(source);
            Object.DestroyImmediate(catalog);
        }

        [Test]
        public void CanonicalToucanPassesReleaseAndDevelopmentValidation()
        {
            var releaseErrors = ContentCatalogValidationService.Validate(ContentValidationMode.Release, writeReports: true);
            Assert.That(releaseErrors, Is.Empty);
            var developmentErrors = ContentCatalogValidationService.Validate(ContentValidationMode.Development, writeReports: true);
            Assert.That(developmentErrors, Is.Empty);
            Assert.That(File.Exists("artifacts/reports/content-catalog-development.json"), Is.True);
            Assert.That(File.ReadAllText("artifacts/reports/content-catalog-release.md"), Does.Contain("`PASS`"));
        }

        [Test]
        public void ExplicitIdGeneratorNeverOverwritesExistingId()
        {
            var asset = ScriptableObject.CreateInstance<DiscoveryDefinitionAsset>();
            asset.name = "PH_River Toucan";
            Assert.That(ContentIdGenerator.TryGenerate(asset, out string generated), Is.True);
            Assert.That(generated, Is.EqualTo("discovery.draft.river-toucan"));
            Assert.That(ContentIdGenerator.TryGenerate(asset, out _), Is.False);
            Assert.That(asset.RawId, Is.EqualTo(generated));
            Object.DestroyImmediate(asset);
        }

        private static DiscoveryDefinition CreateDiscovery(string id) => new DiscoveryDefinition(
            DiscoveryId.Parse(id), WorldId.Parse("world.jungle"), CategoryId.Parse("category.nature.placeholder"),
            Array.Empty<TagId>(), Array.Empty<EducationalFactId>(), LocalizationKeys.DiscoveryPlaceholderName,
            new AudioCueId("audio.feedback.confirm"), VisualAssetId.Parse("visual.discovery.jungle.placeholder"),
            new EditorialMetadata(EditorialState.Approved, false, "Test", string.Empty));

        private static void SetObjectArray(SerializedObject serialized, string propertyName, params Object[] values)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            property.arraySize = values.Length;
            for (int index = 0; index < values.Length; index++) property.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private sealed class AcceptAllResolver : IContentReferenceResolver
        {
            public string Describe(Object asset) => asset?.name ?? "<missing>";
            public bool HasLocalization(LocalizedKey key) => true;
            public bool HasAudioCue(AudioCueId cueId) => true;
            public bool HasVisualAsset(VisualAssetId id, Object asset) => true;
            public bool HasDiscovery(DiscoveryId id) => true;
        }

        private sealed class RejectAllResolver : IContentReferenceResolver
        {
            public string Describe(Object asset) => asset?.name ?? "<missing>";
            public bool HasLocalization(LocalizedKey key) => false;
            public bool HasAudioCue(AudioCueId cueId) => false;
            public bool HasVisualAsset(VisualAssetId id, Object asset) => false;
            public bool HasDiscovery(DiscoveryId id) => false;
        }
    }
}
