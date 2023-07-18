using RabbitMQPublisher.Common.Events;

namespace RabbitMQPublisher.Interface
{
    public interface IPublisher : IDisposable
    {
        event EventHandler<PublisherMessageReturnEventArgs> OnMessageReturn;
        event EventHandler<PublisherMessageReturnEventArgs> OnMessageFailed;

        void SendMessage(RabbitMessageOutbound @object);
    }
}
