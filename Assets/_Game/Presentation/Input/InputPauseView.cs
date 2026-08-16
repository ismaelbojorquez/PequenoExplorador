using System;
using PequenoExplorador.Application.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Input
{
    [DisallowMultipleComponent]
    public sealed class InputPauseView : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Text _title;
        [SerializeField] private Button _resumeButton;
        private ILocalizationService _localization;

        public event Action ResumeRequested;
        public bool IsVisible => _panel != null && _panel.activeSelf;

        private void Awake() => _resumeButton?.onClick.AddListener(RequestResume);

        public void Bind(ILocalizationService localization)
        {
            Unbind();
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _localization.LocaleChanged += OnLocaleChanged;
            RefreshCopy();
            Show(false);
        }

        public void Show(bool visible)
        {
            if (visible) RefreshCopy();
            _panel?.SetActive(visible);
        }

        public void Unbind()
        {
            if (_localization != null) _localization.LocaleChanged -= OnLocaleChanged;
            _localization = null;
        }

        private void RequestResume() => ResumeRequested?.Invoke();
        private void OnLocaleChanged(string localeCode) => RefreshCopy();

        private void RefreshCopy()
        {
            if (_localization == null) return;
            string title = TryResolve(LocalizationKeys.PauseTitle);
            string resume = TryResolve(LocalizationKeys.ActionResume);
            if (_title != null) _title.text = title;
            Text label = _resumeButton == null ? null : _resumeButton.GetComponentInChildren<Text>(true);
            if (label != null) label.text = resume;
        }

        private string TryResolve(LocalizedKey key)
        {
            try { return _localization.Resolve(key); }
            catch (InvalidOperationException) { return string.Empty; }
        }

        private void OnDestroy()
        {
            _resumeButton?.onClick.RemoveListener(RequestResume);
            Unbind();
        }
    }
}
