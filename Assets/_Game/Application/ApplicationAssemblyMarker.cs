using System;
using PequenoExplorador.Domain;

namespace PequenoExplorador.Application
{
    /// <summary>
    /// Compile-time proof that Application can depend on Domain and nothing Unity-specific.
    /// </summary>
    public static class ApplicationAssemblyMarker
    {
        public static Type DomainDependency => typeof(DomainAssemblyMarker);
    }
}
