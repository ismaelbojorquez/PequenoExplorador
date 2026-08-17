using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Presentation.Album;
using PequenoExplorador.Presentation.Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace PequenoExplorador.Tests.PlayMode
{
    public sealed class AlbumPlayModeTests
    {
        [TearDown]
        public void TearDown()
        {
            foreach (DiagnosticBootstrap bootstrap in Object.FindObjectsByType<DiagnosticBootstrap>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
                Object.DestroyImmediate(bootstrap.gameObject);
        }

        [UnityTest]
        public IEnumerator LockedAlbumRefreshesEsEnPseudoAndRespectsLandscapeLayouts()
        {
            yield return LoadReadyCamp();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Task reset = bootstrap.ResetProgressForTestsAsync(CancellationToken.None);
            yield return WaitForTask(reset);
            AlbumView album = bootstrap.AlbumView;
            Assert.That(album.IsOpenAvailable, Is.True);
            album.OpenButton.onClick.Invoke();
            yield return null;
            Assert.That(album.IsVisible, Is.True);
            Assert.That(album.Snapshot.Total, Is.EqualTo(1));
            Assert.That(album.Snapshot.Discovered, Is.Zero);
            Assert.That(album.FirstVisibleEntryCell.NameText, Is.EqualTo("Por descubrir"));

            Task english = bootstrap.SetLocaleAsync(LocaleCode.English, false, CancellationToken.None);
            yield return WaitForTask(english);
            Assert.That(album.FirstVisibleEntryCell.NameText, Is.EqualTo("Waiting to be discovered"));
            Task pseudo = bootstrap.SetLocaleAsync(LocaleCode.Pseudo, false, CancellationToken.None);
            yield return WaitForTask(pseudo);
            Assert.That(album.FirstVisibleEntryCell.NameText, Is.Not.Empty);
            Assert.That(album.FirstVisibleEntryCell.NameText, Is.Not.EqualTo("Por descubrir"));
            Assert.That(album.FirstVisibleEntryCell.NameText, Is.Not.EqualTo("Waiting to be discovered"));

            var ratios = new[] { new Vector2Int(1024, 768), new Vector2Int(1280, 720), new Vector2Int(1600, 720), new Vector2Int(1280, 800) };
            foreach (Vector2Int ratio in ratios)
            {
                Screen.SetResolution(ratio.x, ratio.y, false);
                yield return null;
                Canvas.ForceUpdateCanvases();
                foreach (Button button in album.GetComponentsInChildren<Button>(true))
                {
                    RectTransform rect = (RectTransform)button.transform;
                    Assert.That(rect.rect.width, Is.GreaterThanOrEqualTo(64f), button.name + " width at " + ratio);
                    Assert.That(rect.rect.height, Is.GreaterThanOrEqualTo(64f), button.name + " height at " + ratio);
                }
                Assert.That(album.GetComponentsInChildren<Text>(true).Where(item => item.gameObject.activeInHierarchy)
                    .All(item => item.resizeTextForBestFit), Is.True, "Visible album copy must support larger localized text.");
            }
            Assert.That(album.TryHandleBack(), Is.True);
            Assert.That(album.IsVisible, Is.False);
        }

        [UnityTest]
        public IEnumerator DiscoveryAndPhotoAppearAfterCaptureWithoutRestartAndDetailCancelsSafely()
        {
            yield return LoadReadyCamp();
            DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
            Task reset = bootstrap.ResetProgressForTestsAsync(CancellationToken.None);
            yield return WaitForTask(reset);
            Task<SceneTransitionResult> enter = bootstrap.GoToExpeditionAsync(CancellationToken.None);
            yield return WaitForTask(enter);
            Assert.That(enter.Result.IsSuccess, Is.True, enter.Result.ErrorCode);
            yield return ActivateAnimalAndCapture(bootstrap);
            Task<SceneTransitionResult> camp = bootstrap.GoToCampAsync(CancellationToken.None);
            yield return WaitForTask(camp);
            Assert.That(camp.Result.IsSuccess, Is.True, camp.Result.ErrorCode);

            AlbumView album = bootstrap.AlbumView;
            album.Open();
            yield return null;
            Assert.That(album.Snapshot.Discovered, Is.EqualTo(1));
            Assert.That(album.FirstVisibleEntryCell.NameText, Is.EqualTo("Tucán pico canoa"));
            album.FirstVisibleEntryCell.Button.onClick.Invoke();
            yield return WaitForDetailPhoto(album);
            Assert.That(album.IsDetailVisible, Is.True);
            Assert.That(album.DetailNameText, Is.EqualTo("Tucán pico canoa"));
            Assert.That(album.DetailPhotoSprite, Is.Not.Null, "The local best photo should load without blocking the detail.");
            Assert.That(album.ReplayInteractable, Is.False, "Placeholder confirm cue is not exposed as factual animal audio.");
            Assert.That(album.TryHandleBack(), Is.True);
            Assert.That(album.IsDetailVisible, Is.False);
            Assert.That(album.IsVisible, Is.True);
            album.Close();
            Assert.That(album.IsVisible, Is.False);
        }

        private static IEnumerator ActivateAnimalAndCapture(DiagnosticBootstrap bootstrap)
        {
            WorldInteractableView animal = bootstrap.InteractionRoot.Targets
                .Single(item => item.RawInteractionId == "interaction.jungle.keel-billed-toucan");
            Vector3 screen = Camera.main.WorldToScreenPoint(animal.transform.position + Vector3.up * 0.7f);
            Assert.That(bootstrap.InteractionRoot.TryHandleTap(new ScreenPoint(screen.x, screen.y)), Is.True);
            float deadline = Time.realtimeSinceStartup + 8f;
            while (Time.realtimeSinceStartup < deadline &&
                   bootstrap.InteractionRoot.Coordinator.Snapshot.State != InteractionOutcome.Ready) yield return null;
            Assert.That(bootstrap.InteractionRoot.Coordinator.Snapshot.State, Is.EqualTo(InteractionOutcome.Ready));
            bootstrap.InteractionPrompt.ActionButton.onClick.Invoke();
            yield return null;
            bootstrap.PhotographyRoot.ActiveTarget.SetSampleOverrideForEditorAndTests(
                new PhotoFrameSample(0.30f, 3f, true, 0.05f, 1f));
            bootstrap.PhotographyView.ShutterButton.onClick.Invoke();
            deadline = Time.realtimeSinceStartup + 6f;
            while (Time.realtimeSinceStartup < deadline && !bootstrap.PhotographyRoot.LastCapture.ProgressCaptured) yield return null;
            Assert.That(bootstrap.PhotographyRoot.LastCapture.ProgressCaptured, Is.True);
        }

        private static IEnumerator LoadReadyCamp()
        {
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
            yield return null;
            float deadline = Time.realtimeSinceStartup + 20f;
            while (Time.realtimeSinceStartup < deadline)
            {
                DiagnosticBootstrap bootstrap = Object.FindFirstObjectByType<DiagnosticBootstrap>();
                if (bootstrap != null && bootstrap.State == ApplicationState.Ready &&
                    bootstrap.SceneFlow?.Current == SceneFlowState.Camp && !bootstrap.SceneFlow.IsTransitioning)
                    yield break;
                yield return null;
            }
            Assert.Fail("Bootstrap did not become Ready at Camp.");
        }

        private static IEnumerator WaitForDetailPhoto(AlbumView album)
        {
            float deadline = Time.realtimeSinceStartup + 5f;
            while (Time.realtimeSinceStartup < deadline && album.DetailPhotoSprite == null) yield return null;
        }

        private static IEnumerator WaitForTask(Task task)
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (!task.IsCompleted && Time.realtimeSinceStartup < deadline) yield return null;
            if (task.IsFaulted) throw task.Exception;
            Assert.That(task.IsCompleted, Is.True);
        }
    }
}
