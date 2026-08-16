using System;

namespace PequenoExplorador.Application.Input
{
    public sealed class GestureRecognizer
    {
        private const int MaximumPointers = 5;
        private readonly PointerState[] _pointers = new PointerState[MaximumPointers];
        private readonly GestureThresholds _thresholds;
        private InputMapId _map;

        public GestureRecognizer(GestureThresholds thresholds, InputMapId initialMap = InputMapId.UI)
        {
            _thresholds = thresholds ?? throw new ArgumentNullException(nameof(thresholds));
            _map = initialMap;
        }

        public event Action<InputIntent> IntentRaised;
        public int ActivePointerCount { get; private set; }

        public void SetMap(InputMapId map)
        {
            if (_map == map) return;
            CancelAll();
            _map = map;
        }

        public void BeginPointer(int pointerId, ScreenPoint position, double time)
        {
            if (Find(pointerId) >= 0) return;
            int slot = FindFree();
            if (slot < 0) return;

            bool hasAnotherPointer = ActivePointerCount > 0;
            if (hasAnotherPointer)
            {
                for (int index = 0; index < _pointers.Length; index++)
                {
                    if (_pointers[index].Active) _pointers[index].SuppressTap = true;
                }
            }

            _pointers[slot] = new PointerState(pointerId, position, time, hasAnotherPointer);
            ActivePointerCount++;
        }

        public void MovePointer(int pointerId, ScreenPoint position, double time)
        {
            int index = Find(pointerId);
            if (index < 0) return;

            PointerState state = _pointers[index];
            ScreenPoint previous = state.Current;
            state.Current = position;
            InputMapPolicy policy = InputMapPolicy.For(_map);
            float movement = state.Start.DistanceTo(position);
            if (!state.Dragging && policy.AllowDrag && movement >= _thresholds.DragStartPixels)
            {
                state.Dragging = true;
                Raise(InputGestureKind.DragStarted, state, position - previous);
            }
            else if (state.Dragging)
            {
                Raise(InputGestureKind.Dragged, state, position - previous);
            }

            _pointers[index] = state;
            EmitPinchIfNeeded(index, previous, position, policy);
        }

        public void AdvanceTime(double time)
        {
            InputMapPolicy policy = InputMapPolicy.For(_map);
            if (!policy.AllowHold) return;
            for (int index = 0; index < _pointers.Length; index++)
            {
                PointerState state = _pointers[index];
                if (!state.Active || state.HoldSent || state.Dragging || state.SuppressTap) continue;
                if (time - state.StartTime < _thresholds.HoldMinimumSeconds) continue;
                if (state.Start.DistanceTo(state.Current) > _thresholds.TapMovementPixels) continue;
                state.HoldSent = true;
                _pointers[index] = state;
                Raise(InputGestureKind.PressAndHold, state, default);
            }
        }

        public void EndPointer(int pointerId, ScreenPoint position, double time)
        {
            int index = Find(pointerId);
            if (index < 0) return;
            PointerState state = _pointers[index];
            state.Current = position;
            InputMapPolicy policy = InputMapPolicy.For(_map);
            if (state.Dragging)
            {
                Raise(InputGestureKind.DragEnded, state, default);
            }
            else if (!state.SuppressTap && !state.HoldSent && policy.AllowTap &&
                     time - state.StartTime <= _thresholds.TapMaximumSeconds &&
                     state.Start.DistanceTo(position) <= _thresholds.TapMovementPixels)
            {
                Raise(InputGestureKind.Tap, state, default);
            }
            else if (!state.SuppressTap && !state.HoldSent && policy.AllowHold &&
                     time - state.StartTime >= _thresholds.HoldMinimumSeconds &&
                     state.Start.DistanceTo(position) <= _thresholds.TapMovementPixels)
            {
                Raise(InputGestureKind.PressAndHold, state, default);
            }

            _pointers[index] = default;
            ActivePointerCount--;
        }

        public void CancelPointer(int pointerId)
        {
            int index = Find(pointerId);
            if (index < 0) return;
            PointerState state = _pointers[index];
            Raise(InputGestureKind.Cancelled, state, default);
            _pointers[index] = default;
            ActivePointerCount--;
        }

        public void CancelAll()
        {
            for (int index = 0; index < _pointers.Length; index++)
            {
                if (!_pointers[index].Active) continue;
                Raise(InputGestureKind.Cancelled, _pointers[index], default);
                _pointers[index] = default;
            }
            ActivePointerCount = 0;
        }

        private void EmitPinchIfNeeded(int movedIndex, ScreenPoint previous, ScreenPoint current, InputMapPolicy policy)
        {
            if (!policy.AllowPinch || ActivePointerCount != 2) return;
            int otherIndex = -1;
            for (int index = 0; index < _pointers.Length; index++)
            {
                if (index != movedIndex && _pointers[index].Active) { otherIndex = index; break; }
            }
            if (otherIndex < 0) return;
            ScreenPoint other = _pointers[otherIndex].Current;
            float delta = other.DistanceTo(current) - other.DistanceTo(previous);
            if (Math.Abs(delta) < _thresholds.PinchDeltaPixels) return;
            Raise(InputGestureKind.Pinch, _pointers[movedIndex], default, delta);
        }

        private int Find(int pointerId)
        {
            for (int index = 0; index < _pointers.Length; index++)
                if (_pointers[index].Active && _pointers[index].PointerId == pointerId) return index;
            return -1;
        }

        private int FindFree()
        {
            for (int index = 0; index < _pointers.Length; index++) if (!_pointers[index].Active) return index;
            return -1;
        }

        private void Raise(InputGestureKind kind, PointerState state, ScreenPoint delta, float value = 0f) =>
            IntentRaised?.Invoke(new InputIntent(_map, kind, state.PointerId, state.Current, delta, value));

        private struct PointerState
        {
            public PointerState(int pointerId, ScreenPoint position, double startTime, bool suppressTap)
            {
                Active = true;
                PointerId = pointerId;
                Start = position;
                Current = position;
                StartTime = startTime;
                SuppressTap = suppressTap;
                HoldSent = false;
                Dragging = false;
            }

            public bool Active;
            public int PointerId;
            public ScreenPoint Start;
            public ScreenPoint Current;
            public double StartTime;
            public bool SuppressTap;
            public bool HoldSent;
            public bool Dragging;
        }
    }
}
