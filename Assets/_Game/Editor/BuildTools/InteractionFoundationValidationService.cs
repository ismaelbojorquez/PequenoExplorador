using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Interaction;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class InteractionFoundationValidationService
    {
        public static IReadOnlyList<string> Validate()
        {
            var violations = new List<string>();
            ValidateJungle(violations);
            ValidatePrompt(violations);
            ValidateRuntimeSources(violations);
            return violations;
        }

        private static void ValidateJungle(ICollection<string> violations)
        {
            Scene scene = SceneManager.GetSceneByPath(SceneFlowFoundationSetup.JungleScenePath);
            bool opened = !scene.IsValid() || !scene.isLoaded;
            if (opened) scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.JungleScenePath, OpenSceneMode.Additive);
            GameObject[] roots = scene.GetRootGameObjects();
            InteractionSceneRoot[] sceneRoots = roots
                .SelectMany(item => item.GetComponentsInChildren<InteractionSceneRoot>(true)).ToArray();
            WorldInteractableView[] targets = roots
                .SelectMany(item => item.GetComponentsInChildren<WorldInteractableView>(true)).ToArray();
            if (sceneRoots.Length != 1)
                violations.Add($"INTERACTION101 Jungle requires one interaction root; found {sceneRoots.Length}.");
            if (targets.Length < 3)
                violations.Add($"INTERACTION102 Jungle requires at least three neutral baseline fixtures; found {targets.Length}.");
            foreach (string required in new[]
                     {
                         InteractionFoundationSetup.AnimalId,
                         "interaction.fixture.plant",
                         "interaction.fixture.object"
                     })
            {
                if (!targets.Any(item => string.Equals(item.RawInteractionId, required, StringComparison.Ordinal)))
                    violations.Add($"INTERACTION115 Jungle is missing required baseline fixture '{required}'.");
            }
            if (targets.Select(item => item.RawInteractionId).Distinct(StringComparer.Ordinal).Count() != targets.Length)
                violations.Add("INTERACTION103 fixture IDs must be unique.");
            foreach (WorldInteractableView target in targets)
            {
                bool isApprovedToucan = string.Equals(target.RawInteractionId, InteractionFoundationSetup.AnimalId, StringComparison.Ordinal);
                if (!isApprovedToucan && !target.name.StartsWith("PH_", StringComparison.Ordinal))
                    violations.Add($"INTERACTION104 placeholder fixture '{target.name}' must retain PH_ prefix.");
                if (isApprovedToucan && target.name.StartsWith("PH_", StringComparison.Ordinal))
                    violations.Add($"INTERACTION116 approved toucan fixture '{target.name}' cannot retain a PH_ prefix.");
                if (target.InteractionPoint == null || target.TargetColliders.Count == 0)
                {
                    violations.Add($"INTERACTION105 fixture '{target.name}' needs interaction point/collider.");
                    continue;
                }
                if (!NavMesh.SamplePosition(target.InteractionPoint.position, out _, 0.8f, NavMesh.AllAreas))
                    violations.Add($"INTERACTION106 point for '{target.name}' is outside Jungle NavMesh.");
                foreach (Collider collider in target.TargetColliders)
                {
                    if (collider == null || !collider.isTrigger || collider.bounds.size.magnitude < 1.2f)
                        violations.Add($"INTERACTION107 '{target.name}' needs a large trigger target volume.");
                }
            }
            if (opened) EditorSceneManager.CloseScene(scene, true);
        }

        private static void ValidatePrompt(ICollection<string> violations)
        {
            Scene scene = SceneManager.GetSceneByPath(ProjectFoundationSetup.BootstrapScenePath);
            bool opened = !scene.IsValid() || !scene.isLoaded;
            if (opened) scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Additive);
            GameObject[] roots = scene.GetRootGameObjects();
            InteractionPromptView[] prompts = roots
                .SelectMany(item => item.GetComponentsInChildren<InteractionPromptView>(true)).ToArray();
            if (prompts.Length != 1)
            {
                violations.Add($"INTERACTION108 Bootstrap requires one prompt view; found {prompts.Length}.");
            }
            else
            {
                InteractionPromptView prompt = prompts[0];
                if (prompt.GetComponentInChildren<SafeAreaFitter>(true) == null)
                    violations.Add("INTERACTION109 prompt must be owned by a SafeAreaFitter.");
                foreach (Button button in new[] { prompt.ActionButton, prompt.CancelButton })
                {
                    if (button == null || ((RectTransform)button.transform).rect.width < 64f ||
                        ((RectTransform)button.transform).rect.height < 64f)
                        violations.Add("INTERACTION110 prompt action/cancel targets must be at least 64x64 reference pixels.");
                }
            }
            DiagnosticBootstrap bootstrap = roots
                .SelectMany(item => item.GetComponentsInChildren<DiagnosticBootstrap>(true)).SingleOrDefault();
            if (bootstrap == null)
                violations.Add("INTERACTION111 Bootstrap root missing.");
            else
            {
                var serialized = new SerializedObject(bootstrap);
                if (serialized.FindProperty("_interactionCatalog").objectReferenceValue == null ||
                    serialized.FindProperty("_interactionPrompt").objectReferenceValue == null)
                    violations.Add("INTERACTION112 Bootstrap interaction catalog/prompt is not wired.");
            }
            if (opened) EditorSceneManager.CloseScene(scene, true);
        }

        private static void ValidateRuntimeSources(ICollection<string> violations)
        {
            string applicationRoot = "Assets/_Game/Application/Interaction";
            foreach (string path in Directory.GetFiles(applicationRoot, "*.cs", SearchOption.AllDirectories))
            {
                string source = File.ReadAllText(path);
                if (source.IndexOf("animal", StringComparison.OrdinalIgnoreCase) >= 0)
                    violations.Add("INTERACTION113 Application interaction core must not hardcode animal behavior: " + path);
            }
            string detector = File.ReadAllText(
                "Assets/_Game/Presentation/Interaction/InteractionDetector.cs");
            if (detector.Contains("GetComponent") || detector.Contains("FindObject"))
                violations.Add("INTERACTION114 detector must use its collider index, not component searches.");
        }
    }
}
