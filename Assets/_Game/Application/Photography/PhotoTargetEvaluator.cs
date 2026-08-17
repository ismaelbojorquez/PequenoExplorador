using System;

namespace PequenoExplorador.Application.Photography
{
    public sealed class PhotoTargetEvaluator
    {
        public PhotoEvaluation Evaluate(PhotoTarget target, PhotoFrameSample sample)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            PhotoEvaluationSettings settings = target.Settings;
            bool closeEnough = sample.Distance <= settings.MaximumDistance;
            bool covered = sample.ViewportCoverage >= settings.MinimumCoverage;
            bool centered = sample.CenterOffset <= settings.MaximumCenterOffset;
            bool oriented = sample.OrientationAlignment >= settings.MinimumOrientationAlignment;
            bool ready = closeEnough && covered && centered && oriented && sample.HasLineOfSight;

            float coverage = Clamp01(sample.ViewportCoverage / Math.Max(settings.MinimumCoverage * 2f, 0.01f));
            float distance = Clamp01(1f - sample.Distance / settings.MaximumDistance);
            float center = Clamp01(1f - sample.CenterOffset / Math.Max(settings.MaximumCenterOffset, 0.01f));
            float score = sample.HasLineOfSight
                ? coverage * 0.35f + distance * 0.20f + center * 0.30f + sample.OrientationAlignment * 0.15f
                : 0f;
            PhotoGuidance guidance = !closeEnough || !covered
                ? PhotoGuidance.MoveCloser
                : ready ? PhotoGuidance.Ready : PhotoGuidance.CenterTarget;
            return new PhotoEvaluation(ready, guidance, (int)Math.Round(Clamp01(score) * 1000f));
        }

        private static float Clamp01(float value) => value < 0f ? 0f : value > 1f ? 1f : value;
    }
}
