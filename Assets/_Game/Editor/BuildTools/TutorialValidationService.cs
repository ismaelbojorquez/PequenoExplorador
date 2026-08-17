using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Tutorial;
using PequenoExplorador.Content.Audio;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Content.Tutorial;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Tutorial;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class TutorialValidationService
    {
        public static IReadOnlyList<string> Validate(ContentValidationMode mode)
        {
            var errors = new List<string>();
            TutorialDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<TutorialDefinitionAsset>(TutorialFoundationSetup.DefinitionPath);
            TutorialDefinition definition = null;
            if (asset == null) errors.Add("TUTORIAL001 canonical tutorial definition is missing.");
            else
            {
                try { definition = asset.ToRuntime(); } catch (Exception exception) { errors.Add("TUTORIAL002 invalid tutorial definition: " + exception.Message); }
                if (asset.PlaceholderId != "PH_TUTORIAL_VERTICAL_SLICE") errors.Add("TUTORIAL003 placeholder metadata ID is invalid.");
                if (mode == ContentValidationMode.Release && asset.ReleaseState != "Approved")
                    errors.Add("TUTORIAL004 Release is blocked until human ES/EN narration and child playtest are approved.");
            }
            if (definition != null)
            {
                if (definition.Id != "tutorial.vertical-slice" || definition.Version != 1 || definition.Steps.Count != 7)
                    errors.Add("TUTORIAL005 Vertical Slice tutorial must remain version 1 with seven bounded steps.");
                TutorialTrigger[] expected = { TutorialTrigger.ExpeditionEntered, TutorialTrigger.MovementAccepted,
                    TutorialTrigger.InteractionCompleted, TutorialTrigger.PhotoCaptured, TutorialTrigger.Continue,
                    TutorialTrigger.CampReturned, TutorialTrigger.AlbumOpened };
                if (!definition.Steps.Select(value => value.Trigger).SequenceEqual(expected))
                    errors.Add("TUTORIAL006 tutorial semantic trigger order is invalid.");
                AudioCueCatalogAsset audio = AssetDatabase.LoadAssetAtPath<AudioCueCatalogAsset>(AudioFoundationSetup.CatalogPath);
                foreach (var step in definition.Steps)
                    if (audio == null || !audio.Cues.Any(cue => cue != null && cue.CueId == step.VoiceCue))
                        errors.Add("TUTORIAL007 missing localized voice cue: " + step.VoiceCue);
            }
            if (mode == ContentValidationMode.Release) return errors;
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            TutorialView[] views = scene.GetRootGameObjects().SelectMany(value => value.GetComponentsInChildren<TutorialView>(true)).ToArray();
            if (views.Length != 1) errors.Add($"TUTORIAL008 Bootstrap requires exactly one TutorialView; found {views.Length}.");
            else
            {
                TutorialView view = views[0];
                if (view.GetComponent<SafeAreaFitter>() == null) errors.Add("TUTORIAL009 TutorialView requires the central safe-area adapter.");
                foreach (Button button in view.GetComponentsInChildren<Button>(true))
                {
                    RectTransform rect = button.transform as RectTransform;
                    if (rect != null && (rect.rect.width < 64f || rect.rect.height < 64f))
                        errors.Add("TUTORIAL010 touch target below 64 logical units: " + button.name);
                }
            }
            return errors;
        }
    }
}
