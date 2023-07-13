using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQLib.Interface;
using RabbitMQLib.Model;

namespace RabbitMQLib.Service
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly RabbitMqConfiguration _configuration;
        public RabbitMqService(IOptions<RabbitMqConfiguration> options)
        {
            _configuration = options.Value;
        }
        public IConnection CreateChannel()
        {
            ConnectionFactory connection = new ConnectionFactory()
            {
                UserName = _configuration.Username,
                Password = _configuration.Password,
                HostName = _configuration.HostName,
                Port = _configuration.Port,
                VirtualHost = _configuration.VirtualHost,
                SocketReadTimeout = _configuration.SocketReadTimeout,
                SocketWriteTimeout = _configuration.SocketWriteTimeout,
            };
            connection.DispatchConsumersAsync = true;
            var channel = connection.CreateConnection();
            return channel;
        }
    }
}
