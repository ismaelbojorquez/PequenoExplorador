using UnityEngine;

namespace PequenoExplorador.Bootstrap
{
    /// <summary>
    /// Marker for the temporary foundation scene. It intentionally contains no gameplay.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DiagnosticBootstrap : MonoBehaviour
    {
        public const string ProductName = "Pequeño Explorador: Aprende Jugando";
        public const string DevelopmentVersion = "0.1.0-dev";
        public const string PlaceholderObjectName = "PH_UI_DIAGNOSTIC";
    }
}
