using System;
using System.Text;
using PequenoExplorador.Content.Data;
using UnityEditor;

namespace PequenoExplorador.Editor
{
    public static class ContentIdGenerator
    {
        public static bool TryGenerate(ContentDefinitionAsset asset, out string generatedId)
        {
            generatedId = string.Empty;
            if (asset == null || !string.IsNullOrWhiteSpace(asset.RawId)) return false;
            string root = RootFor(asset);
            string slug = Slug(asset.name);
            if (string.IsNullOrEmpty(root) || string.IsNullOrEmpty(slug)) return false;
            generatedId = root + ".draft." + slug;
            var serialized = new SerializedObject(asset);
            SerializedProperty id = serialized.FindProperty("_id");
            if (!string.IsNullOrWhiteSpace(id.stringValue)) { generatedId = string.Empty; return false; }
            id.stringValue = generatedId;
            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(asset);
            return true;
        }

        private static string RootFor(ContentDefinitionAsset asset)
        {
            if (asset is DiscoveryDefinitionAsset) return "discovery";
            if (asset is CategoryDefinitionAsset) return "category";
            if (asset is TagDefinitionAsset) return "tag";
            if (asset is EducationalFactDefinitionAsset) return "fact";
            if (asset is ContentSourceRecordAsset) return "source";
            return string.Empty;
        }

        private static string Slug(string value)
        {
            string source = (value ?? string.Empty).Replace("PH_", string.Empty, StringComparison.OrdinalIgnoreCase);
            var builder = new StringBuilder(source.Length);
            bool separator = false;
            foreach (char raw in source)
            {
                char character = char.ToLowerInvariant(raw);
                if (character >= 'a' && character <= 'z' || character >= '0' && character <= '9')
                {
                    builder.Append(character);
                    separator = false;
                }
                else if (!separator && builder.Length > 0)
                {
                    builder.Append('-');
                    separator = true;
                }
            }
            return builder.ToString().TrimEnd('-');
        }
    }
}
