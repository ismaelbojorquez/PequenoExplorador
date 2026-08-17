using System.IO;
using System.Linq;
using NUnit.Framework;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Content.Visuals;
using PequenoExplorador.Editor;
using PequenoExplorador.Editor.BuildTools;
using UnityEditor;
using UnityEngine;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class ToucanFixtureTests
    {
        [Test]
        public void PrefabCarriesStableIdentityEditorialStateAndBoundedGeometry()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ToucanFixtureSetup.PrefabPath);
            Assert.That(prefab, Is.Not.Null);
            ToucanReviewFixtureMetadata metadata = prefab.GetComponent<ToucanReviewFixtureMetadata>();
            Assert.That(metadata.VisualId, Is.EqualTo(ToucanFixtureSetup.VisualId));
            Assert.That(metadata.FutureDiscoveryId, Is.EqualTo(ToucanFixtureSetup.FutureDiscoveryId));
            Assert.That(metadata.FutureInteractionId, Is.EqualTo(ToucanFixtureSetup.FutureInteractionId));
            Assert.That(metadata.EditorialState, Is.EqualTo(EditorialState.Sourced));
            Assert.That(metadata.IsPlaceholder, Is.False);
            Assert.That(metadata.FactualReviewState, Is.EqualTo("PENDING_SPECIALIST_SIGNOFF"));
            Assert.That(metadata.TouchCollider.isTrigger, Is.True);
            Assert.That(metadata.CandidatePhotoBounds.size.magnitude, Is.GreaterThan(1f));
            ToucanFixtureMetrics metrics = ToucanFixtureSetup.MeasurePrefab();
            Assert.That(metrics.Materials, Is.EqualTo(7));
            Assert.That(metrics.Meshes, Is.InRange(8, 24));
            Assert.That(metrics.Vertices, Is.LessThanOrEqualTo(10000));
            Assert.That(metrics.Triangles, Is.LessThanOrEqualTo(10000));
        }

        [Test]
        public void GeneratorIsIdempotentAndPreservesPrefabGuid()
        {
            string before = AssetDatabase.AssetPathToGUID(ToucanFixtureSetup.PrefabPath);
            ToucanFixtureMetrics first = ToucanFixtureSetup.ApplyAssetsAndScene(false);
            string middle = AssetDatabase.AssetPathToGUID(ToucanFixtureSetup.PrefabPath);
            ToucanFixtureMetrics second = ToucanFixtureSetup.ApplyAssetsAndScene(false);
            string after = AssetDatabase.AssetPathToGUID(ToucanFixtureSetup.PrefabPath);
            Assert.That(before, Is.Not.Empty);
            Assert.That(middle, Is.EqualTo(before));
            Assert.That(after, Is.EqualTo(before));
            Assert.That(second.Meshes, Is.EqualTo(first.Meshes));
            Assert.That(second.Vertices, Is.EqualTo(first.Vertices));
            Assert.That(second.Triangles, Is.EqualTo(first.Triangles));
            Assert.That(second.Materials, Is.EqualTo(first.Materials));
            Assert.That(second.Bounds.center, Is.EqualTo(first.Bounds.center));
            Assert.That(second.Bounds.size, Is.EqualTo(first.Bounds.size));
        }

        [Test]
        public void DevelopmentPassesAndReleaseRemainsFailClosed()
        {
            Assert.That(ToucanFixtureValidationService.Validate(ContentValidationMode.Development), Is.Empty);
            Assert.That(ToucanFixtureValidationService.Validate(ContentValidationMode.Release)
                .Any(item => item.StartsWith(ToucanFixtureValidationService.ReleaseBlockCode)), Is.True);
            Assert.That(File.Exists(ToucanFixtureSetup.ProvenancePath), Is.True);
            string ledger = File.ReadAllText(ToucanFixtureSetup.ProvenancePath);
            StringAssert.Contains("\"externalMedia\": false", ledger);
            StringAssert.Contains("\"editorialState\": \"Sourced\"", ledger);
            StringAssert.Contains("PENDING_SPECIALIST_SIGNOFF", ledger);
        }
    }
}
