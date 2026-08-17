using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.UI;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Presentation.UI;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class UiCompositionValidationService
    {
        public static IReadOnlyList<string> Validate()
        {
            var violations = new List<string>();
            Scene scene = SceneManager.GetSceneByPath(GateBUiCompositionSetup.BootstrapPath);
            bool opened = !scene.IsValid() || !scene.isLoaded;
            if (opened) scene = EditorSceneManager.OpenScene(GateBUiCompositionSetup.BootstrapPath, OpenSceneMode.Additive);
            try
            {
                GameObject[] roots = scene.GetRootGameObjects();
                DiagnosticBootstrap[] bootstraps = roots.SelectMany(root => root.GetComponentsInChildren<DiagnosticBootstrap>(true)).ToArray();
                UiCompositionCoordinator[] coordinators = roots.SelectMany(root => root.GetComponentsInChildren<UiCompositionCoordinator>(true)).ToArray();
                if (bootstraps.Length != 1) violations.Add($"UISTATE001 Bootstrap requires one composition root; found {bootstraps.Length}.");
                if (coordinators.Length != 1) violations.Add($"UISTATE002 Bootstrap requires one UiCompositionCoordinator; found {coordinators.Length}.");
                if (roots.SelectMany(root => root.GetComponentsInChildren<EventSystem>(true)).Count() != 1)
                    violations.Add("UISTATE003 Bootstrap requires exactly one EventSystem.");
                if (coordinators.Length == 1)
                {
                    UiCompositionCoordinator coordinator = coordinators[0];
                    try { coordinator.ValidateOrThrow(); }
                    catch (Exception exception) { violations.Add("UISTATE004 " + exception.Message); }
                    foreach (UiCompositionCoordinator.SurfaceBinding surface in coordinator.Surfaces)
                    {
                        if (surface?.Root == null) continue;
                        if (surface.Canvas == null || surface.Canvas.gameObject != surface.Root)
                            violations.Add($"UISTATE005 {surface.Id} lacks a dedicated Canvas on its root.");
                        if (surface.CanvasGroup == null) violations.Add($"UISTATE005 {surface.Id} lacks CanvasGroup.");
                        if (surface.Raycaster == null) violations.Add($"UISTATE006 {surface.Id} lacks GraphicRaycaster.");
                    }
                }

                foreach (AppUiState state in Enum.GetValues(typeof(AppUiState)))
                {
                    IReadOnlyList<UiSurfaceId> primary = UiCompositionPolicy.PrimarySurfaces(state);
                    if (primary.Count > 1) violations.Add($"UISTATE007 {state} declares {primary.Count} primary UI surfaces.");
                    _ = AppUiStatePolicy.InputMap(state);
                }

                if (UiCompositionPolicy.IsVisible(AppUiState.Camp, UiSurfaceId.AudioDiagnostics, false) ||
                    UiCompositionPolicy.IsVisible(AppUiState.Camp, UiSurfaceId.Economy, false) ||
                    UiCompositionPolicy.IsVisible(AppUiState.Camp, UiSurfaceId.Status, false))
                    violations.Add("UISTATE008 Camp exposes diagnostic/legacy surfaces by default.");
                return violations;
            }
            finally
            {
                if (opened) EditorSceneManager.CloseScene(scene, removeScene: true);
            }
        }
    }
}
