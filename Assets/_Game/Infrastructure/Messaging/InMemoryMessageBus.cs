using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Messaging;

namespace PequenoExplorador.Infrastructure.Messaging
{
    public sealed class InMemoryMessageBus : IMessageBus
    {
        private readonly Dictionary<Type, List<Subscription>> _subscriptions =
            new Dictionary<Type, List<Subscription>>();
        private bool _isInitialized;

        public string ServiceId => "MessageBus";

        public int ActiveSubscriptionCount => _subscriptions.Values.Sum(items => items.Count);

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _isInitialized = true;
            return Task.CompletedTask;
        }

        public IDisposable Subscribe<TMessage>(Action<TMessage> handler)
        {
            EnsureInitialized();
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Type messageType = typeof(TMessage);
            if (!_subscriptions.TryGetValue(messageType, out List<Subscription> subscriptions))
            {
                subscriptions = new List<Subscription>();
                _subscriptions.Add(messageType, subscriptions);
            }

            var subscription = new Subscription(this, messageType, message => handler((TMessage)message));
            subscriptions.Add(subscription);
            return subscription;
        }

        public void Publish<TMessage>(TMessage message)
        {
            EnsureInitialized();
            if (!_subscriptions.TryGetValue(typeof(TMessage), out List<Subscription> subscriptions))
            {
                return;
            }

            foreach (Subscription subscription in subscriptions.ToArray())
            {
                subscription.Invoke(message);
            }
        }

        public void Shutdown()
        {
            foreach (Subscription subscription in _subscriptions.Values.SelectMany(items => items).ToArray())
            {
                subscription.Detach();
            }

            _subscriptions.Clear();
            _isInitialized = false;
        }

        private void Remove(Subscription subscription)
        {
            if (_subscriptions.TryGetValue(subscription.MessageType, out List<Subscription> subscriptions))
            {
                subscriptions.Remove(subscription);
                if (subscriptions.Count == 0)
                {
                    _subscriptions.Remove(subscription.MessageType);
                }
            }
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Message bus is not initialized.");
            }
        }

        private sealed class Subscription : IDisposable
        {
            private InMemoryMessageBus _owner;
            private Action<object> _handler;

            public Subscription(InMemoryMessageBus owner, Type messageType, Action<object> handler)
            {
                _owner = owner;
                MessageType = messageType;
                _handler = handler;
            }

            public Type MessageType { get; }

            public void Invoke(object message)
            {
                _handler?.Invoke(message);
            }

            public void Dispose()
            {
                if (_owner == null)
                {
                    return;
                }

                InMemoryMessageBus owner = _owner;
                Detach();
                owner.Remove(this);
            }

            public void Detach()
            {
                _owner = null;
                _handler = null;
            }
        }
    }
}
