using PequenoExplorador.Content.Data;
using UnityEngine;

namespace PequenoExplorador.Content.Learning
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Learning/Concept", fileName = "LearningConcept")]
    public sealed class LearningConceptDefinitionAsset : ContentDefinitionAsset
    {
        [SerializeField] private string _labelTable = "UI";
        [SerializeField] private string _labelKey;
        public string LabelTable => _labelTable; public string LabelKey => _labelKey;
    }
}
