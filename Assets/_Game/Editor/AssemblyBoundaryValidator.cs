using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PequenoExplorador.Editor
{
    public sealed class AssemblyDefinitionSnapshot
    {
        public AssemblyDefinitionSnapshot(
            string name,
            string rootNamespace,
            IEnumerable<string> references,
            IEnumerable<string> includePlatforms,
            bool noEngineReferences,
            bool overrideReferences,
            bool autoReferenced,
            string assetPath)
        {
            Name = name;
            RootNamespace = rootNamespace;
            References = references.ToArray();
            IncludePlatforms = includePlatforms.ToArray();
            NoEngineReferences = noEngineReferences;
            OverrideReferences = overrideReferences;
            AutoReferenced = autoReferenced;
            AssetPath = assetPath;
        }

        public string Name { get; }
        public string RootNamespace { get; }
        public IReadOnlyList<string> References { get; }
        public IReadOnlyList<string> IncludePlatforms { get; }
        public bool NoEngineReferences { get; }
        public bool OverrideReferences { get; }
        public bool AutoReferenced { get; }
        public string AssetPath { get; }

        public AssemblyDefinitionSnapshot WithReferences(IEnumerable<string> references)
        {
            return new AssemblyDefinitionSnapshot(
                Name,
                RootNamespace,
                references,
                IncludePlatforms,
                NoEngineReferences,
                OverrideReferences,
                AutoReferenced,
                AssetPath);
        }
    }

    public static class AssemblyBoundaryRules
    {
        private static readonly IReadOnlyDictionary<string, string[]> AllowedReferences =
            new Dictionary<string, string[]>(StringComparer.Ordinal)
            {
                ["PequenoExplorador.Domain"] = Array.Empty<string>(),
                ["PequenoExplorador.Application"] = new[]
                {
                    "PequenoExplorador.Domain"
                },
                ["PequenoExplorador.Content"] = new[]
                {
                    "PequenoExplorador.Application",
                    "PequenoExplorador.Domain",
                    "Unity.Addressables",
                    "UnityEngine.AudioModule"
                },
                ["PequenoExplorador.Infrastructure"] = new[]
                {
                    "PequenoExplorador.Application",
                    "PequenoExplorador.Domain",
                    "Unity.Addressables",
                    "Unity.Localization",
                    "Unity.ResourceManager",
                    "Unity.InputSystem",
                    "UnityEngine.AudioModule"
                },
                ["PequenoExplorador.Presentation"] = new[]
                {
                    "PequenoExplorador.Application",
                    "PequenoExplorador.Domain",
                    "UnityEngine.AIModule"
                },
                ["PequenoExplorador.Bootstrap"] = new[]
                {
                    "PequenoExplorador.Application",
                    "PequenoExplorador.Content",
                    "PequenoExplorador.Domain",
                    "PequenoExplorador.Infrastructure",
                    "PequenoExplorador.Presentation",
                    "Unity.InputSystem",
                    "UnityEngine.AudioModule"
                },
                ["PequenoExplorador.Editor"] = new[]
                {
                    "PequenoExplorador.Application",
                    "PequenoExplorador.Bootstrap",
                    "PequenoExplorador.Content",
                    "PequenoExplorador.Domain",
                    "PequenoExplorador.Infrastructure",
                    "PequenoExplorador.Presentation",
                    "Unity.Addressables",
                    "Unity.Addressables.Editor",
                    "Unity.InputSystem",
                    "Unity.Localization",
                    "Unity.Localization.Editor",
                    "Unity.AI.Navigation",
                    "Unity.RenderPipelines.Universal.Runtime",
                    "UnityEngine.AudioModule"
                },
                ["PequenoExplorador.Tests.EditMode"] = new[]
                {
                    "PequenoExplorador.Application",
                    "PequenoExplorador.Bootstrap",
                    "PequenoExplorador.Content",
                    "PequenoExplorador.Domain",
                    "PequenoExplorador.Editor",
                    "PequenoExplorador.Infrastructure",
                    "PequenoExplorador.Presentation",
                    "Unity.InputSystem",
                    "Unity.Localization",
                    "Unity.Localization.Editor",
                    "UnityEngine.AudioModule"
                },
                ["PequenoExplorador.Tests.PlayMode"] = new[]
                {
                    "PequenoExplorador.Application",
                    "PequenoExplorador.Bootstrap",
                    "PequenoExplorador.Domain",
                    "PequenoExplorador.Infrastructure",
                    "PequenoExplorador.Presentation",
                    "Unity.InputSystem",
                    "Unity.InputSystem.TestFramework",
                    "UnityEngine.AIModule",
                    "UnityEngine.AudioModule"
                }
            };

        public static IReadOnlyList<string> Validate(IReadOnlyCollection<AssemblyDefinitionSnapshot> definitions)
        {
            var violations = new List<string>();
            Dictionary<string, AssemblyDefinitionSnapshot> byName = definitions
                .GroupBy(definition => definition.Name, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);

            foreach (IGrouping<string, AssemblyDefinitionSnapshot> duplicate in definitions
                         .GroupBy(definition => definition.Name, StringComparer.Ordinal)
                         .Where(group => group.Count() > 1))
            {
                violations.Add($"ARCH001 duplicate assembly name: {duplicate.Key}");
            }

            foreach (string expectedName in AllowedReferences.Keys.OrderBy(name => name, StringComparer.Ordinal))
            {
                if (!byName.ContainsKey(expectedName))
                {
                    violations.Add($"ARCH002 missing required assembly: {expectedName}");
                }
            }

            foreach (AssemblyDefinitionSnapshot definition in definitions.OrderBy(item => item.Name, StringComparer.Ordinal))
            {
                if (!AllowedReferences.TryGetValue(definition.Name, out string[] allowed))
                {
                    violations.Add($"ARCH003 unreviewed project assembly: {definition.Name}");
                    continue;
                }

                string[] actualReferences = definition.References.OrderBy(name => name, StringComparer.Ordinal).ToArray();
                string[] expectedReferences = allowed.OrderBy(name => name, StringComparer.Ordinal).ToArray();
                if (!actualReferences.SequenceEqual(expectedReferences, StringComparer.Ordinal))
                {
                    violations.Add(
                        $"ARCH004 {definition.Name} references [{string.Join(", ", actualReferences)}]; allowed [{string.Join(", ", expectedReferences)}]");
                }

                if (!string.Equals(definition.RootNamespace, definition.Name, StringComparison.Ordinal))
                {
                    violations.Add($"ARCH005 {definition.Name} rootNamespace must equal its assembly name");
                }

                if (definition.OverrideReferences)
                {
                    violations.Add($"ARCH006 {definition.Name} must not enable overrideReferences");
                }

                if (definition.AutoReferenced)
                {
                    violations.Add($"ARCH007 {definition.Name} must not be auto-referenced");
                }
            }

            RequireEngineFree(byName, "PequenoExplorador.Domain", violations);
            RequireEngineFree(byName, "PequenoExplorador.Application", violations);
            RequireEditorOnly(byName, "PequenoExplorador.Editor", violations);
            RequireEditorOnly(byName, "PequenoExplorador.Tests.EditMode", violations);
            RequireNoPlatformRestriction(byName, "PequenoExplorador.Tests.PlayMode", violations);
            DetectCycles(byName, violations);

            return violations;
        }

        private static void RequireEngineFree(
            IReadOnlyDictionary<string, AssemblyDefinitionSnapshot> byName,
            string assemblyName,
            ICollection<string> violations)
        {
            if (byName.TryGetValue(assemblyName, out AssemblyDefinitionSnapshot definition) && !definition.NoEngineReferences)
            {
                violations.Add($"ARCH008 {assemblyName} must set noEngineReferences=true");
            }
        }

        private static void RequireEditorOnly(
            IReadOnlyDictionary<string, AssemblyDefinitionSnapshot> byName,
            string assemblyName,
            ICollection<string> violations)
        {
            if (!byName.TryGetValue(assemblyName, out AssemblyDefinitionSnapshot definition))
            {
                return;
            }

            if (definition.IncludePlatforms.Count != 1 ||
                !string.Equals(definition.IncludePlatforms[0], "Editor", StringComparison.Ordinal))
            {
                violations.Add($"ARCH009 {assemblyName} must be restricted to Editor");
            }
        }

        private static void RequireNoPlatformRestriction(
            IReadOnlyDictionary<string, AssemblyDefinitionSnapshot> byName,
            string assemblyName,
            ICollection<string> violations)
        {
            if (byName.TryGetValue(assemblyName, out AssemblyDefinitionSnapshot definition) &&
                definition.IncludePlatforms.Count != 0)
            {
                violations.Add($"ARCH010 {assemblyName} must remain available to the PlayMode runner");
            }
        }

        private static void DetectCycles(
            IReadOnlyDictionary<string, AssemblyDefinitionSnapshot> byName,
            ICollection<string> violations)
        {
            var states = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (string name in byName.Keys)
            {
                states[name] = 0;
            }

            foreach (string name in byName.Keys.OrderBy(item => item, StringComparer.Ordinal))
            {
                Visit(name, byName, states, new List<string>(), violations);
            }
        }

        private static void Visit(
            string name,
            IReadOnlyDictionary<string, AssemblyDefinitionSnapshot> byName,
            IDictionary<string, int> states,
            IList<string> path,
            ICollection<string> violations)
        {
            if (states[name] == 2)
            {
                return;
            }

            if (states[name] == 1)
            {
                int cycleStart = path.IndexOf(name);
                IEnumerable<string> cycle = path.Skip(cycleStart).Concat(new[] { name });
                string message = $"ARCH011 cyclic dependency: {string.Join(" -> ", cycle)}";
                if (!violations.Contains(message))
                {
                    violations.Add(message);
                }

                return;
            }

            states[name] = 1;
            path.Add(name);
            foreach (string reference in byName[name].References.Where(byName.ContainsKey))
            {
                Visit(reference, byName, states, path, violations);
            }

            path.RemoveAt(path.Count - 1);
            states[name] = 2;
        }
    }

    public static class AssemblyDefinitionLoader
    {
        public static IReadOnlyList<AssemblyDefinitionSnapshot> LoadProjectDefinitions()
        {
            string assetsRoot = UnityEngine.Application.dataPath;
            return Directory.GetFiles(assetsRoot, "*.asmdef", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => Load(path, assetsRoot))
                .ToArray();
        }

        private static AssemblyDefinitionSnapshot Load(string fullPath, string assetsRoot)
        {
            AssemblyDefinitionJson json = JsonUtility.FromJson<AssemblyDefinitionJson>(File.ReadAllText(fullPath));
            string assetPath = "Assets" + fullPath.Substring(assetsRoot.Length).Replace(Path.DirectorySeparatorChar, '/');
            return new AssemblyDefinitionSnapshot(
                json.name ?? string.Empty,
                json.rootNamespace ?? string.Empty,
                json.references ?? Array.Empty<string>(),
                json.includePlatforms ?? Array.Empty<string>(),
                json.noEngineReferences,
                json.overrideReferences,
                json.autoReferenced,
                assetPath);
        }

        [Serializable]
        private sealed class AssemblyDefinitionJson
        {
            public string name;
            public string rootNamespace;
            public string[] references;
            public string[] includePlatforms;
            public bool noEngineReferences;
            public bool overrideReferences;
            public bool autoReferenced;
        }
    }

    public static class AssemblyBoundaryValidationCli
    {
        [MenuItem("Pequeño Explorador/Validate Assembly Boundaries")]
        public static void Validate()
        {
            IReadOnlyList<string> violations = AssemblyBoundaryRules.Validate(
                AssemblyDefinitionLoader.LoadProjectDefinitions());

            if (violations.Count > 0)
            {
                throw new InvalidOperationException(
                    "Assembly boundary validation failed:\n" + string.Join("\n", violations));
            }

            Debug.Log("PE_ASSEMBLY_BOUNDARIES_OK assemblies=9 cycles=0");
        }
    }
}
