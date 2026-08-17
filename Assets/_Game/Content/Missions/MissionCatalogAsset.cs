using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Missions;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Domain.Content;
using UnityEngine;

namespace PequenoExplorador.Content.Missions
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Content/Mission Catalog", fileName = "MissionCatalog")]
    public sealed class MissionCatalogAsset : ScriptableObject
    {
        [SerializeField] private MissionDefinitionAsset[] _definitions = Array.Empty<MissionDefinitionAsset>();
        public IReadOnlyList<MissionDefinitionAsset> Definitions => _definitions ?? Array.Empty<MissionDefinitionAsset>();

        public bool TryBuild(ContentValidationMode mode, IRewardCatalog rewards, IContentCatalog content,
            Func<LocalizedKey, bool> hasLocalization, out MissionCatalog catalog, out IReadOnlyList<string> violations)
        {
            var errors = new List<string>();
            var definitions = new List<MissionDefinition>();
            rewards ??= RewardCatalog.Empty;
            content ??= ContentCatalog.Empty;
            foreach (MissionDefinitionAsset asset in Definitions)
            {
                if (asset == null) { errors.Add("MISSION001 catalog contains a missing definition."); continue; }
                try
                {
                    var title = new LocalizedKey(asset.TitleTable, asset.TitleKey);
                    var summary = new LocalizedKey(asset.SummaryTable, asset.SummaryKey);
                    var completion = new LocalizedKey(asset.CompletionTable, asset.CompletionKey);
                    ValidateLocalization(hasLocalization, title, asset.name, errors);
                    ValidateLocalization(hasLocalization, summary, asset.name, errors);
                    ValidateLocalization(hasLocalization, completion, asset.name, errors);
                    MissionObjectiveDefinition[] objectives = asset.Objectives.Select(item => BuildObjective(item, content, hasLocalization, asset.name, errors)).Where(item => item != null).ToArray();
                    MissionId id = MissionId.Parse(asset.RawId);
                    RewardId rewardId = RewardId.Parse(asset.RewardId);
                    if (!rewards.TryGet(rewardId, out RewardDefinition reward) || reward.SourceKind != RewardSourceKind.Mission ||
                        !string.Equals(reward.SourceId, id.Value, StringComparison.Ordinal))
                        errors.Add($"MISSION002 '{asset.name}' reward '{rewardId}' is missing or not owned by mission '{id}'.");
                    EditorialMetadata editorial = asset.Editorial?.ToRuntime() ?? throw new ArgumentException("Editorial metadata is required.");
                    if (mode == ContentValidationMode.Release && !editorial.IsReleaseApproved)
                        errors.Add($"MISSION003 Release rejects {editorial.State} or placeholder mission '{id}'.");
                    definitions.Add(new MissionDefinition(id, title, summary, completion, objectives,
                        asset.PrerequisiteIds.Select(MissionId.Parse), rewardId, editorial));
                }
                catch (Exception exception) when (exception is ArgumentException || exception is FormatException)
                {
                    errors.Add($"MISSION004 invalid definition '{asset.name}': {exception.Message}");
                }
            }
            ValidateGraph(definitions, errors);
            if (!definitions.Any(item => item.Id.Equals(MissionId.Parse("mission.vertical-slice.photograph-toucan"))))
                errors.Add("MISSION005 fixture mission 'mission.vertical-slice.photograph-toucan' is required.");
            try { catalog = errors.Count == 0 ? new MissionCatalog(definitions) : null; }
            catch (Exception exception) { errors.Add("MISSION006 catalog index failed: " + exception.Message); catalog = null; }
            violations = errors;
            return errors.Count == 0;
        }

        private static MissionObjectiveDefinition BuildObjective(MissionObjectiveAsset asset, IContentCatalog content,
            Func<LocalizedKey, bool> hasLocalization, string owner, ICollection<string> errors)
        {
            if (asset == null) { errors.Add($"MISSION007 '{owner}' has a missing objective."); return null; }
            var label = new LocalizedKey(asset.LabelTable, asset.LabelKey);
            ValidateLocalization(hasLocalization, label, owner, errors);
            MissionObjectiveTypeId type = MissionObjectiveTypeId.Parse(asset.TypeId);
            if (!type.Equals(MissionObjectiveTypeIds.DiscoverCount) && !type.Equals(MissionObjectiveTypeIds.PhotographSpecific) && !type.Equals(MissionObjectiveTypeIds.InteractTag))
                errors.Add($"MISSION008 '{owner}' uses unregistered objective type '{type}'.");
            TagId tag = string.IsNullOrWhiteSpace(asset.RequiredTagId) ? default : TagId.Parse(asset.RequiredTagId);
            if (tag.IsValid && !content.TryGetTag(tag, out _)) errors.Add($"MISSION009 '{owner}' references missing tag '{tag}'.");
            if (type.Equals(MissionObjectiveTypeIds.PhotographSpecific))
            {
                if (!DiscoveryId.TryParse(asset.SubjectId, out DiscoveryId discovery) || !content.TryResolveDiscovery(discovery, out _))
                    errors.Add($"MISSION010 '{owner}' references missing photograph discovery '{asset.SubjectId}'.");
            }
            if (type.Equals(MissionObjectiveTypeIds.InteractTag) && !tag.IsValid)
                errors.Add($"MISSION011 '{owner}' interact-tag objective requires a tag.");
            return new MissionObjectiveDefinition(MissionObjectiveId.Parse(asset.Id), type, label,
                asset.TargetCount, asset.SubjectId, tag);
        }

        private static void ValidateLocalization(Func<LocalizedKey, bool> resolver, LocalizedKey key,
            string owner, ICollection<string> errors)
        {
            if (resolver != null && !resolver(key)) errors.Add($"MISSION012 '{owner}' is missing ES/EN localization '{key}'.");
        }

        private static void ValidateGraph(IReadOnlyCollection<MissionDefinition> definitions, ICollection<string> errors)
        {
            var byId = definitions.ToDictionary(item => item.Id);
            foreach (MissionDefinition definition in definitions)
                foreach (MissionId required in definition.Prerequisites)
                    if (!byId.ContainsKey(required)) errors.Add($"MISSION013 '{definition.Id}' prerequisite '{required}' is missing.");
            foreach (MissionDefinition definition in definitions)
                if (HasCycle(definition.Id, definition.Id, byId, new HashSet<MissionId>()))
                    errors.Add($"MISSION014 prerequisite cycle reaches '{definition.Id}'.");
        }

        private static bool HasCycle(MissionId root, MissionId current, IReadOnlyDictionary<MissionId, MissionDefinition> byId, HashSet<MissionId> visited)
        {
            if (!visited.Add(current) || !byId.TryGetValue(current, out MissionDefinition definition)) return false;
            foreach (MissionId next in definition.Prerequisites)
                if (next.Equals(root) || HasCycle(root, next, byId, new HashSet<MissionId>(visited))) return true;
            return false;
        }
    }
}
