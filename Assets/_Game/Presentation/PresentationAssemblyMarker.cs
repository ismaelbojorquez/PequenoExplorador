using System;
using PequenoExplorador.Application;

namespace PequenoExplorador.Presentation
{
    /// <summary>
    /// Identifies the Unity-facing adapter without implementing UI or gameplay.
    /// </summary>
    public static class PresentationAssemblyMarker
    {
        public static Type ApplicationDependency => typeof(ApplicationAssemblyMarker);
    }
}
