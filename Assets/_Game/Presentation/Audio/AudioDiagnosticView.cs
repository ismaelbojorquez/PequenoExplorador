using System;
using System.Threading;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using UnityEngine;
using UnityEngine.UI;
using AudioSettingsModel = PequenoExplorador.Application.Audio.AudioSettings;

namespace PequenoExplorador.Presentation.Audio
{
    [DisallowMultipleComponent]
    public sealed class AudioDiagnosticView : MonoBehaviour
    {
        [SerializeField] private GameObject _developmentPanel;
        [SerializeField] private Text _subtitleText;
        [SerializeField] private Button _playInstructionButton;
        [SerializeField] private Button _replayButton;
        [SerializeField] private Button _feedbackButton;
        [SerializeField] private Toggle _subtitlesToggle;
        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _ambienceSlider;
        [SerializeField] private Slider _effectsSlider;
        [SerializeField] private Slider _voiceSlider;

        private IAudioService _audio;
        private ILocalizationService _localization;
        private CancellationTokenSource _lifetime;
        private SubtitleModel _subtitle;
        private bool _binding;

        public string CurrentSubtitle => _subtitleText == null ? string.Empty : _subtitleText.text;
        private bool _developmentAllowed;

        public void Bind(IAudioService audio, ILocalizationService localization, bool developmentVisible)
        {
            Unbind();
            _audio = audio ?? throw new ArgumentNullException(nameof(audio));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _lifetime = new CancellationTokenSource();
            _binding = true;
            _developmentAllowed = developmentVisible;
            _developmentPanel?.SetActive(false);
            if (developmentVisible)
            {
                SetDevelopmentLabel(_playInstructionButton, "INSTRUCCIÓN");
                SetDevelopmentLabel(_replayButton, "REPETIR");
                SetDevelopmentLabel(_feedbackButton, "FEEDBACK");
                Text toggleLabel = _subtitlesToggle == null ? null : _subtitlesToggle.GetComponentInChildren<Text>(true);
                if (toggleLabel != null) toggleLabel.text = "SUBTÍTULOS";
            }
            _playInstructionButton?.onClick.AddListener(PlayInstruction);
            _replayButton?.onClick.AddListener(Replay);
            _feedbackButton?.onClick.AddListener(PlayFeedback);
            _subtitlesToggle?.onValueChanged.AddListener(OnSubtitlesChanged);
            AddSliderListeners();
            ApplySettingsToControls(_audio.Settings);
            _audio.SubtitleChanged += OnSubtitleChanged;
            _localization.LocaleChanged += OnLocaleChanged;
            _binding = false;
            RenderSubtitle();
        }

        public void SetDevelopmentVisible(bool visible) => _developmentPanel?.SetActive(_developmentAllowed && visible);

        public void Unbind()
        {
            if (_audio != null)
            {
                _audio.SubtitleChanged -= OnSubtitleChanged;
            }
            if (_localization != null)
            {
                _localization.LocaleChanged -= OnLocaleChanged;
            }
            _playInstructionButton?.onClick.RemoveListener(PlayInstruction);
            _replayButton?.onClick.RemoveListener(Replay);
            _feedbackButton?.onClick.RemoveListener(PlayFeedback);
            _subtitlesToggle?.onValueChanged.RemoveListener(OnSubtitlesChanged);
            RemoveSliderListeners();
            _lifetime?.Cancel();
            _lifetime?.Dispose();
            _lifetime = null;
            _audio = null;
            _localization = null;
            _developmentAllowed = false;
            _subtitle = SubtitleModel.Hidden;
            if (_subtitleText != null) _subtitleText.text = string.Empty;
        }

        private void PlayInstruction() => _audio?.Play(AudioCueIds.ExploreInstruction);
        private void Replay() => _audio?.ReplayLastInstruction();
        private void PlayFeedback() => _audio?.Play(AudioCueIds.ConfirmFeedback);

        private void OnSubtitleChanged(SubtitleModel subtitle)
        {
            _subtitle = subtitle;
            RenderSubtitle();
        }

        private void OnLocaleChanged(string localeCode) => RenderSubtitle();
        private void OnSubtitlesChanged(bool value) => PersistControls();
        private void OnSliderChanged(float value) => PersistControls();

        private async void PersistControls()
        {
            if (_binding || _audio == null || _lifetime == null)
            {
                return;
            }

            try
            {
                await _audio.UpdateSettingsAsync(new AudioSettingsModel(
                    Value(_masterSlider),
                    Value(_musicSlider),
                    Value(_ambienceSlider),
                    Value(_effectsSlider),
                    Value(_voiceSlider),
                    _subtitlesToggle == null || _subtitlesToggle.isOn), _lifetime.Token);
            }
            catch (OperationCanceledException) { }
            catch (Exception) { ApplySettingsToControls(_audio.Settings); }
        }

        private void RenderSubtitle()
        {
            if (_subtitleText == null)
            {
                return;
            }

            if (!_subtitle.Visible || _localization == null)
            {
                _subtitleText.text = string.Empty;
                _subtitleText.gameObject.SetActive(false);
                return;
            }

            _subtitleText.text = _localization.Resolve(_subtitle.TextKey);
            _subtitleText.gameObject.SetActive(true);
        }

        private void ApplySettingsToControls(AudioSettingsModel settings)
        {
            _binding = true;
            Set(_masterSlider, settings.Master);
            Set(_musicSlider, settings.Music);
            Set(_ambienceSlider, settings.Ambience);
            Set(_effectsSlider, settings.Effects);
            Set(_voiceSlider, settings.Voice);
            if (_subtitlesToggle != null) _subtitlesToggle.isOn = settings.SubtitlesEnabled;
            _binding = false;
        }

        private void AddSliderListeners()
        {
            foreach (Slider slider in Sliders()) slider?.onValueChanged.AddListener(OnSliderChanged);
        }

        private void RemoveSliderListeners()
        {
            foreach (Slider slider in Sliders()) slider?.onValueChanged.RemoveListener(OnSliderChanged);
        }

        private Slider[] Sliders() => new[] { _masterSlider, _musicSlider, _ambienceSlider, _effectsSlider, _voiceSlider };
        private static float Value(Slider slider) => slider == null ? 1f : slider.value;
        private static void Set(Slider slider, float value) { if (slider != null) slider.SetValueWithoutNotify(value); }

        private static void SetDevelopmentLabel(Button button, string value)
        {
            Text label = button == null ? null : button.GetComponentInChildren<Text>(true);
            if (label != null) label.text = value;
        }

        private void OnDestroy() => Unbind();
    }
}
