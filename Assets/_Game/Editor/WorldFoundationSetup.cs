using System;
using System.IO;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Worlds;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Content.Worlds;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PequenoExplorador.Editor
{
    public static class WorldFoundationSetup
    {
        public const string Root = "Assets/_Game/Content/Worlds";
        public const string CatalogPath = Root + "/WorldCatalog.asset";
        public const string JungleManifestPath = Root + "/PH_World_Jungle.asset";
        public const string JungleSceneAddress = "scene/jungle";
        public const string JungleWorldLabel = "world-jungle";
        public const string JungleSpawnId = "spawn.jungle.entry";
        public const string JungleCheckpointId = "checkpoint.jungle.entry";

        [MenuItem("Pequeño Explorador/Development/Worlds/Apply World Foundation")]
        public static void Apply()
        {
            try
            {
                EnsureFolder(Root);
                WorldManifestAsset jungle = LoadOrCreate<WorldManifestAsset>(JungleManifestPath);
                WorldCatalogAsset catalog = LoadOrCreate<WorldCatalogAsset>(CatalogPath);
                ContentCatalogAsset content = AssetDatabase.LoadAssetAtPath<ContentCatalogAsset>(ContentFoundationSetup.CatalogPath);
                if (content == null) throw new InvalidOperationException("Canonical content catalog is missing.");
                ConfigureJungle(jungle, content);
                ConfigureCatalog(catalog, jungle);
                ConfigureJungleScene();
                ConfigureAddressables(jungle);
                ConfigureBootstrap(catalog);
                AssetDatabase.SaveAssets();
                Debug.Log("PE_WORLD_FOUNDATION_SETUP_OK worlds=1 jungle=Draft remote=false");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        private static void ConfigureJungle(WorldManifestAsset manifest, ContentCatalogAsset content)
        {
            var serialized = new SerializedObject(manifest);
            SetIfEmpty(serialized.FindProperty("_id"), "world.jungle");
            serialized.FindProperty("_manifestVersion").intValue = 1;
            serialized.FindProperty("_contentVersion").stringValue = "0.1.0-placeholder";
            serialized.FindProperty("_displayNameTable").stringValue = "Content";
            serialized.FindProperty("_displayNameKey").stringValue = "content.world.jungle.name";
            SerializedProperty scene = serialized.FindProperty("_scene");
            scene.FindPropertyRelative("m_AssetGUID").stringValue = AssetDatabase.AssetPathToGUID(SceneFlowFoundationSetup.JungleScenePath);
            serialized.FindProperty("_sceneAddress").stringValue = JungleSceneAddress;
            SetStrings(serialized.FindProperty("_labels"), SceneFlowFoundationSetup.SceneLabel, JungleWorldLabel);
            serialized.FindProperty("_spawnPointId").stringValue = JungleSpawnId;
            SetStrings(serialized.FindProperty("_checkpointIds"), JungleCheckpointId);
            SetObjects(serialized.FindProperty("_contentCatalogs"), content);
            serialized.FindProperty("_musicCueId").stringValue = "audio.music.camp";
            serialized.FindProperty("_ambienceCueId").stringValue = "audio.ambience.camp";
            serialized.FindProperty("_requirements").arraySize = 0;
            serialized.FindProperty("_estimatedInstalledBytes").longValue = new FileInfo(SceneFlowFoundationSetup.JungleScenePath).Length;
            serialized.FindProperty("_availability").enumValueIndex = (int)WorldAvailabilityState.Available;
            SerializedProperty editorial = serialized.FindProperty("_editorial");
            editorial.FindPropertyRelative("_state").enumValueIndex = (int)EditorialState.Draft;
            editorial.FindPropertyRelative("_isPlaceholder").boolValue = true;
            editorial.FindPropertyRelative("_owner").stringValue = "World Design";
            editorial.FindPropertyRelative("_developmentWatermark").stringValue = "BORRADOR · PH_";
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(manifest);
        }

        private static void ConfigureCatalog(WorldCatalogAsset catalog, WorldManifestAsset jungle)
        {
            var serialized = new SerializedObject(catalog);
            SetObjects(serialized.FindProperty("_worlds"), jungle);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
        }

        private static void ConfigureJungleScene()
        {
            Scene scene = EditorSceneManager.OpenScene(SceneFlowFoundationSetup.JungleScenePath, OpenSceneMode.Single);
            WorldSpawnPointMarker marker = UnityEngine.Object.FindFirstObjectByType<WorldSpawnPointMarker>();
            if (marker == null)
            {
                var markerObject = new GameObject("PH_SPAWN_JUNGLE_ENTRY");
                marker = markerObject.AddComponent<WorldSpawnPointMarker>();
            }
            marker.ConfigureForEditor(JungleSpawnId);
            EditorUtility.SetDirty(marker);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureAddressables(WorldManifestAsset manifest)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) throw new InvalidOperationException("Addressable settings are missing.");
            settings.AddLabel(JungleWorldLabel);
            AddressableAssetGroup jungle = settings.FindGroup(SceneFlowFoundationSetup.JungleGroupName);
            if (jungle == null) throw new InvalidOperationException("JungleLocal group is missing.");
            string sceneGuid = AssetDatabase.AssetPathToGUID(SceneFlowFoundationSetup.JungleScenePath);
            AddressableAssetEntry sceneEntry = settings.CreateOrMoveEntry(sceneGuid, jungle, false, false);
            sceneEntry.address = JungleSceneAddress;
            sceneEntry.SetLabel(SceneFlowFoundationSetup.SceneLabel, true, true, false);
            sceneEntry.SetLabel(JungleWorldLabel, true, true, false);
            EditorUtility.SetDirty(settings);
        }

        private static void ConfigureBootstrap(WorldCatalogAsset catalog)
        {
            Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
            DiagnosticBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<DiagnosticBootstrap>();
            if (bootstrap == null) throw new InvalidOperationException("Bootstrap scene has no DiagnosticBootstrap.");
            bootstrap.ConfigureWorldsForEditorAndTests(catalog);
            EditorUtility.SetDirty(bootstrap);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            asset.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void SetIfEmpty(SerializedProperty property, string value)
        {
            if (string.IsNullOrWhiteSpace(property.stringValue)) property.stringValue = value;
        }

        private static void SetStrings(SerializedProperty property, params string[] values)
        {
            property.arraySize = values.Length;
            for (int index = 0; index < values.Length; index++) property.GetArrayElementAtIndex(index).stringValue = values[index];
        }

        private static void SetObjects(SerializedProperty property, params UnityEngine.Object[] values)
        {
            property.arraySize = values.Length;
            for (int index = 0; index < values.Length; index++) property.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}
