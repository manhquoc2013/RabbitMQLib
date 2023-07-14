using RabbitMQ.Client;
using RabbitMQPublisher.Common.Options;

namespace RabbitMQPublisher.Interface
{
    public interface IQueueService : IDisposable
    {
        internal IConnection Connection { get; }

        RabbitMQCoreOptions Options { get; }

        public event Action OnConnectionShutdown;

        bool IsConnected { get; }

        void Connect();
        IPublisher CreatePublisher(Action<PublisherOptions> options);
    }
}
