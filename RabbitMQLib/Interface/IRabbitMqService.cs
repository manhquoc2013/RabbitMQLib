using RabbitMQ.Client;

namespace RabbitMQLib.Interface
{
    public interface IRabbitMqService
    {
        IConnection CreateChannel();
    }
}
