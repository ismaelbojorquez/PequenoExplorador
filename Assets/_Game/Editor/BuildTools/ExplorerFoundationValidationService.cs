using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Presentation.Explorer;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class ExplorerFoundationValidationService
    {
        public static IReadOnlyList<string> Validate()
        {
            var violations = new List<string>();
            ValidatePackage(violations);
            ValidatePrefab(violations);
            ValidateJungleScene(violations);
            ValidateBootstrap(violations);
            return violations;
        }

        private static void ValidatePackage(ICollection<string> violations)
        {
            string manifest = File.ReadAllText("Packages/manifest.json");
            string lockFile = File.ReadAllText("Packages/packages-lock.json");
            if (!manifest.Contains("\"com.unity.ai.navigation\": \"2.0.9\""))
                violations.Add("EXPLORER001 manifest must pin com.unity.ai.navigation exactly to 2.0.9.");
            int packageStart = lockFile.IndexOf("\"com.unity.ai.navigation\"", StringComparison.Ordinal);
            int packageEnd = packageStart < 0
                ? -1
                : lockFile.IndexOf("\n    \"", packageStart + 1, StringComparison.Ordinal);
            int version = packageStart < 0
                ? -1
                : lockFile.IndexOf("\"version\": \"2.0.9\"", packageStart, StringComparison.Ordinal);
            if (packageStart < 0 || version < 0 || (packageEnd >= 0 && version >= packageEnd))
                violations.Add("EXPLORER002 packages-lock must resolve AI Navigation 2.0.9.");
        }

        private static void ValidatePrefab(ICollection<string> violations)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ExplorerFoundationSetup.PrefabPath);
            if (prefab == null)
            {
                violations.Add("EXPLORER003 missing PH_Explorer prefab; run ExplorerFoundationSetup.Apply.");
                return;
            }
            ExplorerLocomotionRoot root = prefab.GetComponent<ExplorerLocomotionRoot>();
            NavMeshAgent agent = prefab.GetComponent<NavMeshAgent>();
            if (root == null || agent == null)
                violations.Add("EXPLORER004 prefab requires one ExplorerLocomotionRoot and NavMeshAgent.");
            else
            {
                if (Math.Abs(agent.speed - 2.4f) > 0.001f || Math.Abs(agent.acceleration - 8f) > 0.001f ||
                    Math.Abs(agent.radius - 0.35f) > 0.001f || Math.Abs(agent.stoppingDistance - 0.18f) > 0.001f)
                    violations.Add("EXPLORER005 agent tuning drifted; expected speed=2.4 acceleration=8 radius=0.35 stop=0.18.");
            }
            if (prefab.GetComponentInChildren<Animator>(true) != null)
                violations.Add("EXPLORER006 placeholder must not use Animator/root motion; keep procedural motion until final rig.");
            if (!prefab.name.StartsWith("PH_", StringComparison.Ordinal))
                violations.Add("EXPLORER007 placeholder root must retain PH_ prefix metadata name.");
        }

        private static void ValidateJungleScene(ICollection<string> violations)
        {
            Scene scene = SceneManager.GetSceneByPath(SceneFlowFoundationSetup.JungleScenePath);
            bool opened = !scene.IsValid() || !scene.isLoaded;
            if (opened) scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.JungleScenePath, OpenSceneMode.Additive);
            GameObject[] roots = scene.GetRootGameObjects();
            ExplorerLocomotionRoot[] explorers = roots
                .SelectMany(item => item.GetComponentsInChildren<ExplorerLocomotionRoot>(true)).ToArray();
            NavMeshSurface[] surfaces = roots
                .SelectMany(item => item.GetComponentsInChildren<NavMeshSurface>(true)).ToArray();
            WalkableSurfaceMarker[] walkable = roots
                .SelectMany(item => item.GetComponentsInChildren<WalkableSurfaceMarker>(true)).ToArray();
            if (explorers.Length != 1) violations.Add($"EXPLORER008 Jungle must contain one explorer root; found {explorers.Length}.");
            if (surfaces.Length != 1 || surfaces[0].navMeshData == null)
                violations.Add($"EXPLORER009 Jungle must contain one baked NavMeshSurface; found {surfaces.Length}.");
            if (walkable.Length != 1) violations.Add($"EXPLORER010 Jungle must contain one explicit walkable marker; found {walkable.Length}.");
            if (AssetDatabase.LoadAssetAtPath<NavMeshData>(ExplorerFoundationSetup.NavMeshDataPath) == null)
                violations.Add("EXPLORER011 external PH_Jungle_NavMesh asset is missing.");
            if (opened) EditorSceneManager.CloseScene(scene, true);
        }

        private static void ValidateBootstrap(ICollection<string> violations)
        {
            Scene scene = SceneManager.GetSceneByPath(ProjectFoundationSetup.BootstrapScenePath);
            bool opened = !scene.IsValid() || !scene.isLoaded;
            if (opened) scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Additive);
            DiagnosticBootstrap bootstrap = scene.GetRootGameObjects()
                .SelectMany(item => item.GetComponentsInChildren<DiagnosticBootstrap>(true)).SingleOrDefault();
            if (bootstrap == null)
                violations.Add("EXPLORER012 Bootstrap root missing.");
            else if (new SerializedObject(bootstrap).FindProperty("_worldCamera").objectReferenceValue == null)
                violations.Add("EXPLORER013 Bootstrap world camera is not wired.");
            if (opened) EditorSceneManager.CloseScene(scene, true);
        }
    }
}
