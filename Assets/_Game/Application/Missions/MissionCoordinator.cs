using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Missions
{
    public sealed class MissionCoordinator : IMissionFactSink
    {
        private readonly IMissionCatalog _catalog;
        private readonly MissionObjectiveStrategyRegistry _strategies;
        private readonly IMissionRepository _repository;
        private readonly GrantRewardUseCase _grantRewards;

        public MissionCoordinator(IMissionCatalog catalog, MissionObjectiveStrategyRegistry strategies,
            IMissionRepository repository, GrantRewardUseCase grantRewards)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _grantRewards = grantRewards ?? throw new ArgumentNullException(nameof(grantRewards));
        }

        public MissionActivationResult Activate(MissionId id)
        {
            PlayerProgress current = _repository.Current;
            if (_repository.IsReadOnly) return new MissionActivationResult(MissionActivationOutcome.ReadOnly, null);
            if (!_catalog.TryGet(id, out MissionDefinition definition)) return new MissionActivationResult(MissionActivationOutcome.Missing, null);
            MissionProgress existing = current.Missions.FirstOrDefault(item => item.Id.Equals(id));
            if (existing != null)
                return new MissionActivationResult(existing.IsCompleted ? MissionActivationOutcome.AlreadyCompleted : MissionActivationOutcome.AlreadyActive, existing);
            if (definition.Prerequisites.Any(required => !current.CompletedMissionIds.Contains(required.Value, StringComparer.Ordinal)))
                return new MissionActivationResult(MissionActivationOutcome.PrerequisitesMissing, null);
            var progress = new MissionProgress(id, MissionProgressStatus.Active, current.LastMissionFactSequence,
                definition.Objectives.Select(item => new MissionObjectiveProgress(item.Id, 0)));
            var missions = current.Missions.Concat(new[] { progress }).ToArray();
            _repository.Commit(current.WithMissionState(missions, current.CompletedMissionIds,
                current.ProcessedMissionFactIds, current.LastMissionFactSequence));
            return new MissionActivationResult(MissionActivationOutcome.Activated, progress);
        }

        public MissionFactResult Record(GameplayFact fact)
        {
            if (fact == null) throw new ArgumentNullException(nameof(fact));
            PlayerProgress current = _repository.Current;
            if (_repository.IsReadOnly) return new MissionFactResult(MissionFactOutcome.ReadOnly, default, default);
            if (current.ProcessedMissionFactIds.Contains(fact.Id.Value, StringComparer.Ordinal))
                return RetryPendingReward(current, MissionFactOutcome.Duplicate);

            long sequence = checked(current.LastMissionFactSequence + 1);
            GameplayFact recorded = fact.WithSequence(sequence);
            var processedFacts = new List<string>(current.ProcessedMissionFactIds) { fact.Id.Value };
            var completedIds = new List<string>(current.CompletedMissionIds);
            var states = new List<MissionProgress>();
            var newlyCompleted = new List<MissionDefinition>();
            MissionId changedMission = default;
            bool progressed = false;

            foreach (MissionProgress state in current.Missions)
            {
                if (state.IsCompleted || !_catalog.TryGet(state.Id, out MissionDefinition definition) || recorded.Sequence <= state.ActivationSequence)
                {
                    states.Add(state);
                    continue;
                }

                var objectiveStates = new List<MissionObjectiveProgress>();
                bool missionMatched = false;
                foreach (MissionObjectiveDefinition objective in definition.Objectives)
                {
                    MissionObjectiveProgress objectiveState = state.Objectives.First(item => item.Id.Equals(objective.Id));
                    if (!_strategies.TryGet(objective.TypeId, out IMissionObjectiveStrategy strategy))
                        throw new InvalidOperationException("Mission objective strategy is not registered: " + objective.TypeId.Value);
                    MissionObjectiveEvaluation evaluation = strategy.Evaluate(objective, objectiveState.Count, recorded);
                    missionMatched |= evaluation.Matched;
                    objectiveStates.Add(objectiveState.WithCount(evaluation.Count));
                }
                bool missionCompleted = objectiveStates.Zip(definition.Objectives,
                    (value, objective) => value.Count >= objective.TargetCount).All(value => value);
                MissionProgress updated = state.With(objectiveStates, missionCompleted);
                states.Add(updated);
                if (!missionMatched) continue;
                changedMission = state.Id;
                progressed = true;
                if (missionCompleted)
                {
                    newlyCompleted.Add(definition);
                    if (!completedIds.Contains(state.Id.Value, StringComparer.Ordinal)) completedIds.Add(state.Id.Value);
                }
            }

            PlayerProgress updatedProgress = current.WithMissionState(states, completedIds, processedFacts, sequence);
            _repository.Commit(updatedProgress);
            GrantRewardResult representativeReward = default;
            bool hasRepresentativeReward = false;
            foreach (MissionDefinition definition in newlyCompleted)
            {
                GrantRewardResult reward = EnsureReward(definition);
                if (!hasRepresentativeReward)
                {
                    representativeReward = reward;
                    hasRepresentativeReward = true;
                }
            }
            MissionId representativeMission = newlyCompleted.Count > 0 ? newlyCompleted[0].Id : changedMission;
            return new MissionFactResult(newlyCompleted.Count > 0 ? MissionFactOutcome.Completed : progressed ? MissionFactOutcome.Progressed : MissionFactOutcome.Ignored,
                representativeMission, representativeReward);
        }

        public int ReconcileCompletedRewards()
        {
            if (_repository.IsReadOnly) return 0;
            int granted = 0;
            foreach (MissionProgress state in _repository.Current.Missions.Where(item => item.IsCompleted))
                if (_catalog.TryGet(state.Id, out MissionDefinition definition) && definition.HasReward && EnsureReward(definition).Granted)
                    granted++;
            return granted;
        }

        private MissionFactResult RetryPendingReward(PlayerProgress current, MissionFactOutcome outcome)
        {
            foreach (MissionProgress state in current.Missions.Where(item => item.IsCompleted))
                if (_catalog.TryGet(state.Id, out MissionDefinition definition) && definition.HasReward)
                {
                    GrantRewardResult reward = EnsureReward(definition);
                    if (reward.Granted) return new MissionFactResult(outcome, state.Id, reward);
                }
            return new MissionFactResult(outcome, default, default);
        }

        private GrantRewardResult EnsureReward(MissionDefinition definition)
        {
            if (!definition.HasReward) return default;
            return _grantRewards.Execute(definition.RewardId,
                EconomyTransactionId.Parse("economy-tx.mission." + definition.Id.Value),
                RewardSourceKind.Mission, definition.Id.Value);
        }
    }
}
