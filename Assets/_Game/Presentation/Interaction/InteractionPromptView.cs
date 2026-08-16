using System;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Application.Interaction;
using PequenoExplorador.Application.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Interaction
{
    [DisallowMultipleComponent]
    public sealed class InteractionPromptView : MonoBehaviour
    {
        public const string PlaceholderObjectName = "PH_UI_INTERACTION_PROMPT";

        [SerializeField] private GameObject _panel;
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _statusText;
        [SerializeField] private Button _actionButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Image _icon;

        private InteractionCoordinator _coordinator;
        private ILocalizationService _localization;
        private IAudioService _audio;
        private DiscoveryInteractionAction _discovery;
        private bool _showDevelopmentCount;
        private int _lastDiscoveryCount;
        private InteractionSnapshot _lastSnapshot = InteractionSnapshot.Idle;

        public bool IsVisible => _panel != null && _panel.activeSelf;
        public string NameText => _nameText == null ? string.Empty : _nameText.text;
        public string StatusText => _statusText == null ? string.Empty : _statusText.text;
        public Button ActionButton => _actionButton;
        public Button CancelButton => _cancelButton;

        private void Awake()
        {
            _actionButton?.onClick.AddListener(Activate);
            _cancelButton?.onClick.AddListener(Cancel);
        }

        public void Bind(
            InteractionCoordinator coordinator,
            ILocalizationService localization,
            IAudioService audio,
            DiscoveryInteractionAction discovery = null,
            bool showDevelopmentCount = false)
        {
            Unbind();
            _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _audio = audio ?? throw new ArgumentNullException(nameof(audio));
            _discovery = discovery;
            _showDevelopmentCount = showDevelopmentCount;
            _coordinator.Changed += HandleChanged;
            _localization.LocaleChanged += HandleLocaleChanged;
            if (_discovery != null) _discovery.Completed += HandleDiscoveryCompleted;
            _lastSnapshot = _coordinator.Snapshot;
            Render(playFeedback: false);
        }

        public void Unbind()
        {
            if (_coordinator != null) _coordinator.Changed -= HandleChanged;
            if (_localization != null) _localization.LocaleChanged -= HandleLocaleChanged;
            if (_discovery != null) _discovery.Completed -= HandleDiscoveryCompleted;
            _coordinator = null;
            _localization = null;
            _audio = null;
            _discovery = null;
            _showDevelopmentCount = false;
            _lastDiscoveryCount = 0;
            _lastSnapshot = InteractionSnapshot.Idle;
            if (_panel != null) _panel.SetActive(false);
        }

        private void Activate() => _coordinator?.Activate();
        private void Cancel() => _coordinator?.Cancel();

        private void HandleChanged(InteractionSnapshot snapshot)
        {
            _lastSnapshot = snapshot;
            Render(playFeedback: true);
        }

        private void HandleLocaleChanged(string localeCode) => Render(playFeedback: false);

        private void HandleDiscoveryCompleted(DiscoverResult result)
        {
            _lastDiscoveryCount = result.Count;
            Render(playFeedback: false);
        }

        private void Render(bool playFeedback)
        {
            bool visible = _lastSnapshot?.HasFocus == true;
            if (_panel != null) _panel.SetActive(visible);
            if (!visible || _localization == null) return;

            InteractionDefinition definition = _lastSnapshot.Definition;
            if (_nameText != null) _nameText.text = Resolve(definition.DisplayName);
            if (_statusText != null)
            {
                LocalizedKey status = StatusKey(_lastSnapshot);
                bool isDiscoveryFeedback = status.Equals(LocalizationKeys.DiscoveryNew) ||
                    status.Equals(LocalizationKeys.DiscoveryRepeated);
                _statusText.text = _showDevelopmentCount && _lastDiscoveryCount > 0 && isDiscoveryFeedback
                    ? Resolve(LocalizationKeys.DiscoveryDebugCount, Resolve(status), _lastDiscoveryCount)
                    : Resolve(status);
            }
            SetLabel(_actionButton, definition.Prompt);
            SetLabel(_cancelButton, LocalizationKeys.InteractionCancel);
            if (_actionButton != null)
            {
                _actionButton.gameObject.SetActive(_lastSnapshot.CanActivate);
                _actionButton.interactable = _lastSnapshot.CanActivate;
            }
            if (_cancelButton != null) _cancelButton.gameObject.SetActive(true);
            if (_icon != null) _icon.enabled = true;
            AudioCueId cue = _lastSnapshot.Result.AudioCue;
            if (playFeedback && !string.IsNullOrWhiteSpace(cue.Value)) _audio?.Play(cue);
        }

        private static LocalizedKey StatusKey(InteractionSnapshot snapshot)
        {
            switch (snapshot.State)
            {
                case InteractionOutcome.Approaching: return LocalizationKeys.InteractionApproaching;
                case InteractionOutcome.Ready: return snapshot.Definition.Prompt;
                case InteractionOutcome.Unavailable: return snapshot.Definition.Unavailable;
                case InteractionOutcome.Completed:
                    return string.IsNullOrWhiteSpace(snapshot.Result.Feedback.Entry)
                        ? LocalizationKeys.InteractionCompleted
                        : snapshot.Result.Feedback;
                case InteractionOutcome.CoolingDown: return LocalizationKeys.InteractionWait;
                default: return LocalizationKeys.SafeFallback;
            }
        }

        private string Resolve(LocalizedKey key)
        {
            return Resolve(key, Array.Empty<object>());
        }

        private string Resolve(LocalizedKey key, params object[] arguments)
        {
            try { return _localization.Resolve(key, arguments); }
            catch (InvalidOperationException) { return string.Empty; }
        }

        private void SetLabel(Button button, LocalizedKey key)
        {
            Text label = button == null ? null : button.GetComponentInChildren<Text>(true);
            if (label != null) label.text = Resolve(key);
        }

        private void OnDestroy()
        {
            if (_actionButton != null) _actionButton.onClick.RemoveListener(Activate);
            if (_cancelButton != null) _cancelButton.onClick.RemoveListener(Cancel);
            Unbind();
        }
    }
}
