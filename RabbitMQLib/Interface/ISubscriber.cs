using RabbitMQSubcriber.Common.Events;

namespace RabbitMQSubcriber.Interface
{
    public interface ISubscriber : IDisposable
    {
        void Subscribe(Action<RabbitMessageInbound> action);
        void Unsubscribe();
        void Acknowledge(ulong DeliveryTag);
        void NotAcknowledge(ulong DeliveryTag);
    }
}
