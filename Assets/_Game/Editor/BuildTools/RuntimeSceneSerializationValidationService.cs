using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class RuntimeSceneSerializationValidationService
    {
        private static readonly Regex EmbeddedMonoScriptDocument = new Regex(
            @"(?ms)^--- !u!115 &(?<id>-?\d+)\r?\nMonoScript:\r?\n(?<body>.*?)(?=^--- !u!|\z)",
            RegexOptions.CultureInvariant);

        private static readonly Regex LocalScriptReference = new Regex(
            @"(?m)^  m_Script: \{fileID: -?\d+\}\s*$",
            RegexOptions.CultureInvariant);

        public static IReadOnlyList<string> Validate()
        {
            var violations = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes.Where(value => value.enabled))
            {
                string fullPath = ToProjectAbsolutePath(scene.path);
                if (!File.Exists(fullPath))
                {
                    violations.Add($"SCENE001 enabled runtime scene is missing: {scene.path}");
                    continue;
                }

                violations.AddRange(ValidateSceneText(scene.path, File.ReadAllText(fullPath)));
            }

            return violations;
        }

        internal static string ToProjectAbsolutePath(string assetPath)
        {
            string projectRoot = Directory.GetParent(UnityEngine.Application.dataPath)?.FullName;
            if (string.IsNullOrWhiteSpace(projectRoot))
            {
                throw new InvalidOperationException("Unity project root could not be resolved from Application.dataPath.");
            }

            return Path.GetFullPath(Path.Combine(projectRoot, assetPath));
        }

        public static IReadOnlyList<string> ValidateSceneText(string assetPath, string yaml)
        {
            var violations = new List<string>();
            MatchCollection embedded = EmbeddedMonoScriptDocument.Matches(yaml ?? string.Empty);
            if (embedded.Count > 0)
            {
                string ids = string.Join(",", embedded.Cast<Match>().Select(value => value.Groups["id"].Value));
                violations.Add(
                    $"SCENE002 embedded MonoScript documents are forbidden in runtime scene {assetPath}: {ids}. " +
                    "Persist each MonoBehaviour in a same-named .cs asset and repair its m_Script GUID before building.");
            }

            MatchCollection localReferences = LocalScriptReference.Matches(yaml ?? string.Empty);
            if (localReferences.Count > 0)
            {
                violations.Add(
                    $"SCENE003 local m_Script references are forbidden in runtime scene {assetPath}: " +
                    $"count={localReferences.Count}.");
            }

            return violations;
        }

        internal static MatchCollection FindEmbeddedMonoScripts(string yaml)
        {
            return EmbeddedMonoScriptDocument.Matches(yaml ?? string.Empty);
        }
    }

    public static class RuntimeSceneSerializationRepair
    {
        private static readonly Regex Field = new Regex(
            @"(?m)^  (?<name>m_ClassName|m_Namespace|m_AssemblyName):\s*(?<value>.*)\s*$",
            RegexOptions.CultureInvariant);

        [MenuItem("Pequeño Explorador/Repair/Runtime Scene Script References")]
        public static void RepairFromMenu()
        {
            RepairEnabledScenes();
        }

        public static void RunCli()
        {
            try
            {
                int repaired = RepairEnabledScenes();
                IReadOnlyList<string> violations = RuntimeSceneSerializationValidationService.Validate();
                if (violations.Count > 0)
                {
                    throw new InvalidOperationException(
                        "Runtime scene serialization remains invalid:\n" + string.Join("\n", violations));
                }

                Debug.Log($"PE_RUNTIME_SCENE_REPAIR_OK repairedReferences={repaired}");
                if (UnityEngine.Application.isBatchMode)
                {
                    EditorApplication.Exit(0);
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode)
                {
                    EditorApplication.Exit(2);
                }

                throw;
            }
        }

        private static int RepairEnabledScenes()
        {
            int repairedReferences = 0;
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes.Where(value => value.enabled))
            {
                string fullPath = RuntimeSceneSerializationValidationService.ToProjectAbsolutePath(scene.path);
                string yaml = File.ReadAllText(fullPath);
                Match[] documents = RuntimeSceneSerializationValidationService
                    .FindEmbeddedMonoScripts(yaml)
                    .Cast<Match>()
                    .ToArray();
                if (documents.Length == 0)
                {
                    continue;
                }

                foreach (Match document in documents)
                {
                    string localId = document.Groups["id"].Value;
                    IReadOnlyDictionary<string, string> fields = ParseFields(document.Groups["body"].Value);
                    string className = Required(fields, "m_ClassName", localId);
                    string namespaceName = Required(fields, "m_Namespace", localId);
                    string assemblyName = Required(fields, "m_AssemblyName", localId);
                    string guid = ResolveScriptGuid(className, namespaceName, assemblyName);
                    string localReference = $"m_Script: {{fileID: {localId}}}";
                    string externalReference = $"m_Script: {{fileID: 11500000, guid: {guid}, type: 3}}";
                    int references = CountOccurrences(yaml, localReference);
                    if (references == 0)
                    {
                        throw new InvalidOperationException(
                            $"Embedded MonoScript {localId} ({namespaceName}.{className}) has no component references.");
                    }

                    yaml = yaml.Replace(localReference, externalReference);
                    repairedReferences += references;
                }

                yaml = RemoveEmbeddedDocuments(yaml);
                IReadOnlyList<string> remaining = RuntimeSceneSerializationValidationService.ValidateSceneText(scene.path, yaml);
                if (remaining.Count > 0)
                {
                    throw new InvalidOperationException(
                        $"Refusing to write partially repaired scene {scene.path}:\n" + string.Join("\n", remaining));
                }

                File.WriteAllText(fullPath, yaml, new UTF8Encoding(false));
                AssetDatabase.ImportAsset(scene.path, ImportAssetOptions.ForceUpdate);
                Scene opened = EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
                if (!opened.IsValid())
                {
                    throw new InvalidOperationException($"Unity could not reopen repaired scene {scene.path}.");
                }

                EditorSceneManager.SaveScene(opened);
            }

            AssetDatabase.SaveAssets();
            return repairedReferences;
        }

        private static IReadOnlyDictionary<string, string> ParseFields(string body)
        {
            return Field.Matches(body)
                .Cast<Match>()
                .ToDictionary(
                    value => value.Groups["name"].Value,
                    value => value.Groups["value"].Value.Trim(),
                    StringComparer.Ordinal);
        }

        private static string Required(IReadOnlyDictionary<string, string> fields, string field, string localId)
        {
            if (!fields.TryGetValue(field, out string value) || string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Embedded MonoScript {localId} is missing {field}.");
            }

            return value;
        }

        private static string ResolveScriptGuid(string className, string namespaceName, string assemblyName)
        {
            string fullName = string.IsNullOrWhiteSpace(namespaceName)
                ? className
                : namespaceName + "." + className;
            string[] matches = AssetDatabase.FindAssets($"t:MonoScript {className}", new[] { "Assets/_Game" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path =>
                {
                    MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                    Type type = script == null ? null : script.GetClass();
                    return type != null &&
                           string.Equals(type.FullName, fullName, StringComparison.Ordinal) &&
                           string.Equals(type.Assembly.GetName().Name, assemblyName, StringComparison.Ordinal);
                })
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            if (matches.Length != 1)
            {
                throw new InvalidOperationException(
                    $"Expected one external MonoScript for {fullName} in {assemblyName}; found {matches.Length}: " +
                    string.Join(",", matches));
            }

            return AssetDatabase.AssetPathToGUID(matches[0]);
        }

        private static int CountOccurrences(string value, string search)
        {
            int count = 0;
            int offset = 0;
            while ((offset = value.IndexOf(search, offset, StringComparison.Ordinal)) >= 0)
            {
                count++;
                offset += search.Length;
            }

            return count;
        }

        private static string RemoveEmbeddedDocuments(string yaml)
        {
            Match[] documents = RuntimeSceneSerializationValidationService
                .FindEmbeddedMonoScripts(yaml)
                .Cast<Match>()
                .OrderByDescending(value => value.Index)
                .ToArray();
            var builder = new StringBuilder(yaml);
            foreach (Match document in documents)
            {
                builder.Remove(document.Index, document.Length);
            }

            return builder.ToString();
        }
    }
}
