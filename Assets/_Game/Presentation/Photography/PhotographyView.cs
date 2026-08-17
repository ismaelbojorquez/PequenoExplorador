using System;
using System.Collections;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Photography;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Photography
{
    [DisallowMultipleComponent]
    public sealed class PhotographyView : MonoBehaviour
    {
        public const string PlaceholderObjectName = "PH_UI_PHOTOGRAPHY";
        [SerializeField] private GameObject _panel;
        [SerializeField] private Image _reticle;
        [SerializeField] private Text _guidance;
        [SerializeField] private Button _shutter;
        [SerializeField] private Button _exit;
        [SerializeField] private GameObject _card;
        [SerializeField] private Text _cardText;
        [SerializeField] private Button _learn;
        [SerializeField] private Image _flash;
        private ILocalizationService _localization;
        private Coroutine _flashRoutine;
        private bool _reduceMotion;
        private bool _captureBusy;
        private PhotoEvaluation _lastEvaluation;
        private bool _hasEvaluation;
        private PhotoCaptureResult _lastCapture;
        private bool _hasCapture;
        public event Action ShutterRequested;
        public event Action ExitRequested;
        public event Action LearnRequested;
        public bool IsVisible => _panel != null && _panel.activeSelf;
        public string GuidanceText => _guidance == null ? string.Empty : _guidance.text;
        public bool CardVisible => _card != null && _card.activeSelf;
        public Button ShutterButton => _shutter;
        public Button ExitButton => _exit;
        public Button LearnButton => _learn;

        private void Awake() { _shutter?.onClick.AddListener(HandleShutter); _exit?.onClick.AddListener(HandleExit); _learn?.onClick.AddListener(HandleLearn); }
        public void Bind(ILocalizationService localization, bool reduceMotion)
        {
            Unbind();
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _localization.LocaleChanged += HandleLocaleChanged;
            _reduceMotion = reduceMotion;
            Hide();
        }
        public bool FlashVisible => _flash != null && _flash.gameObject.activeSelf;
        public void SetCaptureBusy(bool busy) { _captureBusy = busy; if (_shutter != null) _shutter.interactable = !busy; }
        public void SetReduceMotion(bool enabled) => _reduceMotion = enabled;
        public void Show(PhotoEvaluation evaluation) { if (_panel != null) _panel.SetActive(true); if (_card != null) _card.SetActive(false); _hasCapture = false; _hasEvaluation = false; UpdateEvaluation(evaluation); }
        public void UpdateEvaluation(PhotoEvaluation evaluation)
        {
            bool changed = !_hasEvaluation || _lastEvaluation.Guidance != evaluation.Guidance ||
                           _lastEvaluation.IsReady != evaluation.IsReady;
            _lastEvaluation = evaluation;
            _hasEvaluation = true;
            if (changed) RenderEvaluation(evaluation);
        }
        public void ShowCapture(PhotoCaptureResult result)
        {
            _lastCapture = result;
            _hasCapture = true;
            if (_card != null) _card.SetActive(true);
            if (_cardText != null) _cardText.text = Resolve(CaptureKey(result.Outcome));
            if (_learn != null) _learn.gameObject.SetActive(result.ProgressCaptured);
            if (!_reduceMotion && _flash != null)
            {
                if (_flashRoutine != null) StopCoroutine(_flashRoutine);
                _flashRoutine = StartCoroutine(FlashBriefly());
            }
        }
        public void Hide()
        {
            if (_flashRoutine != null) { StopCoroutine(_flashRoutine); _flashRoutine = null; }
            if (_panel != null) _panel.SetActive(false);
            if (_card != null) _card.SetActive(false);
            if (_learn != null) _learn.gameObject.SetActive(false);
            if (_flash != null) _flash.gameObject.SetActive(false);
        }
        public void Unbind()
        {
            if (_localization != null) _localization.LocaleChanged -= HandleLocaleChanged;
            _localization = null; Hide();
        }
        private void RenderEvaluation(PhotoEvaluation evaluation)
        {
            if (_localization == null) return;
            if (_guidance != null) _guidance.text = Resolve(GuidanceKey(evaluation.Guidance));
            if (_reticle != null) _reticle.color = evaluation.IsReady ? new Color(0.25f, 0.92f, 0.55f, 0.92f) : new Color(1f, 0.78f, 0.22f, 0.92f);
            if (_shutter != null) _shutter.interactable = !_captureBusy;
            SetButtonLabel(_shutter, LocalizationKeys.PhotographyCapture);
            SetButtonLabel(_exit, LocalizationKeys.PhotographyExit);
            SetButtonLabel(_learn, LocalizationKeys.LearningActivityContinue);
        }
        private IEnumerator FlashBriefly()
        {
            _flash.gameObject.SetActive(true); _flash.color = new Color(1f, 1f, 1f, 0.18f);
            yield return null;
            _flash.gameObject.SetActive(false); _flashRoutine = null;
        }
        private void HandleLocaleChanged(string locale)
        {
            if (!IsVisible) return;
            RenderEvaluation(_lastEvaluation);
            if (_hasCapture && _cardText != null) _cardText.text = Resolve(CaptureKey(_lastCapture.Outcome));
        }
        private void HandleShutter() => ShutterRequested?.Invoke();
        private void HandleExit() => ExitRequested?.Invoke();
        private void HandleLearn() => LearnRequested?.Invoke();
        private string Resolve(LocalizedKey key) { try { return _localization?.Resolve(key) ?? string.Empty; } catch { return string.Empty; } }
        private void SetButtonLabel(Button button, LocalizedKey key) { Text label = button == null ? null : button.GetComponentInChildren<Text>(true); if (label != null) label.text = Resolve(key); }
        private static LocalizedKey GuidanceKey(PhotoGuidance guidance) => guidance == PhotoGuidance.MoveCloser
            ? LocalizationKeys.PhotographyMoveCloser : guidance == PhotoGuidance.Ready ? LocalizationKeys.PhotographyReady : LocalizationKeys.PhotographyCenter;
        private static LocalizedKey CaptureKey(PhotoCaptureOutcome outcome) => outcome == PhotoCaptureOutcome.CapturedNew
            ? LocalizationKeys.PhotographyCapturedNew : outcome == PhotoCaptureOutcome.CapturedWithoutThumbnail
                ? LocalizationKeys.PhotographyStorageFallback : outcome == PhotoCaptureOutcome.NotReady
                    ? LocalizationKeys.PhotographyPositiveHint : LocalizationKeys.PhotographyCapturedRepeated;
        private void OnDestroy() { _shutter?.onClick.RemoveListener(HandleShutter); _exit?.onClick.RemoveListener(HandleExit); _learn?.onClick.RemoveListener(HandleLearn); Unbind(); }
    }
}
