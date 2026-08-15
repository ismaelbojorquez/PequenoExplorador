using System;
using PequenoExplorador.Application.Lifecycle;

namespace PequenoExplorador.Application.Messaging
{
    public interface IMessageBus : IApplicationService
    {
        IDisposable Subscribe<TMessage>(Action<TMessage> handler);

        void Publish<TMessage>(TMessage message);
    }
}
