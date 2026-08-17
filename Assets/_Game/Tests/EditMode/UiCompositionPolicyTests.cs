using System;
using System.Linq;
using NUnit.Framework;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.UI;
using PequenoExplorador.Presentation.UI;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class UiCompositionPolicyTests
    {
        [Test]
        public void EveryStateHasAtMostOnePrimarySurfaceAndAnInputMap()
        {
            foreach (AppUiState state in Enum.GetValues(typeof(AppUiState)))
            {
                Assert.That(UiCompositionPolicy.PrimarySurfaces(state).Count, Is.LessThanOrEqualTo(1), state.ToString());
                Assert.That(AppUiStatePolicy.InputMap(state), Is.Not.EqualTo(InputMapId.None), state.ToString());
                Assert.That(Enum.IsDefined(typeof(UiBackAction), AppUiStatePolicy.BackAction(state)), Is.True, state.ToString());
            }
        }

        [Test]
        public void CampIsFailClosedAgainstLegacyAndDiagnosticSurfaces()
        {
            UiSurfaceId[] prohibited =
            {
                UiSurfaceId.Status, UiSurfaceId.SceneFlow, UiSurfaceId.AudioDiagnostics,
                UiSurfaceId.Economy, UiSurfaceId.Missions, UiSurfaceId.Photography,
                UiSurfaceId.Learning, UiSurfaceId.Customization, UiSurfaceId.InputFoundation
            };
            Assert.That(prohibited.Any(surface => UiCompositionPolicy.IsVisible(AppUiState.Camp, surface, false)), Is.False);
            Assert.That(UiCompositionPolicy.IsVisible(AppUiState.Camp, UiSurfaceId.Camp, false), Is.True);
        }

        [Test]
        public void TutorialIsOnlyAnExplicitOverlayAndDiagnosticsReplaceProduct()
        {
            Assert.That(UiCompositionPolicy.IsVisible(AppUiState.Expedition, UiSurfaceId.Tutorial, true), Is.True);
            Assert.That(UiCompositionPolicy.IsVisible(AppUiState.Expedition, UiSurfaceId.Tutorial, false), Is.False);
            Assert.That(UiCompositionPolicy.IsVisible(AppUiState.DevelopmentDiagnostics, UiSurfaceId.Camp, true), Is.False);
            Assert.That(UiCompositionPolicy.PrimarySurfaces(AppUiState.DevelopmentDiagnostics).Single(),
                Is.EqualTo(UiSurfaceId.InputFoundation));
        }
    }
}
