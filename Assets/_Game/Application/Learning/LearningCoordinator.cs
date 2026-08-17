using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Missions;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Learning
{
    public sealed class LearningCoordinator
    {
        private readonly ILearningCatalog _catalog;
        private readonly LearningActivityStrategyRegistry _strategies;
        private readonly ILearningRepository _repository;
        private readonly GrantRewardUseCase _rewards;
        private readonly IMissionFactSink _missions;
        private readonly IClock _clock;
        private readonly bool _allowUnapproved;
        private readonly TimeSpan _localOffset;

        public LearningCoordinator(ILearningCatalog catalog, LearningActivityStrategyRegistry strategies,
            ILearningRepository repository, GrantRewardUseCase rewards, IMissionFactSink missions,
            IClock clock, bool allowUnapproved, TimeSpan localOffset)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _rewards = rewards ?? throw new ArgumentNullException(nameof(rewards));
            _missions = missions ?? throw new ArgumentNullException(nameof(missions));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _allowUnapproved = allowUnapproved;
            _localOffset = localOffset;
        }

        public LearningActivityResult Start(ActivityId id)
        {
            if (_repository.IsReadOnly) return Result(ActivityOutcome.ReadOnly, null);
            if (!TryDefinition(id, out LearningActivityDefinition definition, out ActivityOutcome unavailable)) return Result(unavailable, null);
            PlayerProgress current = _repository.Current;
            LearningSession existing = current.LearningSessions.FirstOrDefault(item => item.ActivityId.Equals(id));
            if (existing != null && existing.IsCompleted) return CompleteResult(ActivityOutcome.AlreadyCompleted, definition, existing);
            LearningSession session;
            ActivityOutcome outcome;
            if (existing == null) { session = new LearningSession(id, LearningSessionStatus.Active, 0, 0); outcome = ActivityOutcome.Started; }
            else { session = existing.Resume(); outcome = ActivityOutcome.Resumed; }
            CommitSession(current, session, outcome == ActivityOutcome.Started ? definition.Concepts : Array.Empty<LearningConceptId>(), false);
            return Result(outcome, session);
        }

        public LearningActivityResult Submit(ActivityId id, LearningOptionId optionId)
        {
            if (_repository.IsReadOnly) return Result(ActivityOutcome.ReadOnly, null);
            if (!TryDefinition(id, out LearningActivityDefinition definition, out ActivityOutcome unavailable)) return Result(unavailable, null);
            LearningSession session = _repository.Current.LearningSessions.FirstOrDefault(item => item.ActivityId.Equals(id));
            if (session == null || session.Status != LearningSessionStatus.Active) return Result(ActivityOutcome.NotActive, session);
            if (!_strategies.TryGet(definition.TypeId, out ILearningActivityStrategy strategy)) throw new InvalidOperationException("Learning strategy is not registered: " + definition.TypeId.Value);
            LearningEvaluation evaluation = strategy.Evaluate(definition, session, new LearningSubmission(optionId));
            if (!evaluation.Accepted) return Result(ActivityOutcome.InvalidOption, session);
            if (!evaluation.Correct)
            {
                int attempts = checked(session.Attempts + 1);
                int level = definition.HintPolicy.ResolveLevel(attempts, session.HintLevel);
                LearningSession updated = session.RecordIncorrect(level);
                CommitSession(_repository.Current, updated, Array.Empty<LearningConceptId>(), false);
                return Result(level > session.HintLevel ? ActivityOutcome.Hint : ActivityOutcome.TryAgain, updated);
            }
            LearningSession completed = session.Complete();
            CommitSession(_repository.Current, completed, definition.Concepts, true);
            return CompleteResult(ActivityOutcome.Completed, definition, completed);
        }

        public LearningActivityResult RequestHint(ActivityId id)
        {
            if (_repository.IsReadOnly) return Result(ActivityOutcome.ReadOnly, null);
            if (!TryDefinition(id, out LearningActivityDefinition definition, out ActivityOutcome unavailable)) return Result(unavailable, null);
            LearningSession session = _repository.Current.LearningSessions.FirstOrDefault(item => item.ActivityId.Equals(id));
            if (session == null || session.Status != LearningSessionStatus.Active) return Result(ActivityOutcome.NotActive, session);
            int level = Math.Min(definition.HintPolicy.MaximumLevel, session.HintLevel + 1);
            LearningSession updated = new LearningSession(id, LearningSessionStatus.Active, session.Attempts, level);
            CommitSession(_repository.Current, updated, Array.Empty<LearningConceptId>(), false);
            return Result(ActivityOutcome.Hint, updated);
        }

        public LearningActivityResult Exit(ActivityId id)
        {
            if (_repository.IsReadOnly) return Result(ActivityOutcome.ReadOnly, null);
            if (!TryDefinition(id, out LearningActivityDefinition definition, out ActivityOutcome unavailable)) return Result(unavailable, null);
            LearningSession session = _repository.Current.LearningSessions.FirstOrDefault(item => item.ActivityId.Equals(id));
            if (session == null || session.Status != LearningSessionStatus.Active) return Result(ActivityOutcome.NotActive, session);
            LearningSession updated = definition.Resumable ? session.Exit() : session.Restart().Exit();
            CommitSession(_repository.Current, updated, Array.Empty<LearningConceptId>(), false);
            return Result(ActivityOutcome.Exited, updated);
        }

        public LearningActivityResult Restart(ActivityId id)
        {
            if (_repository.IsReadOnly) return Result(ActivityOutcome.ReadOnly, null);
            if (!TryDefinition(id, out LearningActivityDefinition definition, out ActivityOutcome unavailable)) return Result(unavailable, null);
            LearningSession existing = _repository.Current.LearningSessions.FirstOrDefault(item => item.ActivityId.Equals(id));
            if (existing != null && existing.IsCompleted) return CompleteResult(ActivityOutcome.AlreadyCompleted, definition, existing);
            LearningSession restarted = new LearningSession(id, LearningSessionStatus.Active, 0, 0);
            CommitSession(_repository.Current, restarted, definition.Concepts, false);
            return Result(ActivityOutcome.Restarted, restarted);
        }

        public int ReconcileCompleted()
        {
            if (_repository.IsReadOnly) return 0;
            int reconciled = 0;
            foreach (LearningSession session in _repository.Current.LearningSessions.Where(item => item.IsCompleted))
                if (TryDefinition(session.ActivityId, out LearningActivityDefinition definition, out _))
                {
                    LearningActivityResult result = CompleteResult(ActivityOutcome.AlreadyCompleted, definition, session);
                    if (result.Reward.Granted) reconciled++;
                }
            return reconciled;
        }

        private LearningActivityResult CompleteResult(ActivityOutcome outcome, LearningActivityDefinition definition, LearningSession session)
        {
            GrantRewardResult reward = _rewards.Execute(definition.RewardId,
                EconomyTransactionId.Parse("economy-tx.activity." + definition.Id.Value), RewardSourceKind.Activity, definition.Id.Value);
            MissionFactResult mission = _missions.Record(new GameplayFact(
                GameplayFactId.Parse("gameplay-fact.learning." + definition.Id.Value), GameplayFactTypes.LearningCompleted,
                definition.Id.Value, Array.Empty<TagId>(), GameplayFactScope.Persistent));
            return new LearningActivityResult(outcome, session, reward, mission);
        }

        private bool TryDefinition(ActivityId id, out LearningActivityDefinition definition, out ActivityOutcome outcome)
        {
            if (!_catalog.TryGetActivity(id, out definition)) { outcome = ActivityOutcome.Missing; return false; }
            if (!_allowUnapproved && !definition.Editorial.IsReleaseApproved) { outcome = ActivityOutcome.Unavailable; return false; }
            outcome = ActivityOutcome.Started; return true;
        }

        private void CommitSession(PlayerProgress current, LearningSession session, IEnumerable<LearningConceptId> concepts, bool completed)
        {
            var sessions = current.LearningSessions.Where(item => !item.ActivityId.Equals(session.ActivityId)).Concat(new[] { session }).ToArray();
            var aggregates = new List<LearningConceptDailyProgress>(current.LearningConcepts);
            string day = _clock.UtcNow.ToOffset(_localOffset).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            foreach (LearningConceptId concept in concepts)
            {
                int index = aggregates.FindIndex(item => item.ConceptId.Equals(concept) && item.LocalDate == day);
                LearningConceptDailyProgress value = index < 0 ? new LearningConceptDailyProgress(concept, day, 0, 0) : aggregates[index];
                value = completed ? value.AddCompleted() : value.AddSeen();
                if (index < 0) aggregates.Add(value); else aggregates[index] = value;
            }
            _repository.Commit(current.WithLearningState(sessions, aggregates));
        }

        private static LearningActivityResult Result(ActivityOutcome outcome, LearningSession session) => new LearningActivityResult(outcome, session);
    }
}
