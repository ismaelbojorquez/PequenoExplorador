using PequenoExplorador.Content.Data;
using UnityEditor;
using UnityEngine;

namespace PequenoExplorador.Editor
{
    [CustomEditor(typeof(ContentCatalogAsset))]
    public sealed class ContentCatalogInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("El catálogo se compila a modelos readonly e índices O(1). El orden del array no gobierna runtime; IDs y aliases sí.", MessageType.Info);
            EditorGUILayout.HelpBox("Release acepta únicamente contenido Approved y no-placeholder. Ejecute scripts/validate-content y revise artifacts/reports.", MessageType.Warning);
            DrawDefaultInspector();
            var catalog = (ContentCatalogAsset)target;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Compiled discovery preview", EditorStyles.boldLabel);
            foreach (DiscoveryDefinitionAsset discovery in catalog.Discoveries)
            {
                if (discovery == null) continue;
                EditorGUILayout.HelpBox($"{discovery.RawId} · {discovery.Editorial.State} · {discovery.Editorial.DevelopmentWatermark}", MessageType.None);
            }
        }
    }
}
