using System.Collections;
using NUnit.Framework;
using PequenoExplorador.Application;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Bootstrap;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PequenoExplorador.Tests.PlayMode
{
    public sealed class BootstrapPlayModeTests
    {
        [UnityTest]
        public IEnumerator BootstrapReachesReadyExactlyOnce()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return WaitForReady();

            DiagnosticBootstrap[] bootstraps = Object.FindObjectsByType<DiagnosticBootstrap>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            DiagnosticBootstrap diagnostic = bootstraps[0];

            Assert.That(bootstraps, Has.Length.EqualTo(1));
            Assert.That(diagnostic.gameObject.activeInHierarchy, Is.True);
            Assert.That(diagnostic.gameObject.name, Is.EqualTo(DiagnosticBootstrap.PlaceholderObjectName));
            Assert.That(diagnostic.State, Is.EqualTo(ApplicationState.Ready));
            Assert.That(diagnostic.Environment, Is.EqualTo(ApplicationEnvironment.Development));
            Assert.That(diagnostic.StatusText, Is.EqualTo("Ready"));
        }

        [UnityTest]
        public IEnumerator SceneReloadShutsDownTheOldRootAndDoesNotDuplicateBootstrap()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return WaitForReady();
            DiagnosticBootstrap previous = Object.FindFirstObjectByType<DiagnosticBootstrap>();

            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return WaitForReady();
            DiagnosticBootstrap[] bootstraps = Object.FindObjectsByType<DiagnosticBootstrap>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            Assert.That(previous == null, Is.True, "Reload must destroy and shutdown the previous root.");
            Assert.That(bootstraps, Has.Length.EqualTo(1));
            Assert.That(bootstraps[0].State, Is.EqualTo(ApplicationState.Ready));
        }

        private static IEnumerator WaitForReady()
        {
            const int frameLimit = 120;
            for (int frame = 0; frame < frameLimit; frame++)
            {
                DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
                if (bootstrap != null && bootstrap.State == ApplicationState.Ready)
                {
                    yield break;
                }

                if (bootstrap != null && bootstrap.State == ApplicationState.Failed)
                {
                    Assert.Fail("Bootstrap entered recoverable failure instead of Ready.");
                }

                yield return null;
            }

            Assert.Fail("Bootstrap did not reach Ready within the frame limit.");
        }
    }
}
