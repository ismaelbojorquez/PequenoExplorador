using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.SceneFlow;
using PequenoExplorador.Application.Worlds;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.SceneFlow
{
    [DisallowMultipleComponent]
    public sealed class SceneTransitionView : MonoBehaviour
    {
        [SerializeField] private GameObject _transitionPanel;
        [SerializeField] private Text _statusText;
        [SerializeField] private Text _currentLocationText;
        [SerializeField] private Slider _progress;
        [SerializeField] private Button _enterJungleButton;
        [SerializeField] private Button _returnCampButton;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _simulateFailureButton;
        [SerializeField] private Button _localeSpanishButton;
        [SerializeField] private Button _localeEnglishButton;
        [SerializeField] private Button _localePseudoButton;
        [SerializeField] private GameObject _developmentControls;

        private ISceneFlowService _service;
        private ILocalizationService _localization;
        private IWorldCatalog _worlds;
        private WorldCatalogEntry _selectedWorld;
        private WorldLoadResult _lastWorldResult;
        private CancellationTokenSource _localeCancellation;

        public event Action<WorldManifest> WorldRequested;
        public event Action ReturnCampRequested;
        public event Action RetryRequested;
        public event Action SimulateFailureRequested;

        public string StatusText => _statusText == null ? string.Empty : _statusText.text;
        public Button EnterExpeditionButton => _enterJungleButton;
        public Button ReturnCampButton => _returnCampButton;

        private void Awake()
        {
            _enterJungleButton?.onClick.AddListener(() =>
            {
                if (_selectedWorld != null) WorldRequested?.Invoke(_selectedWorld.Manifest);
            });
            _returnCampButton?.onClick.AddListener(() => ReturnCampRequested?.Invoke());
            _retryButton?.onClick.AddListener(() => RetryRequested?.Invoke());
            _simulateFailureButton?.onClick.AddListener(() => SimulateFailureRequested?.Invoke());
            _localeSpanishButton?.onClick.AddListener(() => ChangeLocale(LocaleCode.Spanish, persist: true));
            _localeEnglishButton?.onClick.AddListener(() => ChangeLocale(LocaleCode.English, persist: true));
            _localePseudoButton?.onClick.AddListener(() => ChangeLocale(LocaleCode.Pseudo, persist: false));
        }

        public void Bind(
            ISceneFlowService service,
            IWorldCatalog worlds,
            ILocalizationService localization,
            bool developmentControlsEnabled)
        {
            Unbind();
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _worlds = worlds ?? throw new ArgumentNullException(nameof(worlds));
            _selectedWorld = _worlds.Worlds.FirstOrDefault();
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _localeCancellation = new CancellationTokenSource();
            _service.Changed += Render;
            _localization.LocaleChanged += OnLocaleChanged;
            if (_developmentControls != null)
            {
                _developmentControls.SetActive(developmentControlsEnabled);
            }

            Render(_service.Snapshot);
        }

        public void Unbind()
        {
            if (_service != null)
            {
                _service.Changed -= Render;
                _service = null;
            }

            if (_localization != null)
            {
                _localization.LocaleChanged -= OnLocaleChanged;
                _localization = null;
            }

            _localeCancellation?.Cancel();
            _localeCancellation?.Dispose();
            _localeCancellation = null;
            _worlds = null;
            _selectedWorld = null;
            _lastWorldResult = null;
        }

        public void ShowWorldResult(WorldLoadResult result)
        {
            _lastWorldResult = result;
            if (_service != null) Render(_service.Snapshot);
        }

        private void Render(SceneFlowSnapshot snapshot)
        {
            bool showTransition = snapshot.IsTransitioning || snapshot.HasRecoverableError;
            _transitionPanel?.SetActive(showTransition);
            if (_currentLocationText != null)
            {
                _currentLocationText.text = FriendlyName(snapshot.Current);
            }
            if (_progress != null)
            {
                _progress.value = snapshot.Progress;
                _progress.gameObject.SetActive(snapshot.IsTransitioning);
            }

            if (_statusText != null)
            {
                if (_lastWorldResult?.Outcome == WorldLoadOutcome.Unavailable)
                    _statusText.text = TryResolve(LocalizationKeys.WorldUnavailable);
                else if (_lastWorldResult?.Outcome == WorldLoadOutcome.Missing)
                    _statusText.text = TryResolve(LocalizationKeys.WorldMissing);
                else
                    _statusText.text = snapshot.HasRecoverableError
                        ? TryResolve(LocalizationKeys.TransitionError)
                        : snapshot.IsTransitioning
                            ? TryResolve(LocalizationKeys.TransitionPreparing, FriendlyName(snapshot.Target))
                            : FriendlyName(snapshot.Current);
            }

            if (_selectedWorld != null) SetButtonLabel(_enterJungleButton, _selectedWorld.Manifest.DisplayName);
            SetButtonLabel(_returnCampButton, LocalizationKeys.ActionReturnCamp);
            SetButtonLabel(_retryButton, LocalizationKeys.ActionRetry);
            SetButtonLabel(_simulateFailureButton, LocalizationKeys.ActionSimulateFailure);
            SetButtonLabel(_localeSpanishButton, LocalizationKeys.LocaleSpanish);
            SetButtonLabel(_localeEnglishButton, LocalizationKeys.LocaleEnglish);
            SetButtonLabel(_localePseudoButton, LocalizationKeys.LocalePseudo);

            bool interactive = !snapshot.IsTransitioning && !snapshot.HasRecoverableError;
            SetButton(_enterJungleButton, interactive && snapshot.Current == SceneFlowState.Camp && _selectedWorld != null);
            SetButton(_returnCampButton, interactive && snapshot.Current == SceneFlowState.Expedition);
            SetButton(_retryButton, snapshot.HasRecoverableError);
            if (_simulateFailureButton != null)
            {
                _simulateFailureButton.interactable = !snapshot.IsTransitioning;
            }
        }

        private string FriendlyName(SceneFlowState state)
        {
            switch (state)
            {
                case SceneFlowState.Camp:
                    return TryResolve(LocalizationKeys.WorldCamp);
                case SceneFlowState.Expedition:
                    WorldCatalogEntry active = _worlds?.Worlds.FirstOrDefault(entry =>
                        entry.Manifest.Scene == _service.Snapshot.CurrentContent);
                    return active == null ? TryResolve(LocalizationKeys.SafeFallback) : TryResolve(active.Manifest.DisplayName);
                default:
                    return TryResolve(LocalizationKeys.WorldBoot);
            }
        }

        private async void ChangeLocale(string localeCode, bool persist)
        {
            if (_localization == null || _localeCancellation == null)
            {
                return;
            }

            try
            {
                await _localization.SetLocaleAsync(localeCode, persist, _localeCancellation.Token);
            }
            catch (OperationCanceledException)
            {
                // View lifecycle owns cancellation.
            }
            catch (Exception)
            {
                if (_service != null)
                {
                    Render(_service.Snapshot);
                }
            }
        }

        private void OnLocaleChanged(string localeCode)
        {
            if (_service != null)
            {
                Render(_service.Snapshot);
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

        private void SetButtonLabel(Button button, LocalizedKey key)
        {
            Text label = button == null ? null : button.GetComponentInChildren<Text>(true);
            if (label != null)
            {
                label.text = TryResolve(key);
            }
        }

        private static void SetButton(Button button, bool visible)
        {
            if (button != null)
            {
                button.gameObject.SetActive(visible);
                button.interactable = visible;
            }
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}
