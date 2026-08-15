using System;
using System.IO;
using UnityEngine;

namespace PequenoExplorador.Editor.BuildTools
{
    internal static class BuildArtifactPaths
    {
        public static string ProjectRoot => Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

        public static string ArtifactsRoot
        {
            get
            {
                string configured = CommandLineArguments.Read("-artifactsPath");
                string candidate = string.IsNullOrWhiteSpace(configured)
                    ? Path.Combine(ProjectRoot, "artifacts")
                    : configured;
                return RequireInsideArtifacts(candidate);
            }
        }

        public static string RequireInsideArtifacts(string path)
        {
            string allowedRoot = Path.GetFullPath(Path.Combine(ProjectRoot, "artifacts"));
            string fullPath = Path.GetFullPath(path);
            string prefix = allowedRoot + Path.DirectorySeparatorChar;
            if (!string.Equals(fullPath, allowedRoot, StringComparison.Ordinal) &&
                !fullPath.StartsWith(prefix, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Build outputs must remain under the ignored project artifacts/ directory.");
            }

            return fullPath;
        }

        public static string RelativeToProject(string path)
        {
            Uri root = new Uri(ProjectRoot.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
            return Uri.UnescapeDataString(root.MakeRelativeUri(new Uri(Path.GetFullPath(path))).ToString());
        }
    }

    internal static class CommandLineArguments
    {
        public static string Read(string name)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            int index = Array.IndexOf(arguments, name);
            return index >= 0 && index + 1 < arguments.Length ? arguments[index + 1] : null;
        }
    }
}
