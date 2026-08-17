using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Content
{
    public interface IActivityDefinition
    {
        ActivityId Id { get; }
        LearningActivityTypeId TypeId { get; }
        EditorialMetadata Editorial { get; }
    }
}
