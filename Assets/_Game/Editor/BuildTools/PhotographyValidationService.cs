using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PequenoExplorador.Presentation.Photography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class PhotographyValidationService
    {
        public static IReadOnlyList<string> Validate()
        {
            var violations = new List<string>();
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ToucanFixtureSetup.PrefabPath);
            PhotographableView target = prefab == null ? null : prefab.GetComponent<PhotographableView>();
            if (target == null) violations.Add("PHOTO001 approved toucan prefab requires PhotographableView.");
            else
            {
                if (target.RawDiscoveryId != PhotographyFoundationSetup.DiscoveryId) violations.Add("PHOTO002 target discovery ID mismatch.");
                if (target.PhotoAnchor == null || target.CandidateLocalBounds.size.sqrMagnitude <= 0.01f)
                    violations.Add("PHOTO003 target requires reviewed anchor/bounds.");
            }
            ValidateJungle(violations);
            ValidateBootstrap(violations);
            ValidateSourceGuardrails(violations);
            return violations;
        }

        private static void ValidateJungle(ICollection<string> violations)
        {
            Scene scene = SceneManager.GetSceneByPath(SceneFlowFoundationSetup.JungleScenePath);
            bool opened = !scene.IsValid() || !scene.isLoaded;
            if (opened) scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.JungleScenePath, OpenSceneMode.Additive);
            PhotographySceneRoot[] roots = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<PhotographySceneRoot>(true)).ToArray();
            PhotographableView[] targets = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<PhotographableView>(true)).ToArray();
            if (roots.Length != 1 || roots[0].TargetCount != 1) violations.Add($"PHOTO004 Jungle requires one photography root/target; found {roots.Length}/{targets.Length}.");
            else if (roots[0].ThumbnailWidth != 384 || roots[0].ThumbnailHeight != 216 ||
                     roots[0].ThumbnailFormat != RenderTextureFormat.ARGB32)
                violations.Add("PHOTO012 Jungle thumbnail profile must be 384x216 ARGB32 until reprofiled.");
            if (targets.Length != 1 || targets[0].RawDiscoveryId != PhotographyFoundationSetup.DiscoveryId)
                violations.Add("PHOTO005 Jungle photographable must be the approved toucan.");
            if (opened) EditorSceneManager.CloseScene(scene, true);
        }

        private static void ValidateBootstrap(ICollection<string> violations)
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            PhotographyView[] views = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<PhotographyView>(true)).ToArray();
            if (views.Length != 1) violations.Add($"PHOTO006 Bootstrap requires one photography view; found {views.Length}.");
            else
            {
                if (views[0].ShutterButton == null || views[0].ExitButton == null) violations.Add("PHOTO007 view requires shutter/exit.");
                else if (views[0].ShutterButton.GetComponent<RectTransform>().rect.width < 64f ||
                         views[0].ExitButton.GetComponent<RectTransform>().rect.width < 64f)
                    violations.Add("PHOTO008 photography touch targets must be at least 64 logical units.");
            }
        }

        private static void ValidateSourceGuardrails(ICollection<string> violations)
        {
            string presentation = Path.Combine(Directory.GetCurrentDirectory(), "Assets/_Game/Presentation/Photography");
            foreach (string file in Directory.GetFiles(presentation, "*.cs", SearchOption.AllDirectories))
                if (File.ReadAllText(file).Contains("ScreenCapture")) violations.Add("PHOTO009 ScreenCapture is forbidden; use bounded RenderTexture.");
            foreach (string file in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "*.xml", SearchOption.AllDirectories))
                if (File.ReadAllText(file).Contains("android.permission.CAMERA")) violations.Add("PHOTO010 device CAMERA permission is forbidden.");
            if (UnityPhotoThumbnailRenderer.DefaultWidth != 384 || UnityPhotoThumbnailRenderer.DefaultHeight != 216)
                violations.Add("PHOTO011 thumbnail contract must remain 384x216 unless reprofiled.");
        }
    }
}
