using System;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Content.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PequenoExplorador.Editor
{
    public static class ContentFoundationSetup
    {
        public const string Root = "Assets/_Game/Content/Data";
        public const string CatalogPath = Root + "/ContentCatalog.asset";
        public const string DiscoveryPath = Root + "/Definitions/PH_Discovery_Jungle.asset";
        private const string CategoryPath = Root + "/Definitions/PH_Category_Nature.asset";
        private const string TagPath = Root + "/Definitions/PH_Tag_Jungle.asset";
        private const string SourcePath = Root + "/Definitions/PH_Source_PendingReview.asset";
        private const string FactPath = Root + "/Definitions/PH_Fact_PendingReview.asset";
        private const string VisualPath = "Assets/_Game/Content/Placeholders/PH_DISCOVERY_JUNGLE.placeholder.json";

        [MenuItem("Pequeño Explorador/Development/Content/Apply Data Foundation")]
        public static void Apply()
        {
            try
            {
                EnsureFolder(Root + "/Definitions");
                CategoryDefinitionAsset category = LoadOrCreate<CategoryDefinitionAsset>(CategoryPath);
                TagDefinitionAsset tag = LoadOrCreate<TagDefinitionAsset>(TagPath);
                ContentSourceRecordAsset source = LoadOrCreate<ContentSourceRecordAsset>(SourcePath);
                EducationalFactDefinitionAsset fact = LoadOrCreate<EducationalFactDefinitionAsset>(FactPath);
                DiscoveryDefinitionAsset discovery = LoadOrCreate<DiscoveryDefinitionAsset>(DiscoveryPath);
                ContentCatalogAsset catalog = LoadOrCreate<ContentCatalogAsset>(CatalogPath);

                ConfigureBase(category, "category.nature.placeholder");
                ConfigureBase(tag, "tag.jungle.placeholder");
                ConfigureBase(source, "source.pending.human-review");
                ConfigureBase(fact, "fact.jungle.placeholder.pending");
                ConfigureFact(fact, source);
                ConfigureBase(discovery, "discovery.jungle.placeholder");
                ConfigureDiscovery(discovery, category, tag, fact);
                ConfigureCatalog(catalog, category, tag, source, fact, discovery);

                Scene scene = EditorSceneManager.OpenScene(ProjectFoundationSetup.BootstrapScenePath, OpenSceneMode.Single);
                DiagnosticBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<DiagnosticBootstrap>();
                if (bootstrap == null) throw new InvalidOperationException("Bootstrap scene has no DiagnosticBootstrap.");
                bootstrap.ConfigureContentForEditorAndTests(catalog);
                EditorUtility.SetDirty(bootstrap);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                AssetDatabase.SaveAssets();
                Debug.Log("PE_CONTENT_FOUNDATION_SETUP_OK discoveries=1 state=Draft release=false");
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(2);
                throw;
            }
        }

        private static void ConfigureBase(ContentDefinitionAsset asset, string id)
        {
            var serialized = new SerializedObject(asset);
            SerializedProperty stableId = serialized.FindProperty("_id");
            if (string.IsNullOrWhiteSpace(stableId.stringValue)) stableId.stringValue = id;
            SerializedProperty editorial = serialized.FindProperty("_editorial");
            editorial.FindPropertyRelative("_state").enumValueIndex = (int)EditorialState.Draft;
            editorial.FindPropertyRelative("_isPlaceholder").boolValue = true;
            editorial.FindPropertyRelative("_owner").stringValue = "Content Design";
            editorial.FindPropertyRelative("_developmentWatermark").stringValue = "BORRADOR · PH_";
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static void ConfigureFact(EducationalFactDefinitionAsset fact, ContentSourceRecordAsset source)
        {
            var serialized = new SerializedObject(fact);
            serialized.FindProperty("_childCopyTable").stringValue = "Content";
            serialized.FindProperty("_childCopyKey").stringValue = "content.discovery.placeholder.name";
            serialized.FindProperty("_claimForReview").stringValue = "PH_PENDING_FACTUAL_REVIEW";
            SetObjectArray(serialized.FindProperty("_sources"), source);
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureDiscovery(DiscoveryDefinitionAsset discovery, CategoryDefinitionAsset category, TagDefinitionAsset tag, EducationalFactDefinitionAsset fact)
        {
            var serialized = new SerializedObject(discovery);
            serialized.FindProperty("_worldId").stringValue = "world.jungle";
            serialized.FindProperty("_category").objectReferenceValue = category;
            SetObjectArray(serialized.FindProperty("_tags"), tag);
            SetObjectArray(serialized.FindProperty("_facts"), fact);
            serialized.FindProperty("_displayNameTable").stringValue = "Content";
            serialized.FindProperty("_displayNameKey").stringValue = "content.discovery.placeholder.name";
            serialized.FindProperty("_nameAudioCueId").stringValue = "audio.feedback.confirm";
            serialized.FindProperty("_visualAssetId").stringValue = "visual.discovery.jungle.placeholder";
            serialized.FindProperty("_visualAsset").objectReferenceValue = AssetDatabase.LoadAssetAtPath<TextAsset>(VisualPath);
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureCatalog(ContentCatalogAsset catalog, CategoryDefinitionAsset category, TagDefinitionAsset tag, ContentSourceRecordAsset source, EducationalFactDefinitionAsset fact, DiscoveryDefinitionAsset discovery)
        {
            var serialized = new SerializedObject(catalog);
            SetObjectArray(serialized.FindProperty("_categories"), category);
            SetObjectArray(serialized.FindProperty("_tags"), tag);
            SetObjectArray(serialized.FindProperty("_sources"), source);
            SetObjectArray(serialized.FindProperty("_facts"), fact);
            SetObjectArray(serialized.FindProperty("_discoveries"), discovery);
            serialized.FindProperty("_discoveryAliases").arraySize = 0;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
        }

        private static void SetObjectArray(SerializedProperty property, params UnityEngine.Object[] values)
        {
            property.arraySize = values.Length;
            for (int index = 0; index < values.Length; index++) property.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            asset.name = System.IO.Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, System.IO.Path.GetFileName(path));
        }
    }
}
