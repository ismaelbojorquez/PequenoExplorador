using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Camp;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PequenoExplorador.Content.Camp
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Camp/Upgrade Definition", fileName = "PH_CampUpgrade")]
    public sealed class CampUpgradeDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _stationId;
        [SerializeField] private string _displayNameTable = "UI";
        [SerializeField] private string _displayNameKey;
        [SerializeField] private string _descriptionTable = "UI";
        [SerializeField] private string _descriptionKey;
        [SerializeField] private string _previewTable = "UI";
        [SerializeField] private string _previewKey;
        [SerializeField, Min(1)] private int _starCost = 3;
        [SerializeField] private string _spendReasonId;
        [SerializeField] private string _beforeVisualId;
        [SerializeField] private string _afterVisualId;
        [SerializeField] private AssetReferenceGameObject _beforeVariant;
        [SerializeField] private AssetReferenceGameObject _afterVariant;
        [SerializeField] private string[] _prerequisiteIds = Array.Empty<string>();
        [SerializeField] private EditorialMetadataAsset _editorial = new EditorialMetadataAsset();

        public string RawId => _id;
        public string RawStationId => _stationId;
        public int StarCost => _starCost;
        public AssetReferenceGameObject BeforeVariant => _beforeVariant;
        public AssetReferenceGameObject AfterVariant => _afterVariant;
        public IReadOnlyList<string> PrerequisiteIds => _prerequisiteIds ?? Array.Empty<string>();
        public EditorialMetadataAsset Editorial => _editorial;
        public CampUpgradeDefinition ToRuntime() => new CampUpgradeDefinition(
            CampUpgradeId.Parse(_id), CampStationId.Parse(_stationId),
            new LocalizedKey(_displayNameTable, _displayNameKey),
            new LocalizedKey(_descriptionTable, _descriptionKey),
            new LocalizedKey(_previewTable, _previewKey),
            new ExplorerStars(_starCost), RewardId.Parse(_spendReasonId),
            VisualAssetId.Parse(_beforeVisualId), VisualAssetId.Parse(_afterVisualId),
            PrerequisiteIds.Select(CampUpgradeId.Parse), _editorial.IsPlaceholder);
    }
}
