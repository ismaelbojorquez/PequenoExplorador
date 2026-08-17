using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Customization;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Customization
{
    [DisallowMultipleComponent]
    public sealed class CustomizationView : MonoBehaviour
    {
        public const string PlaceholderObjectName = "PH_UI_CUSTOMIZATION";
        [SerializeField] private GameObject _panel;
        [SerializeField] private Text _title;
        [SerializeField] private Text _balance;
        [SerializeField] private CustomizationSlotButtonView[] _slotButtons = Array.Empty<CustomizationSlotButtonView>();
        [SerializeField] private CustomizationOptionButtonView[] _optionButtons = Array.Empty<CustomizationOptionButtonView>();
        [SerializeField] private Text _selectedName;
        [SerializeField] private Text _selectedState;
        [SerializeField] private Button _unlockButton;
        [SerializeField] private Text _unlockLabel;
        [SerializeField] private Button _equipButton;
        [SerializeField] private Text _equipLabel;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Text _closeLabel;
        [SerializeField] private Text _feedback;
        [SerializeField] private Button _debugUnlockAllButton;

        private ICustomizationCatalog _catalog;
        private IEconomyRepository _repository;
        private UnlockCosmeticUseCase _unlock;
        private EquipCosmeticUseCase _equip;
        private CustomizationSelectionResolver _resolver;
        private ILocalizationService _localization;
        private ISceneFlowService _sceneFlow;
        private Action _debugUnlockAll;
        private CustomizationSlotId _selectedSlot;
        private CosmeticDefinition _selected;
        private ExplorerCustomizationRig _previewRig;

        public bool IsVisible => _panel != null && _panel.activeSelf;
        public CosmeticDefinition Selected => _selected;
        public IReadOnlyList<CustomizationSlotButtonView> SlotButtons => _slotButtons ?? Array.Empty<CustomizationSlotButtonView>();
        public IReadOnlyList<CustomizationOptionButtonView> OptionButtons => _optionButtons ?? Array.Empty<CustomizationOptionButtonView>();
        public string FeedbackText => _feedback == null ? string.Empty : _feedback.text;

        private void Awake()
        {
            _unlockButton?.onClick.AddListener(UnlockSelected);
            _equipButton?.onClick.AddListener(EquipSelected);
            _closeButton?.onClick.AddListener(Close);
            _debugUnlockAllButton?.onClick.AddListener(DebugUnlockAll);
        }

        public void Bind(ICustomizationCatalog catalog, IEconomyRepository repository, UnlockCosmeticUseCase unlock,
            EquipCosmeticUseCase equip, ILocalizationService localization, ISceneFlowService sceneFlow,
            Action debugUnlockAll, bool diagnosticsEnabled)
        {
            Unbind();
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unlock = unlock ?? throw new ArgumentNullException(nameof(unlock));
            _equip = equip ?? throw new ArgumentNullException(nameof(equip));
            _resolver = new CustomizationSelectionResolver(catalog);
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _sceneFlow = sceneFlow ?? throw new ArgumentNullException(nameof(sceneFlow));
            _debugUnlockAll = debugUnlockAll;
            _repository.Changed += HandleProgressChanged;
            _localization.LocaleChanged += HandleLocaleChanged;
            _sceneFlow.Changed += HandleSceneChanged;
            _debugUnlockAllButton?.gameObject.SetActive(diagnosticsEnabled && debugUnlockAll != null);
            _selectedSlot = _catalog.Slots.FirstOrDefault()?.Id ?? default;
            Close();
        }

        public void BindPreviewRig(ExplorerCustomizationRig rig)
        {
            _previewRig?.Unbind();
            _previewRig = rig;
            _previewRig?.Bind(_catalog, _repository);
            if (_selected != null) _previewRig?.Preview(_selected);
        }

        public void UnbindPreviewRig()
        {
            _previewRig?.Unbind();
            _previewRig = null;
        }

        public void Open()
        {
            if (_sceneFlow?.Snapshot.Current != SceneFlowState.Camp || _sceneFlow.Snapshot.IsTransitioning) return;
            _panel?.SetActive(true);
            BindSlots();
            SelectSlot(_selectedSlot);
        }

        public void Close()
        {
            _previewRig?.ClearPreview();
            _panel?.SetActive(false);
        }

        public bool TryHandleBack() { if (!IsVisible) return false; Close(); return true; }
        public void SelectSlotForTests(CustomizationSlotId id) => SelectSlot(id);
        public void SelectCosmeticForTests(CosmeticId id) { if (_catalog.TryGetCosmetic(id, out CosmeticDefinition value)) SelectCosmetic(value); }
        public UnlockCosmeticResult UnlockSelectedForTests() => UnlockCurrent();
        public EquipCosmeticResult EquipSelectedForTests() => EquipCurrent();

        private void SelectSlot(CustomizationSlotId id)
        {
            if (!_catalog.TryGetSlot(id, out _)) return;
            _selectedSlot = id;
            CosmeticDefinition equipped = _resolver.Resolve(_repository.Current).FirstOrDefault(value => value.SlotId == id);
            _selected = equipped ?? _catalog.GetForSlot(id).FirstOrDefault();
            BindOptions(); Render(); _previewRig?.Preview(_selected);
        }

        private void SelectCosmetic(CosmeticDefinition definition)
        { _selected = definition; Render(); _previewRig?.Preview(definition); }

        private void BindSlots()
        {
            for (int index = 0; index < SlotButtons.Count; index++)
            {
                CustomizationSlotButtonView view = SlotButtons[index];
                if (view == null) continue;
                if (index >= _catalog.Slots.Count) { view.gameObject.SetActive(false); continue; }
                view.Bind(_catalog.Slots[index], _localization, SelectSlot);
            }
        }

        private void BindOptions()
        {
            var values = _catalog.GetForSlot(_selectedSlot);
            PlayerProgress progress = _repository.Current;
            for (int index = 0; index < OptionButtons.Count; index++)
            {
                CustomizationOptionButtonView view = OptionButtons[index];
                if (view == null) continue;
                if (index >= values.Count) { view.gameObject.SetActive(false); continue; }
                CosmeticDefinition definition = values[index];
                bool available = _resolver.IsAvailable(definition, progress);
                bool equipped = progress.EquippedCosmetics.Any(value => value.SlotId == definition.SlotId && value.CosmeticId == definition.Id) ||
                    (!progress.EquippedCosmetics.Any(value => value.SlotId == definition.SlotId) &&
                     _catalog.TryGetSlot(definition.SlotId, out CustomizationSlotDefinition slot) && slot.DefaultCosmeticId == definition.Id);
                view.Bind(definition, _localization, available, equipped, SelectCosmetic);
            }
        }

        private void UnlockSelected() => UnlockCurrent();
        private UnlockCosmeticResult UnlockCurrent()
        {
            UnlockCosmeticResult result = _selected == null
                ? new UnlockCosmeticResult(UnlockCosmeticOutcome.MissingDefinition, null, _repository.Current.Wallet)
                : _unlock.Execute(_selected.Id);
            SetFeedback(result.Outcome == UnlockCosmeticOutcome.Unlocked || result.Outcome == UnlockCosmeticOutcome.AlreadyAvailable
                ? LocalizationKeys.CustomizationUnlocked
                : result.Outcome == UnlockCosmeticOutcome.InsufficientStars ? LocalizationKeys.EconomyInsufficient
                : result.Outcome == UnlockCosmeticOutcome.PrerequisiteLocked ? LocalizationKeys.CustomizationProgressLocked
                : LocalizationKeys.CustomizationUnavailable);
            Render(); return result;
        }

        private void EquipSelected() => EquipCurrent();
        private EquipCosmeticResult EquipCurrent()
        {
            EquipCosmeticResult result = _selected == null
                ? new EquipCosmeticResult(EquipCosmeticOutcome.MissingDefinition, null)
                : _equip.Execute(_selected.Id);
            SetFeedback(result.Outcome == EquipCosmeticOutcome.Equipped || result.Outcome == EquipCosmeticOutcome.AlreadyEquipped
                ? LocalizationKeys.CustomizationEquipped
                : result.Outcome == EquipCosmeticOutcome.Incompatible ? LocalizationKeys.CustomizationIncompatible
                : LocalizationKeys.CustomizationUnavailable);
            Render(); return result;
        }

        private void Render()
        {
            if (_repository == null || _localization == null) return;
            PlayerProgress progress = _repository.Current;
            if (_title != null) _title.text = Resolve(LocalizationKeys.CustomizationTitle);
            if (_balance != null) _balance.text = Resolve(LocalizationKeys.CustomizationBalance, progress.Stars);
            if (_closeLabel != null) _closeLabel.text = Resolve(LocalizationKeys.CustomizationClose);
            if (_selected == null) return;
            if (_selectedName != null) _selectedName.text = Resolve(_selected.DisplayName);
            bool available = _resolver.IsAvailable(_selected, progress);
            bool equipped = _resolver.Resolve(progress).Any(value => value.Id == _selected.Id);
            if (_selectedState != null) _selectedState.text = Resolve(equipped ? LocalizationKeys.CustomizationEquipped :
                available ? LocalizationKeys.CustomizationAvailable : _selected.RequiredCampUpgradeId.IsValid ?
                LocalizationKeys.CustomizationProgressLocked : LocalizationKeys.CustomizationCost, _selected.StarCost.Value);
            if (_unlockButton != null) { _unlockButton.gameObject.SetActive(!available); _unlockButton.interactable = !available; }
            if (_unlockLabel != null) _unlockLabel.text = Resolve(_selected.RequiredCampUpgradeId.IsValid ?
                LocalizationKeys.CustomizationUnlockProgress : LocalizationKeys.CustomizationUnlockStars, _selected.StarCost.Value);
            if (_equipButton != null) { _equipButton.gameObject.SetActive(available); _equipButton.interactable = available && !equipped; }
            if (_equipLabel != null) _equipLabel.text = Resolve(LocalizationKeys.CustomizationEquip);
            BindOptions();
        }

        private void SetFeedback(LocalizedKey key) { if (_feedback != null) _feedback.text = Resolve(key); }
        private string Resolve(LocalizedKey key, params object[] arguments) { try { return _localization.Resolve(key, arguments); } catch { return string.Empty; } }
        private void HandleProgressChanged(PlayerProgress _) => Render();
        private void HandleLocaleChanged(string _) { BindSlots(); BindOptions(); Render(); }
        private void HandleSceneChanged(SceneFlowSnapshot snapshot) { if (snapshot == null || snapshot.Current != SceneFlowState.Camp || snapshot.IsTransitioning) Close(); }
        private void DebugUnlockAll() => _debugUnlockAll?.Invoke();

        public void Unbind()
        {
            UnbindPreviewRig();
            if (_repository != null) _repository.Changed -= HandleProgressChanged;
            if (_localization != null) _localization.LocaleChanged -= HandleLocaleChanged;
            if (_sceneFlow != null) _sceneFlow.Changed -= HandleSceneChanged;
            foreach (CustomizationSlotButtonView value in SlotButtons) value?.Unbind();
            foreach (CustomizationOptionButtonView value in OptionButtons) value?.Unbind();
            _catalog = null; _repository = null; _unlock = null; _equip = null; _resolver = null;
            _localization = null; _sceneFlow = null; _debugUnlockAll = null; _selected = null; _selectedSlot = default;
        }

        private void OnDestroy()
        {
            _unlockButton?.onClick.RemoveListener(UnlockSelected); _equipButton?.onClick.RemoveListener(EquipSelected);
            _closeButton?.onClick.RemoveListener(Close); _debugUnlockAllButton?.onClick.RemoveListener(DebugUnlockAll); Unbind();
        }
    }
}
