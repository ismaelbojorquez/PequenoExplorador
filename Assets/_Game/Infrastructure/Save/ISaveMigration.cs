namespace PequenoExplorador.Infrastructure.Save
{
    public interface ISaveMigration
    {
        int FromVersion { get; }
        int ToVersion { get; }
        string Migrate(string sourcePayload);
    }
}
