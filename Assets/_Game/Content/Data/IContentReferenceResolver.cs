using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;
using UnityEngine;

namespace PequenoExplorador.Content.Data
{
    public interface IContentReferenceResolver
    {
        string Describe(Object asset);
        bool HasLocalization(LocalizedKey key);
        bool HasAudioCue(AudioCueId cueId);
        bool HasVisualAsset(VisualAssetId id, Object asset);
    }
}
