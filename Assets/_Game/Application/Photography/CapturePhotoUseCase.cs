using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Domain.Economy;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Application.Photography
{
    public sealed class CapturePhotoUseCase
    {
        private readonly PhotoTargetEvaluator _evaluator;
        private readonly IPhotoThumbnailRenderer _renderer;
        private readonly IPhotoStore _store;
        private readonly IPhotoProgressRepository _photos;
        private readonly DiscoverUseCase _discover;
        private readonly IRewardCatalog _rewards;
        private readonly GrantRewardUseCase _grantRewards;
        private int _captureInProgress;

        public CapturePhotoUseCase(PhotoTargetEvaluator evaluator, IPhotoThumbnailRenderer renderer, IPhotoStore store,
            IPhotoProgressRepository photos, DiscoverUseCase discover,
            IRewardCatalog rewards = null, GrantRewardUseCase grantRewards = null)
        {
            _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _photos = photos ?? throw new ArgumentNullException(nameof(photos));
            _discover = discover ?? throw new ArgumentNullException(nameof(discover));
            _rewards = rewards;
            _grantRewards = grantRewards;
        }

        public async Task<PhotoCaptureResult> ExecuteAsync(IPhotographable photographable, string captureId, CancellationToken cancellationToken)
        {
            if (photographable == null || !photographable.IsAlive || photographable.Target == null)
                return new PhotoCaptureResult(PhotoCaptureOutcome.Unavailable, default, default, null);
            if (!IsSafeCaptureId(captureId)) throw new ArgumentException("Capture ID is invalid.", nameof(captureId));
            PhotoEvaluation evaluation = _evaluator.Evaluate(photographable.Target, photographable.Sample());
            if (!evaluation.IsReady) return new PhotoCaptureResult(PhotoCaptureOutcome.NotReady, evaluation, default, null);
            if (Interlocked.CompareExchange(ref _captureInProgress, 1, 0) != 0)
                return new PhotoCaptureResult(PhotoCaptureOutcome.Busy, evaluation, default, null);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                DiscoveryId discoveryId = photographable.Target.DiscoveryId;
                DiscoverResult discovery = _discover.Execute(discoveryId, DiscoveryGrantId.Parse(
                    string.Concat("grant.photo.", captureId, ".", discoveryId.Value)));
                GrantRewardResult reward = GrantDiscoveryReward(discoveryId);
                PhotoProgress existing = _photos.Current.Photos.FirstOrDefault(item => item.DiscoveryId == discoveryId);
                if (existing != null && existing.ScorePermille >= evaluation.ScorePermille)
                    return new PhotoCaptureResult(PhotoCaptureOutcome.ExistingPhotoKept, evaluation, discovery, existing, reward);

                try
                {
                    PhotoThumbnail thumbnail = await _renderer.CaptureAsync(cancellationToken);
                    PhotoStoreResult stored = await _store.SaveAsync(discoveryId, evaluation.ScorePermille, thumbnail, cancellationToken);
                    var photo = new PhotoProgress(discoveryId, stored.FileReference, evaluation.ScorePermille,
                        thumbnail.Width, thumbnail.Height, stored.ByteLength);
                    var updated = new List<PhotoProgress>(_photos.Current.Photos.Where(item => item.DiscoveryId != discoveryId)) { photo };
                    if (!_photos.IsReadOnly) _photos.Commit(_photos.Current.WithPhotos(updated));
                    return new PhotoCaptureResult(
                        discovery.Outcome == DiscoverOutcome.First ? PhotoCaptureOutcome.CapturedNew : PhotoCaptureOutcome.CapturedRepeated,
                        evaluation, discovery, photo, reward);
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception)
                {
                    return new PhotoCaptureResult(PhotoCaptureOutcome.CapturedWithoutThumbnail, evaluation, discovery, existing, reward);
                }
            }
            catch (OperationCanceledException)
            {
                return new PhotoCaptureResult(PhotoCaptureOutcome.Cancelled, evaluation, default, null);
            }
            finally
            {
                Volatile.Write(ref _captureInProgress, 0);
            }
        }

        private GrantRewardResult GrantDiscoveryReward(DiscoveryId discoveryId)
        {
            if (_rewards == null || _grantRewards == null || !_rewards.TryGetDiscoveryReward(discoveryId, out RewardDefinition definition))
                return default;
            return _grantRewards.Execute(definition.Id,
                EconomyTransactionId.Parse("economy-tx.discovery." + discoveryId.Value),
                RewardSourceKind.Discovery,
                discoveryId.Value);
        }

        private static bool IsSafeCaptureId(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 80) return false;
            foreach (char character in value)
                if (!(character >= 'a' && character <= 'z') && !(character >= '0' && character <= '9') && character != '-') return false;
            return true;
        }
    }
}
