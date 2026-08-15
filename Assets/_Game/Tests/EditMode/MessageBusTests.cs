using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Infrastructure.Messaging;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class MessageBusTests
    {
        [Test]
        public async Task SubscriptionDisposeAndShutdownRemoveEveryListener()
        {
            var bus = new InMemoryMessageBus();
            await bus.InitializeAsync(CancellationToken.None);
            int received = 0;
            System.IDisposable subscription = bus.Subscribe<int>(value => received += value);

            bus.Publish(2);
            subscription.Dispose();
            bus.Publish(3);
            bus.Subscribe<int>(value => received += value);
            bus.Shutdown();

            Assert.That(received, Is.EqualTo(2));
            Assert.That(bus.ActiveSubscriptionCount, Is.Zero);
            Assert.Throws<System.InvalidOperationException>(() => bus.Publish(1));
        }
    }
}
