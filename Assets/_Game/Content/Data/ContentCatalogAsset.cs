using System;
using System.Collections.Generic;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Domain.Content;
using UnityEngine;

namespace PequenoExplorador.Content.Data
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Content/Content Catalog", fileName = "ContentCatalog")]
    public sealed class ContentCatalogAsset : ScriptableObject
    {
        [SerializeField] private string _id = "catalog.jungle.placeholder";
        [SerializeField] private CategoryDefinitionAsset[] _categories = Array.Empty<CategoryDefinitionAsset>();
        [SerializeField] private TagDefinitionAsset[] _tags = Array.Empty<TagDefinitionAsset>();
        [SerializeField] private ContentSourceRecordAsset[] _sources = Array.Empty<ContentSourceRecordAsset>();
        [SerializeField] private EducationalFactDefinitionAsset[] _facts = Array.Empty<EducationalFactDefinitionAsset>();
        [SerializeField] private DiscoveryDefinitionAsset[] _discoveries = Array.Empty<DiscoveryDefinitionAsset>();
        [SerializeField] private DiscoveryAliasAsset[] _discoveryAliases = Array.Empty<DiscoveryAliasAsset>();

        public IReadOnlyList<CategoryDefinitionAsset> Categories => _categories ?? Array.Empty<CategoryDefinitionAsset>();
        public string RawId => _id;
        public ContentCatalogId Id => ContentCatalogId.Parse(_id);
        public IReadOnlyList<TagDefinitionAsset> Tags => _tags ?? Array.Empty<TagDefinitionAsset>();
        public IReadOnlyList<ContentSourceRecordAsset> Sources => _sources ?? Array.Empty<ContentSourceRecordAsset>();
        public IReadOnlyList<EducationalFactDefinitionAsset> Facts => _facts ?? Array.Empty<EducationalFactDefinitionAsset>();
        public IReadOnlyList<DiscoveryDefinitionAsset> Discoveries => _discoveries ?? Array.Empty<DiscoveryDefinitionAsset>();
        public IReadOnlyList<DiscoveryAliasAsset> DiscoveryAliases => _discoveryAliases ?? Array.Empty<DiscoveryAliasAsset>();

        public bool TryBuildRuntimeCatalog(ContentValidationMode mode, out ContentCatalog catalog, out IReadOnlyList<string> violations) =>
            ContentCatalogCompiler.TryCompile(this, mode, null, out catalog, out violations);
    }
}
