using System;

namespace PequenoExplorador.Application.Explorer
{
    public sealed class ExplorerLocomotionController
    {
        private readonly IPathNavigator _navigator;
        private readonly ExplorerLocomotionSettings _settings;
        private bool _suspended;

        public ExplorerLocomotionController(
            IPathNavigator navigator,
            ExplorerLocomotionSettings settings)
        {
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            State = ExplorerLocomotionState.Idle;
        }

        public event Action<ExplorerLocomotionState> Changed;
        public ExplorerLocomotionState State { get; private set; }

        public bool MoveTo(WorldPosition destination)
        {
            if (_suspended || !_navigator.IsAvailable)
            {
                SetState(_suspended
                    ? ExplorerLocomotionState.Suspended
                    : ExplorerLocomotionState.InvalidDestination);
                return false;
            }

            if (!_navigator.TrySetDestination(destination))
            {
                SetState(ExplorerLocomotionState.InvalidDestination);
                return false;
            }

            SetState(ExplorerLocomotionState.PathPending);
            return true;
        }

        public void Cancel()
        {
            _navigator.Stop();
            SetState(_suspended ? ExplorerLocomotionState.Suspended : ExplorerLocomotionState.Idle);
        }

        public void RejectDestination()
        {
            if (_suspended) return;
            SetState(ExplorerLocomotionState.InvalidDestination);
        }

        public void SetSuspended(bool suspended)
        {
            if (_suspended == suspended) return;
            _suspended = suspended;
            _navigator.Stop();
            SetState(suspended ? ExplorerLocomotionState.Suspended : ExplorerLocomotionState.Idle);
        }

        public void Tick()
        {
            if (_suspended || State == ExplorerLocomotionState.InvalidDestination ||
                State == ExplorerLocomotionState.Idle || State == ExplorerLocomotionState.Arrived)
                return;

            if (!_navigator.IsAvailable)
            {
                _navigator.Stop();
                SetState(ExplorerLocomotionState.InvalidDestination);
                return;
            }

            if (_navigator.IsPathPending)
            {
                SetState(ExplorerLocomotionState.PathPending);
                return;
            }

            if (_navigator.HasPath &&
                (_navigator.RemainingDistance > _settings.StoppingDistance ||
                 _navigator.Speed > _settings.ArrivalSpeed))
            {
                SetState(ExplorerLocomotionState.Moving);
                return;
            }

            _navigator.Stop();
            SetState(ExplorerLocomotionState.Arrived);
        }

        private void SetState(ExplorerLocomotionState value)
        {
            if (State == value) return;
            State = value;
            Changed?.Invoke(value);
        }
    }
}
