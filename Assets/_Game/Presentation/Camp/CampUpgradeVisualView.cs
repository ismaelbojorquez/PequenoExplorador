using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;
using System.Linq;
using UnityEngine;

namespace PequenoExplorador.Presentation.Camp
{
    [DisallowMultipleComponent]
    public sealed class CampUpgradeVisualView : MonoBehaviour
    {
        [SerializeField] private string _upgradeId;
        [SerializeField] private GameObject _beforeVariant;
        [SerializeField] private GameObject _afterVariant;
        private bool _unlocked;
        public string RawUpgradeId => _upgradeId;
        public CampUpgradeId UpgradeId => CampUpgradeId.Parse(_upgradeId);
        public bool IsShowingAfter => _afterVariant != null && _afterVariant.activeSelf;

        public void Render(PlayerProgress progress)
        {
            _unlocked = progress != null && progress.UnlockedCampUpgradeIds.Contains(_upgradeId);
            Apply(_unlocked);
        }

        public void PreviewAfter()
        {
            if (!_unlocked) Apply(true);
        }

        public void ClearPreview() => Apply(_unlocked);

        private void Apply(bool showAfter)
        {
            if (_beforeVariant != null) _beforeVariant.SetActive(!showAfter);
            if (_afterVariant != null) _afterVariant.SetActive(showAfter);
        }
    }
}
