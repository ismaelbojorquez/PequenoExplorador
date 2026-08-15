using System;
using PequenoExplorador.Bootstrap;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor
{
    public static class ProjectFoundationSetup
    {
        public const string BootstrapScenePath = "Assets/_Game/Bootstrap/Bootstrap.unity";

        private const string RendererPath = "Assets/_Game/Content/Rendering/MobileRenderer.asset";
        private const string PipelinePath = "Assets/_Game/Content/Rendering/MobileURP.asset";
        private const string ApplicationIdentifier = "com.placeholder.pequenoexplorador";

        public static void Apply()
        {
            EnsureFolders();
            UniversalRenderPipelineAsset pipeline = CreateOrUpdatePipeline();
            ConfigureProject(pipeline);
            MoveGeneratedRenderingAssets();
            CreateBootstrapScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("PE_FOUNDATION_SETUP_OK");
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets/_Game/Content/Rendering");
        }

        private static void EnsureFolder(string path)
        {
            string[] segments = path.Split('/');
            string current = segments[0];

            for (int index = 1; index < segments.Length; index++)
            {
                string next = $"{current}/{segments[index]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[index]);
                }

                current = next;
            }
        }

        private static UniversalRenderPipelineAsset CreateOrUpdatePipeline()
        {
            UniversalRendererData renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath);
            if (renderer == null)
            {
                renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, RendererPath);
            }

            UniversalRenderPipelineAsset pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (pipeline == null)
            {
                pipeline = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(pipeline, PipelinePath);
            }

            pipeline.name = "MobileURP";
            pipeline.supportsHDR = false;
            pipeline.msaaSampleCount = 2;
            pipeline.renderScale = 1f;
            pipeline.shadowDistance = 20f;

            SerializedObject serializedPipeline = new SerializedObject(pipeline);
            serializedPipeline.FindProperty("m_MainLightShadowsSupported").boolValue = true;
            serializedPipeline.FindProperty("m_AdditionalLightsRenderingMode").intValue = (int)LightRenderingMode.PerVertex;
            serializedPipeline.FindProperty("m_AdditionalLightShadowsSupported").boolValue = false;
            serializedPipeline.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(renderer);
            EditorUtility.SetDirty(pipeline);
            return pipeline;
        }

        private static void ConfigureProject(RenderPipelineAsset pipeline)
        {
            PlayerSettings.companyName = "Placeholder Studio";
            PlayerSettings.productName = DiagnosticBootstrap.ProductName;
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, ApplicationIdentifier);
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, ApplicationIdentifier);
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel36;
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
            PlayerSettings.Android.forceInternetPermission = false;
            PlayerSettings.Android.forceSDCardPermission = false;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);

            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;
            QualitySettings.vSyncCount = 1;
        }

        private static void MoveGeneratedRenderingAssets()
        {
            MoveAssetIfPresent(
                "Assets/UniversalRenderPipelineGlobalSettings.asset",
                "Assets/_Game/Content/Rendering/UniversalRenderPipelineGlobalSettings.asset");
            MoveAssetIfPresent(
                "Assets/DefaultVolumeProfile.asset",
                "Assets/_Game/Content/Rendering/DefaultVolumeProfile.asset");
        }

        private static void MoveAssetIfPresent(string source, string destination)
        {
            if (AssetDatabase.LoadMainAssetAtPath(source) == null || AssetDatabase.LoadMainAssetAtPath(destination) != null)
            {
                return;
            }

            string error = AssetDatabase.MoveAsset(source, destination);
            if (!string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException($"Could not move {source} to {destination}: {error}");
            }
        }

        private static void CreateBootstrapScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.035f, 0.12f, 0.10f, 1f);
            camera.orthographic = true;

            GameObject canvasObject = new GameObject("Diagnostic Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject panelObject = CreateUiObject(DiagnosticBootstrap.PlaceholderObjectName, canvasObject.transform);
            Image panel = panelObject.AddComponent<Image>();
            panel.color = new Color(0.055f, 0.22f, 0.18f, 1f);
            Stretch(panel.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            panelObject.AddComponent<DiagnosticBootstrap>();

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            CreateText(panelObject.transform, "Product Name", DiagnosticBootstrap.ProductName, font, 62, new Vector2(0.08f, 0.52f), new Vector2(0.92f, 0.72f));
            CreateText(panelObject.transform, "Development Version", DiagnosticBootstrap.DevelopmentVersion, font, 34, new Vector2(0.08f, 0.37f), new Vector2(0.92f, 0.50f));
            CreateText(panelObject.transform, "Temporary Notice", "DIAGNÓSTICO TEMPORAL · SIN GAMEPLAY", font, 24, new Vector2(0.08f, 0.20f), new Vector2(0.92f, 0.32f));

            if (!EditorSceneManager.SaveScene(scene, BootstrapScenePath))
            {
                throw new InvalidOperationException($"Could not save {BootstrapScenePath}.");
            }

            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(BootstrapScenePath, true) };
            Selection.activeGameObject = panelObject;
        }

        private static GameObject CreateUiObject(string name, Transform parent)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            return gameObject;
        }

        private static void CreateText(
            Transform parent,
            string objectName,
            string value,
            Font font,
            int fontSize,
            Vector2 anchorMin,
            Vector2 anchorMax)
        {
            GameObject textObject = CreateUiObject(objectName, parent);
            Text text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = font;
            text.fontSize = fontSize;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.96f, 0.94f, 0.76f, 1f);
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 16;
            text.resizeTextMaxSize = fontSize;
            Stretch(text.rectTransform, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
        }

        private static void Stretch(
            RectTransform rectTransform,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 offsetMin,
            Vector2 offsetMax)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;
        }
    }
}
