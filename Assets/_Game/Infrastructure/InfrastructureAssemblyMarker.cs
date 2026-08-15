using System;
using PequenoExplorador.Application;

namespace PequenoExplorador.Infrastructure
{
    /// <summary>
    /// Identifies the future platform adapter without implementing a service.
    /// </summary>
    public static class InfrastructureAssemblyMarker
    {
        public static Type ApplicationDependency => typeof(ApplicationAssemblyMarker);
    }
}
