using System;
using System.Linq;
using System.Threading;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Discovery;
using PequenoExplorador.Application.Economy;
using PequenoExplorador.Application.Input;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Missions;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Application.Services;
using PequenoExplorador.Domain.Content;
using PequenoExplorador.Presentation.Explorer;
using UnityEngine;

namespace PequenoExplorador.Presentation.Photography
{
    [DisallowMultipleComponent]
    public sealed class PhotographySceneRoot : MonoBehaviour
    {
        public const string RuntimeRootName = "PH_PHOTOGRAPHY_RUNTIME";
        [SerializeField] private PhotographableView[] _targets = Array.Empty<PhotographableView>();
        [SerializeField, Range(64, 512)] private int _thumbnailWidth = UnityPhotoThumbnailRenderer.DefaultWidth;
        [SerializeField, Range(64, 512)] private int _thumbnailHeight = UnityPhotoThumbnailRenderer.DefaultHeight;
        [SerializeField] private RenderTextureFormat _thumbnailFormat = RenderTextureFormat.ARGB32;
        private PhotographyInteractionAction _entryAction;
        private IInputService _input;
        private IClock _clock;
        private IAudioService _audio;
        private ExplorerLocomotionRoot _explorer;
        private PhotographyView _view;
        private CapturePhotoUseCase _capture;
        private readonly PhotoTargetEvaluator _evaluator = new PhotoTargetEvaluator();
        private CancellationTokenSource _lifetime;
        private DiscoveryId _pending;
        private PhotographableView _active;
        private int _captureSequence;
        private bool _captureRequestActive;
        public bool IsActive => _active != null;
        public PhotoEvaluation LastEvaluation { get; private set; }
        public PhotoCaptureResult LastCapture { get; private set; }
        public PhotographableView ActiveTarget => _active;
        public int TargetCount => ResolveTargets().Length;
        public int CaptureAttemptCount { get; private set; }
        public int ThumbnailWidth => _thumbnailWidth;
        public int ThumbnailHeight => _thumbnailHeight;
        public RenderTextureFormat ThumbnailFormat => _thumbnailFormat;

        public void Bind(PhotographyInteractionAction entryAction, IInputService input, IClock clock, IAudioService audio,
            ExplorerLocomotionRoot explorer, Camera camera, IPhotoStore store, IPhotoProgressRepository photos,
            DiscoverUseCase discoveries, IRewardCatalog rewards, GrantRewardUseCase grantRewards,
            IMissionFactSink missionFacts, ILocalizationService localization, PhotographyView view, bool reduceMotion)
        {
            Unbind();
            _entryAction = entryAction ?? throw new ArgumentNullException(nameof(entryAction));
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _audio = audio ?? throw new ArgumentNullException(nameof(audio));
            _explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _targets = ResolveTargets();
            if (_targets == null || _targets.Length == 0 || _targets.Any(item => item == null))
                throw new InvalidOperationException("Photography scene root requires photographable targets.");
            foreach (PhotographableView target in _targets) target.Bind(camera);
            _capture = new CapturePhotoUseCase(_evaluator,
                new UnityPhotoThumbnailRenderer(camera, _thumbnailWidth, _thumbnailHeight, _thumbnailFormat),
                store, photos, discoveries, rewards, grantRewards, missionFacts);
            _lifetime = new CancellationTokenSource();
            _entryAction.Requested += HandleRequested;
            _view.Bind(localization, reduceMotion);
            _view.ShutterRequested += CaptureRequested;
            _view.ExitRequested += ExitRequested;
        }

        public void SetReduceMotion(bool enabled) { _explorer?.SetReduceMotion(enabled); _view?.SetReduceMotion(enabled); }
        private void Update()
        {
            if (_pending.IsValid) { DiscoveryId requested = _pending; _pending = default; Begin(requested); }
            if (_active == null) return;
            if (!_active.IsAlive) { End(false); return; }
            if (_input.CurrentMap != InputMapId.Photography) return;
            LastEvaluation = _evaluator.Evaluate(_active.Target, _active.Sample());
            _view.UpdateEvaluation(LastEvaluation);
        }
        private void HandleRequested(DiscoveryId discoveryId) => _pending = discoveryId;
        public bool Begin(DiscoveryId discoveryId)
        {
            PhotographableView target = _targets.SingleOrDefault(item => item.Target.DiscoveryId == discoveryId);
            if (target == null) return false;
            _active = target;
            _explorer.CancelMovement();
            _explorer.SetPhotographyFocus(target.PhotoAnchor);
            _input.SetMap(InputMapId.Photography);
            LastEvaluation = _evaluator.Evaluate(target.Target, target.Sample());
            _view.Show(LastEvaluation);
            return true;
        }
        private async void CaptureRequested()
        {
            if (_active == null || _capture == null || _captureRequestActive) return;
            _captureRequestActive = true;
            _view.SetCaptureBusy(true);
            CaptureAttemptCount++;
            try
            {
                string id = string.Concat(_clock.UtcNow.UtcTicks.ToString(System.Globalization.CultureInfo.InvariantCulture), "-",
                    (++_captureSequence).ToString(System.Globalization.CultureInfo.InvariantCulture));
                LastCapture = await _capture.ExecuteAsync(_active, id, _lifetime.Token);
                if (!this || _view == null) return;
                _view.ShowCapture(LastCapture);
                _audio.Play(LastCapture.ProgressCaptured ? AudioCueIds.ConfirmFeedback : AudioCueIds.RetryFeedback);
            }
            finally
            {
                _captureRequestActive = false;
                if (this && _view != null) _view.SetCaptureBusy(false);
            }
        }
        private void ExitRequested() => End(true);
        private void End(bool restoreExplorerMap)
        {
            _active = null; _pending = default;
            _explorer?.SetPhotographyFocus(null);
            _view?.Hide();
            if (restoreExplorerMap && _input != null) _input.SetMap(InputMapId.Explorer);
        }
        public void Unbind()
        {
            End(false);
            if (_entryAction != null) _entryAction.Requested -= HandleRequested;
            if (_view != null)
            {
                _view.ShutterRequested -= CaptureRequested;
                _view.ExitRequested -= ExitRequested;
                _view.Unbind();
            }
            _lifetime?.Cancel(); _lifetime?.Dispose(); _lifetime = null;
            _captureRequestActive = false;
            foreach (PhotographableView target in _targets ?? Array.Empty<PhotographableView>()) target?.Unbind();
            _entryAction = null; _input = null; _clock = null; _audio = null; _explorer = null; _view = null; _capture = null;
        }
        private PhotographableView[] ResolveTargets()
        {
            PhotographableView[] configured = (_targets ?? Array.Empty<PhotographableView>())
                .Where(item => item != null).ToArray();
            if (configured.Length > 0) return configured;
            if (!gameObject.scene.IsValid() || !gameObject.scene.isLoaded) return Array.Empty<PhotographableView>();
            return gameObject.scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<PhotographableView>(true))
                .Where(item => item != null)
                .ToArray();
        }
        private void OnDisable() => Unbind();
        private void OnDestroy() => Unbind();
    }
}
