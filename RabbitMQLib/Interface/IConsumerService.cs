using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQLib.Interface
{
    public interface IConsumerService
    {
        Task ReadMessgaes(string exchange, string route, AsyncEventHandler<BasicDeliverEventArgs> received);
        void CancelMessage(ulong deliveryTag);
    }
}
