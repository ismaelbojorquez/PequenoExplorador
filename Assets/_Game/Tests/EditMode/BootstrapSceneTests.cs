using System.Linq;
using NUnit.Framework;
using PequenoExplorador.Bootstrap;
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
            Assert.That(DiagnosticBootstrap.ProductName, Is.EqualTo("Pequeño Explorador: Aprende Jugando"));
            Assert.That(DiagnosticBootstrap.DevelopmentVersion, Does.EndWith("-dev"));
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
