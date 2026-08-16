using System.Collections.Generic;

namespace PequenoExplorador.Application.Configuration
{
    public interface IFeatureFlags
    {
        IReadOnlyList<FeatureFlag> Enabled { get; }
        bool IsEnabled(FeatureFlag flag);
    }
}
