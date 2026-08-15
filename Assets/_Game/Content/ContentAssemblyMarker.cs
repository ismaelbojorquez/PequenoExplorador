using System;
using PequenoExplorador.Application;

namespace PequenoExplorador.Content
{
    /// <summary>
    /// Identifies the future authoring adapter without creating content APIs.
    /// </summary>
    public static class ContentAssemblyMarker
    {
        public static Type ApplicationDependency => typeof(ApplicationAssemblyMarker);
    }
}
