using System.Linq;
using NUnit.Framework;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Application.Configuration;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class BootstrapSceneTests
    {
        private const string ScenePath = "Assets/_Game/Bootstrap/Bootstrap.unity";

        [Test]
        public void BootstrapSceneContainsOnlyTheExpectedFoundationContract()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            DiagnosticBootstrap diagnostic = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Camera mainCamera = Camera.main;

            Assert.That(diagnostic, Is.Not.Null, "Temporary diagnostic marker is missing.");
            Assert.That(diagnostic.gameObject.name, Is.EqualTo(DiagnosticBootstrap.PlaceholderObjectName));
            Assert.That(mainCamera, Is.Not.Null, "Bootstrap scene requires one tagged camera.");
            var serializedBootstrap = new SerializedObject(diagnostic);
            Assert.That(
                serializedBootstrap.FindProperty("_statusView").objectReferenceValue,
                Is.Not.Null,
                "BootstrapStatusView must be explicitly wired, not found globally.");
            Assert.That(
                serializedBootstrap.FindProperty("_sceneFlowView").objectReferenceValue,
                Is.Not.Null,
                "SceneTransitionView must be explicitly wired by the composition root scene.");
            var serializedStatus = new SerializedObject(
                serializedBootstrap.FindProperty("_statusView").objectReferenceValue);
            Assert.That(serializedStatus.FindProperty("_productNameText").objectReferenceValue, Is.Not.Null);
            Assert.That(serializedStatus.FindProperty("_appVersionText").objectReferenceValue, Is.Not.Null);
            Assert.That(AppConfigDefaults.ProductName, Is.EqualTo("Pequeño Explorador: Aprende Jugando"));
        }

        [Test]
        public void BootstrapIsTheOnlyEnabledBuildScene()
        {
            string[] enabledScenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            Assert.That(enabledScenes, Is.EqualTo(new[] { ScenePath }));
        }
    }
}
