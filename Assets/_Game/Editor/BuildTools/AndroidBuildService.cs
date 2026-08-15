using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace PequenoExplorador.Editor.BuildTools
{
    internal static class AndroidBuildService
    {
        public static void BuildDevelopment()
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
            {
                throw new InvalidOperationException(
                    "Android Build Support is unavailable. Install AndroidPlayer, SDK, NDK and OpenJDK for the pinned Editor through Unity Hub.");
            }

            string configured = CommandLineArguments.Read("-buildPath");
            string buildPath = BuildArtifactPaths.RequireInsideArtifacts(
                string.IsNullOrWhiteSpace(configured)
                    ? Path.Combine(BuildArtifactPaths.ArtifactsRoot, "builds", "PequenoExplorador-development.apk")
                    : configured);
            if (!buildPath.EndsWith(".apk", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Android Development output must use the .apk extension.");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(buildPath));
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel36;
            EditorUserBuildSettings.buildAppBundle = false;

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

            var stopwatch = Stopwatch.StartNew();
            BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = buildPath,
                target = BuildTarget.Android,
                options = BuildOptions.Development,
                extraScriptingDefines = new[] { "PE_DEVELOPMENT_SERVICES" }
            });
            stopwatch.Stop();

            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Android build ended with {report.summary.result} and {report.summary.totalErrors} errors.");
            }

            ArtifactReportWriter.WriteBuildManifest(buildPath, stopwatch.ElapsedMilliseconds);
            UnityEngine.Debug.Log(
                $"PE_ANDROID_BUILD_OK profile=Development path={BuildArtifactPaths.RelativeToProject(buildPath)} bytes={new FileInfo(buildPath).Length}");
        }
    }
}
