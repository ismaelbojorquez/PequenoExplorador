using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Content.Visuals;
using PequenoExplorador.Presentation.Interaction;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class ToucanFixtureValidationService
    {
        public static IReadOnlyList<string> Validate(ContentValidationMode mode)
        {
            var violations = new List<string>();
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ToucanFixtureSetup.PrefabPath);
            if (prefab == null)
            {
                violations.Add("TOUCAN001 missing review prefab: " + ToucanFixtureSetup.PrefabPath);
                return violations;
            }

            ToucanReviewFixtureMetadata metadata = prefab.GetComponent<ToucanReviewFixtureMetadata>();
            if (metadata == null) violations.Add("TOUCAN002 prefab metadata is missing.");
            else
            {
                if (metadata.VisualId != ToucanFixtureSetup.VisualId ||
                    metadata.FutureDiscoveryId != ToucanFixtureSetup.FutureDiscoveryId ||
                    metadata.FutureInteractionId != ToucanFixtureSetup.FutureInteractionId)
                    violations.Add("TOUCAN003 stable visual/future IDs do not match the Vertical Slice contract.");
                if (metadata.Author != "Ismael Bojórquez") violations.Add("TOUCAN004 declared author is missing.");
                if (metadata.EditorialState != EditorialState.Approved || metadata.IsPlaceholder)
                    violations.Add("TOUCAN005 reviewed asset must be Approved and non-placeholder.");
                if (metadata.VisualReviewState != "APPROVED" ||
                    metadata.VisualApprovedBy != "Ismael Bojórquez — Creador/Propietario" ||
                    metadata.VisualApprovalDate != "2026-08-16" ||
                    metadata.VisualApprovalReference != ToucanFixtureSetup.VisualApprovalReference)
                    violations.Add("TOUCAN017 asset-specific visual approval H-008 is missing or inconsistent.");
                if (metadata.FactualReviewState != "APPROVED" ||
                    metadata.FactualReviewedBy != "Ismael Bojórquez — Investigador" ||
                    metadata.FactualReviewerCompetence != "Experiencia en búsqueda ampliada de información" ||
                    metadata.FactualReviewDate != "2026-08-16" ||
                    metadata.FactualApprovalReference != ToucanFixtureSetup.FactualApprovalReference)
                    violations.Add("TOUCAN006 factual approval H-009 is missing or inconsistent.");
                if (metadata.VisualRoot == null || metadata.InteractionPoint == null ||
                    metadata.TouchCollider == null || !metadata.TouchCollider.isTrigger)
                    violations.Add("TOUCAN007 visual root, interaction point and broad trigger collider are required.");
                if (metadata.CandidatePhotoBounds.size.magnitude <= 1f)
                    violations.Add("TOUCAN008 candidate photo bounds are invalid.");
            }

            ToucanFixtureMetrics metrics = ToucanFixtureSetup.MeasurePrefab();
            if (metrics.Meshes < 8 || metrics.Meshes > 24 || metrics.Renderers != metrics.Meshes)
                violations.Add($"TOUCAN009 unexpected mesh/renderer budget: {metrics.Meshes}/{metrics.Renderers}.");
            if (metrics.Materials != 7) violations.Add($"TOUCAN010 expected 7 shared materials; found {metrics.Materials}.");
            if (metrics.Vertices > 10000 || metrics.Triangles > 10000)
                violations.Add($"TOUCAN011 provisional geometry budget exceeded: {metrics.Vertices}v/{metrics.Triangles}t.");
            if (AssetDatabase.LoadAssetAtPath<TextAsset>(ToucanFixtureSetup.ProvenancePath) == null)
                violations.Add("TOUCAN012 provenance ledger is missing.");
            ValidateScene(violations);

            return violations;
        }

        private static void ValidateScene(ICollection<string> violations)
        {
            Scene scene = SceneManager.GetSceneByPath(SceneFlowFoundationSetup.JungleScenePath);
            bool opened = !scene.IsValid() || !scene.isLoaded;
            if (opened) scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.JungleScenePath, OpenSceneMode.Additive);
            WorldInteractableView animal = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<WorldInteractableView>(true))
                .SingleOrDefault(item => item.RawInteractionId == InteractionFoundationSetup.AnimalId);
            if (animal == null) violations.Add("TOUCAN013 approved Vertical Slice toucan interactable is missing.");
            else
            {
                ToucanReviewFixtureMetadata metadata = animal.GetComponentInChildren<ToucanReviewFixtureMetadata>(true);
                if (metadata == null) violations.Add("TOUCAN014 approved interactable is not using the reviewed visual.");
                if (animal.transform.Find("VS_TOUCAN_INTERACTABLE_VISUAL") != null ||
                    animal.transform.Find("PH_FIXTURE_ANIMAL_VISUAL") != null)
                    violations.Add("TOUCAN015 legacy capsule visual remains attached.");
                if (metadata != null && !animal.TargetColliders.Contains(metadata.TouchCollider))
                    violations.Add("TOUCAN016 interactable is not wired to the prefab touch collider.");
            }
            if (opened) EditorSceneManager.CloseScene(scene, true);
        }
    }
}
