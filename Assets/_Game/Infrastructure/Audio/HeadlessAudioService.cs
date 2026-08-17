using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Domain.Progress;

namespace PequenoExplorador.Infrastructure.Audio
{
    public sealed class HeadlessAudioService : IAudioService
    {
        private readonly ISaveService _save;
        private readonly AutosaveCoordinator _checkpoints;

        public HeadlessAudioService(ISaveService save, AutosaveCoordinator checkpoints = null)
        {
            _save = save ?? throw new ArgumentNullException(nameof(save));
            _checkpoints = checkpoints;
            Settings = AudioSettings.CreateDefault();
        }

        public event Action<SubtitleModel> SubtitleChanged;
        public string ServiceId => "Audio";
        public AudioSettings Settings { get; private set; }
        public bool IsVoiceDucking => false;
        public int ActiveSourceCount => 0;
        public int QueuedVoiceCount => 0;

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            PlayerPreferences preferences = _save.Current.Preferences;
            Settings = new AudioSettings(
                preferences.MasterVolume,
                preferences.MusicVolume,
                preferences.AmbienceVolume,
                preferences.EffectsVolume,
                preferences.VoiceVolume,
                preferences.SubtitlesEnabled);
            return Task.CompletedTask;
        }

        public void Shutdown() => SubtitleChanged = null;
        public AudioPlayResult Play(AudioCueId cueId) => new AudioPlayResult(AudioPlayStatus.Missing, "AudioHeadless");
        public AudioPlayResult ReplayLastInstruction() => new AudioPlayResult(AudioPlayStatus.Missing, "AudioHeadless");

        public async Task UpdateSettingsAsync(AudioSettings settings, CancellationToken cancellationToken)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            SaveOperationResult result;
            if (_checkpoints == null)
            {
                PlayerPreferences updated = _save.Current.Preferences.WithAudioSettings(
                    settings.Master,
                    settings.Music,
                    settings.Ambience,
                    settings.Effects,
                    settings.Voice,
                    settings.SubtitlesEnabled);
                result = await _save.SaveAsync(_save.Current.WithPreferences(updated), cancellationToken);
            }
            else
            {
                result = await _checkpoints.UpdateAndFlushAsync(
                    progress => progress.WithPreferences(progress.Preferences.WithAudioSettings(
                        settings.Master,
                        settings.Music,
                        settings.Ambience,
                        settings.Effects,
                        settings.Voice,
                        settings.SubtitlesEnabled)),
                    cancellationToken);
            }
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException("Audio settings could not be saved: " + result.ErrorCode);
            }
        }

        public void SetApplicationSuspended(bool suspended) { }
    }
}
