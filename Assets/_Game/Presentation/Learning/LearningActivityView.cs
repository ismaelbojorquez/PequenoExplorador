using System;
using System.Linq;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Learning;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;
using UnityEngine;
using UnityEngine.UI;

namespace PequenoExplorador.Presentation.Learning
{
    [DisallowMultipleComponent]
    public sealed class LearningActivityView : MonoBehaviour
    {
        public const string PlaceholderObjectName = "PH_UI_LEARNING";
        public static readonly ActivityId FixtureActivityId = ActivityId.Parse("activity.fixture.visual-matching");
        [SerializeField] private Text _title; [SerializeField] private Text _instruction; [SerializeField] private Text _feedback;
        [SerializeField] private Button[] _options = Array.Empty<Button>(); [SerializeField] private Button _hint; [SerializeField] private Button _replay; [SerializeField] private Button _exit;
        private ILearningCatalog _catalog; private ILearningRepository _repository; private LearningCoordinator _coordinator;
        private ILocalizationService _localization; private IAudioService _audio; private LearningActivityDefinition _definition;
        public ActivityOutcome LastOutcome { get; private set; }
        public string FeedbackText => _feedback == null ? string.Empty : _feedback.text;
        public string TitleText => _title == null ? string.Empty : _title.text;
        public int OptionCount => _options?.Length ?? 0;

        public void Bind(ILearningCatalog catalog, ILearningRepository repository, LearningCoordinator coordinator,
            ILocalizationService localization, IAudioService audio)
        {
            Unbind(); _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog)); _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator)); _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _audio = audio ?? throw new ArgumentNullException(nameof(audio));
            if (!_catalog.TryGetActivity(FixtureActivityId, out _definition)) throw new InvalidOperationException("Learning fixture is missing.");
            _localization.LocaleChanged += HandleLocaleChanged;
            for (int index = 0; index < _options.Length; index++) { int captured = index; _options[index].onClick.AddListener(() => Submit(captured)); }
            _hint?.onClick.AddListener(HandleHint); _replay?.onClick.AddListener(Replay); _exit?.onClick.AddListener(HandleExit);
            RenderDefinition();
        }

        public LearningActivityResult StartFixture() { LearningActivityResult result = _coordinator.Start(FixtureActivityId); Apply(result); return result; }
        public LearningActivityResult Submit(int optionIndex)
        {
            if (_definition == null || optionIndex < 0 || optionIndex >= _definition.Options.Count) return default;
            LearningActivityResult result = _coordinator.Submit(FixtureActivityId, _definition.Options[optionIndex].Id); Apply(result); return result;
        }
        public LearningActivityResult RequestHint() { LearningActivityResult result = _coordinator.RequestHint(FixtureActivityId); Apply(result); return result; }
        public LearningActivityResult Exit() { LearningActivityResult result = _coordinator.Exit(FixtureActivityId); Apply(result); return result; }
        public void Replay() => _audio?.ReplayLastInstruction();
        private void HandleHint() => RequestHint();
        private void HandleExit() => Exit();

        private void Apply(LearningActivityResult result)
        {
            LastOutcome = result.Outcome;
            if (_feedback == null || _definition == null) return;
            switch (result.Outcome)
            {
                case ActivityOutcome.Completed:
                case ActivityOutcome.AlreadyCompleted: _feedback.text = Resolve(_definition.Success); _audio?.Play(AudioCueIds.ConfirmFeedback); break;
                case ActivityOutcome.Hint:
                    int level = Math.Max(1, result.Session?.HintLevel ?? 1); _feedback.text = Resolve(_definition.Hints[Math.Min(level, _definition.Hints.Count) - 1]); _audio?.Play(AudioCueIds.RetryFeedback); break;
                case ActivityOutcome.TryAgain: _feedback.text = Resolve(_definition.TryAgain); _audio?.Play(AudioCueIds.RetryFeedback); break;
                case ActivityOutcome.Exited: _feedback.text = Resolve(LocalizationKeys.LearningExitSafe); break;
                default: _feedback.text = string.Empty; break;
            }
        }

        private void RenderDefinition()
        {
            if (_definition == null) return;
            if (_title != null) _title.text = Resolve(_definition.Title);
            if (_instruction != null) _instruction.text = Resolve(_definition.Instruction);
            for (int index = 0; index < _options.Length; index++)
            {
                bool visible = index < _definition.Options.Count; _options[index].gameObject.SetActive(visible);
                Text label = _options[index].GetComponentInChildren<Text>(true); if (visible && label != null) label.text = Resolve(_definition.Options[index].Label);
            }
            SetButtonLabel(_hint, LocalizationKeys.LearningHint); SetButtonLabel(_replay, LocalizationKeys.LearningReplay); SetButtonLabel(_exit, LocalizationKeys.LearningExit);
        }
        private string Resolve(LocalizedKey key) { try { return _localization.Resolve(key); } catch { return string.Empty; } }
        private void SetButtonLabel(Button button, LocalizedKey key) { Text label = button == null ? null : button.GetComponentInChildren<Text>(true); if (label != null) label.text = Resolve(key); }
        private void HandleLocaleChanged(string _) { RenderDefinition(); Apply(new LearningActivityResult(LastOutcome, _repository.Current.LearningSessions.FirstOrDefault(item => item.ActivityId.Equals(FixtureActivityId)))); }
        public void Unbind()
        {
            if (_localization != null) _localization.LocaleChanged -= HandleLocaleChanged;
            if (_options != null) foreach (Button button in _options) button?.onClick.RemoveAllListeners();
            _hint?.onClick.RemoveAllListeners(); _replay?.onClick.RemoveAllListeners(); _exit?.onClick.RemoveAllListeners();
            _catalog = null; _repository = null; _coordinator = null; _localization = null; _audio = null; _definition = null;
        }
        private void OnDestroy() => Unbind();
    }
}
