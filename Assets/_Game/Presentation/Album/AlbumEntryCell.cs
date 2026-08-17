using System;
using PequenoExplorador.Application.Album;
using PequenoExplorador.Application.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Album
{
    [DisallowMultipleComponent]
    public sealed class AlbumEntryCell : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private Text _name;
        [SerializeField] private Text _state;
        private AlbumEntryViewModel _model;
        private ILocalizationService _localization;
        private Action<AlbumEntryViewModel> _selected;

        public AlbumEntryViewModel Model => _model;
        public Image Image => _image;
        public Button Button => _button;
        public string NameText => _name == null ? string.Empty : _name.text;

        private void Awake() => _button?.onClick.AddListener(HandleSelected);

        public void Bind(
            AlbumEntryViewModel model,
            ILocalizationService localization,
            Action<AlbumEntryViewModel> selected)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _selected = selected;
            gameObject.SetActive(true);
            Render();
        }

        public void ApplyPhoto(Sprite sprite)
        {
            if (_image == null) return;
            _image.sprite = sprite;
            _image.preserveAspect = true;
            _image.color = sprite == null
                ? (_model != null && _model.IsDiscovered ? new Color(0.22f, 0.55f, 0.43f, 1f) : new Color(0.28f, 0.34f, 0.36f, 1f))
                : Color.white;
        }

        public void RefreshLocale() => Render();

        public void Clear()
        {
            _model = null;
            _localization = null;
            _selected = null;
            if (_image != null) _image.sprite = null;
            gameObject.SetActive(false);
        }

        private void Render()
        {
            if (_model == null || _localization == null) return;
            if (_name != null)
                _name.text = _model.IsDiscovered
                    ? Resolve(_model.DisplayName)
                    : Resolve(LocalizationKeys.AlbumLockedName);
            if (_state != null)
                _state.text = _model.IsDiscovered
                    ? Resolve(LocalizationKeys.AlbumDiscovered)
                    : Resolve(LocalizationKeys.AlbumLockedHint);
            if (_button != null) _button.interactable = true;
            ApplyPhoto(null);
        }

        private void HandleSelected()
        {
            if (_model != null) _selected?.Invoke(_model);
        }

        private string Resolve(LocalizedKey key)
        {
            try { return _localization.Resolve(key); }
            catch (InvalidOperationException) { return string.Empty; }
        }

#if UNITY_EDITOR
        public void ConfigureForEditorAndTests(Button button, Image image, Text name, Text state)
        {
            _button = button;
            _image = image;
            _name = name;
            _state = state;
        }
#endif

        private void OnDestroy() => _button?.onClick.RemoveListener(HandleSelected);
    }
}
