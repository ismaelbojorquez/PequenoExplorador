using System.Collections;
using PequenoExplorador.Domain.Content;
using UnityEngine;

namespace PequenoExplorador.Presentation.Learning
{
    [DisallowMultipleComponent]
    public sealed class AnimalLearningReactionView : MonoBehaviour
    {
        [SerializeField] private Transform _visual;
        [SerializeField, Range(0.05f, 0.5f)] private float _duration = 0.28f;
        [SerializeField, Range(0.02f, 0.4f)] private float _positiveHop = 0.14f;
        [SerializeField, Range(1f, 20f)] private float _neutralTurnDegrees = 7f;
        private LearningActivityView _source;
        private Coroutine _animation;
        private Vector3 _basePosition;
        private Quaternion _baseRotation;

        public LearningReactionId LastReaction { get; private set; }
        public int ReactionCount { get; private set; }
        public bool LastUsedReducedMotion { get; private set; }

        public void Bind(LearningActivityView source)
        {
            Unbind();
            _source = source;
            if (_source == null || _visual == null)
                throw new System.InvalidOperationException("Learning animal reaction requires source and visual.");
            _basePosition = _visual.localPosition;
            _baseRotation = _visual.localRotation;
            _source.ReactionRequested += Play;
        }

        public void ConfigureForEditorAndTests(Transform visual) => _visual = visual;

        public void Play(LearningReactionId reactionId, bool reduceMotion)
        {
            if (!reactionId.IsValid || _visual == null) return;
            LastReaction = reactionId;
            LastUsedReducedMotion = reduceMotion;
            ReactionCount++;
            if (_animation != null) StopCoroutine(_animation);
            Restore();
            if (!reduceMotion) _animation = StartCoroutine(Animate(reactionId));
        }

        private IEnumerator Animate(LearningReactionId reactionId)
        {
            bool positive = reactionId.Value.EndsWith(".positive", System.StringComparison.Ordinal);
            float elapsed = 0f;
            while (elapsed < _duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float phase = Mathf.Clamp01(elapsed / _duration);
                float wave = Mathf.Sin(phase * Mathf.PI);
                if (positive) _visual.localPosition = _basePosition + Vector3.up * (_positiveHop * wave);
                else _visual.localRotation = _baseRotation * Quaternion.Euler(0f, _neutralTurnDegrees * wave, 0f);
                yield return null;
            }
            Restore();
            _animation = null;
        }

        private void Restore()
        {
            if (_visual == null) return;
            _visual.localPosition = _basePosition;
            _visual.localRotation = _baseRotation;
        }

        public void Unbind()
        {
            if (_source != null) _source.ReactionRequested -= Play;
            if (_animation != null) StopCoroutine(_animation);
            _animation = null;
            Restore();
            _source = null;
        }

        private void OnDisable() => Unbind();
        private void OnDestroy() => Unbind();
    }
}
