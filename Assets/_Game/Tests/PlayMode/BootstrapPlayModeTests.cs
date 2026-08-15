using System.Collections;
using NUnit.Framework;
using PequenoExplorador.Bootstrap;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PequenoExplorador.Tests.PlayMode
{
    public sealed class BootstrapPlayModeTests
    {
        [UnityTest]
        public IEnumerator BootstrapKeepsTheTemporaryDiagnosticVisible()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;

            DiagnosticBootstrap diagnostic = Object.FindFirstObjectByType<DiagnosticBootstrap>();

            Assert.That(diagnostic, Is.Not.Null, "The Phase 03 diagnostic marker must remain visible.");
            Assert.That(diagnostic.gameObject.activeInHierarchy, Is.True);
            Assert.That(diagnostic.gameObject.name, Is.EqualTo(DiagnosticBootstrap.PlaceholderObjectName));
            Assert.That(DiagnosticBootstrap.DevelopmentVersion, Does.EndWith("-dev"));
        }
    }
}
