using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace PequenoExplorador.Editor.BuildTools
{
    internal static class ArtifactReportWriter
    {
        public static void WriteEnvironmentReport()
        {
            string path = Path.Combine(BuildArtifactPaths.ArtifactsRoot, "reports", "environment.json");
            var report = new EnvironmentReport
            {
                generatedUtc = DateTime.UtcNow.ToString("O"),
                unityVersion = Application.unityVersion,
                operatingSystem = SystemInfo.operatingSystem,
                batchMode = Application.isBatchMode,
                directPackages = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages()
                    .Where(package => package.isDirectDependency)
                    .OrderBy(package => package.name, StringComparer.Ordinal)
                    .Select(package => package.name + "@" + package.version)
                    .ToArray()
            };
            WriteJson(path, report);
            Debug.Log($"PE_ENVIRONMENT_REPORT_OK path={BuildArtifactPaths.RelativeToProject(path)}");
        }

        public static void WriteBuildManifest(string buildPath, long elapsedMilliseconds)
        {
            var info = new FileInfo(buildPath);
            string path = Path.Combine(BuildArtifactPaths.ArtifactsRoot, "reports", "android-development.json");
            var manifest = new AndroidBuildManifest
            {
                generatedUtc = DateTime.UtcNow.ToString("O"),
                unityVersion = Application.unityVersion,
                gitCommit = CommandLineArguments.Read("-gitCommit") ?? "unknown",
                profile = "Development",
                artifact = BuildArtifactPaths.RelativeToProject(buildPath),
                bytes = info.Length,
                sha256 = ComputeSha256(buildPath),
                elapsedMilliseconds = elapsedMilliseconds,
                minApi = 26,
                targetApi = 36,
                scriptingBackend = "IL2CPP",
                architectures = new[] { "ARM64" },
                appBundle = false,
                externallySigned = false
            };
            WriteJson(path, manifest);
            Debug.Log($"PE_BUILD_MANIFEST_OK path={BuildArtifactPaths.RelativeToProject(path)} sha256={manifest.sha256}");
        }

        public static void WriteReleaseBlockedReport(string reason)
        {
            string path = Path.Combine(BuildArtifactPaths.ArtifactsRoot, "reports", "android-release-blocked.json");
            WriteJson(path, new ReleaseBlockedReport
            {
                generatedUtc = DateTime.UtcNow.ToString("O"),
                status = "BLOCKED",
                reason = reason,
                signingMaterialLoaded = false
            });
        }

        public static void WriteAddressablesManifest(
            string profile,
            string outputPath,
            double durationSeconds,
            int locationCount)
        {
            string path = Path.Combine(BuildArtifactPaths.ArtifactsRoot, "reports", "addressables-local.json");
            string outputDirectory = Path.GetDirectoryName(outputPath);
            FileInfo[] files = Directory.Exists(outputDirectory)
                ? new DirectoryInfo(outputDirectory).GetFiles("*", SearchOption.AllDirectories)
                : Array.Empty<FileInfo>();
            WriteJson(path, new AddressablesBuildManifest
            {
                generatedUtc = DateTime.UtcNow.ToString("O"),
                unityVersion = Application.unityVersion,
                packageVersion = "4.0.1",
                profile = profile,
                target = EditorUserBuildSettings.activeBuildTarget.ToString(),
                output = BuildArtifactPaths.RelativeToProject(outputPath),
                locationCount = locationCount,
                fileCount = files.Length,
                bytes = files.Sum(file => file.Length),
                durationSeconds = durationSeconds,
                remoteCatalog = false
            });
        }

        private static void WriteJson(string path, object value)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, JsonUtility.ToJson(value, true) + Environment.NewLine);
        }

        private static string ComputeSha256(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            using (SHA256 sha256 = SHA256.Create())
            {
                return string.Concat(sha256.ComputeHash(stream).Select(value => value.ToString("x2")));
            }
        }

        [Serializable]
        private sealed class EnvironmentReport
        {
            public string generatedUtc;
            public string unityVersion;
            public string operatingSystem;
            public bool batchMode;
            public string[] directPackages;
        }

        [Serializable]
        private sealed class AndroidBuildManifest
        {
            public string generatedUtc;
            public string unityVersion;
            public string gitCommit;
            public string profile;
            public string artifact;
            public long bytes;
            public string sha256;
            public long elapsedMilliseconds;
            public int minApi;
            public int targetApi;
            public string scriptingBackend;
            public string[] architectures;
            public bool appBundle;
            public bool externallySigned;
        }

        [Serializable]
        private sealed class ReleaseBlockedReport
        {
            public string generatedUtc;
            public string status;
            public string reason;
            public bool signingMaterialLoaded;
        }

        [Serializable]
        private sealed class AddressablesBuildManifest
        {
            public string generatedUtc;
            public string unityVersion;
            public string packageVersion;
            public string profile;
            public string target;
            public string output;
            public int locationCount;
            public int fileCount;
            public long bytes;
            public double durationSeconds;
            public bool remoteCatalog;
        }
    }
}
