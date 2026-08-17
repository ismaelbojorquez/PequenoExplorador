using System;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Tutorial;
using PequenoExplorador.Domain.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PequenoExplorador.DesignSystem;

namespace PequenoExplorador.Presentation.Tutorial
{
    [DisallowMultipleComponent]
    public sealed class TutorialView : MonoBehaviour
    {
        public const string PlaceholderObjectName = "PH_UI_TUTORIAL";
        [SerializeField] private GameObject _instructionPanel;
        [SerializeField] private TMP_Text _instruction;
        [SerializeField] private TMP_Text _progress;
        [SerializeField] private GameObject _gesture;
        [SerializeField] private UIIconGraphic _gestureIcon;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _replayButton;
        [SerializeField] private Button _skipButton;
        [SerializeField] private GameObject _guideChoicePanel;
        [SerializeField] private TMP_Text _guideChoiceTitle;
        [SerializeField] private Button _moreGuidanceButton;
        [SerializeField] private Button _standardGuidanceButton;
        [SerializeField] private Button _replayTutorialButton;

        private TutorialCoordinator _coordinator;
        private ILocalizationService _localization;
        private IAudioService _audio;
        private bool _reduceMotion;
        private TutorialSnapshot _snapshot;
        private float _gesturePhase;
        private bool _replayEntryAvailable;

        public bool IsInstructionVisible => _instructionPanel != null && _instructionPanel.activeSelf;
        public bool IsGuideChoiceVisible => _guideChoicePanel != null && _guideChoicePanel.activeSelf;
        public string InstructionText => _instruction == null ? string.Empty : _instruction.text;
        public TutorialSnapshot Snapshot => _snapshot;
        public Button SkipButton => _skipButton;
        public Button ReplayButton => _replayButton;
        public Button ContinueButton => _continueButton;
        public Button MoreGuidanceButton => _moreGuidanceButton;
        public Button StandardGuidanceButton => _standardGuidanceButton;
        public Button ReplayTutorialButton => _replayTutorialButton;
        public bool GestureVisible => _gesture != null && _gesture.activeSelf;
        public event Action<bool> VisibilityChanged;
        public void SetReplayEntryVisible(bool visible)
        {
            _replayEntryAvailable = visible;
            if (_replayTutorialButton != null)
                _replayTutorialButton.gameObject.SetActive(visible && _snapshot != null && !_snapshot.IsActive && !_snapshot.NeedsGuideChoice);
        }

        private void Awake()
        {
            _continueButton?.onClick.AddListener(Continue);
            _replayButton?.onClick.AddListener(ReplayInstruction);
            _skipButton?.onClick.AddListener(Skip);
            _moreGuidanceButton?.onClick.AddListener(ChooseMoreGuidance);
            _standardGuidanceButton?.onClick.AddListener(ChooseStandardGuidance);
            _replayTutorialButton?.onClick.AddListener(ReplayTutorial);
        }

        public void Bind(TutorialCoordinator coordinator, ILocalizationService localization, IAudioService audio, bool reduceMotion)
        {
            Unbind();
            _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _audio = audio ?? throw new ArgumentNullException(nameof(audio));
            _reduceMotion = reduceMotion;
            _coordinator.Changed += HandleChanged;
            _localization.LocaleChanged += HandleLocaleChanged;
            HandleChanged(_coordinator.Snapshot, playVoice: false);
        }

        public void SetReduceMotion(bool enabled) { _reduceMotion = enabled; if (enabled) ResetGesture(); }
        public void Signal(TutorialTrigger trigger) => _coordinator?.Signal(trigger);

        private void Update()
        {
            if (_coordinator == null) return;
            _coordinator.Tick(Time.unscaledDeltaTime);
            if (_gesture == null || !_gesture.activeSelf || _reduceMotion) return;
            _gesturePhase += Time.unscaledDeltaTime * 3f;
            RectTransform rect = _gesture.transform as RectTransform;
            if (rect != null) rect.localScale = Vector3.one * (1f + Mathf.Sin(_gesturePhase) * 0.06f);
        }

        private void HandleChanged(TutorialSnapshot snapshot) => HandleChanged(snapshot, playVoice: true);
        private void HandleChanged(TutorialSnapshot snapshot, bool playVoice)
        {
            _snapshot = snapshot;
            bool choose = snapshot?.NeedsGuideChoice == true;
            bool active = snapshot?.IsActive == true;
            _guideChoicePanel?.SetActive(choose);
            _instructionPanel?.SetActive(active);
            if (_replayTutorialButton != null) _replayTutorialButton.gameObject.SetActive(_replayEntryAvailable && !choose && !active);
            if (!active)
            {
                ResetGesture();
                RenderStaticLabels();
                VisibilityChanged?.Invoke(choose);
                return;
            }
            _instruction.text = Resolve(snapshot.Step.Instruction);
            if (_progress != null) _progress.text = Resolve(LocalizationKeys.TutorialProgress, snapshot.Progress.StepIndex + 1, 7);
            bool continueStep = snapshot.Step.Trigger == TutorialTrigger.Continue;
            if (_continueButton != null) _continueButton.gameObject.SetActive(continueStep);
            if (_gesture != null) _gesture.SetActive(!continueStep && (snapshot.HelpLevel > 0 || snapshot.GuidanceMode == GuidanceMode.MoreGuidance));
            if (_gestureIcon != null) _gestureIcon.SetKind(IconFor(snapshot.Step.Spotlight));
            RenderStaticLabels();
            VisibilityChanged?.Invoke(true);
            if (playVoice) _audio.Play(snapshot.Step.VoiceCue);
        }

        private void RenderStaticLabels()
        {
            if (_guideChoiceTitle != null) _guideChoiceTitle.text = Resolve(LocalizationKeys.TutorialGuideChoice);
            SetLabel(_moreGuidanceButton, LocalizationKeys.TutorialMoreGuidance);
            SetLabel(_standardGuidanceButton, LocalizationKeys.TutorialStandardGuidance);
            SetLabel(_continueButton, LocalizationKeys.TutorialContinue);
            SetLabel(_replayButton, LocalizationKeys.TutorialReplay);
            SetLabel(_skipButton, LocalizationKeys.TutorialSkip);
            SetLabel(_replayTutorialButton, LocalizationKeys.TutorialReplayFromSettings);
        }

        private void Continue() => _coordinator?.Signal(TutorialTrigger.Continue);
        private void ReplayInstruction() { _coordinator?.ReplayInstruction(); if (_snapshot?.Step != null) _audio?.Play(_snapshot.Step.VoiceCue); }
        private void Skip() => _coordinator?.Skip();
        private void ChooseMoreGuidance() => _coordinator?.SelectGuidance(GuidanceMode.MoreGuidance);
        private void ChooseStandardGuidance() => _coordinator?.SelectGuidance(GuidanceMode.Standard);
        private void ReplayTutorial() => _coordinator?.Replay();
        private void HandleLocaleChanged(string _) => HandleChanged(_coordinator?.Snapshot, playVoice: false);
        private string Resolve(LocalizedKey key, params object[] args) { try { return _localization?.Resolve(key, args) ?? string.Empty; } catch { return string.Empty; } }
        private void SetLabel(Button button, LocalizedKey key)
        {
            TMP_Text label = button == null ? null : button.GetComponentInChildren<TMP_Text>(true);
            if (label != null) label.text = Resolve(key);
        }
        private void ResetGesture() { _gesturePhase = 0; if (_gesture != null) _gesture.transform.localScale = Vector3.one; }
        private static UIIconKind IconFor(TutorialSpotlight spotlight) => spotlight switch
        {
            TutorialSpotlight.Expedition => UIIconKind.Explore,
            TutorialSpotlight.Ground => UIIconKind.GestureTap,
            TutorialSpotlight.Interactable => UIIconKind.GestureTap,
            TutorialSpotlight.Shutter => UIIconKind.Camera,
            TutorialSpotlight.DiscoveryReward => UIIconKind.Star,
            TutorialSpotlight.ReturnCamp => UIIconKind.Back,
            TutorialSpotlight.Album => UIIconKind.Album,
            _ => UIIconKind.Hint
        };

        public void Unbind()
        {
            if (_coordinator != null) _coordinator.Changed -= HandleChanged;
            if (_localization != null) _localization.LocaleChanged -= HandleLocaleChanged;
            _coordinator = null; _localization = null; _audio = null; _snapshot = null;
            if (_instructionPanel != null) _instructionPanel.SetActive(false);
            if (_guideChoicePanel != null) _guideChoicePanel.SetActive(false);
            VisibilityChanged?.Invoke(false);
            ResetGesture();
        }

        private void OnDestroy()
        {
            _continueButton?.onClick.RemoveListener(Continue); _replayButton?.onClick.RemoveListener(ReplayInstruction);
            _skipButton?.onClick.RemoveListener(Skip); _moreGuidanceButton?.onClick.RemoveListener(ChooseMoreGuidance);
            _standardGuidanceButton?.onClick.RemoveListener(ChooseStandardGuidance); _replayTutorialButton?.onClick.RemoveListener(ReplayTutorial);
            Unbind();
        }
    }
}
