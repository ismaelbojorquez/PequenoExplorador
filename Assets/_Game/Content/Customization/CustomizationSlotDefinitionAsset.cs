using PequenoExplorador.Application.Customization;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;
using UnityEngine;

namespace PequenoExplorador.Content.Customization
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Customization/Slot", fileName = "PH_CustomizationSlot")]
    public sealed class CustomizationSlotDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayNameTable = "UI";
        [SerializeField] private string _displayNameKey;
        [SerializeField] private int _displayOrder;
        [SerializeField] private string _defaultCosmeticId;
        public string RawId => _id;
        public string RawDefaultCosmeticId => _defaultCosmeticId;
        public CustomizationSlotDefinition ToRuntime() => new CustomizationSlotDefinition(CustomizationSlotId.Parse(_id),
            new LocalizedKey(_displayNameTable, _displayNameKey), _displayOrder, CosmeticId.Parse(_defaultCosmeticId));
    }
}
