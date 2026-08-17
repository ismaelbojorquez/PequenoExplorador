using System;
using UnityEngine;

namespace PequenoExplorador.Content.Missions
{
    [Serializable]
    public sealed class MissionObjectiveAsset
    {
        [SerializeField] private string _id;
        [SerializeField] private string _typeId;
        [SerializeField] private string _labelTable = "UI";
        [SerializeField] private string _labelKey;
        [SerializeField, Min(1)] private int _targetCount = 1;
        [SerializeField] private string _subjectId;
        [SerializeField] private string _requiredTagId;
        public string Id => _id;
        public string TypeId => _typeId;
        public string LabelTable => _labelTable;
        public string LabelKey => _labelKey;
        public int TargetCount => _targetCount;
        public string SubjectId => _subjectId;
        public string RequiredTagId => _requiredTagId;
    }
}
