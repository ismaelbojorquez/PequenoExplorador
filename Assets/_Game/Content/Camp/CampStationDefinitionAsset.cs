using PequenoExplorador.Application.Camp;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;
using UnityEngine;

namespace PequenoExplorador.Content.Camp
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Camp/Station Definition", fileName = "PH_CampStation")]
    public sealed class CampStationDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _actionId;
        [SerializeField] private string _displayNameTable = "UI";
        [SerializeField] private string _displayNameKey;
        [SerializeField] private string _descriptionTable = "UI";
        [SerializeField] private string _descriptionKey;
        [SerializeField, Min(0)] private int _displayOrder;
        [SerializeField] private bool _available;
        [SerializeField] private bool _parentRestricted;

        public string RawId => _id;
        public string RawActionId => _actionId;
        public bool IsAvailable => _available;
        public bool IsParentRestricted => _parentRestricted;
        public CampStationDefinition ToRuntime() => new CampStationDefinition(
            CampStationId.Parse(_id), CampStationActionId.Parse(_actionId),
            new LocalizedKey(_displayNameTable, _displayNameKey),
            new LocalizedKey(_descriptionTable, _descriptionKey),
            _displayOrder, _available, _parentRestricted);
    }
}
