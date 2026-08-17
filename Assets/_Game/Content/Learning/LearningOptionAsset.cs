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
        public string Id => _id; public string Table => _table; public string Key => _key;
    }
}
