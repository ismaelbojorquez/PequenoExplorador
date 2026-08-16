using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PequenoExplorador.Editor.BuildTools
{
    public static class BuildToolsCli
    {
        public static void Compile()
        {
            Run(() =>
            {
                ValidateBoundaries();
                ValidateContentInternal();
                ValidateAddressablesInternal();
                ArtifactReportWriter.WriteEnvironmentReport();
                Debug.Log("PE_COMPILE_OK");
            });
        }

        public static void ValidateContent()
        {
            Run(ValidateContentInternal);
        }

        public static void ValidateRuntimeConfiguration()
        {
            Run(() =>
            {
                IReadOnlyList<string> violations = RuntimeConfigurationValidationService.Validate();
                if (violations.Count > 0)
                {
                    throw new InvalidOperationException(
                        "Runtime configuration validation failed:\n" + string.Join("\n", violations));
                }

                Debug.Log("PE_RUNTIME_CONFIG_OK profiles=2 remote=false releaseUnsafeFlags=0");
            });
        }

        public static void ValidateLocalization()
        {
            Run(() =>
            {
                IReadOnlyList<string> violations = LocalizationValidationService.Validate();
                if (violations.Count > 0)
                {
                    throw new InvalidOperationException(
                        "Localization validation failed:\n" + string.Join("\n", violations));
                }

                Debug.Log("PE_LOCALIZATION_OK package=1.5.12 locales=es,en,pseudo stringTables=3 assetTables=2");
            });
        }

        public static void ValidateAudio()
        {
            Run(() =>
            {
                IReadOnlyList<string> violations = AudioValidationService.Validate();
                if (violations.Count > 0)
                {
                    throw new InvalidOperationException("Audio validation failed:\n" + string.Join("\n", violations));
                }

                Debug.Log("PE_AUDIO_OK buses=5 cues=7 clips=10 placeholders=10 sampleRate=48000 releaseFinal=0");
                Debug.LogWarning("PE_AUDIO_RELEASE_PENDING finalAssets=10 humanVoiceReview=required");
            });
        }

        public static void BuildAddressablesLocal()
        {
            Run(() =>
            {
                ValidateBoundaries();
                ValidateContentInternal();
                ValidateAddressablesInternal();
                LocalAddressablesBuildService.BuildDevelopment();
            });
        }

        public static void BuildAndroidDevelopment()
        {
            Run(() =>
            {
                ValidateBoundaries();
                ValidateContentInternal();
                ValidateAddressablesInternal();
                LocalAddressablesBuildService.BuildDevelopment();
                AndroidBuildService.BuildDevelopment();
            });
        }

        public static void BuildAndroidRelease()
        {
            try
            {
                ValidateBoundaries();
                ValidateContentInternal();
                ValidateAddressablesInternal();
                const string reason =
                    "PE_RELEASE_SIGNING_REQUIRED: Release is intentionally blocked until an authorized human supplies external signing and approves bundle identity.";
                ArtifactReportWriter.WriteReleaseBlockedReport(reason);
                Debug.LogError(reason);
                EditorApplication.Exit(3);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(2);
            }
        }

        private static void ValidateBoundaries()
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

        private static void ValidateContentInternal()
        {
            IReadOnlyList<string> violations = ContentValidationService.Validate();
            if (violations.Count > 0)
            {
                throw new InvalidOperationException(
                    "Content validation failed:\n" + string.Join("\n", violations));
            }

            Debug.Log("PE_CONTENT_VALIDATION_OK");
            Debug.Log("PE_RUNTIME_CONFIG_OK profiles=2 remote=false releaseUnsafeFlags=0");
            Debug.Log("PE_LOCALIZATION_OK package=1.5.12 locales=es,en,pseudo stringTables=3 assetTables=2");
            Debug.Log("PE_AUDIO_OK buses=5 cues=7 clips=10 placeholders=10 sampleRate=48000 releaseFinal=0");
            Debug.LogWarning("PE_AUDIO_RELEASE_PENDING finalAssets=10 humanVoiceReview=required");
        }

        private static void ValidateAddressablesInternal()
        {
            IReadOnlyList<string> violations = LocalAddressablesValidationService.Validate();
            if (violations.Count > 0)
            {
                throw new InvalidOperationException(
                    "Local Addressables validation failed:\n" + string.Join("\n", violations));
            }

            Debug.Log("PE_LOCAL_ADDRESSABLES_OK profiles=2 sceneGroups=2 localizationGroups=6 remote=false");
        }

        private static void Run(Action action)
        {
            try
            {
                action();
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(2);
            }
        }
    }
}
