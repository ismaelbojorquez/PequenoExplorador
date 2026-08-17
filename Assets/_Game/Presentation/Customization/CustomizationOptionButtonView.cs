using System;
using PequenoExplorador.Application.Customization;
using PequenoExplorador.Application.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Customization
{
    [DisallowMultipleComponent]
    public sealed class CustomizationOptionButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Text _label;
        [SerializeField] private Image _swatch;
        [SerializeField] private GameObject _lockedBadge;
        [SerializeField] private GameObject _equippedBadge;
        private CosmeticDefinition _definition;
        private Action<CosmeticDefinition> _selected;

        public Button Button => _button;
        public Text Label => _label;
        public CosmeticDefinition Definition => _definition;

        public void Bind(
            CosmeticDefinition definition,
            ILocalizationService localization,
            bool available,
            bool equipped,
            Action<CosmeticDefinition> selected)
        {
            Unbind();
            _definition = definition;
            _selected = selected;
            if (_label != null)
            {
                _label.text = localization.Resolve(definition.DisplayName);
            }

            if (_swatch != null)
            {
                _swatch.color = new Color32(
                    definition.Color.Red,
                    definition.Color.Green,
                    definition.Color.Blue,
                    definition.Color.Alpha);
            }

            _lockedBadge?.SetActive(!available);
            _equippedBadge?.SetActive(equipped);
            if (_button != null)
            {
                _button.interactable = true;
                _button.onClick.AddListener(Select);
            }

            gameObject.SetActive(true);
        }

        private void Select()
        {
            _selected?.Invoke(_definition);
        }

        public void Unbind()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(Select);
            }

            _selected = null;
            _definition = null;
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}
