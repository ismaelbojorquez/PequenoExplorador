using System;

namespace PequenoExplorador.Application.Explorer
{
    public sealed class ExplorerLocomotionSettings
    {
        public ExplorerLocomotionSettings(float stoppingDistance, float arrivalSpeed)
        {
            if (float.IsNaN(stoppingDistance) || stoppingDistance <= 0f)
                throw new ArgumentOutOfRangeException(nameof(stoppingDistance));
            if (float.IsNaN(arrivalSpeed) || arrivalSpeed < 0f)
                throw new ArgumentOutOfRangeException(nameof(arrivalSpeed));
            StoppingDistance = stoppingDistance;
            ArrivalSpeed = arrivalSpeed;
        }

        public float StoppingDistance { get; }
        public float ArrivalSpeed { get; }
    }
}
