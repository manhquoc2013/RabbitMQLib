using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQLib.Constants;
using RabbitMQLib.Interface;
using System.Collections;
using System.Text;
using System.Threading.Channels;

namespace RabbitMQLib.Service
{
    public class ConsumerService : IConsumerService, IDisposable
    {
        public readonly IModel _model;
        private readonly IConnection _connection;
        public ConsumerService(IRabbitMqService rabbitMqService)
        {
            _connection = rabbitMqService.CreateChannel();
            _model = _connection.CreateModel();
        }
        public async Task ReadMessgaes(string exchange, string queueName, AsyncEventHandler<BasicDeliverEventArgs> received)
        {
            _model.ExchangeDeclare(QueueNameConstant.DefaultDeadLetterExchangeName, ExchangeType.Fanout);

            //Declare default exchange
            Hashtable arguments = new Hashtable()
            {
                {"x-dead-letter-exchange", QueueNameConstant.DefaultDeadLetterExchangeName},
                {"x-message-ttl", 1000}
            };
            _model.QueueDeclare(QueueNameConstant.DefaultDeadLetterQueueName, true, false, false, arguments.Cast<DictionaryEntry>().ToDictionary(kvp => (string)kvp.Key, kvp => kvp.Value));
            _model.QueueDeclare(queueName, true, false, false, null);
            if (!string.IsNullOrEmpty(exchange))
            {
                _model.ExchangeDeclare(exchange, ExchangeType.Direct);
                _model.QueueBind(queueName, exchange, QueueNameConstant.DefaultDeadLetterRoutingKey, null);
            }

            var consumer = new AsyncEventingBasicConsumer(_model);
            consumer.Received += received;
            _model.BasicConsume(queueName, true, consumer);

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_model.IsOpen)
                _model.Close();
            if (_connection.IsOpen)
                _connection.Close();
        }

        public void CancelMessage(ulong deliveryTag)
        {
            Console.WriteLine($"Cancel Message: {deliveryTag}");
        }
    }
}
