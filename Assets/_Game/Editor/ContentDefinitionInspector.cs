using PequenoExplorador.Application.Content;
using PequenoExplorador.Content.Data;
using UnityEditor;
using UnityEngine;

namespace PequenoExplorador.Editor
{
    [CustomEditor(typeof(ContentDefinitionAsset), true)]
    public sealed class ContentDefinitionInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var definition = (ContentDefinitionAsset)target;
            EditorialState state = definition.Editorial?.State ?? EditorialState.Draft;
            if (state == EditorialState.Draft || definition.Editorial?.IsPlaceholder == true)
                EditorGUILayout.HelpBox("BORRADOR · PH_: visible solo en Development con watermark. Release lo rechazará.", MessageType.Warning);
            else if (state == EditorialState.Approved)
                EditorGUILayout.HelpBox("Approved no sustituye revisión de referencias, licencia ni build validator.", MessageType.Info);

            DrawDefaultInspector();
            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(!string.IsNullOrWhiteSpace(definition.RawId)))
            {
                if (GUILayout.Button("Generate stable ID if empty"))
                {
                    if (ContentIdGenerator.TryGenerate(definition, out string id))
                        Debug.Log("PE_CONTENT_ID_GENERATED asset=" + AssetDatabase.GetAssetPath(definition) + " id=" + id);
                }
            }
            if (!string.IsNullOrWhiteSpace(definition.RawId))
                EditorGUILayout.HelpBox("El ID existente es estable y el generador nunca lo sobrescribe. Si se retira, registre alias/migración en ContentCatalog.", MessageType.None);
        }
    }
}
