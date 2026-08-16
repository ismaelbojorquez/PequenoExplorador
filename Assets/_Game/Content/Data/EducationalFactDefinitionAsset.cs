using System;
using System.Collections.Generic;
using UnityEngine;

namespace PequenoExplorador.Content.Data
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Content/Educational Fact Definition", fileName = "PH_Fact")]
    public sealed class EducationalFactDefinitionAsset : ContentDefinitionAsset
    {
        [SerializeField] private string _childCopyTable = "Content";
        [SerializeField] private string _childCopyKey;
        [SerializeField, TextArea] private string _claimForReview;
        [SerializeField] private ContentSourceRecordAsset[] _sources = Array.Empty<ContentSourceRecordAsset>();
        public string ChildCopyTable => _childCopyTable;
        public string ChildCopyKey => _childCopyKey;
        public string ClaimForReview => _claimForReview;
        public IReadOnlyList<ContentSourceRecordAsset> Sources => _sources ?? Array.Empty<ContentSourceRecordAsset>();
    }
}
