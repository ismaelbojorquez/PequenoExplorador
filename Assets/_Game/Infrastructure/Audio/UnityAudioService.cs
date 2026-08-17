using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Logging;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Domain.Progress;
using UnityEngine;
using UnityEngine.Audio;
using AudioSettingsModel = PequenoExplorador.Application.Audio.AudioSettings;

namespace PequenoExplorador.Infrastructure.Audio
{
    public sealed class UnityAudioService : IAudioService
    {
        private const int EffectPoolSize = 4;
        private const int VoiceQueueCapacity = 4;
        private const float DuckMultiplier = 0.35f;

        private readonly GameObject _host;
        private readonly IReadOnlyDictionary<AudioCueId, UnityAudioCue> _cues;
        private readonly ISaveService _save;
        private readonly AutosaveCoordinator _checkpoints;
        private readonly ILocalizationService _localization;
        private readonly IAppLogger _logger;
        private readonly AudioMixerGroup _musicGroup;
        private readonly AudioMixerGroup _ambienceGroup;
        private readonly AudioMixerGroup _effectsGroup;
        private readonly AudioMixerGroup _voiceGroup;
        private readonly VoiceQueueScheduler _voiceQueue = new VoiceQueueScheduler(VoiceQueueCapacity);
        private readonly AudioCooldownTracker _cooldowns;
        private readonly AudioSource[] _effects = new AudioSource[EffectPoolSize];
        private readonly AudioPriority[] _effectPriorities = new AudioPriority[EffectPoolSize];
        private AudioServiceDriver _driver;
        private AudioSource _music;
        private AudioSource _ambience;
        private AudioSource _voice;
        private UnityAudioCue _lastInstruction;
        private SubtitleModel _currentSubtitle;
        private bool _initialized;
        private bool _suspended;

        public UnityAudioService(
            GameObject host,
            IEnumerable<UnityAudioCue> cues,
            ISaveService save,
            ILocalizationService localization,
            IAppLogger logger,
            AudioMixerGroup musicGroup,
            AudioMixerGroup ambienceGroup,
            AudioMixerGroup effectsGroup,
            AudioMixerGroup voiceGroup,
            Func<double> now = null,
            AutosaveCoordinator checkpoints = null)
        {
            _host = host != null ? host : throw new ArgumentNullException(nameof(host));
            _save = save ?? throw new ArgumentNullException(nameof(save));
            _checkpoints = checkpoints;
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _musicGroup = musicGroup;
            _ambienceGroup = ambienceGroup;
            _effectsGroup = effectsGroup;
            _voiceGroup = voiceGroup;
            UnityAudioCue[] cueArray = (cues ?? throw new ArgumentNullException(nameof(cues))).ToArray();
            if (cueArray.Any(cue => cue == null) || cueArray.Select(cue => cue.Id).Distinct().Count() != cueArray.Length)
            {
                throw new ArgumentException("Audio cues must be non-null with unique IDs.", nameof(cues));
            }

            _cues = cueArray.ToDictionary(cue => cue.Id);
            _cooldowns = new AudioCooldownTracker(now ?? UnityNow);
            Settings = AudioSettingsModel.CreateDefault();
        }

        private static double UnityNow() => UnityEngine.Time.unscaledTimeAsDouble;

        public event Action<SubtitleModel> SubtitleChanged;

        public string ServiceId => "Audio";
        public AudioSettingsModel Settings { get; private set; }
        public bool IsVoiceDucking { get; private set; }
        public int QueuedVoiceCount => _voiceQueue.PendingCount;
        public int ActiveSourceCount => Sources().Count(source => source != null && source.isPlaying);

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (_initialized)
            {
                return Task.CompletedTask;
            }

            cancellationToken.ThrowIfCancellationRequested();
            PlayerPreferences preferences = _save.Current.Preferences;
            Settings = new AudioSettingsModel(
                preferences.MasterVolume,
                preferences.MusicVolume,
                preferences.AmbienceVolume,
                preferences.EffectsVolume,
                preferences.VoiceVolume,
                preferences.SubtitlesEnabled);
            CreateSources();
            ApplyVolumes();
            _localization.LocaleChanged += OnLocaleChanged;
            _driver.Tick += Tick;
            _initialized = true;
            return Task.CompletedTask;
        }

        public void Shutdown()
        {
            if (!_initialized)
            {
                return;
            }

            _localization.LocaleChanged -= OnLocaleChanged;
            if (_driver != null)
            {
                _driver.Tick -= Tick;
            }
            foreach (AudioSource source in Sources())
            {
                source?.Stop();
            }
            _voiceQueue.Clear();
            _cooldowns.Clear();
            _lastInstruction = null;
            SetDucking(false);
            PublishSubtitle(SubtitleModel.Hidden);
            SubtitleChanged = null;
            _initialized = false;
            _suspended = false;
        }

        public AudioPlayResult Play(AudioCueId cueId)
        {
            if (!_initialized)
            {
                return new AudioPlayResult(AudioPlayStatus.Disabled, "AudioNotInitialized");
            }

            if (_suspended)
            {
                return new AudioPlayResult(AudioPlayStatus.Suspended, "AudioSuspended");
            }

            if (!_cues.TryGetValue(cueId, out UnityAudioCue cue))
            {
                _logger.Write(new AppLogEntry(AppLogLevel.Warning, "Audio", "MissingCue", cueId.ToString()));
                return new AudioPlayResult(AudioPlayStatus.Missing, "AudioCueMissing");
            }

            AudioClip clip = cue.ClipFor(_localization.CurrentLocaleCode);
            if (clip == null)
            {
                _logger.Write(new AppLogEntry(AppLogLevel.Warning, "Audio", "MissingClip", cueId.ToString()));
                return new AudioPlayResult(AudioPlayStatus.Missing, "AudioClipMissing");
            }

            if (!_cooldowns.TryConsume(cue.Id, cue.CooldownSeconds))
            {
                return new AudioPlayResult(AudioPlayStatus.Cooldown, "AudioCueCooldown");
            }

            switch (cue.Category)
            {
                case AudioCueCategory.Music:
                    return PlayExclusive(_music, cue, clip, Settings.Music);
                case AudioCueCategory.Ambience:
                    return PlayExclusive(_ambience, cue, clip, Settings.Ambience);
                case AudioCueCategory.VoiceName:
                case AudioCueCategory.VoiceInstruction:
                case AudioCueCategory.Narration:
                    return OfferVoice(cue);
                default:
                    return PlayEffect(cue, clip);
            }
        }

        public AudioPlayResult ReplayLastInstruction()
        {
            return _lastInstruction == null
                ? new AudioPlayResult(AudioPlayStatus.Missing, "AudioInstructionUnavailable")
                : Play(_lastInstruction.Id);
        }

        public async Task UpdateSettingsAsync(AudioSettingsModel settings, CancellationToken cancellationToken)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            AudioSettingsModel previous = Settings;
            Settings = settings;
            ApplyVolumes();
            SaveOperationResult result = _checkpoints == null
                ? await SaveSettingsDirectlyAsync(settings, cancellationToken)
                : await _checkpoints.UpdateAndFlushAsync(
                    progress => progress.WithPreferences(progress.Preferences.WithAudioSettings(
                        settings.Master,
                        settings.Music,
                        settings.Ambience,
                        settings.Effects,
                        settings.Voice,
                        settings.SubtitlesEnabled)),
                    cancellationToken);
            if (!result.IsSuccess)
            {
                Settings = previous;
                ApplyVolumes();
                throw new InvalidOperationException("Audio settings could not be saved: " + result.ErrorCode);
            }

            if (!Settings.SubtitlesEnabled)
            {
                PublishSubtitle(SubtitleModel.Hidden);
            }
            else if (_voiceQueue.Current != null)
            {
                PublishVoiceSubtitle(_voiceQueue.Current);
            }
        }

        private Task<SaveOperationResult> SaveSettingsDirectlyAsync(
            AudioSettingsModel settings,
            CancellationToken cancellationToken)
        {
            PlayerPreferences updatedPreferences = _save.Current.Preferences.WithAudioSettings(
                settings.Master,
                settings.Music,
                settings.Ambience,
                settings.Effects,
                settings.Voice,
                settings.SubtitlesEnabled);
            return _save.SaveAsync(_save.Current.WithPreferences(updatedPreferences), cancellationToken);
        }

        public void SetApplicationSuspended(bool suspended)
        {
            if (!_initialized || _suspended == suspended)
            {
                return;
            }

            _suspended = suspended;
            foreach (AudioSource source in Sources())
            {
                if (source == null)
                {
                    continue;
                }

                if (suspended)
                {
                    source.Pause();
                }
                else
                {
                    source.UnPause();
                }
            }
        }

        private void CreateSources()
        {
            _driver = _host.GetComponent<AudioServiceDriver>() ?? _host.AddComponent<AudioServiceDriver>();
            if (_music != null)
            {
                return;
            }
            _music = CreateSource(_musicGroup, loop: true);
            _ambience = CreateSource(_ambienceGroup, loop: true);
            _voice = CreateSource(_voiceGroup, loop: false);
            for (int index = 0; index < _effects.Length; index++)
            {
                _effects[index] = CreateSource(_effectsGroup, loop: false);
            }
        }

        private AudioSource CreateSource(AudioMixerGroup group, bool loop)
        {
            var source = _host.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            source.outputAudioMixerGroup = group;
            source.priority = 128;
            return source;
        }

        private AudioPlayResult PlayExclusive(AudioSource source, UnityAudioCue cue, AudioClip clip, float busVolume)
        {
            bool replaced = source.isPlaying;
            source.Stop();
            source.clip = clip;
            source.loop = cue.Loop;
            source.volume = cue.Gain * Settings.Master * busVolume *
                            (IsVoiceDucking ? DuckMultiplier : 1f);
            source.Play();
            return new AudioPlayResult(replaced ? AudioPlayStatus.Replaced : AudioPlayStatus.Started);
        }

        private AudioPlayResult PlayEffect(UnityAudioCue cue, AudioClip clip)
        {
            int index = Array.FindIndex(_effects, source => !source.isPlaying);
            bool replaced = false;
            if (index < 0)
            {
                index = Array.IndexOf(_effectPriorities, _effectPriorities.Min());
                if (_effectPriorities[index] > cue.Priority)
                {
                    return new AudioPlayResult(AudioPlayStatus.Disabled, "AudioConcurrencyLimit");
                }
                replaced = true;
                _effects[index].Stop();
            }

            _effectPriorities[index] = cue.Priority;
            AudioSource source = _effects[index];
            source.clip = clip;
            source.loop = false;
            source.volume = cue.Gain * Settings.Master * Settings.Effects;
            source.Play();
            return new AudioPlayResult(replaced ? AudioPlayStatus.Replaced : AudioPlayStatus.Started);
        }

        private AudioPlayResult OfferVoice(UnityAudioCue cue)
        {
            VoiceOfferResult result = _voiceQueue.Offer(cue);
            switch (result)
            {
                case VoiceOfferResult.Start:
                    StartVoice(cue);
                    return new AudioPlayResult(AudioPlayStatus.Started);
                case VoiceOfferResult.Interrupt:
                    _voice.Stop();
                    StartVoice(cue);
                    return new AudioPlayResult(AudioPlayStatus.Replaced);
                case VoiceOfferResult.Queue:
                    return new AudioPlayResult(AudioPlayStatus.Queued);
                default:
                    return new AudioPlayResult(AudioPlayStatus.Disabled, "AudioVoiceQueueFull");
            }
        }

        private void StartVoice(UnityAudioCue cue)
        {
            AudioClip clip = cue.ClipFor(_localization.CurrentLocaleCode);
            if (clip == null)
            {
                TickVoiceCompletion();
                return;
            }

            if (cue.Category == AudioCueCategory.VoiceInstruction)
            {
                _lastInstruction = cue;
            }
            _voice.clip = clip;
            _voice.loop = false;
            _voice.volume = cue.Gain * Settings.Master * Settings.Voice;
            SetDucking(true);
            PublishVoiceSubtitle(cue);
            _voice.Play();
        }

        private void Tick()
        {
            if (!_initialized || _suspended || _voiceQueue.Current == null || _voice.isPlaying)
            {
                return;
            }

            TickVoiceCompletion();
        }

        private void TickVoiceCompletion()
        {
            UnityAudioCue next = _voiceQueue.CompleteAndTakeNext();
            if (next == null)
            {
                SetDucking(false);
                PublishSubtitle(SubtitleModel.Hidden);
                return;
            }

            StartVoice(next);
        }

        private void SetDucking(bool active)
        {
            IsVoiceDucking = active;
            ApplyVolumes();
        }

        private void ApplyVolumes()
        {
            float duck = IsVoiceDucking ? DuckMultiplier : 1f;
            if (_music != null)
            {
                _music.volume = (_music.clip == null ? 1f : CueGain(_music.clip)) * Settings.Master * Settings.Music * duck;
            }
            if (_ambience != null)
            {
                _ambience.volume = (_ambience.clip == null ? 1f : CueGain(_ambience.clip)) * Settings.Master * Settings.Ambience * duck;
            }
            if (_voice != null)
            {
                _voice.volume = (_voice.clip == null ? 1f : CueGain(_voice.clip)) * Settings.Master * Settings.Voice;
            }
            foreach (AudioSource source in _effects)
            {
                if (source != null)
                {
                    source.volume = (source.clip == null ? 1f : CueGain(source.clip)) * Settings.Master * Settings.Effects;
                }
            }
        }

        private float CueGain(AudioClip clip)
        {
            return _cues.Values.FirstOrDefault(cue => cue.SpanishClip == clip || cue.EnglishClip == clip)?.Gain ?? 0.5f;
        }

        private void PublishVoiceSubtitle(UnityAudioCue cue)
        {
            PublishSubtitle(Settings.SubtitlesEnabled && cue.HasSubtitle
                ? new SubtitleModel(cue.Id, cue.SubtitleKey, true)
                : SubtitleModel.Hidden);
        }

        private void PublishSubtitle(SubtitleModel model)
        {
            _currentSubtitle = model;
            SubtitleChanged?.Invoke(model);
        }

        private void OnLocaleChanged(string localeCode)
        {
            if (_currentSubtitle.Visible)
            {
                SubtitleChanged?.Invoke(_currentSubtitle);
            }
        }

        private IEnumerable<AudioSource> Sources()
        {
            if (_music != null) yield return _music;
            if (_ambience != null) yield return _ambience;
            if (_voice != null) yield return _voice;
            foreach (AudioSource source in _effects) yield return source;
        }
    }
}
