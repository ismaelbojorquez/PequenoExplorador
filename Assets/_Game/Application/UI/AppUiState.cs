using PequenoExplorador.Application.Input;

namespace PequenoExplorador.Application.UI
{
    public enum AppUiState
    {
        Boot = 0,
        Transition = 1,
        Camp = 2,
        Expedition = 3,
        Interaction = 4,
        LearningActivity = 5,
        Photography = 6,
        DiscoveryResult = 7,
        Album = 8,
        Missions = 9,
        CampUpgrade = 10,
        Customization = 11,
        Pause = 12,
        ErrorRecovery = 13,
        DevelopmentDiagnostics = 14
    }

    public enum UiBackAction
    {
        Ignore = 0,
        OpenPause = 1,
        CloseSurface = 2,
        Resume = 3,
        RetryOrStay = 4,
        CloseDiagnostics = 5
    }

    public static class AppUiStatePolicy
    {
        public static InputMapId InputMap(AppUiState state)
        {
            switch (state)
            {
                case AppUiState.Expedition:
                case AppUiState.Interaction:
                    return InputMapId.Explorer;
                case AppUiState.Photography:
                case AppUiState.DiscoveryResult:
                    return InputMapId.Photography;
                default:
                    return InputMapId.UI;
            }
        }

        public static UiBackAction BackAction(AppUiState state)
        {
            switch (state)
            {
                case AppUiState.Boot:
                case AppUiState.Transition:
                    return UiBackAction.Ignore;
                case AppUiState.Camp:
                case AppUiState.Expedition:
                case AppUiState.Interaction:
                    return UiBackAction.OpenPause;
                case AppUiState.LearningActivity:
                case AppUiState.Photography:
                case AppUiState.DiscoveryResult:
                case AppUiState.Album:
                case AppUiState.Missions:
                case AppUiState.CampUpgrade:
                case AppUiState.Customization:
                    return UiBackAction.CloseSurface;
                case AppUiState.Pause:
                    return UiBackAction.Resume;
                case AppUiState.ErrorRecovery:
                    return UiBackAction.RetryOrStay;
                case AppUiState.DevelopmentDiagnostics:
                    return UiBackAction.CloseDiagnostics;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
