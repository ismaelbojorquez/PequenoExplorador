using System;
using UnityEngine;

namespace PequenoExplorador.Content.Data
{
    [Serializable]
    public sealed class DiscoveryAliasAsset
    {
        [SerializeField] private string _previousId;
        [SerializeField] private DiscoveryDefinitionAsset _current;
        public string PreviousId => _previousId;
        public DiscoveryDefinitionAsset Current => _current;
    }
}
