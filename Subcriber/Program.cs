using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using RabbitMQLib;
using RabbitMQLib.Constants;
using RabbitMQLib.Interface;
using System.Text;

namespace Subcriber
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddCommonService(builder.Build())
                .BuildServiceProvider();

            var consumerService = serviceProvider.GetService<IConsumerService>();

            AsyncEventHandler<BasicDeliverEventArgs> received = async (sender, @event) =>
            {
                Console.WriteLine("Start Received...");
                var body = @event.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Message received: {message}");

                consumerService!.CancelMessage(@event.DeliveryTag);

                await Task.Yield();
            };

            await consumerService!.ReadMessgaes(QueueNameConstant.DefaultExchangeName, QueueNameConstant.DefaultQueueName, received);

            Console.ReadKey();
        }
    }
}