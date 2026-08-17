using System;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Camp
{
    public sealed class CampStationDefinition
    {
        public CampStationDefinition(
            CampStationId id,
            CampStationActionId actionId,
            LocalizedKey displayName,
            LocalizedKey description,
            int displayOrder,
            bool available,
            bool parentRestricted)
        {
            if (!id.IsValid || !actionId.IsValid) throw new ArgumentException("Camp station IDs must be valid.");
            if (displayOrder < 0) throw new ArgumentOutOfRangeException(nameof(displayOrder));
            Id = id;
            ActionId = actionId;
            DisplayName = displayName;
            Description = description;
            DisplayOrder = displayOrder;
            IsAvailable = available;
            IsParentRestricted = parentRestricted;
        }

        public CampStationId Id { get; }
        public CampStationActionId ActionId { get; }
        public LocalizedKey DisplayName { get; }
        public LocalizedKey Description { get; }
        public int DisplayOrder { get; }
        public bool IsAvailable { get; }
        public bool IsParentRestricted { get; }
    }
}
