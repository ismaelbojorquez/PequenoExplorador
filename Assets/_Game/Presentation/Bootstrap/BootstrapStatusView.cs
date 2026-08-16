using System;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Save;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Bootstrap
{
    [DisallowMultipleComponent]
    public sealed class BootstrapStatusView : MonoBehaviour
    {
        [SerializeField] private Text _statusText;
        [SerializeField] private Text _productNameText;
        [SerializeField] private Text _appVersionText;
        [SerializeField] private Text _diagnosticNoticeText;
        [SerializeField] private GameObject[] _developmentOnlyObjects = Array.Empty<GameObject>();

        private ILocalizationService _localization;
        private IAppConfig _config;
        private LocalizedKey _currentStatusKey;

        public string CurrentStatus => _statusText == null ? string.Empty : _statusText.text;

        public void BindLocalization(ILocalizationService localization)
        {
            if (_localization != null)
            {
                _localization.LocaleChanged -= OnLocaleChanged;
            }

            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _localization.LocaleChanged += OnLocaleChanged;
        }

        public void ConfigureProduct(IAppConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            if (_productNameText == null || _appVersionText == null || _diagnosticNoticeText == null)
            {
                throw new InvalidOperationException(
                    "Bootstrap localized Text references must be wired explicitly.");
            }

            RefreshLocalizedText();
        }

        public void SetDevelopmentDiagnosticsVisible(bool visible)
        {
            foreach (GameObject target in _developmentOnlyObjects)
            {
                if (target != null)
                {
                    target.SetActive(visible);
                }
            }
        }

        public void ShowInitializing()
        {
            SetStatus(LocalizationKeys.StatusInitializing);
        }

        public void ShowReady(SaveUserNotice saveNotice = SaveUserNotice.None)
        {
            switch (saveNotice)
            {
                case SaveUserNotice.ProgressRecovered:
                    SetStatus(LocalizationKeys.StatusRecovered);
                    break;
                case SaveUserNotice.NewerSaveVersionDetected:
                    SetStatus(LocalizationKeys.StatusNewerProtected);
                    break;
                default:
                    SetStatus(LocalizationKeys.StatusReady);
                    break;
            }
        }

        public void ShowRecoverableFailure()
        {
            SetStatus(LocalizationKeys.StatusFailure);
        }

        public void ShowShutdown()
        {
            SetStatus(LocalizationKeys.StatusStopped);
        }

        private void SetStatus(LocalizedKey key)
        {
            _currentStatusKey = key;
            if (_statusText == null)
            {
                Debug.LogError("PE_BOOTSTRAP_VIEW_MISSING statusText");
                return;
            }

            _statusText.text = TryResolve(key);
        }

        private void OnLocaleChanged(string localeCode)
        {
            RefreshLocalizedText();
        }

        private void RefreshLocalizedText()
        {
            if (_localization == null)
            {
                return;
            }

            if (_config != null && _productNameText != null && _appVersionText != null && _diagnosticNoticeText != null)
            {
                _productNameText.text = TryResolve(LocalizationKeys.ProductName);
                _appVersionText.text = TryResolve(LocalizationKeys.Version, _config.AppVersion);
                _diagnosticNoticeText.text = TryResolve(LocalizationKeys.DiagnosticNotice);
            }

            if (_statusText != null && !string.IsNullOrEmpty(_currentStatusKey.Entry))
            {
                _statusText.text = TryResolve(_currentStatusKey);
            }
        }

        private string TryResolve(LocalizedKey key, params object[] arguments)
        {
            try
            {
                return _localization?.Resolve(key, arguments) ?? string.Empty;
            }
            catch (InvalidOperationException)
            {
                return string.Empty;
            }
        }

        private void OnDestroy()
        {
            if (_localization != null)
            {
                _localization.LocaleChanged -= OnLocaleChanged;
                _localization = null;
            }
        }
    }
}
