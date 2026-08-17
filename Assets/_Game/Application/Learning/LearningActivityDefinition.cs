using System;
using System.Collections.Generic;
using System.Linq;
using PequenoExplorador.Application.Content;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Learning
{
    public sealed class LearningActivityDefinition : IActivityDefinition
    {
        private readonly LearningConceptId[] _concepts;
        private readonly LearningOptionDefinition[] _options;
        private readonly LocalizedKey[] _hints;
        public LearningActivityDefinition(ActivityId id, LearningActivityTypeId typeId, LocalizedKey title,
            LocalizedKey instruction, LocalizedKey success, LocalizedKey tryAgain,
            IEnumerable<LearningConceptId> concepts, IEnumerable<LearningOptionDefinition> options,
            LearningOptionId correctOptionId, IEnumerable<LocalizedKey> hints, HintPolicy hintPolicy,
            bool resumable, RewardId rewardId, EditorialMetadata editorial,
            TagId correctTagId = default,
            EducationalFactId factId = default,
            LocalizedKey factCopy = default,
            AudioCueId instructionCueId = default,
            AudioCueId factCueId = default,
            AudioCueId retryCueId = default,
            LearningReactionId positiveReactionId = default,
            LearningReactionId neutralReactionId = default)
        {
            if (!id.IsValid || !typeId.IsValid) throw new ArgumentException("Activity and type IDs are required.");
            if (string.IsNullOrWhiteSpace(title.Entry) || string.IsNullOrWhiteSpace(instruction.Entry) || string.IsNullOrWhiteSpace(success.Entry) || string.IsNullOrWhiteSpace(tryAgain.Entry)) throw new ArgumentException("Activity localization keys are required.");
            _concepts = (concepts ?? throw new ArgumentNullException(nameof(concepts))).ToArray();
            _options = (options ?? throw new ArgumentNullException(nameof(options))).ToArray();
            _hints = (hints ?? throw new ArgumentNullException(nameof(hints))).ToArray();
            if (_concepts.Length == 0 || _concepts.Any(item => !item.IsValid) || _concepts.Distinct().Count() != _concepts.Length) throw new ArgumentException("Activity concepts must be non-empty, valid and unique.");
            if (_options.Length < 2 || _options.Any(item => item == null) || _options.Select(item => item.Id).Distinct().Count() != _options.Length) throw new ArgumentException("Activity requires at least two unique options.");
            if (correctTagId.IsValid)
            {
                if (_options.Any(item => !item.TagId.IsValid) || !_options.Any(item => item.TagId == correctTagId))
                    throw new ArgumentException("Tagged activities require valid option tags and a matching correct tag.");
            }
            else if (!_options.Any(item => item.Id.Equals(correctOptionId)))
                throw new ArgumentException("Correct option must belong to the activity.");
            if (_hints.Length == 0 || _hints.Any(item => string.IsNullOrWhiteSpace(item.Entry))) throw new ArgumentException("Activity requires localized hints.");
            if (!rewardId.IsValid) throw new ArgumentException("Activity reward is required.", nameof(rewardId));
            Id = id; TypeId = typeId; Title = title; Instruction = instruction; Success = success; TryAgain = tryAgain;
            CorrectOptionId = correctOptionId; HintPolicy = hintPolicy ?? throw new ArgumentNullException(nameof(hintPolicy));
            Resumable = resumable; RewardId = rewardId; Editorial = editorial ?? throw new ArgumentNullException(nameof(editorial));
            CorrectTagId = correctTagId; FactId = factId; FactCopy = factCopy;
            InstructionCueId = instructionCueId; FactCueId = factCueId; RetryCueId = retryCueId;
            PositiveReactionId = positiveReactionId; NeutralReactionId = neutralReactionId;
        }
        public ActivityId Id { get; }
        public LearningActivityTypeId TypeId { get; }
        public LocalizedKey Title { get; }
        public LocalizedKey Instruction { get; }
        public LocalizedKey Success { get; }
        public LocalizedKey TryAgain { get; }
        public IReadOnlyList<LearningConceptId> Concepts => _concepts;
        public IReadOnlyList<LearningOptionDefinition> Options => _options;
        public LearningOptionId CorrectOptionId { get; }
        public TagId CorrectTagId { get; }
        public IReadOnlyList<LocalizedKey> Hints => _hints;
        public HintPolicy HintPolicy { get; }
        public bool Resumable { get; }
        public RewardId RewardId { get; }
        public EditorialMetadata Editorial { get; }
        public EducationalFactId FactId { get; }
        public LocalizedKey FactCopy { get; }
        public AudioCueId InstructionCueId { get; }
        public AudioCueId FactCueId { get; }
        public AudioCueId RetryCueId { get; }
        public LearningReactionId PositiveReactionId { get; }
        public LearningReactionId NeutralReactionId { get; }
    }
}
