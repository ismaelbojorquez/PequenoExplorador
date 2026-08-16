using System;
using System.Collections.Generic;
using PequenoExplorador.Application.Worlds;
using PequenoExplorador.Content.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PequenoExplorador.Content.Worlds
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Worlds/World Manifest", fileName = "PH_WorldManifest")]
    public sealed class WorldManifestAsset : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private int _manifestVersion = 1;
        [SerializeField] private string _contentVersion = "0.1.0-placeholder";
        [SerializeField] private string _displayNameTable = "Content";
        [SerializeField] private string _displayNameKey;
        [SerializeField] private AssetReference _scene;
        [SerializeField] private string _sceneAddress;
        [SerializeField] private string[] _labels = Array.Empty<string>();
        [SerializeField] private string _spawnPointId;
        [SerializeField] private string[] _checkpointIds = Array.Empty<string>();
        [SerializeField] private ContentCatalogAsset[] _contentCatalogs = Array.Empty<ContentCatalogAsset>();
        [SerializeField] private string _musicCueId;
        [SerializeField] private string _ambienceCueId;
        [SerializeField] private string[] _requirements = Array.Empty<string>();
        [SerializeField] private long _estimatedInstalledBytes;
        [SerializeField] private WorldAvailabilityState _availability = WorldAvailabilityState.Available;
        [SerializeField] private EditorialMetadataAsset _editorial = new EditorialMetadataAsset();

        public string RawId => _id;
        public int ManifestVersion => _manifestVersion;
        public string ContentVersion => _contentVersion;
        public string DisplayNameTable => _displayNameTable;
        public string DisplayNameKey => _displayNameKey;
        public AssetReference Scene => _scene;
        public string SceneAddress => _sceneAddress;
        public IReadOnlyList<string> Labels => _labels ?? Array.Empty<string>();
        public string SpawnPointId => _spawnPointId;
        public IReadOnlyList<string> CheckpointIds => _checkpointIds ?? Array.Empty<string>();
        public IReadOnlyList<ContentCatalogAsset> ContentCatalogs => _contentCatalogs ?? Array.Empty<ContentCatalogAsset>();
        public string MusicCueId => _musicCueId;
        public string AmbienceCueId => _ambienceCueId;
        public IReadOnlyList<string> Requirements => _requirements ?? Array.Empty<string>();
        public long EstimatedInstalledBytes => _estimatedInstalledBytes;
        public WorldAvailabilityState Availability => _availability;
        public EditorialMetadataAsset Editorial => _editorial;
    }
}
