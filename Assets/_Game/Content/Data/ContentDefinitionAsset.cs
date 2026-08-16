using UnityEngine;

namespace PequenoExplorador.Content.Data
{
    public abstract class ContentDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private EditorialMetadataAsset _editorial = new EditorialMetadataAsset();
        public string RawId => _id;
        public EditorialMetadataAsset Editorial => _editorial;

#if UNITY_EDITOR
        public void ConfigureIdentityForEditorAndTests(string id) => _id = id;
#endif
    }
}
