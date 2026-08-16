using UnityEngine;

namespace PequenoExplorador.Content.Data
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Content/Source Record", fileName = "PH_Source")]
    public sealed class ContentSourceRecordAsset : ContentDefinitionAsset
    {
        [SerializeField] private string _institution;
        [SerializeField] private string _author;
        [SerializeField] private string _title;
        [SerializeField] private string _reference;
        [SerializeField] private string _consultedOn;
        [SerializeField] private string _reviewer;
        public string Institution => _institution;
        public string Author => _author;
        public string Title => _title;
        public string Reference => _reference;
        public string ConsultedOn => _consultedOn;
        public string Reviewer => _reviewer;
    }
}
