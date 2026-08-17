using System;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Domain.Content;
using UnityEngine;

namespace PequenoExplorador.Presentation.Photography
{
    [DisallowMultipleComponent]
    public sealed class PhotographableView : MonoBehaviour, IPhotographable
    {
        [SerializeField] private string _discoveryId;
        [SerializeField] private Transform _photoAnchor;
        [SerializeField] private Transform _facingTransform;
        [SerializeField] private Bounds _candidateLocalBounds;
        [SerializeField, Range(0.01f, 0.5f)] private float _minimumCoverage = 0.08f;
        [SerializeField, Range(1f, 20f)] private float _maximumDistance = 10f;
        [SerializeField, Range(0.05f, 1f)] private float _maximumCenterOffset = 0.36f;
        [SerializeField, Range(0f, 1f)] private float _minimumOrientationAlignment = 0.35f;
        private Camera _camera;
        private bool _destroyed;
#if UNITY_EDITOR
        private bool _hasSampleOverride;
        private PhotoFrameSample _sampleOverride;
#endif
        public PhotoTarget Target { get; private set; }
        public bool IsAlive => !_destroyed && this != null && gameObject != null;
        public string RawDiscoveryId => _discoveryId;
        public Transform PhotoAnchor => _photoAnchor;
        public Bounds CandidateLocalBounds => _candidateLocalBounds;

        public void Bind(Camera worldCamera)
        {
            _camera = worldCamera != null ? worldCamera : throw new ArgumentNullException(nameof(worldCamera));
            if (_photoAnchor == null || _facingTransform == null || _candidateLocalBounds.size.sqrMagnitude <= 0.01f)
                throw new InvalidOperationException("Photographable target requires anchor, facing transform and reviewed bounds.");
            Target = new PhotoTarget(DiscoveryId.Parse(_discoveryId), new PhotoEvaluationSettings(
                _minimumCoverage, _maximumDistance, _maximumCenterOffset, _minimumOrientationAlignment));
        }

        public void Unbind() { _camera = null; Target = null; }

        public PhotoFrameSample Sample()
        {
#if UNITY_EDITOR
            if (_hasSampleOverride) return _sampleOverride;
#endif
            if (_camera == null || Target == null || !IsAlive) return new PhotoFrameSample(0f, 50f, false, 1f, 0f);
            Vector3 center = transform.TransformPoint(_candidateLocalBounds.center);
            Vector3 extents = _candidateLocalBounds.extents;
            float minX = 1f, minY = 1f, maxX = 0f, maxY = 0f;
            bool anyInFront = false;
            for (int index = 0; index < 8; index++)
            {
                Vector3 local = _candidateLocalBounds.center + new Vector3(
                    (index & 1) == 0 ? -extents.x : extents.x,
                    (index & 2) == 0 ? -extents.y : extents.y,
                    (index & 4) == 0 ? -extents.z : extents.z);
                Vector3 viewport = _camera.WorldToViewportPoint(transform.TransformPoint(local));
                if (viewport.z <= 0f) continue;
                anyInFront = true;
                minX = Mathf.Min(minX, viewport.x); minY = Mathf.Min(minY, viewport.y);
                maxX = Mathf.Max(maxX, viewport.x); maxY = Mathf.Max(maxY, viewport.y);
            }
            float coverage = anyInFront
                ? Mathf.Max(0f, Mathf.Min(1f, maxX) - Mathf.Max(0f, minX)) *
                  Mathf.Max(0f, Mathf.Min(1f, maxY) - Mathf.Max(0f, minY))
                : 0f;
            Vector2 rectCenter = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
            float centerOffset = anyInFront ? Mathf.Clamp01(Vector2.Distance(rectCenter, new Vector2(0.5f, 0.5f)) * 1.414214f) : 1f;
            Vector3 cameraToCenter = center - _camera.transform.position;
            float distance = cameraToCenter.magnitude;
            bool lineOfSight = distance > 0.01f && !Physics.Raycast(_camera.transform.position,
                cameraToCenter / distance, distance - 0.05f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Vector3 targetToCamera = distance > 0.01f ? -cameraToCenter / distance : _facingTransform.forward;
            float alignment = Mathf.Abs(Vector3.Dot(_facingTransform.forward.normalized, targetToCamera));
            return new PhotoFrameSample(Mathf.Clamp01(coverage), distance, lineOfSight, centerOffset, Mathf.Clamp01(alignment));
        }

#if UNITY_EDITOR
        public void SetSampleOverrideForEditorAndTests(PhotoFrameSample sample) { _sampleOverride = sample; _hasSampleOverride = true; }
        public void ClearSampleOverrideForEditorAndTests() => _hasSampleOverride = false;
#endif

        private void OnDestroy() { _destroyed = true; Unbind(); }
    }
}
