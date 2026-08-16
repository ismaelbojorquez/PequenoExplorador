using System;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Lifecycle;

namespace PequenoExplorador.Application.Audio
{
    public interface IAudioService : IApplicationService
    {
        event Action<SubtitleModel> SubtitleChanged;

        AudioSettings Settings { get; }
        bool IsVoiceDucking { get; }
        int ActiveSourceCount { get; }
        int QueuedVoiceCount { get; }

        AudioPlayResult Play(AudioCueId cueId);
        AudioPlayResult ReplayLastInstruction();
        Task UpdateSettingsAsync(AudioSettings settings, CancellationToken cancellationToken);
        void SetApplicationSuspended(bool suspended);
    }
}
