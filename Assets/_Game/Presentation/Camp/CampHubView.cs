using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Camp;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Camp
{
    [DisallowMultipleComponent]
    public sealed class CampHubView : MonoBehaviour
    {
        public const string PlaceholderObjectName = "PH_UI_CAMP_HUB";
        [SerializeField] private GameObject _panel;
        [SerializeField] private Text _title;
        [SerializeField] private CampStationButtonView[] _stationButtons = Array.Empty<CampStationButtonView>();
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Text _upgradeButtonLabel;
        [SerializeField] private GameObject _previewPanel;
        [SerializeField] private Text _previewTitle;
        [SerializeField] private Text _previewDescription;
        [SerializeField] private Text _previewCost;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Text _confirmLabel;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Text _cancelLabel;
        [SerializeField] private Text _feedback;

        private ICampCatalog _catalog;
        private IEconomyRepository _repository;
        private PurchaseCampUpgradeUseCase _purchase;
        private ILocalizationService _localization;
        private ISceneFlowService _sceneFlow;
        private IReadOnlyDictionary<CampStationActionId, Action> _actions;
        private CampUpgradeDefinition _selectedUpgrade;
        private CampSceneRoot _sceneRoot;

        public bool IsVisible => _panel != null && _panel.activeSelf;
        public bool IsPreviewVisible => _previewPanel != null && _previewPanel.activeSelf;
        public string FeedbackText => _feedback == null ? string.Empty : _feedback.text;
        public IReadOnlyList<CampStationButtonView> StationButtons => _stationButtons ?? Array.Empty<CampStationButtonView>();
        public bool CurrentUpgradeUnlocked => _selectedUpgrade != null && _repository != null &&
            _repository.Current.UnlockedCampUpgradeIds.Contains(_selectedUpgrade.Id.Value, StringComparer.Ordinal);

        private void Awake()
        {
            _upgradeButton?.onClick.AddListener(OpenUpgradePreview);
            _confirmButton?.onClick.AddListener(ConfirmUpgrade);
            _cancelButton?.onClick.AddListener(ClosePreview);
        }

        public void Bind(ICampCatalog catalog, IEconomyRepository repository, PurchaseCampUpgradeUseCase purchase,
            ILocalizationService localization, ISceneFlowService sceneFlow,
            IReadOnlyDictionary<CampStationActionId, Action> actions)
        {
            Unbind();
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _purchase = purchase ?? throw new ArgumentNullException(nameof(purchase));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _sceneFlow = sceneFlow ?? throw new ArgumentNullException(nameof(sceneFlow));
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _selectedUpgrade = _catalog.Upgrades.FirstOrDefault();
            _repository.Changed += HandleProgressChanged;
            _localization.LocaleChanged += HandleLocaleChanged;
            _sceneFlow.Changed += HandleSceneChanged;
            BindStations();
            Render(_repository.Current);
            HandleSceneChanged(_sceneFlow.Snapshot);
        }

        public void BindScene(CampSceneRoot sceneRoot)
        {
            _sceneRoot = sceneRoot;
            if (_sceneRoot != null) _sceneRoot.Render(_repository?.Current);
        }

        public void UnbindScene()
        {
            if (_selectedUpgrade != null && _sceneRoot != null) _sceneRoot.ClearPreview(_selectedUpgrade.Id);
            _sceneRoot = null;
        }

        public bool TryHandleBack()
        {
            if (!IsPreviewVisible) return false;
            ClosePreview();
            return true;
        }

        public void OpenUpgradePreview()
        {
            if (_selectedUpgrade == null || _repository == null || CurrentUpgradeUnlocked) return;
            _previewPanel?.SetActive(true);
            Render(_repository.Current);
            if (_sceneRoot != null) _sceneRoot.Preview(_selectedUpgrade.Id);
        }

        public PurchaseCampUpgradeResult ConfirmUpgradeForTests() => PurchaseSelected();
        private void ConfirmUpgrade() => PurchaseSelected();

        private PurchaseCampUpgradeResult PurchaseSelected()
        {
            if (_selectedUpgrade == null || _purchase == null)
                return new PurchaseCampUpgradeResult(PurchaseCampUpgradeOutcome.MissingDefinition, null,
                    _repository == null ? default : _repository.Current.Wallet);
            PurchaseCampUpgradeResult result = _purchase.Execute(_selectedUpgrade.Id);
            if (_feedback != null)
            {
                LocalizedKey key = result.Outcome == PurchaseCampUpgradeOutcome.Purchased ||
                                   result.Outcome == PurchaseCampUpgradeOutcome.AlreadyUnlocked
                    ? LocalizationKeys.CampUpgradePurchased
                    : result.Outcome == PurchaseCampUpgradeOutcome.InsufficientStars
                        ? LocalizationKeys.EconomyInsufficient
                        : LocalizationKeys.CampUpgradeUnavailable;
                _feedback.text = Resolve(key);
            }
            if (result.Outcome == PurchaseCampUpgradeOutcome.Purchased ||
                result.Outcome == PurchaseCampUpgradeOutcome.AlreadyUnlocked)
                ClosePreview();
            Render(_repository.Current);
            return result;
        }

        private void ClosePreview()
        {
            if (_selectedUpgrade != null && _sceneRoot != null) _sceneRoot.ClearPreview(_selectedUpgrade.Id);
            _previewPanel?.SetActive(false);
        }

        private void BindStations()
        {
            for (int index = 0; index < _stationButtons.Length; index++)
            {
                CampStationButtonView view = _stationButtons[index];
                if (view == null) continue;
                if (index >= _catalog.Stations.Count) { view.gameObject.SetActive(false); continue; }
                CampStationDefinition station = _catalog.Stations[index];
                _actions.TryGetValue(station.ActionId, out Action action);
                view.Bind(station, _localization, action);
            }
        }

        private void HandleProgressChanged(PlayerProgress progress)
        {
            if (_sceneRoot != null && !IsPreviewVisible) _sceneRoot.Render(progress);
            Render(progress);
        }
        private void HandleLocaleChanged(string _) { BindStations(); Render(_repository?.Current); }
        private void HandleSceneChanged(SceneFlowSnapshot snapshot)
        {
            bool visible = snapshot != null && !snapshot.IsTransitioning && snapshot.Current == SceneFlowState.Camp;
            _panel?.SetActive(visible);
            if (!visible) ClosePreview();
        }

        private void Render(PlayerProgress progress)
        {
            if (progress == null || _localization == null) return;
            if (_title != null) _title.text = Resolve(LocalizationKeys.CampHubTitle);
            if (_selectedUpgrade == null) return;
            bool unlocked = progress.UnlockedCampUpgradeIds.Contains(_selectedUpgrade.Id.Value, StringComparer.Ordinal);
            if (_upgradeButton != null) _upgradeButton.interactable = !unlocked;
            if (_upgradeButtonLabel != null)
                _upgradeButtonLabel.text = Resolve(unlocked ? LocalizationKeys.CampUpgradeCompleted : _selectedUpgrade.DisplayName);
            if (_previewTitle != null) _previewTitle.text = Resolve(_selectedUpgrade.DisplayName);
            if (_previewDescription != null) _previewDescription.text = Resolve(_selectedUpgrade.PreviewCopy);
            if (_previewCost != null) _previewCost.text = Resolve(LocalizationKeys.CampUpgradeCost, _selectedUpgrade.StarCost.Value);
            if (_confirmLabel != null) _confirmLabel.text = Resolve(LocalizationKeys.CampUpgradeConfirm);
            if (_cancelLabel != null) _cancelLabel.text = Resolve(LocalizationKeys.CampUpgradeCancel);
            if (_confirmButton != null) _confirmButton.interactable = !unlocked;
            if (_sceneRoot != null) _sceneRoot.Render(progress);
        }

        private string Resolve(LocalizedKey key, params object[] arguments)
        { try { return _localization.Resolve(key, arguments); } catch { return string.Empty; } }

        public void Unbind()
        {
            UnbindScene();
            if (_repository != null) _repository.Changed -= HandleProgressChanged;
            if (_localization != null) _localization.LocaleChanged -= HandleLocaleChanged;
            if (_sceneFlow != null) _sceneFlow.Changed -= HandleSceneChanged;
            foreach (CampStationButtonView view in _stationButtons ?? Array.Empty<CampStationButtonView>()) view?.Unbind();
            _catalog = null; _repository = null; _purchase = null; _localization = null; _sceneFlow = null; _actions = null; _selectedUpgrade = null;
        }

        private void OnDestroy()
        {
            if (_upgradeButton != null) _upgradeButton.onClick.RemoveListener(OpenUpgradePreview);
            if (_confirmButton != null) _confirmButton.onClick.RemoveListener(ConfirmUpgrade);
            if (_cancelButton != null) _cancelButton.onClick.RemoveListener(ClosePreview);
            Unbind();
        }
    }
}
