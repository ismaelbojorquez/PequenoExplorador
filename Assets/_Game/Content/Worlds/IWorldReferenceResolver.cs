using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Domain.Content;
using UnityEngine.AddressableAssets;

namespace PequenoExplorador.Content.Worlds
{
    public interface IWorldReferenceResolver
    {
        string Describe(WorldManifestAsset asset);
        bool HasLocalization(LocalizedKey key);
        bool HasAudioCue(AudioCueId cueId);
        bool HasLocalScene(AssetReference scene, SceneContentId address, WorldId worldId, System.Collections.Generic.IReadOnlyList<string> labels);
        bool HasSpawnPoint(AssetReference scene, SpawnPointId spawnPoint);
    }
}
