using System;
using System.Collections.Generic;
using PequenoExplorador.Application.UI;

namespace PequenoExplorador.Presentation.UI
{
    public enum UiSurfaceId
    {
        Status = 0,
        SceneFlow = 1,
        Camp = 2,
        Interaction = 3,
        Learning = 4,
        Photography = 5,
        Album = 6,
        Missions = 7,
        Economy = 8,
        Customization = 9,
        InputFoundation = 10,
        Tutorial = 11,
        AudioDiagnostics = 12
    }

    public enum UiSurfaceRole
    {
        Primary = 0,
        Overlay = 1,
        Diagnostic = 2
    }

    public static class UiCompositionPolicy
    {
        private static readonly UiSurfaceId[] Empty = Array.Empty<UiSurfaceId>();

        public static IReadOnlyList<UiSurfaceId> PrimarySurfaces(AppUiState state)
        {
            switch (state)
            {
                case AppUiState.Boot:
                case AppUiState.ErrorRecovery:
                    return new[] { UiSurfaceId.Status };
                case AppUiState.Transition:
                    return new[] { UiSurfaceId.SceneFlow };
                case AppUiState.Camp:
                case AppUiState.CampUpgrade:
                    return new[] { UiSurfaceId.Camp };
                case AppUiState.Expedition:
                case AppUiState.Interaction:
                    return Empty;
                case AppUiState.LearningActivity:
                    return new[] { UiSurfaceId.Learning };
                case AppUiState.Photography:
                case AppUiState.DiscoveryResult:
                    return new[] { UiSurfaceId.Photography };
                case AppUiState.Album:
                    return new[] { UiSurfaceId.Album };
                case AppUiState.Missions:
                    return new[] { UiSurfaceId.Missions };
                case AppUiState.Customization:
                    return new[] { UiSurfaceId.Customization };
                case AppUiState.Pause:
                case AppUiState.DevelopmentDiagnostics:
                    return new[] { UiSurfaceId.InputFoundation };
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public static bool IsVisible(AppUiState state, UiSurfaceId surface, bool tutorialVisible)
        {
            IReadOnlyList<UiSurfaceId> primary = PrimarySurfaces(state);
            for (int index = 0; index < primary.Count; index++)
            {
                if (primary[index] == surface) return true;
            }

            if (surface == UiSurfaceId.Tutorial)
                return tutorialVisible && state != AppUiState.Boot && state != AppUiState.ErrorRecovery &&
                       state != AppUiState.Transition && state != AppUiState.Pause &&
                       state != AppUiState.DevelopmentDiagnostics;
            if (surface == UiSurfaceId.Interaction)
                return state == AppUiState.Expedition || state == AppUiState.Interaction;
            return false;
        }

        public static UiSurfaceRole Role(UiSurfaceId surface)
        {
            switch (surface)
            {
                case UiSurfaceId.Interaction:
                case UiSurfaceId.Tutorial:
                    return UiSurfaceRole.Overlay;
                case UiSurfaceId.AudioDiagnostics:
                    return UiSurfaceRole.Diagnostic;
                default:
                    return UiSurfaceRole.Primary;
            }
        }

        public static int SortingOrder(UiSurfaceId surface)
        {
            switch (Role(surface))
            {
                case UiSurfaceRole.Primary: return 100;
                case UiSurfaceRole.Overlay: return surface == UiSurfaceId.Tutorial ? 300 : 200;
                case UiSurfaceRole.Diagnostic: return 400;
                default: throw new ArgumentOutOfRangeException(nameof(surface), surface, null);
            }
        }
    }
}
