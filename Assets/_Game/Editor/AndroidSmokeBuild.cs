using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace PequenoExplorador.Editor
{
    public static class AndroidSmokeBuild
    {
        private const string DefaultBuildPath = "/tmp/pequeno-explorador-builds/PequenoExplorador-smoke.apk";
        private const string DefaultReleaseBuildPath = "/tmp/pequeno-explorador-builds/PequenoExplorador-release.aab";

        public static void Build()
        {
            try
            {
                if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
                {
                    throw new InvalidOperationException(
                        "Android Build Support is not installed for this Unity Editor. Install AndroidPlayer with SDK, NDK and OpenJDK through Unity Hub.");
                }

                string profile = ReadArgument("-peProfile") ?? "Development";
                BuildOptions options = ResolveOptions(profile);
                bool release = string.Equals(profile, "Release", StringComparison.OrdinalIgnoreCase);
                string buildPath = ReadArgument("-buildPath") ?? (release ? DefaultReleaseBuildPath : DefaultBuildPath);
                buildPath = ValidateAndResolveBuildPath(buildPath, release);

                Directory.CreateDirectory(Path.GetDirectoryName(buildPath));
                PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
                PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel36;
                EditorUserBuildSettings.buildAppBundle = release;

                if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
                {
                    throw new InvalidOperationException("Unity could not switch the active build target to Android.");
                }

                string[] scenes = EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                    .Select(scene => scene.path)
                    .ToArray();

                if (scenes.Length == 0)
                {
                    throw new InvalidOperationException("No enabled scene exists in EditorBuildSettings.");
                }

                BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = scenes,
                    locationPathName = buildPath,
                    target = BuildTarget.Android,
                    options = options
                });

                if (report.summary.result != BuildResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Android build ended with {report.summary.result} and {report.summary.totalErrors} errors.");
                }

                Debug.Log($"PE_ANDROID_BUILD_OK profile={profile} path={buildPath} bytes={report.summary.totalSize}");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(2);
            }
        }

        private static BuildOptions ResolveOptions(string profile)
        {
            if (string.Equals(profile, "Debug", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOptions.Development | BuildOptions.AllowDebugging;
            }

            if (string.Equals(profile, "Development", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOptions.Development;
            }

            if (string.Equals(profile, "Release", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOptions.None;
            }

            throw new ArgumentException($"Unknown -peProfile '{profile}'. Use Debug, Development or Release.");
        }

        private static string ValidateAndResolveBuildPath(string buildPath, bool release)
        {
            string expectedExtension = release ? ".aab" : ".apk";
            if (!buildPath.EndsWith(expectedExtension, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"The selected profile requires a '{expectedExtension}' build path.");
            }

            string fullBuildPath = Path.GetFullPath(buildPath);
            string projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string projectPrefix = projectPath + Path.DirectorySeparatorChar;
            if (fullBuildPath.StartsWith(projectPrefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Build output must be outside the Unity project and Git worktree.");
            }

            return fullBuildPath;
        }

        private static string ReadArgument(string name)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            int index = Array.IndexOf(arguments, name);
            return index >= 0 && index + 1 < arguments.Length ? arguments[index + 1] : null;
        }
    }
}
