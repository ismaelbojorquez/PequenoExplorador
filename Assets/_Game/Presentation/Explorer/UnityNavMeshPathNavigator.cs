using PequenoExplorador.Application.Explorer;
using UnityEngine;
using UnityEngine.AI;

namespace PequenoExplorador.Presentation.Explorer
{
    internal sealed class UnityNavMeshPathNavigator : IPathNavigator
    {
        private readonly NavMeshAgent _agent;

        public UnityNavMeshPathNavigator(NavMeshAgent agent)
        {
            _agent = agent;
        }

        public bool IsAvailable => _agent != null && _agent.enabled && _agent.isOnNavMesh;
        public bool IsPathPending => IsAvailable && _agent.pathPending;
        public bool HasPath => IsAvailable && _agent.hasPath;
        public float RemainingDistance => IsAvailable ? _agent.remainingDistance : float.PositiveInfinity;
        public float Speed => IsAvailable ? _agent.velocity.magnitude : 0f;

        public bool TrySetDestination(WorldPosition destination)
        {
            if (!IsAvailable) return false;
            return _agent.SetDestination(new Vector3(destination.X, destination.Y, destination.Z));
        }

        public void Stop()
        {
            if (!IsAvailable) return;
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
        }
    }
}
