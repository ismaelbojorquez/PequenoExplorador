using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Input;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Input;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class InputFoundationSetup
    {
        public const string ActionsPath = "Assets/_Game/Content/Input/PequenoExploradorInputActions.inputactions";
        public const string ThresholdsPath = "Assets/_Game/Content/Input/GestureThresholds.asset";
        private const string BootstrapPath = "Assets/_Game/Bootstrap/Bootstrap.unity";

        [MenuItem("Pequeño Explorador/Development/Input/Apply Foundation")]
        public static void Apply()
        {
            try
            {
                EnsureFolder("Assets/_Game/Content/Input");
                GestureThresholdsAsset thresholds = EnsureThresholds();
                AssetDatabase.ImportAsset(ActionsPath, ImportAssetOptions.ForceUpdate);
                InputActionAsset actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(ActionsPath);
                if (actions == null) throw new InvalidOperationException("Input actions asset did not import: " + ActionsPath);

                Scene scene = EditorSceneManager.OpenScene(BootstrapPath, OpenSceneMode.Single);
                DiagnosticBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<DiagnosticBootstrap>();
                if (bootstrap == null) throw new InvalidOperationException("Bootstrap scene has no DiagnosticBootstrap.");

                GameObject previous = GameObject.Find("PH_UI_INPUT_FOUNDATION");
                if (previous != null) UnityEngine.Object.DestroyImmediate(previous);
                GameObject foundation = CreateCanvas("PH_UI_INPUT_FOUNDATION", 250);
                RectTransform inputSafeRoot = CreateSafeRoot(foundation.GetComponent<Canvas>());
                InputPauseView pauseView = CreatePauseView(foundation, inputSafeRoot);
                TouchDiagnosticOverlay touchOverlay = CreateTouchOverlay(foundation, inputSafeRoot);
                DeviceAspectOverlay aspectOverlay = CreateAspectOverlay(inputSafeRoot);

                var fitters = new List<SafeAreaFitter>();
                foreach (Canvas canvas in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    RectTransform safeRoot = canvas == foundation.GetComponent<Canvas>()
                        ? inputSafeRoot
                        : WrapCanvas(canvas);
                    SafeAreaFitter fitter = safeRoot.GetComponent<SafeAreaFitter>() ?? safeRoot.gameObject.AddComponent<SafeAreaFitter>();
                    fitters.Add(fitter);
                }

                bootstrap.ConfigureInputForEditorAndTests(
                    actions,
                    thresholds,
                    fitters.ToArray(),
                    pauseView,
                    touchOverlay,
                    aspectOverlay);
                EditorUtility.SetDirty(bootstrap);

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("PE_INPUT_SETUP_OK maps=UI,Explorer,Photography,Parents,Debug safeArea=central haptics=noop");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        private static GestureThresholdsAsset EnsureThresholds()
        {
            GestureThresholdsAsset asset = AssetDatabase.LoadAssetAtPath<GestureThresholdsAsset>(ThresholdsPath);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<GestureThresholdsAsset>();
            asset.name = "GestureThresholds";
            AssetDatabase.CreateAsset(asset, ThresholdsPath);
            return asset;
        }

        private static GameObject CreateCanvas(string name, int sortingOrder)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            return root;
        }

        private static RectTransform CreateSafeRoot(Canvas canvas)
        {
            var root = new GameObject("Safe Area", typeof(RectTransform));
            root.transform.SetParent(canvas.transform, false);
            RectTransform rect = (RectTransform)root.transform;
            Stretch(rect);
            root.AddComponent<SafeAreaFitter>();
            return rect;
        }

        private static RectTransform WrapCanvas(Canvas canvas)
        {
            Transform existing = canvas.transform.Find("Safe Area");
            RectTransform safe = existing as RectTransform;
            if (safe == null) safe = CreateSafeRoot(canvas);
            var children = new List<Transform>();
            for (int index = 0; index < canvas.transform.childCount; index++)
            {
                Transform child = canvas.transform.GetChild(index);
                if (child != safe) children.Add(child);
            }
            foreach (Transform child in children) child.SetParent(safe, true);
            Stretch(safe);
            return safe;
        }

        private static InputPauseView CreatePauseView(GameObject foundation, RectTransform safeRoot)
        {
            GameObject panel = CreatePanel(safeRoot, "Pause Panel", new Color(0.02f, 0.06f, 0.08f, 0.96f));
            Text title = CreateText(panel.transform, "Pause Title", 48, TextAnchor.MiddleCenter);
            SetRect(title.rectTransform, new Vector2(0.2f, 0.58f), new Vector2(0.8f, 0.76f));
            Button resume = CreateButton(panel.transform, "Resume", "Continuar");
            SetRect((RectTransform)resume.transform, new Vector2(0.36f, 0.32f), new Vector2(0.64f, 0.48f));
            InputPauseView view = foundation.AddComponent<InputPauseView>();
            var serialized = new SerializedObject(view);
            serialized.FindProperty("_panel").objectReferenceValue = panel;
            serialized.FindProperty("_title").objectReferenceValue = title;
            serialized.FindProperty("_resumeButton").objectReferenceValue = resume;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            panel.SetActive(false);
            return view;
        }

        private static TouchDiagnosticOverlay CreateTouchOverlay(GameObject foundation, RectTransform safeRoot)
        {
            GameObject root = CreatePanel(safeRoot, "Development Touch Overlay", new Color(0f, 0f, 0f, 0.28f));
            SetRect((RectTransform)root.transform, new Vector2(0f, 0f), new Vector2(1f, 1f));
            root.GetComponent<Image>().raycastTarget = false;
            var markerObject = new GameObject("Touch Marker", typeof(RectTransform), typeof(Image));
            markerObject.transform.SetParent(root.transform, false);
            RectTransform marker = (RectTransform)markerObject.transform;
            marker.sizeDelta = new Vector2(72f, 72f);
            marker.GetComponent<Image>().color = new Color(0.2f, 0.85f, 1f, 0.7f);
            marker.GetComponent<Image>().raycastTarget = false;
            Text summary = CreateText(root.transform, "Input Summary", 24, TextAnchor.UpperLeft);
            summary.raycastTarget = false;
            SetRect(summary.rectTransform, new Vector2(0.02f, 0.88f), new Vector2(0.7f, 0.98f));
            TouchDiagnosticOverlay overlay = foundation.AddComponent<TouchDiagnosticOverlay>();
            var serialized = new SerializedObject(overlay);
            serialized.FindProperty("_root").objectReferenceValue = root;
            serialized.FindProperty("_marker").objectReferenceValue = marker;
            serialized.FindProperty("_summary").objectReferenceValue = summary;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            root.SetActive(false);
            return overlay;
        }

        private static DeviceAspectOverlay CreateAspectOverlay(RectTransform safeRoot)
        {
            Text label = CreateText(safeRoot, "Device Aspect Preview", 20, TextAnchor.UpperRight);
            label.raycastTarget = false;
            SetRect(label.rectTransform, new Vector2(0.55f, 0.92f), new Vector2(0.98f, 0.98f));
            DeviceAspectOverlay overlay = label.gameObject.AddComponent<DeviceAspectOverlay>();
            var serialized = new SerializedObject(overlay);
            serialized.FindProperty("_label").objectReferenceValue = label;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return overlay;
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            Stretch((RectTransform)panel.transform);
            panel.GetComponent<Image>().color = color;
            return panel;
        }

        private static Text CreateText(Transform parent, string name, int fontSize, TextAnchor alignment)
        {
            var child = new GameObject(name, typeof(RectTransform), typeof(Text));
            child.transform.SetParent(parent, false);
            Text text = child.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 16;
            return text;
        }

        private static Button CreateButton(Transform parent, string name, string label)
        {
            GameObject buttonObject = DefaultControls.CreateButton(new DefaultControls.Resources());
            buttonObject.name = name;
            buttonObject.transform.SetParent(parent, false);
            Text text = buttonObject.GetComponentInChildren<Text>();
            text.text = string.Empty;
            text.fontSize = 30;
            text.resizeTextForBestFit = true;
            return buttonObject.GetComponent<Button>();
        }

        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void Stretch(RectTransform rect) => SetRect(rect, Vector2.zero, Vector2.one);

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/');
            string leaf = System.IO.Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, leaf);
        }
    }
}
