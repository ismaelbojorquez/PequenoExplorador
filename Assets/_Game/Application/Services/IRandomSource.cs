namespace PequenoExplorador.Application.Services
{
    public interface IRandomSource
    {
        int Next(int maxExclusive);
    }
}
