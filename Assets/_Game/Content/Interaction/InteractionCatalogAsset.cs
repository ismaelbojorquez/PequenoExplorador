using System;
using System.Collections.Generic;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Content.Data;
using UnityEngine;

namespace PequenoExplorador.Content.Interaction
{
    [CreateAssetMenu(
        menuName = "Pequeño Explorador/Content/Interaction Catalog",
        fileName = "InteractionCatalog")]
    public sealed class InteractionCatalogAsset : ScriptableObject
    {
        [SerializeField] private InteractionDefinitionAsset[] _definitions =
            Array.Empty<InteractionDefinitionAsset>();

        public IReadOnlyList<InteractionDefinitionAsset> Definitions =>
            _definitions ?? Array.Empty<InteractionDefinitionAsset>();

        public bool TryBuildRuntimeCatalog(
            ContentValidationMode mode,
            out InteractionCatalog catalog,
            out IReadOnlyList<string> violations) =>
            InteractionCatalogCompiler.TryCompile(this, mode, null, out catalog, out violations);
    }
}
