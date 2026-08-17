using System;
using System.Linq;
using PequenoExplorador.Application.UI;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Album;
using PequenoExplorador.Presentation.Audio;
using PequenoExplorador.Presentation.Bootstrap;
using PequenoExplorador.Presentation.Camp;
using PequenoExplorador.Presentation.Customization;
using PequenoExplorador.Presentation.Economy;
using PequenoExplorador.Presentation.Input;
using PequenoExplorador.Presentation.Interaction;
using PequenoExplorador.Presentation.Learning;
using PequenoExplorador.Presentation.Missions;
using PequenoExplorador.Presentation.Photography;
using PequenoExplorador.Presentation.SceneFlow;
using PequenoExplorador.Presentation.Tutorial;
using PequenoExplorador.Presentation.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class GateBUiCompositionSetup
    {
        public const string BootstrapPath = "Assets/_Game/Bootstrap/Bootstrap.unity";

        [MenuItem("Pequeño Explorador/Development/Gate B/Apply UI Composition")]
        public static void Apply()
        {
            try
            {
                Scene scene = EditorSceneManager.OpenScene(BootstrapPath, OpenSceneMode.Single);
                DiagnosticBootstrap bootstrap = FindSingle<DiagnosticBootstrap>(scene);
                UiCompositionCoordinator coordinator = bootstrap.GetComponent<UiCompositionCoordinator>() ??
                                                     bootstrap.gameObject.AddComponent<UiCompositionCoordinator>();
                SurfaceLifecycleAdapter lifecycle = bootstrap.GetComponent<SurfaceLifecycleAdapter>() ??
                                                    bootstrap.gameObject.AddComponent<SurfaceLifecycleAdapter>();

                UiCompositionCoordinator.SurfaceBinding[] surfaces =
                {
                    Binding(UiSurfaceId.Status, ResolveStatusCanvasRoot(FindSingle<BootstrapStatusView>(scene))),
                    Binding(UiSurfaceId.SceneFlow, FindSingle<SceneTransitionView>(scene).gameObject),
                    Binding(UiSurfaceId.Camp, FindSingle<CampHubView>(scene).gameObject),
                    Binding(UiSurfaceId.Interaction, FindSingle<InteractionPromptView>(scene).gameObject),
                    Binding(UiSurfaceId.Learning, FindSingle<LearningActivityView>(scene).gameObject),
                    Binding(UiSurfaceId.Photography, FindSingle<PhotographyView>(scene).gameObject),
                    Binding(UiSurfaceId.Album, FindSingle<AlbumView>(scene).gameObject),
                    Binding(UiSurfaceId.Missions, FindSingle<MissionView>(scene).gameObject),
                    Binding(UiSurfaceId.Economy, FindSingle<EconomyView>(scene).gameObject),
                    Binding(UiSurfaceId.Customization, FindSingle<CustomizationView>(scene).gameObject),
                    Binding(UiSurfaceId.InputFoundation, FindSingle<InputPauseView>(scene).gameObject),
                    Binding(UiSurfaceId.Tutorial, FindSingle<TutorialView>(scene).gameObject),
                    Binding(UiSurfaceId.AudioDiagnostics, FindSingle<AudioDiagnosticView>(scene).gameObject)
                };
                coordinator.ConfigureForEditorAndTests(surfaces);
                Camera worldCamera = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<Camera>(true))
                    .Single(value => value.CompareTag("MainCamera"));
                lifecycle.ConfigureForEditorAndTests(worldCamera,
                    scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<Canvas>(true)).ToArray());
                bootstrap.ConfigureUiCompositionForEditorAndTests(coordinator, lifecycle);
                var serializedBootstrap = new SerializedObject(bootstrap);
                SafeAreaFitter[] fitters = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<SafeAreaFitter>(true)).ToArray();
                SerializedProperty fittersProperty = serializedBootstrap.FindProperty("_safeAreaFitters");
                fittersProperty.arraySize = fitters.Length;
                for (int index = 0; index < fitters.Length; index++)
                    fittersProperty.GetArrayElementAtIndex(index).objectReferenceValue = fitters[index];
                serializedBootstrap.ApplyModifiedPropertiesWithoutUndo();

                EditorUtility.SetDirty(coordinator);
                EditorUtility.SetDirty(lifecycle);
                EditorUtility.SetDirty(bootstrap);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("PE_GATE_B_UI_COMPOSITION_SETUP_OK surfaces=13 diagnostics=closed-default");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        private static UiCompositionCoordinator.SurfaceBinding Binding(UiSurfaceId id, GameObject root)
        {
            if (root == null) throw new InvalidOperationException("UI surface root is missing for " + id + ".");
            if (root.GetComponent<Canvas>() == null) root.AddComponent<Canvas>();
            SafeAreaFitter[] fitters = root.GetComponentsInChildren<SafeAreaFitter>(true);
            if (fitters.Length == 0) root.AddComponent<SafeAreaFitter>();
            else if (fitters.Length > 1 && root.TryGetComponent(out SafeAreaFitter rootFitter))
                UnityEngine.Object.DestroyImmediate(rootFitter);
            if (root.GetComponentsInChildren<SafeAreaFitter>(true).Length != 1)
                throw new InvalidOperationException($"UI surface {id} requires exactly one SafeAreaFitter in its Canvas hierarchy.");
            if (root.GetComponent<CanvasGroup>() == null) root.AddComponent<CanvasGroup>();
            if (root.GetComponent<GraphicRaycaster>() == null) root.AddComponent<GraphicRaycaster>();
            return new UiCompositionCoordinator.SurfaceBinding(id, root);
        }

        private static GameObject ResolveStatusCanvasRoot(BootstrapStatusView view)
        {
            GameObject viewRoot = view.gameObject;
            // Early remediation iterations temporarily promoted this content node to a nested Canvas.
            // Restore the established Diagnostic Canvas ownership before binding the Status surface.
            if (viewRoot.TryGetComponent(out GraphicRaycaster nestedRaycaster))
                UnityEngine.Object.DestroyImmediate(nestedRaycaster);
            if (viewRoot.TryGetComponent(out Canvas nestedCanvas))
                UnityEngine.Object.DestroyImmediate(nestedCanvas);
            if (viewRoot.TryGetComponent(out SafeAreaFitter nestedFitter))
                UnityEngine.Object.DestroyImmediate(nestedFitter);
            if (viewRoot.TryGetComponent(out CanvasGroup nestedGroup))
                UnityEngine.Object.DestroyImmediate(nestedGroup);

            Canvas owner = viewRoot.GetComponentInParent<Canvas>(true);
            if (owner == null) throw new InvalidOperationException("Status view requires its established parent Canvas.");
            return owner.gameObject;
        }

        private static T FindSingle<T>(Scene scene) where T : Component
        {
            T[] values = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<T>(true)).ToArray();
            if (values.Length != 1) throw new InvalidOperationException($"Bootstrap requires one {typeof(T).Name}; found {values.Length}.");
            return values[0];
        }
    }
}
