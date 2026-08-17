using System;
using System.Linq;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.Learning;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Learning
{
    [DisallowMultipleComponent]
    public sealed class LearningActivityView : MonoBehaviour
    {
        public const string PlaceholderObjectName = "PH_UI_LEARNING";
        public static readonly ActivityId FixtureActivityId = ActivityId.Parse("activity.fixture.visual-matching");
        public static readonly ActivityId ToucanActivityId = ActivityId.Parse("activity.jungle.keel-billed-toucan.choose-food");

        [SerializeField] private Text _title;
        [SerializeField] private Text _instruction;
        [SerializeField] private Text _feedback;
        [SerializeField] private Text _watermark;
        [SerializeField] private Button[] _options = Array.Empty<Button>();
        [SerializeField] private Button _hint;
        [SerializeField] private Button _replay;
        [SerializeField] private Button _exit;

        private ILearningCatalog _catalog;
        private ILearningRepository _repository;
        private LearningCoordinator _coordinator;
        private ILocalizationService _localization;
        private IAudioService _audio;
        private IInputService _input;
        private LearningInteractionAction _entryAction;
        private PhotographyInteractionAction _photography;
        private LearningActivityDefinition _definition;
        private ActivityId _activeActivityId;
        private DiscoveryId _continuationDiscoveryId;

        public event Action<LearningReactionId, bool> ReactionRequested;
        public event Action<LearningActivityResult> ActivityCompleted;
        public event Action<bool> VisibilityChanged;
        public ActivityOutcome LastOutcome { get; private set; }
        public string FeedbackText => _feedback == null ? string.Empty : _feedback.text;
        public string TitleText => _title == null ? string.Empty : _title.text;
        public int OptionCount => _options?.Length ?? 0;
        public bool IsVisible => gameObject.activeSelf;
        public ActivityId ActiveActivityId => _activeActivityId;
        public bool ReduceMotion { get; private set; }

        public void Bind(ILearningCatalog catalog, ILearningRepository repository, LearningCoordinator coordinator,
            ILocalizationService localization, IAudioService audio, IInputService input = null,
            LearningInteractionAction entryAction = null, PhotographyInteractionAction photography = null,
            bool reduceMotion = false)
        {
            Unbind();
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _audio = audio ?? throw new ArgumentNullException(nameof(audio));
            _input = input;
            _entryAction = entryAction;
            _photography = photography;
            ReduceMotion = reduceMotion;
            if (!_catalog.TryGetActivity(FixtureActivityId, out _))
                throw new InvalidOperationException("Learning fixture is missing.");
            _localization.LocaleChanged += HandleLocaleChanged;
            if (_entryAction != null) _entryAction.Requested += HandleRequested;
            for (int index = 0; index < _options.Length; index++)
            {
                int captured = index;
                _options[index].onClick.AddListener(() => Submit(captured));
            }
            _hint?.onClick.AddListener(HandleHint);
            _replay?.onClick.AddListener(Replay);
            _exit?.onClick.AddListener(HandleExit);
            gameObject.SetActive(false);
        }

        public LearningActivityResult StartFixture() => Open(FixtureActivityId, default);

        public LearningActivityResult Open(ActivityId activityId, DiscoveryId continuationDiscoveryId)
        {
            if (_catalog == null || !_catalog.TryGetActivity(activityId, out _definition))
                return default;
            bool wasVisible = IsVisible;
            _activeActivityId = activityId;
            _continuationDiscoveryId = continuationDiscoveryId;
            gameObject.SetActive(true);
            if (!wasVisible) VisibilityChanged?.Invoke(true);
            _input?.SetMap(InputMapId.UI);
            RenderDefinition();
            LearningActivityResult result = _coordinator.Start(activityId);
            Apply(result);
            PlayCue(_definition.InstructionCueId);
            return result;
        }

        public LearningActivityResult Submit(int optionIndex)
        {
            if (_definition == null || optionIndex < 0 || optionIndex >= _definition.Options.Count) return default;
            LearningActivityResult result = _coordinator.Submit(_activeActivityId, _definition.Options[optionIndex].Id);
            Apply(result);
            if (result.Outcome == ActivityOutcome.Completed) ActivityCompleted?.Invoke(result);
            return result;
        }

        public LearningActivityResult RequestHint()
        {
            LearningActivityResult result = _coordinator.RequestHint(_activeActivityId);
            Apply(result);
            return result;
        }

        public LearningActivityResult Exit()
        {
            LearningActivityResult result = _coordinator.Exit(_activeActivityId);
            Apply(result);
            return result;
        }

        public void Replay() => _audio?.ReplayLastInstruction();
        public void SetReduceMotion(bool enabled) => ReduceMotion = enabled;
        public void CloseForSceneUnload()
        {
            if (!IsVisible || !_activeActivityId.IsValid) return;
            Exit();
            Hide();
        }
        private void HandleRequested(ActivityId activityId, DiscoveryId discoveryId) => Open(activityId, discoveryId);
        private void HandleHint() => RequestHint();

        private void HandleExit()
        {
            bool completed = CurrentSession()?.Status == LearningSessionStatus.Completed;
            Exit();
            DiscoveryId continuation = _continuationDiscoveryId;
            Hide();
            if (completed && continuation.IsValid) _photography?.Request(continuation);
        }

        public void Close() => HandleExit();

        private void Hide()
        {
            bool wasVisible = IsVisible;
            _input?.SetMap(InputMapId.Explorer);
            gameObject.SetActive(false);
            if (wasVisible) VisibilityChanged?.Invoke(false);
            _definition = null;
            _activeActivityId = default;
            _continuationDiscoveryId = default;
        }

        private void Apply(LearningActivityResult result)
        {
            LastOutcome = result.Outcome;
            if (_feedback == null || _definition == null) return;
            switch (result.Outcome)
            {
                case ActivityOutcome.Completed:
                case ActivityOutcome.AlreadyCompleted:
                    _feedback.text = _definition.FactId.IsValid ? Resolve(_definition.FactCopy) : Resolve(_definition.Success);
                    if (!PlayCue(_definition.FactCueId)) _audio?.Play(AudioCueIds.ConfirmFeedback);
                    if (_definition.PositiveReactionId.IsValid) ReactionRequested?.Invoke(_definition.PositiveReactionId, ReduceMotion);
                    break;
                case ActivityOutcome.Hint:
                    int level = Math.Max(1, result.Session?.HintLevel ?? 1);
                    _feedback.text = Resolve(_definition.Hints[Math.Min(level, _definition.Hints.Count) - 1]);
                    ApplyHintLevel(level);
                    PlayRetry();
                    break;
                case ActivityOutcome.TryAgain:
                    _feedback.text = Resolve(_definition.TryAgain);
                    PlayRetry();
                    if (_definition.NeutralReactionId.IsValid) ReactionRequested?.Invoke(_definition.NeutralReactionId, ReduceMotion);
                    break;
                case ActivityOutcome.Exited:
                    _feedback.text = Resolve(LocalizationKeys.LearningExitSafe);
                    break;
                default:
                    _feedback.text = string.Empty;
                    break;
            }
        }

        private void PlayRetry()
        {
            if (!PlayCue(_definition.RetryCueId)) _audio?.Play(AudioCueIds.RetryFeedback);
        }

        private bool PlayCue(AudioCueId cue)
        {
            if (string.IsNullOrWhiteSpace(cue.Value) || _audio == null) return false;
            return _audio.Play(cue).IsAccepted;
        }

        private void RenderDefinition()
        {
            if (_definition == null) return;
            if (_title != null) _title.text = Resolve(_definition.Title);
            if (_instruction != null) _instruction.text = Resolve(_definition.Instruction);
            if (_watermark != null)
            {
                _watermark.gameObject.SetActive(_definition.Editorial.IsPlaceholder);
                _watermark.text = _definition.Editorial.DevelopmentWatermark;
            }
            for (int index = 0; index < _options.Length; index++)
            {
                bool visible = index < _definition.Options.Count;
                _options[index].gameObject.SetActive(visible);
                if (!visible) continue;
                LearningOptionDefinition option = _definition.Options[index];
                Text label = _options[index].GetComponentInChildren<Text>(true);
                if (label != null) label.text = Resolve(option.Label);
                Image image = _options[index].GetComponent<Image>();
                if (image != null) image.color = new Color32(option.Red, option.Green, option.Blue, 255);
                _options[index].transform.localScale = Vector3.one;
            }
            SetButtonLabel(_hint, LocalizationKeys.LearningHint);
            SetButtonLabel(_replay, LocalizationKeys.LearningReplay);
            SetButtonLabel(_exit, LocalizationKeys.LearningExit);
        }

        private void ApplyHintLevel(int level)
        {
            if (_definition == null || level < _definition.HintPolicy.MaximumLevel) return;
            int correct = _definition.Options.ToList().FindIndex(item => _definition.CorrectTagId.IsValid
                ? item.TagId == _definition.CorrectTagId
                : item.Id.Equals(_definition.CorrectOptionId));
            if (correct >= 0 && correct < _options.Length && !ReduceMotion)
                _options[correct].transform.localScale = new Vector3(1.08f, 1.08f, 1f);
        }

        private LearningSession CurrentSession() => _repository?.Current.LearningSessions
            .FirstOrDefault(item => item.ActivityId.Equals(_activeActivityId));
        private string Resolve(LocalizedKey key)
        {
            try { return _localization.Resolve(key); }
            catch { return string.Empty; }
        }
        private void SetButtonLabel(Button button, LocalizedKey key)
        {
            Text label = button == null ? null : button.GetComponentInChildren<Text>(true);
            if (label != null) label.text = Resolve(key);
        }
        private void HandleLocaleChanged(string _)
        {
            RenderDefinition();
            if (_activeActivityId.IsValid)
                Apply(new LearningActivityResult(LastOutcome, CurrentSession()));
        }

        public void Unbind()
        {
            if (_localization != null) _localization.LocaleChanged -= HandleLocaleChanged;
            if (_entryAction != null) _entryAction.Requested -= HandleRequested;
            if (_options != null)
                foreach (Button button in _options) button?.onClick.RemoveAllListeners();
            _hint?.onClick.RemoveAllListeners();
            _replay?.onClick.RemoveAllListeners();
            _exit?.onClick.RemoveAllListeners();
            _catalog = null; _repository = null; _coordinator = null; _localization = null; _audio = null;
            _input = null; _entryAction = null; _photography = null; _definition = null;
            _activeActivityId = default; _continuationDiscoveryId = default;
        }

        private void OnDestroy() => Unbind();
    }
}
