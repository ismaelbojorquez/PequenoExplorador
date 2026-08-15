namespace PequenoExplorador.Application.Logging
{
    public interface IAppLogger
    {
        void Write(AppLogEntry entry);
    }
}
