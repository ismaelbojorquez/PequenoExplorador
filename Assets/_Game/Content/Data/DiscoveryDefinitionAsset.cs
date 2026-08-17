using System;
using System.Collections.Generic;
using UnityEngine;

namespace PequenoExplorador.Content.Data
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Content/Discovery Definition", fileName = "PH_Discovery")]
    public sealed class DiscoveryDefinitionAsset : ContentDefinitionAsset
    {
        [SerializeField] private string _worldId;
        [SerializeField] private CategoryDefinitionAsset _category;
        [SerializeField] private TagDefinitionAsset[] _tags = Array.Empty<TagDefinitionAsset>();
        [SerializeField] private EducationalFactDefinitionAsset[] _facts = Array.Empty<EducationalFactDefinitionAsset>();
        [SerializeField] private string _displayNameTable = "Content";
        [SerializeField] private string _displayNameKey;
        [SerializeField] private string _nameAudioCueId;
        [SerializeField] private string _visualAssetId;
        [SerializeField] private UnityEngine.Object _visualAsset;
        [Header("Album")]
        [SerializeField] private EducationalFactDefinitionAsset _albumHabitatFact;
        [SerializeField] private EducationalFactDefinitionAsset _albumDietFact;
        [SerializeField] private EducationalFactDefinitionAsset _albumSizeFact;
        [SerializeField] private EducationalFactDefinitionAsset _albumCuriosityFact;
        [SerializeField] private EducationalFactDefinitionAsset _albumSoundFact;
        [SerializeField] private bool _albumHasPlayableAudio;

        public string WorldId => _worldId;
        public CategoryDefinitionAsset Category => _category;
        public IReadOnlyList<TagDefinitionAsset> Tags => _tags ?? Array.Empty<TagDefinitionAsset>();
        public IReadOnlyList<EducationalFactDefinitionAsset> Facts => _facts ?? Array.Empty<EducationalFactDefinitionAsset>();
        public string DisplayNameTable => _displayNameTable;
        public string DisplayNameKey => _displayNameKey;
        public string NameAudioCueId => _nameAudioCueId;
        public string VisualAssetId => _visualAssetId;
        public UnityEngine.Object VisualAsset => _visualAsset;
        public EducationalFactDefinitionAsset AlbumHabitatFact => _albumHabitatFact;
        public EducationalFactDefinitionAsset AlbumDietFact => _albumDietFact;
        public EducationalFactDefinitionAsset AlbumSizeFact => _albumSizeFact;
        public EducationalFactDefinitionAsset AlbumCuriosityFact => _albumCuriosityFact;
        public EducationalFactDefinitionAsset AlbumSoundFact => _albumSoundFact;
        public bool AlbumHasPlayableAudio => _albumHasPlayableAudio;
    }
}
