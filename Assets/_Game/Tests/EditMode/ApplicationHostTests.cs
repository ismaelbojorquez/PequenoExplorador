using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Lifecycle;
using PequenoExplorador.Tests.EditMode.Fixtures;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class ApplicationHostTests
    {
        [Test]
        public async Task InitializesInDeclaredOrderAndShutsDownInReverseOrder()
        {
            var trace = new List<string>();
            var first = new RecordingApplicationService("First", trace);
            var second = new RecordingApplicationService("Second", trace);
            using var host = new ApplicationHost(
                new[] { first, second },
                new RecordingLogger());

            await host.InitializeAsync(CancellationToken.None);
            host.Shutdown();

            Assert.That(trace, Is.EqualTo(new[]
            {
                "initialize:First",
                "initialize:Second",
                "shutdown:Second",
                "shutdown:First"
            }));
            Assert.That(host.State, Is.EqualTo(ApplicationState.Shutdown));
        }

        [Test]
        public async Task InitializeAndShutdownAreIdempotent()
        {
            var service = new RecordingApplicationService("Only", new List<string>());
            using var host = new ApplicationHost(new[] { service }, new RecordingLogger());

            await host.InitializeAsync(CancellationToken.None);
            await host.InitializeAsync(CancellationToken.None);
            host.Shutdown();
            host.Shutdown();

            Assert.That(service.InitializeCount, Is.EqualTo(1));
            Assert.That(service.ShutdownCount, Is.EqualTo(1));
        }

        [Test]
        public async Task DisposeShutsDownInitializedServicesExactlyOnce()
        {
            var service = new RecordingApplicationService("Only", new List<string>());
            var host = new ApplicationHost(new[] { service }, new RecordingLogger());

            await host.InitializeAsync(CancellationToken.None);
            host.Dispose();
            host.Dispose();

            Assert.That(host.State, Is.EqualTo(ApplicationState.Shutdown));
            Assert.That(service.ShutdownCount, Is.EqualTo(1));
            Assert.Throws<ObjectDisposedException>(() => host.InitializeAsync(CancellationToken.None));
        }

        [Test]
        public async Task ConcurrentInitializationSharesOneOperation()
        {
            var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var service = new RecordingApplicationService("Only", new List<string>())
            {
                InitializeBehavior = async cancellationToken => await gate.Task
            };
            using var host = new ApplicationHost(new[] { service }, new RecordingLogger());

            Task first = host.InitializeAsync(CancellationToken.None);
            Task second = host.InitializeAsync(CancellationToken.None);
            gate.SetResult(true);
            await Task.WhenAll(first, second);

            Assert.That(second, Is.SameAs(first));
            Assert.That(service.InitializeCount, Is.EqualTo(1));
            Assert.That(host.State, Is.EqualTo(ApplicationState.Ready));
        }

        [Test]
        public void InitializationFailureCleansInitializedServicesAndRemainsRecoverable()
        {
            var trace = new List<string>();
            var first = new RecordingApplicationService("First", trace);
            var failing = new RecordingApplicationService("Failing", trace)
            {
                InitializeBehavior = cancellationToken => throw new InvalidOperationException("controlled fixture")
            };
            using var host = new ApplicationHost(
                new[] { first, failing },
                new RecordingLogger());

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await host.InitializeAsync(CancellationToken.None));

            Assert.That(host.State, Is.EqualTo(ApplicationState.Failed));
            Assert.That(host.FailureCode, Is.EqualTo(nameof(InvalidOperationException)));
            Assert.That(first.ShutdownCount, Is.EqualTo(1));

            failing.InitializeBehavior = cancellationToken => Task.CompletedTask;
            Assert.DoesNotThrowAsync(async () => await host.InitializeAsync(CancellationToken.None));
            Assert.That(host.State, Is.EqualTo(ApplicationState.Ready));
        }

        [Test]
        public async Task CancellationStopsInitializationAndDisposesCompletedServices()
        {
            var trace = new List<string>();
            var first = new RecordingApplicationService("First", trace);
            var blocking = new RecordingApplicationService("Blocking", trace)
            {
                InitializeBehavior = async cancellationToken =>
                    await Task.Delay(Timeout.Infinite, cancellationToken)
            };
            using var host = new ApplicationHost(
                new[] { first, blocking },
                new RecordingLogger());
            using var cancellation = new CancellationTokenSource();

            Task initialization = host.InitializeAsync(cancellation.Token);
            cancellation.Cancel();

            Assert.CatchAsync<OperationCanceledException>(async () => await initialization);
            Assert.That(host.State, Is.EqualTo(ApplicationState.Shutdown));
            Assert.That(first.ShutdownCount, Is.EqualTo(1));
        }

        [Test]
        public void DuplicateServiceIdsAreRejected()
        {
            var trace = new List<string>();

            Assert.Throws<ArgumentException>(() => new ApplicationHost(
                new[]
                {
                    new RecordingApplicationService("Duplicate", trace),
                    new RecordingApplicationService("Duplicate", trace)
                },
                new RecordingLogger()));
        }
    }
}
