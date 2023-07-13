namespace RabbitMQLib.Interface
{
    public interface IPublisherService<T>
    {
        Task SendMessgaes(string exchange, string route, T message);
    }
}
