using System.Collections;
using NUnit.Framework;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Domain.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PequenoExplorador.Tests.PlayMode
{
    public sealed class ContentCatalogPlayModeTests
    {
        [UnityTest]
        public IEnumerator BootstrapResolvesApprovedToucanAndRetiredAliasWithoutAssetDatabase()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            float deadline = Time.realtimeSinceStartup + 20f;
            DiagnosticBootstrap bootstrap = null;
            while (Time.realtimeSinceStartup < deadline)
            {
                bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
                if (bootstrap != null && bootstrap.State == ApplicationState.Ready) break;
                yield return null;
            }
            Assert.That(bootstrap, Is.Not.Null);
            Assert.That(bootstrap.State, Is.EqualTo(ApplicationState.Ready));
            Assert.That(bootstrap.Content.TryGetDiscovery(DiscoveryId.Parse("discovery.jungle.keel-billed-toucan"), out var discovery), Is.True);
            Assert.That(discovery.Editorial.IsReleaseApproved, Is.True);
            Assert.That(bootstrap.Content.TryResolveDiscovery(DiscoveryId.Parse("discovery.jungle.placeholder"), out var aliased), Is.True);
            Assert.That(aliased.Id, Is.EqualTo(discovery.Id));
        }
    }
}
