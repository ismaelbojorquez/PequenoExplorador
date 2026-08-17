using System;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Learning
{
    public sealed class LearningOptionDefinition
    {
        public LearningOptionDefinition(LearningOptionId id, LocalizedKey label)
        {
            if (!id.IsValid) throw new ArgumentException("Option ID is invalid.", nameof(id));
            if (string.IsNullOrWhiteSpace(label.Table) || string.IsNullOrWhiteSpace(label.Entry)) throw new ArgumentException("Option label is invalid.", nameof(label));
            Id = id; Label = label;
        }
        public LearningOptionId Id { get; }
        public LocalizedKey Label { get; }
    }
}
