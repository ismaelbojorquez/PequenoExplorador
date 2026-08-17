using System.Collections.Generic;
using System.IO;
using System.Linq;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Presentation.Accessibility;
using PequenoExplorador.Presentation.Album;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class AlbumValidationService
    {
        public static IReadOnlyList<string> Validate()
        {
            var violations = new List<string>();
            ValidateContent(violations);
            ValidateBootstrap(violations);
            ValidateSourceBoundaries(violations);
            return violations;
        }

        private static void ValidateContent(ICollection<string> violations)
        {
            var category = AssetDatabase.LoadAssetAtPath<CategoryDefinitionAsset>(
                ContentFoundationSetup.Root + "/Definitions/VS_Category_Animals.asset");
            if (category == null || category.DisplayNameTable != "Content" ||
                category.DisplayNameKey != "content.category.discovery.animals")
                violations.Add("ALBUM001 animal category requires a stable localized display name.");

            var discovery = AssetDatabase.LoadAssetAtPath<DiscoveryDefinitionAsset>(ContentFoundationSetup.DiscoveryPath);
            if (discovery == null) violations.Add("ALBUM002 approved toucan discovery is missing.");
            else
            {
                if (discovery.AlbumHabitatFact == null || discovery.AlbumDietFact == null ||
                    discovery.AlbumCuriosityFact == null || discovery.AlbumSoundFact == null)
                    violations.Add("ALBUM003 toucan album requires approved habitat/diet/curiosity/sound references.");
                if (discovery.AlbumSizeFact != null)
                    violations.Add("ALBUM004 size slot must remain empty until an Approved size claim exists.");
                if (discovery.AlbumHasPlayableAudio)
                    violations.Add("ALBUM005 placeholder confirm cue cannot be exposed as factual animal audio.");
            }
        }

        private static void ValidateBootstrap(ICollection<string> violations)
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            AlbumView[] views = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<AlbumView>(true)).ToArray();
            if (views.Length != 1)
            {
                violations.Add($"ALBUM006 Bootstrap requires exactly one AlbumView; found {views.Length}.");
                return;
            }

            AlbumView view = views[0];
            if (view.GetComponentInChildren<SafeAreaFitter>(true) == null)
                violations.Add("ALBUM007 AlbumView requires one safe-area root.");
            var serialized = new SerializedObject(view);
            if (serialized.FindProperty("_entryCells").arraySize != AlbumView.EntriesPerPage)
                violations.Add($"ALBUM008 album cell pool must contain {AlbumView.EntriesPerPage} reusable cells.");
            if (serialized.FindProperty("_categoryCells").arraySize < 1)
                violations.Add("ALBUM009 album requires a category filter pool.");
            foreach (Button button in view.GetComponentsInChildren<Button>(true))
            {
                Rect rect = ((RectTransform)button.transform).rect;
                if (rect.width < 64f || rect.height < 64f)
                    violations.Add($"ALBUM010 touch target {button.name} is {rect.width:0}x{rect.height:0}; minimum is 64x64 logical units.");
            }
        }

        private static void ValidateSourceBoundaries(ICollection<string> violations)
        {
            string root = Path.Combine(Directory.GetCurrentDirectory(), "Assets/_Game/Presentation/Album");
            string[] forbidden = { "AssetDatabase", "PlayerPrefs", "System.IO", "File.", "Resources.Load", "ScreenCapture" };
            foreach (string file in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
            {
                string source = File.ReadAllText(file);
                foreach (string token in forbidden)
                    if (source.Contains(token)) violations.Add($"ALBUM011 Presentation must not use '{token}' in {Path.GetFileName(file)}.");
            }
        }
    }
}
