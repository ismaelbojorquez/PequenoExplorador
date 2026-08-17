using System;
using UnityEngine;

namespace PequenoExplorador.Content.Learning
{
    [Serializable]
    public sealed class LearningOptionAsset
    {
        [SerializeField] private string _id;
        [SerializeField] private string _table = "UI";
        [SerializeField] private string _key;
        [SerializeField] private string _tagId;
        [SerializeField] private Color32 _color = new Color32(255, 255, 255, 255);
        public string Id => _id; public string Table => _table; public string Key => _key;
        public string TagId => _tagId; public Color32 Color => _color;
    }
}
