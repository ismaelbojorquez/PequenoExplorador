using System;
using PequenoExplorador.Application.Album;
using PequenoExplorador.Application.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Album
{
    [DisallowMultipleComponent]
    public sealed class AlbumCategoryCell : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Text _label;
        private AlbumCategoryViewModel _model;
        private ILocalizationService _localization;
        private Action<AlbumCategoryViewModel> _selected;

        private void Awake() => _button?.onClick.AddListener(HandleSelected);

        public void Bind(AlbumCategoryViewModel model, ILocalizationService localization, Action<AlbumCategoryViewModel> selected)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _selected = selected;
            gameObject.SetActive(true);
            RefreshLocale();
        }

        public void SetSelected(bool selected)
        {
            if (_button?.targetGraphic is Image image)
                image.color = selected ? new Color(0.12f, 0.47f, 0.38f, 1f) : new Color(0.16f, 0.26f, 0.29f, 1f);
        }

        public void RefreshLocale()
        {
            if (_model == null || _localization == null || _label == null) return;
            _label.text = _localization.Resolve(
                LocalizationKeys.AlbumCategoryProgress,
                _localization.Resolve(_model.DisplayName),
                _model.Discovered,
                _model.Total);
        }

        public void Clear()
        {
            _model = null;
            _localization = null;
            _selected = null;
            gameObject.SetActive(false);
        }

        private void HandleSelected()
        {
            if (_model != null) _selected?.Invoke(_model);
        }

#if UNITY_EDITOR
        public void ConfigureForEditorAndTests(Button button, Text label)
        {
            _button = button;
            _label = label;
        }
#endif

        private void OnDestroy() => _button?.onClick.RemoveListener(HandleSelected);
    }
}
