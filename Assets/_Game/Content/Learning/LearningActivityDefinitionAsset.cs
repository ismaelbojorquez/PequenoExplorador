using System;
using System.Collections.Generic;
using PequenoExplorador.Content.Data;
using UnityEngine;

namespace PequenoExplorador.Content.Learning
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Learning/Activity", fileName = "LearningActivity")]
    public sealed class LearningActivityDefinitionAsset : ContentDefinitionAsset
    {
        [SerializeField] private string _typeId;
        [SerializeField] private string _titleTable = "UI"; [SerializeField] private string _titleKey;
        [SerializeField] private string _instructionTable = "UI"; [SerializeField] private string _instructionKey;
        [SerializeField] private string _successTable = "UI"; [SerializeField] private string _successKey;
        [SerializeField] private string _tryAgainTable = "UI"; [SerializeField] private string _tryAgainKey;
        [SerializeField] private string[] _conceptIds = Array.Empty<string>();
        [SerializeField] private LearningOptionAsset[] _options = Array.Empty<LearningOptionAsset>();
        [SerializeField] private string _correctOptionId;
        [SerializeField] private string[] _hintKeys = Array.Empty<string>();
        [SerializeField, Min(1)] private int _firstAutomaticHintAttempt = 2;
        [SerializeField, Min(1)] private int _maximumHintLevel = 3;
        [SerializeField] private bool _resumable = true;
        [SerializeField] private string _rewardId;
        public string TypeId => _typeId; public string TitleTable => _titleTable; public string TitleKey => _titleKey;
        public string InstructionTable => _instructionTable; public string InstructionKey => _instructionKey;
        public string SuccessTable => _successTable; public string SuccessKey => _successKey;
        public string TryAgainTable => _tryAgainTable; public string TryAgainKey => _tryAgainKey;
        public IReadOnlyList<string> ConceptIds => _conceptIds ?? Array.Empty<string>();
        public IReadOnlyList<LearningOptionAsset> Options => _options ?? Array.Empty<LearningOptionAsset>();
        public string CorrectOptionId => _correctOptionId; public IReadOnlyList<string> HintKeys => _hintKeys ?? Array.Empty<string>();
        public int FirstAutomaticHintAttempt => _firstAutomaticHintAttempt; public int MaximumHintLevel => _maximumHintLevel;
        public bool Resumable => _resumable; public string RewardId => _rewardId;
    }
}
