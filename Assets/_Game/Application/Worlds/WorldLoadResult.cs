using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Application.Worlds
{
    public enum WorldLoadOutcome
    {
        Succeeded,
        AlreadyThere,
        Unavailable,
        Missing,
        Busy,
        Canceled,
        Failed
    }

    public sealed class WorldLoadResult
    {
        public WorldLoadResult(WorldLoadOutcome outcome, WorldId worldId, string errorCode = "", WorldManifest manifest = null)
        {
            Outcome = outcome;
            WorldId = worldId;
            ErrorCode = errorCode ?? string.Empty;
            Manifest = manifest;
        }
        public WorldLoadOutcome Outcome { get; }
        public WorldId WorldId { get; }
        public string ErrorCode { get; }
        public WorldManifest Manifest { get; }
        public bool IsSuccess => Outcome == WorldLoadOutcome.Succeeded || Outcome == WorldLoadOutcome.AlreadyThere;
    }
}
