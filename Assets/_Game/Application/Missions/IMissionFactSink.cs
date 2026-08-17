namespace PequenoExplorador.Application.Missions
{
    public interface IMissionFactSink
    {
        MissionFactResult Record(GameplayFact fact);
    }
}
