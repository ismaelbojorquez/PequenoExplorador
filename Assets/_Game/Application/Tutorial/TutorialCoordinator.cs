using System;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Tutorial
{
    public sealed class TutorialCoordinator
    {
        private readonly TutorialDefinition _definition;
        private readonly ITutorialProgressRepository _repository;
        private double _elapsed;
        private int _helpLevel;
        private bool _guideChoiceMade;

        public TutorialCoordinator(TutorialDefinition definition, ITutorialProgressRepository repository)
        {
            _definition = definition ?? throw new ArgumentNullException(nameof(definition));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _guideChoiceMade = _repository.Current.Tutorial.Status != TutorialProgressStatus.NotStarted;
            Snapshot = BuildSnapshot();
        }

        public event Action<TutorialSnapshot> Changed;
        public TutorialSnapshot Snapshot { get; private set; }

        public bool Allows(TutorialAction action) => !Snapshot.IsActive || (Snapshot.Step.AllowedActions & action) != 0;

        public void Initialize()
        {
            ReconcileVersion();
            _guideChoiceMade = _repository.Current.Tutorial.Status != TutorialProgressStatus.NotStarted;
            ResetStepClock();
            Publish();
        }

        public void SelectGuidance(GuidanceMode mode)
        {
            if (!Enum.IsDefined(typeof(GuidanceMode), mode)) throw new ArgumentOutOfRangeException(nameof(mode));
            PlayerProgress current = _repository.Current;
            TutorialProgress progress = new TutorialProgress(_definition.Id, _definition.Version, 0, TutorialProgressStatus.InProgress);
            _repository.Commit(current.WithPreferences(current.Preferences.WithGuidanceMode(mode)).WithTutorialState(progress));
            _guideChoiceMade = true;
            ResetStepClock();
            Publish();
        }

        public bool Signal(TutorialTrigger trigger)
        {
            if (!Snapshot.IsActive || Snapshot.Step.Trigger != trigger) return false;
            int next = Snapshot.Progress.StepIndex + 1;
            TutorialProgress progress = next >= _definition.Steps.Count
                ? new TutorialProgress(_definition.Id, _definition.Version, _definition.Steps.Count, TutorialProgressStatus.Completed)
                : new TutorialProgress(_definition.Id, _definition.Version, next, TutorialProgressStatus.InProgress);
            _repository.Commit(_repository.Current.WithTutorialState(progress));
            ResetStepClock();
            Publish();
            return true;
        }

        public void Tick(double unscaledSeconds)
        {
            if (!Snapshot.IsActive || unscaledSeconds <= 0) return;
            _elapsed += unscaledSeconds;
            double threshold = Snapshot.GuidanceMode == GuidanceMode.MoreGuidance
                ? Snapshot.Step.MoreGuidanceHelpSeconds : Snapshot.Step.StandardHelpSeconds;
            int targetLevel = Math.Min(2, (int)(_elapsed / threshold));
            if (targetLevel <= _helpLevel) return;
            _helpLevel = targetLevel;
            Publish();
        }

        public void ReplayInstruction()
        {
            if (!Snapshot.IsActive) return;
            _helpLevel = Math.Max(1, _helpLevel);
            Publish();
        }

        public void Skip()
        {
            if (!Snapshot.IsActive && !Snapshot.NeedsGuideChoice) return;
            _guideChoiceMade = true;
            _repository.Commit(_repository.Current.WithTutorialState(
                new TutorialProgress(_definition.Id, _definition.Version, _definition.Steps.Count, TutorialProgressStatus.Skipped)));
            ResetStepClock();
            Publish();
        }

        public void Replay()
        {
            _guideChoiceMade = true;
            _repository.Commit(_repository.Current.WithTutorialState(
                new TutorialProgress(_definition.Id, _definition.Version, 0, TutorialProgressStatus.InProgress)));
            ResetStepClock();
            Publish();
        }

#if UNITY_EDITOR || PE_DEVELOPMENT_SERVICES
        public void ResetForDevelopment()
        {
            _guideChoiceMade = false;
            _repository.Commit(_repository.Current.WithTutorialState(
                new TutorialProgress(_definition.Id, _definition.Version, 0, TutorialProgressStatus.NotStarted)));
            ResetStepClock();
            Publish();
        }
#endif

        private void ReconcileVersion()
        {
            TutorialProgress saved = _repository.Current.Tutorial;
            if (saved.TutorialId == _definition.Id && saved.ContentVersion == _definition.Version &&
                saved.StepIndex <= _definition.Steps.Count) return;
            _repository.Commit(_repository.Current.WithTutorialState(
                new TutorialProgress(_definition.Id, _definition.Version, 0, TutorialProgressStatus.NotStarted)));
        }

        private TutorialSnapshot BuildSnapshot()
        {
            PlayerProgress current = _repository.Current;
            TutorialProgress progress = current.Tutorial;
            TutorialStepDefinition step = progress.Status == TutorialProgressStatus.InProgress && progress.StepIndex < _definition.Steps.Count
                ? _definition.Steps[progress.StepIndex] : null;
            return new TutorialSnapshot(progress, step, current.Preferences.GuidanceMode, _helpLevel,
                !_guideChoiceMade && progress.Status == TutorialProgressStatus.NotStarted);
        }

        private void ResetStepClock() { _elapsed = 0; _helpLevel = 0; }
        private void Publish() { Snapshot = BuildSnapshot(); Changed?.Invoke(Snapshot); }
    }
}
