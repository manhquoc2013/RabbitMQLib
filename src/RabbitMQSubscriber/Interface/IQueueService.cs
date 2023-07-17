using RabbitMQ.Client;
using RabbitMQSubscriber.Common.Options;

namespace RabbitMQSubscriber.Interface
{
    public interface IQueueService : IDisposable
    {
        internal IConnection Connection { get; }

        RabbitMQCoreOptions Options { get; }

        public event Action OnConnectionShutdown;

        bool IsConnected { get; }

        void Connect();
        ISubscriber CreateSubscriber(Action<SubscriberOptions> options);
    }
}
