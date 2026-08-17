using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Learning;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Content.Data;
using PequenoExplorador.Domain.Content;
using UnityEngine;

namespace PequenoExplorador.Content.Learning
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Learning/Catalog", fileName = "LearningCatalog")]
    public sealed class LearningCatalogAsset : ScriptableObject
    {
        [SerializeField] private LearningConceptDefinitionAsset[] _concepts = Array.Empty<LearningConceptDefinitionAsset>();
        [SerializeField] private LearningActivityDefinitionAsset[] _activities = Array.Empty<LearningActivityDefinitionAsset>();
        public IReadOnlyList<LearningConceptDefinitionAsset> Concepts => _concepts ?? Array.Empty<LearningConceptDefinitionAsset>();
        public IReadOnlyList<LearningActivityDefinitionAsset> Activities => _activities ?? Array.Empty<LearningActivityDefinitionAsset>();

        public bool TryBuild(ContentValidationMode mode, IRewardCatalog rewards, Func<LocalizedKey, bool> hasLocalization,
            out LearningCatalog catalog, out IReadOnlyList<string> violations)
        {
            var errors = new List<string>(); var concepts = new List<LearningConceptDefinition>(); var activities = new List<LearningActivityDefinition>();
            foreach (LearningConceptDefinitionAsset asset in Concepts)
            {
                if (asset == null) { errors.Add("LEARN001 missing concept reference."); continue; }
                try
                {
                    var label = new LocalizedKey(asset.LabelTable, asset.LabelKey); ValidateKey(label, hasLocalization, asset.name, errors);
                    EditorialMetadata editorial = asset.Editorial.ToRuntime();
                    if (mode == ContentValidationMode.Release && !editorial.IsReleaseApproved) errors.Add($"LEARN002 Release rejects concept '{asset.RawId}'.");
                    concepts.Add(new LearningConceptDefinition(LearningConceptId.Parse(asset.RawId), label, editorial));
                }
                catch (Exception exception) { errors.Add($"LEARN003 invalid concept '{asset.name}': {exception.Message}"); }
            }
            foreach (LearningActivityDefinitionAsset asset in Activities)
            {
                if (asset == null) { errors.Add("LEARN004 missing activity reference."); continue; }
                try
                {
                    var keys = new[] { new LocalizedKey(asset.TitleTable, asset.TitleKey), new LocalizedKey(asset.InstructionTable, asset.InstructionKey),
                        new LocalizedKey(asset.SuccessTable, asset.SuccessKey), new LocalizedKey(asset.TryAgainTable, asset.TryAgainKey) };
                    foreach (LocalizedKey key in keys) ValidateKey(key, hasLocalization, asset.name, errors);
                    LocalizedKey[] hints = asset.HintKeys.Select(key => new LocalizedKey("UI", key)).ToArray();
                    foreach (LocalizedKey hint in hints) ValidateKey(hint, hasLocalization, asset.name, errors);
                    LearningOptionDefinition[] options = asset.Options.Select(item =>
                    {
                        if (item == null) throw new ArgumentException("Option reference is missing.");
                        var key = new LocalizedKey(item.Table, item.Key); ValidateKey(key, hasLocalization, asset.name, errors);
                        return new LearningOptionDefinition(LearningOptionId.Parse(item.Id), key);
                    }).ToArray();
                    ActivityId id = ActivityId.Parse(asset.RawId); RewardId rewardId = RewardId.Parse(asset.RewardId);
                    if (rewards == null || !rewards.TryGet(rewardId, out RewardDefinition reward) || reward.SourceKind != RewardSourceKind.Activity || reward.SourceId != id.Value)
                        errors.Add($"LEARN005 activity '{id}' reward is missing or has the wrong owner.");
                    EditorialMetadata editorial = asset.Editorial.ToRuntime();
                    if (mode == ContentValidationMode.Release && !editorial.IsReleaseApproved) errors.Add($"LEARN006 Release rejects {editorial.State}/placeholder activity '{id}'.");
                    activities.Add(new LearningActivityDefinition(id, LearningActivityTypeId.Parse(asset.TypeId), keys[0], keys[1], keys[2], keys[3],
                        asset.ConceptIds.Select(LearningConceptId.Parse), options, LearningOptionId.Parse(asset.CorrectOptionId), hints,
                        new HintPolicy(asset.FirstAutomaticHintAttempt, asset.MaximumHintLevel), asset.Resumable, rewardId, editorial));
                }
                catch (Exception exception) { errors.Add($"LEARN007 invalid activity '{asset.name}': {exception.Message}"); }
            }
            if (!activities.Any(item => item.Id.Value == "activity.fixture.visual-matching")) errors.Add("LEARN008 Development fixture activity is required.");
            try { catalog = errors.Count == 0 ? new LearningCatalog(activities, concepts) : null; }
            catch (Exception exception) { errors.Add("LEARN009 catalog index failed: " + exception.Message); catalog = null; }
            violations = errors; return errors.Count == 0;
        }

        private static void ValidateKey(LocalizedKey key, Func<LocalizedKey, bool> resolver, string owner, ICollection<string> errors)
        { if (resolver != null && !resolver(key)) errors.Add($"LEARN010 '{owner}' is missing ES/EN localization '{key}'."); }
    }
}
