using System;
using PequenoExplorador.Application.Content;
using UnityEngine;

namespace PequenoExplorador.Content.Data
{
    [Serializable]
    public sealed class EditorialMetadataAsset
    {
        [SerializeField] private EditorialState _state = EditorialState.Draft;
        [SerializeField] private bool _isPlaceholder = true;
        [SerializeField] private string _owner = "Content Design";
        [SerializeField] private string _developmentWatermark = "BORRADOR · PH_";

        public EditorialState State => _state;
        public bool IsPlaceholder => _isPlaceholder;
        public string Owner => _owner;
        public string DevelopmentWatermark => _developmentWatermark;

        public EditorialMetadata ToRuntime() => new EditorialMetadata(_state, _isPlaceholder, _owner, _developmentWatermark);
    }
}
