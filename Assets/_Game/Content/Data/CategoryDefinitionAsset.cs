using UnityEngine;

namespace PequenoExplorador.Content.Data
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Content/Category Definition", fileName = "PH_Category")]
    public sealed class CategoryDefinitionAsset : ContentDefinitionAsset
    {
        [SerializeField] private string _displayNameTable = "Content";
        [SerializeField] private string _displayNameKey;
        public string DisplayNameTable => _displayNameTable;
        public string DisplayNameKey => _displayNameKey;
    }
}
