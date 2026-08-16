using System.IO;
using PequenoExplorador.Infrastructure.Save;
using UnityEditor;
using UnityEngine;

namespace PequenoExplorador.Editor
{
    internal static class SaveTools
    {
        private const string RootMenu = "Pequeño Explorador/Development/Save/";

        [MenuItem(RootMenu + "Inspect Files")]
        private static void InspectFiles()
        {
            string directory = GetSaveDirectory();
            if (!Directory.Exists(directory))
            {
                EditorUtility.DisplayDialog(
                    "Local save",
                    "No local save directory exists yet.",
                    "OK");
                return;
            }

            EditorUtility.RevealInFinder(directory);
        }

        [MenuItem(RootMenu + "Reset Local Progress")]
        private static void ResetLocalProgress()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Reset local progress?",
                "This development-only action permanently deletes primary, backup and temporary local save files.",
                "Reset",
                "Cancel");
            if (!confirmed)
            {
                return;
            }

            string directory = GetSaveDirectory();
            DeleteIfPresent(Path.Combine(directory, SaveFileNames.Temporary));
            DeleteIfPresent(Path.Combine(directory, SaveFileNames.Backup));
            DeleteIfPresent(Path.Combine(directory, SaveFileNames.Primary));
            Debug.Log("PE_SAVE_TOOL result=ResetCompleted scope=LocalKnownFiles");
        }

        [MenuItem(RootMenu + "Inspect Files", true)]
        [MenuItem(RootMenu + "Reset Local Progress", true)]
        private static bool ValidateDevelopmentSaveMenu()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        private static string GetSaveDirectory()
        {
            return Path.Combine(Application.persistentDataPath, "Save");
        }

        private static void DeleteIfPresent(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
