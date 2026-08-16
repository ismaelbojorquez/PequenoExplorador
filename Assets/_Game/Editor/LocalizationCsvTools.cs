using System;
using System.IO;
using System.Text;
using PequenoExplorador.Application.Localization;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.Localization.Plugins.CSV;
using UnityEngine;

namespace PequenoExplorador.Editor
{
    public static class LocalizationCsvTools
    {
        [MenuItem("Pequeño Explorador/Development/Localization/Export CSV to artifacts")]
        public static void ExportAllToArtifacts()
        {
            string output = Path.Combine(
                Directory.GetParent(UnityEngine.Application.dataPath).FullName,
                "artifacts",
                "localization");
            Directory.CreateDirectory(output);
            foreach (string tableName in new[]
                     {
                         LocalizationKeys.SharedTable,
                         LocalizationKeys.UiTable,
                         LocalizationKeys.ContentTable
                     })
            {
                var collection = LocalizationEditorSettings.GetStringTableCollection(tableName) ??
                    throw new InvalidOperationException("Missing localization collection: " + tableName);
                using var writer = new StreamWriter(
                    Path.Combine(output, tableName + ".csv"),
                    false,
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                Csv.Export(writer, collection);
            }

            Debug.Log("PE_LOCALIZATION_CSV_EXPORT_OK tables=3 path=artifacts/localization");
        }

        [MenuItem("Pequeño Explorador/Development/Localization/Import CSV (merge)…")]
        public static void ImportCsvMergeMenu()
        {
            string path = EditorUtility.OpenFilePanel("Import localization CSV", string.Empty, "csv");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string tableName = Path.GetFileNameWithoutExtension(path);
            ImportCollectionFromPath(tableName, path);
        }

        public static void ImportCollectionFromPath(string tableName, string path)
        {
            var collection = LocalizationEditorSettings.GetStringTableCollection(tableName) ??
                throw new InvalidOperationException("CSV filename must match Shared, UI or Content.");
            using var reader = new StreamReader(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            Csv.ImportInto(reader, collection, createUndo: true, removeMissingEntries: false);
            AssetDatabase.SaveAssets();
            Debug.Log("PE_LOCALIZATION_CSV_IMPORT_OK table=" + tableName);
        }
    }
}
