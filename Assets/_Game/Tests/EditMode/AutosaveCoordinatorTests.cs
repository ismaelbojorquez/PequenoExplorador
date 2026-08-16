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
            await coordinator.FlushAsync(CancellationToken.None);

            Assert.That(save.Saved, Has.Count.EqualTo(1));
            Assert.That(save.Saved[0].Stars, Is.EqualTo(3));
        }

        private sealed class RecordingSaveService : ISaveService
        {
            public List<PlayerProgress> Saved { get; } = new List<PlayerProgress>();
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

            public Task<SaveOperationResult> SaveAsync(
                PlayerProgress progress,
                CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Saved.Add(progress);
                Current = progress;
                return Task.FromResult(SaveOperationResult.Saved());
            }

            public Task<SaveOperationResult> ResetAsync(CancellationToken cancellationToken)
            {
                Current = PlayerProgress.CreateDefault();
                return Task.FromResult(SaveOperationResult.Saved());
            }
        }
    }
}
