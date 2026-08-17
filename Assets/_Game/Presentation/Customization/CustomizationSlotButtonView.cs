using System;
using PequenoExplorador.Application.Customization;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Customization
{
    [DisallowMultipleComponent]
    public sealed class CustomizationSlotButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Text _label;
        private CustomizationSlotId _id;
        private Action<CustomizationSlotId> _selected;

        public Button Button => _button;
        public Text Label => _label;
        public CustomizationSlotId SlotId => _id;

        public void Bind(
            CustomizationSlotDefinition definition,
            ILocalizationService localization,
            Action<CustomizationSlotId> selected)
        {
            Unbind();
            _id = definition.Id;
            _selected = selected;
            if (_label != null)
            {
                _label.text = localization.Resolve(definition.DisplayName);
            }

            if (_button != null)
            {
                _button.interactable = true;
                _button.onClick.AddListener(Select);
            }

            gameObject.SetActive(true);
        }

        private void Select()
        {
            _selected?.Invoke(_id);
        }

        public void Unbind()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(Select);
            }

            _selected = null;
            _id = default;
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}
