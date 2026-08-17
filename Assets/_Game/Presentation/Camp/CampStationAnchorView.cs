using PequenoExplorador.Domain.Content;
using UnityEngine;

namespace PequenoExplorador.Presentation.Camp
{
    [DisallowMultipleComponent]
    public sealed class CampStationAnchorView : MonoBehaviour
    {
        [SerializeField] private string _stationId;
        public string RawStationId => _stationId;
        public CampStationId StationId => CampStationId.Parse(_stationId);
    }
}
