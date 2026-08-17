using System;
using System.Linq;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Presentation.Customization;
using UnityEngine;

namespace PequenoExplorador.Presentation.Camp
{
    [DisallowMultipleComponent]
    public sealed class CampSceneRoot : MonoBehaviour
    {
        [SerializeField] private CampStationAnchorView[] _anchors = Array.Empty<CampStationAnchorView>();
        [SerializeField] private CampUpgradeVisualView[] _upgradeVisuals = Array.Empty<CampUpgradeVisualView>();
        [SerializeField] private ExplorerCustomizationRig _customizationPreviewRig;
        public CampStationAnchorView[] Anchors => _anchors ?? Array.Empty<CampStationAnchorView>();
        public CampUpgradeVisualView[] UpgradeVisuals => _upgradeVisuals ?? Array.Empty<CampUpgradeVisualView>();
        public ExplorerCustomizationRig CustomizationPreviewRig => _customizationPreviewRig;

        public void Render(PlayerProgress progress)
        {
            foreach (CampUpgradeVisualView visual in UpgradeVisuals) visual?.Render(progress);
        }

        public void Preview(CampUpgradeId id)
        {
            UpgradeVisuals.FirstOrDefault(value => value != null && value.UpgradeId.Equals(id))?.PreviewAfter();
        }

        public void ClearPreview(CampUpgradeId id)
        {
            UpgradeVisuals.FirstOrDefault(value => value != null && value.UpgradeId.Equals(id))?.ClearPreview();
        }
    }
}
