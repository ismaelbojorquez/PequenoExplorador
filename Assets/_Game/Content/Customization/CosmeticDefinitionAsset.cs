using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Customization;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using UnityEngine;

namespace PequenoExplorador.Content.Customization
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Customization/Cosmetic", fileName = "PH_Cosmetic")]
    public sealed class CosmeticDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _slotId;
        [SerializeField] private string _displayNameTable = "UI";
        [SerializeField] private string _displayNameKey;
        [SerializeField] private string _visualId;
        [SerializeField] private Color32 _color = new Color32(255, 255, 255, 255);
        [SerializeField] private bool _initiallyUnlocked;
        [SerializeField, Min(0)] private int _starCost;
        [SerializeField] private string _spendReasonId;
        [SerializeField] private string _requiredCampUpgradeId;
        [SerializeField] private string[] _compatibilityTags = Array.Empty<string>();
        [SerializeField] private string[] _blockedTags = Array.Empty<string>();
        [SerializeField] private EditorialMetadataAsset _editorial = new EditorialMetadataAsset();
        public string RawId => _id;
        public string RawSlotId => _slotId;
        public int StarCost => _starCost;
        public bool InitiallyUnlocked => _initiallyUnlocked;
        public EditorialMetadataAsset Editorial => _editorial;
        public CosmeticDefinition ToRuntime()
        {
            CampUpgradeId requirement = string.IsNullOrWhiteSpace(_requiredCampUpgradeId) ? default : CampUpgradeId.Parse(_requiredCampUpgradeId);
            RewardId spendReason = string.IsNullOrWhiteSpace(_spendReasonId) ? default : RewardId.Parse(_spendReasonId);
            return new CosmeticDefinition(CosmeticId.Parse(_id), CustomizationSlotId.Parse(_slotId),
                new LocalizedKey(_displayNameTable, _displayNameKey), VisualAssetId.Parse(_visualId),
                new CustomizationColor(_color.r, _color.g, _color.b, _color.a), _initiallyUnlocked,
                new ExplorerStars(_starCost), spendReason, requirement,
                (_compatibilityTags ?? Array.Empty<string>()).Select(CosmeticCompatibilityTagId.Parse),
                (_blockedTags ?? Array.Empty<string>()).Select(CosmeticCompatibilityTagId.Parse), _editorial.IsPlaceholder);
        }
    }
}
