using System;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Learning
{
    public sealed class LearningOptionDefinition
    {
        public LearningOptionDefinition(LearningOptionId id, LocalizedKey label)
            : this(id, label, default, 255, 255, 255)
        {
        }

        public LearningOptionDefinition(LearningOptionId id, LocalizedKey label, TagId tagId,
            byte red, byte green, byte blue)
        {
            if (!id.IsValid) throw new ArgumentException("Option ID is invalid.", nameof(id));
            if (string.IsNullOrWhiteSpace(label.Table) || string.IsNullOrWhiteSpace(label.Entry)) throw new ArgumentException("Option label is invalid.", nameof(label));
            Id = id; Label = label; TagId = tagId; Red = red; Green = green; Blue = blue;
        }
        public LearningOptionId Id { get; }
        public LocalizedKey Label { get; }
        public TagId TagId { get; }
        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }
    }
}
