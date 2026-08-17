using PequenoExplorador.Application.Economy;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using UnityEngine;

namespace PequenoExplorador.Content.Economy
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Economy/Reward Definition", fileName = "RewardDefinition")]
    public sealed class RewardDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField, Min(1)] private int _stars = 1;
        [SerializeField] private RewardSourceKind _sourceKind = RewardSourceKind.Discovery;
        [SerializeField] private string _sourceId;
        public string RawId => _id;
        public int Stars => _stars;
        public RewardSourceKind SourceKind => _sourceKind;
        public string SourceId => _sourceId;
        public RewardDefinition ToRuntime() => new RewardDefinition(RewardId.Parse(_id), new ExplorerStars(_stars), _sourceKind, _sourceId);
    }
}
