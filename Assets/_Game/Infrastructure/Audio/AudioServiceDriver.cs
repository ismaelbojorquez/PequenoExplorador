using System;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Audio
{
    [DisallowMultipleComponent]
    internal sealed class AudioServiceDriver : MonoBehaviour
    {
        public Action Tick;

        private void Update() => Tick?.Invoke();
        private void OnDestroy() => Tick = null;
    }
}
