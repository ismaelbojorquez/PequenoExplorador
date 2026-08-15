using System;
using PequenoExplorador.Application;
using PequenoExplorador.Content;
using PequenoExplorador.Infrastructure;
using PequenoExplorador.Presentation;

namespace PequenoExplorador.Bootstrap
{
    /// <summary>
    /// Compile-time proof of the only assembly allowed to compose concrete adapters.
    /// </summary>
    public static class BootstrapAssemblyMarker
    {
        public static Type ApplicationDependency => typeof(ApplicationAssemblyMarker);
        public static Type ContentDependency => typeof(ContentAssemblyMarker);
        public static Type InfrastructureDependency => typeof(InfrastructureAssemblyMarker);
        public static Type PresentationDependency => typeof(PresentationAssemblyMarker);
    }
}
