using System;
using System.Collections.Generic;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Worlds;
using PequenoExplorador.Content.Data;
using UnityEngine;

namespace PequenoExplorador.Content.Worlds
{
    [CreateAssetMenu(menuName = "Pequeño Explorador/Worlds/World Catalog", fileName = "WorldCatalog")]
    public sealed class WorldCatalogAsset : ScriptableObject
    {
        [SerializeField] private WorldManifestAsset[] _worlds = Array.Empty<WorldManifestAsset>();
        public IReadOnlyList<WorldManifestAsset> Worlds => _worlds ?? Array.Empty<WorldManifestAsset>();

        public bool TryBuildRuntimeCatalog(
            IContentCatalog contentCatalog,
            ContentValidationMode mode,
            out WorldCatalog catalog,
            out IReadOnlyList<string> violations) =>
            WorldCatalogCompiler.TryCompile(this, contentCatalog, mode, null, out catalog, out violations);
    }
}
