using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Economy;
using UnityEngine;

namespace PequenoExplorador.Content.Economy
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Economy/Reward Catalog", fileName = "RewardCatalog")]
    public sealed class RewardCatalogAsset : ScriptableObject
    {
        [SerializeField] private RewardDefinitionAsset[] _definitions = Array.Empty<RewardDefinitionAsset>();
        public IReadOnlyList<RewardDefinitionAsset> Definitions => _definitions ?? Array.Empty<RewardDefinitionAsset>();
        public bool TryBuild(out RewardCatalog catalog, out IReadOnlyList<string> violations)
        {
            var errors = new List<string>();
            RewardDefinition[] definitions = Definitions.Where(item => item != null).Select(item =>
            {
                try { return item.ToRuntime(); }
                catch (Exception exception) { errors.Add($"ECONOMY001 invalid reward '{item.name}': {exception.Message}"); return null; }
            }).Where(item => item != null).ToArray();
            if (Definitions.Any(item => item == null)) errors.Add("ECONOMY002 reward catalog contains a missing reference.");
            if (definitions.Length == 0) errors.Add("ECONOMY003 reward catalog must contain at least one definition.");
            try { catalog = errors.Count == 0 ? new RewardCatalog(definitions) : null; }
            catch (Exception exception) { errors.Add("ECONOMY004 reward catalog failed: " + exception.Message); catalog = null; }
            violations = errors;
            return errors.Count == 0;
        }
    }
}
