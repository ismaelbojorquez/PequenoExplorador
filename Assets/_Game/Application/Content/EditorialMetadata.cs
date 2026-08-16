using System;

namespace PequenoExplorador.Application.Content
{
    public sealed class EditorialMetadata
    {
        public EditorialMetadata(EditorialState state, bool isPlaceholder, string owner, string developmentWatermark)
        {
            if (string.IsNullOrWhiteSpace(owner)) throw new ArgumentException("Editorial owner is required.", nameof(owner));
            if ((state == EditorialState.Draft || isPlaceholder) && string.IsNullOrWhiteSpace(developmentWatermark))
                throw new ArgumentException("Draft and placeholder content requires a Development watermark.", nameof(developmentWatermark));
            State = state;
            IsPlaceholder = isPlaceholder;
            Owner = owner;
            DevelopmentWatermark = developmentWatermark ?? string.Empty;
        }

        public EditorialState State { get; }
        public bool IsPlaceholder { get; }
        public string Owner { get; }
        public string DevelopmentWatermark { get; }
        public bool IsReleaseApproved => State == EditorialState.Approved && !IsPlaceholder;
    }
}
