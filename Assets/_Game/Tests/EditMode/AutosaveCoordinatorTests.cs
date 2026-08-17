using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Tests.EditMode.Fixtures;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class AutosaveCoordinatorTests
    {
        [Test]
        public async Task MultipleRequestsBeforeFlushCoalesceToLatestProgress()
        {
            var save = new RecordingSaveService();
            using var coordinator = new AutosaveCoordinator(
                save,
                new RecordingLogger(),
                TimeSpan.FromMinutes(1));

            coordinator.RequestCheckpoint(PlayerProgress.CreateDefault().WithStars(1));
            coordinator.RequestCheckpoint(PlayerProgress.CreateDefault().WithStars(2));
            coordinator.RequestCheckpoint(PlayerProgress.CreateDefault().WithStars(3));
            Assert.That(coordinator.Latest.Stars, Is.EqualTo(3));
            await coordinator.FlushAsync(CancellationToken.None);

            Assert.That(save.Saved, Has.Count.EqualTo(1));
            Assert.That(save.Saved[0].Stars, Is.EqualTo(3));
        }

        [Test]
        public async Task AtomicPreferenceUpdateUsesLatestPendingProgress()
        {
            var save = new RecordingSaveService();
            using var coordinator = new AutosaveCoordinator(
                save,
                new RecordingLogger(),
                TimeSpan.FromMinutes(1));

            coordinator.RequestCheckpoint(PlayerProgress.CreateDefault().WithStars(7));
            SaveOperationResult result = await coordinator.UpdateAndFlushAsync(
                progress => progress.WithPreferences(
                    progress.Preferences.WithLanguage(LanguagePreference.English)),
                CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(save.Saved, Has.Count.EqualTo(1));
            Assert.That(save.Current.Stars, Is.EqualTo(7));
            Assert.That(save.Current.Preferences.Language, Is.EqualTo(LanguagePreference.English));
        }

        [Test]
        public async Task InFlightProgressRemainsAuthoritativeUntilSaveCompletes()
        {
            var save = new RecordingSaveService { PauseWrites = true };
            using var coordinator = new AutosaveCoordinator(
                save,
                new RecordingLogger(),
                TimeSpan.Zero);

            coordinator.RequestCheckpoint(PlayerProgress.CreateDefault().WithStars(9));
            await save.WriteStarted.Task;
            Assert.That(coordinator.Latest.Stars, Is.EqualTo(9));

            coordinator.RequestCheckpoint(coordinator.Latest.WithPreferences(
                coordinator.Latest.Preferences.WithLanguage(LanguagePreference.English)));
            save.CompleteWrite();
            await coordinator.FlushAsync(CancellationToken.None);

            Assert.That(save.Current.Stars, Is.EqualTo(9));
            Assert.That(save.Current.Preferences.Language, Is.EqualTo(LanguagePreference.English));
        }

        private sealed class RecordingSaveService : ISaveService
        {
            private readonly TaskCompletionSource<bool> _resumeWrite =
                new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            public List<PlayerProgress> Saved { get; } = new List<PlayerProgress>();
            public TaskCompletionSource<bool> WriteStarted { get; } =
                new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            public bool PauseWrites { get; set; }
            public string ServiceId => "RecordingSave";
            public PlayerProgress Current { get; private set; } = PlayerProgress.CreateDefault();
            public SaveLoadResult LastLoadResult { get; } = new SaveLoadResult(
                PlayerProgress.CreateDefault(),
                SaveLoadStatus.DefaultCreated,
                SaveUserNotice.None,
                1);
            public bool IsReadOnly => false;

            public Task InitializeAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public void Shutdown()
            {
            }

            public async Task<SaveOperationResult> SaveAsync(
                PlayerProgress progress,
                CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                WriteStarted.TrySetResult(true);
                if (PauseWrites)
                {
                    await _resumeWrite.Task;
                    PauseWrites = false;
                }
                Saved.Add(progress);
                Current = progress;
                return SaveOperationResult.Saved();
            }

            public void CompleteWrite() => _resumeWrite.TrySetResult(true);

            public Task<SaveOperationResult> ResetAsync(CancellationToken cancellationToken)
            {
                Current = PlayerProgress.CreateDefault();
                return Task.FromResult(SaveOperationResult.Saved());
            }
        }
    }
}
