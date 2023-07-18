using RabbitMQSubscriber.Common.Events;

namespace RabbitMQSubscriber.Interface
{
    public interface ISubscriber : IDisposable
    {
        void Subscribe(Action<RabbitMessageInbound> action);
        void Unsubscribe();
        void Acknowledge(ulong DeliveryTag, bool multiple = false);
        void NotAcknowledge(ulong DeliveryTag, bool multiple = false);
    }
}
