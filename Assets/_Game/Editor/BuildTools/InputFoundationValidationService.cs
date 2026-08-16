using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PequenoExplorador.Content.Input;
using PequenoExplorador.Presentation.Accessibility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PequenoExplorador.Bootstrap;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class InputFoundationValidationService
    {
        private static readonly string[] RequiredMaps = { "UI", "Explorer", "Photography", "Parents", "Debug" };

        public static IReadOnlyList<string> Validate()
        {
            var violations = new List<string>();
            InputActionAsset actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputFoundationSetup.ActionsPath);
            if (actions == null)
            {
                violations.Add("INPUT001 missing InputActionAsset.");
            }
            else
            {
                foreach (string mapName in RequiredMaps)
                {
                    InputActionMap map = actions.FindActionMap(mapName);
                    if (map == null) { violations.Add("INPUT002 missing map " + mapName); continue; }
                    if (mapName == "Debug")
                    {
                        if (map.FindAction("ToggleOverlay") == null) violations.Add("INPUT003 Debug.ToggleOverlay missing.");
                    }
                    else
                    {
                        foreach (string actionName in new[] { "Point", "PrimaryPress", "Back" })
                            if (map.FindAction(actionName) == null) violations.Add($"INPUT004 {mapName}.{actionName} missing.");
                    }
                }
            }

            GestureThresholdsAsset thresholds = AssetDatabase.LoadAssetAtPath<GestureThresholdsAsset>(InputFoundationSetup.ThresholdsPath);
            if (thresholds == null) violations.Add("INPUT005 missing gesture thresholds asset.");
            else
            {
                try { thresholds.ToRuntime(); }
                catch (Exception exception) { violations.Add("INPUT006 invalid thresholds: " + exception.Message); }
            }

            string[] runtimeSources = Directory.GetFiles("Assets/_Game", "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.Contains("/Editor/") && !path.Contains("/Tests/"))
                .ToArray();
            foreach (string path in runtimeSources)
            {
                string source = File.ReadAllText(path);
                if (source.Contains("UnityEngine.Input.") || source.Contains("Input.Get") || source.Contains("Touchscreen.current"))
                    violations.Add("INPUT007 prohibited input API in " + path);
            }

            const string bootstrapPath = "Assets/_Game/Bootstrap/Bootstrap.unity";
            Scene scene = SceneManager.GetSceneByPath(bootstrapPath);
            bool openedForValidation = !scene.IsValid() || !scene.isLoaded;
            if (openedForValidation) scene = EditorSceneManager.OpenScene(bootstrapPath, OpenSceneMode.Additive);
            Canvas.ForceUpdateCanvases();
            GameObject[] roots = scene.GetRootGameObjects();
            DiagnosticBootstrap bootstrap = roots.SelectMany(root => root.GetComponentsInChildren<DiagnosticBootstrap>(true)).FirstOrDefault();
            if (bootstrap == null) violations.Add("INPUT010 Bootstrap root missing.");
            else
            {
                var serializedBootstrap = new SerializedObject(bootstrap);
                if (serializedBootstrap.FindProperty("_inputActions").objectReferenceValue == null ||
                    serializedBootstrap.FindProperty("_gestureThresholds").objectReferenceValue == null)
                    violations.Add("INPUT011 Bootstrap input assets are not wired.");
            }
            Canvas[] canvases = roots.SelectMany(root => root.GetComponentsInChildren<Canvas>(true)).ToArray();
            foreach (Canvas canvas in canvases)
            {
                SafeAreaFitter[] fitters = canvas.GetComponentsInChildren<SafeAreaFitter>(true);
                if (fitters.Length != 1) violations.Add($"INPUT008 canvas {canvas.name} must own exactly one SafeAreaFitter; found {fitters.Length}.");
            }
            foreach (Button button in roots.SelectMany(root => root.GetComponentsInChildren<Button>(true)))
            {
                Rect rect = ((RectTransform)button.transform).rect;
                if (rect.width < 64f || rect.height < 64f)
                    violations.Add($"INPUT009 touch target {button.name} is {rect.width:0}x{rect.height:0}; minimum 64x64 reference pixels.");
            }
            if (openedForValidation) EditorSceneManager.CloseScene(scene, removeScene: true);
            return violations;
        }
    }
}
