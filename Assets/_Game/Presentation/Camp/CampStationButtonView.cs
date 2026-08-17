using System;
using PequenoExplorador.Application.Camp;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Camp
{
    [DisallowMultipleComponent]
    public sealed class CampStationButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Text _title;
        [SerializeField] private Text _description;
        [SerializeField] private GameObject _parentBadge;
        private Action _action;
        public CampStationId StationId { get; private set; }
        public bool IsInteractable => _button != null && _button.interactable;

        public void Bind(CampStationDefinition definition, ILocalizationService localization, Action action)
        {
            Unbind();
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (localization == null) throw new ArgumentNullException(nameof(localization));
            StationId = definition.Id;
            _action = action;
            if (_title != null) _title.text = Resolve(localization, definition.DisplayName);
            if (_description != null) _description.text = Resolve(localization, definition.Description);
            if (_parentBadge != null) _parentBadge.SetActive(definition.IsParentRestricted);
            if (_button != null)
            {
                _button.interactable = definition.IsAvailable && action != null;
                _button.onClick.AddListener(InvokeAction);
            }
            gameObject.SetActive(true);
        }

        public void Unbind()
        {
            if (_button != null) _button.onClick.RemoveListener(InvokeAction);
            _action = null;
            StationId = default;
        }

        private void InvokeAction() => _action?.Invoke();
        private static string Resolve(ILocalizationService localization, LocalizedKey key)
        { try { return localization.Resolve(key); } catch { return string.Empty; } }
        private void OnDestroy() => Unbind();
    }
}
