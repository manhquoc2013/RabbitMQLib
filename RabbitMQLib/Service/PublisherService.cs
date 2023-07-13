using RabbitMQ.Client;
using RabbitMQLib.Interface;
using System.Text;
using System.Text.Json;

namespace RabbitMQLib.Service
{
    public class PublisherService : IPublisherService<object>
    {
        private readonly IModel _model;
        private readonly IConnection _connection;
        public PublisherService(IRabbitMqService rabbitMqService)
        {
            _connection = rabbitMqService.CreateChannel();
            _model = _connection.CreateModel();
        }
        public async Task SendMessgaes(string exchange, string queueName, object message)
        {
            _model.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            _model.BasicPublish(exchange, queueName, basicProperties: null, body: body);
            await Task.CompletedTask;
        }
    }
}
