using UnityEngine;

namespace PequenoExplorador.Content.Worlds
{
    [DisallowMultipleComponent]
    public sealed class WorldSpawnPointMarker : MonoBehaviour
    {
        [SerializeField] private string _spawnPointId;
        public string SpawnPointId => _spawnPointId;
#if UNITY_EDITOR
        public void ConfigureForEditor(string id) => _spawnPointId = id;
#endif
    }
}
