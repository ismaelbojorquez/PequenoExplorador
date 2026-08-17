using System;
using System.Collections.Generic;
using PequenoExplorador.Content.Data;
using UnityEngine;

namespace PequenoExplorador.Content.Missions
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Content/Mission Definition", fileName = "MissionDefinition")]
    public sealed class MissionDefinitionAsset : ContentDefinitionAsset
    {
        [SerializeField] private string _titleTable = "UI";
        [SerializeField] private string _titleKey;
        [SerializeField] private string _summaryTable = "UI";
        [SerializeField] private string _summaryKey;
        [SerializeField] private string _completionTable = "UI";
        [SerializeField] private string _completionKey;
        [SerializeField] private MissionObjectiveAsset[] _objectives = Array.Empty<MissionObjectiveAsset>();
        [SerializeField] private string[] _prerequisiteIds = Array.Empty<string>();
        [SerializeField] private string _rewardId;
        public string TitleTable => _titleTable;
        public string TitleKey => _titleKey;
        public string SummaryTable => _summaryTable;
        public string SummaryKey => _summaryKey;
        public string CompletionTable => _completionTable;
        public string CompletionKey => _completionKey;
        public IReadOnlyList<MissionObjectiveAsset> Objectives => _objectives ?? Array.Empty<MissionObjectiveAsset>();
        public IReadOnlyList<string> PrerequisiteIds => _prerequisiteIds ?? Array.Empty<string>();
        public string RewardId => _rewardId;
    }
}
