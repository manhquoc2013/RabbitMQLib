using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQPublisher.Common.Enums;
using RabbitMQPublisher.Common.Events;
using RabbitMQPublisher.DependencyInjectionExtensions;
using RabbitMQPublisher.Interface;

internal class Program
{
    private static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
                    .AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                    })
                    .AddRabbitMQCore(option =>
                    {
                        option.HostName = "10.10.74.40";
                        option.UserName = "rabbitmq";
                        option.Password = "Epay2023";
                        option.Port = 5672;
                    })
                    .BuildServiceProvider();

        var queueService = serviceProvider.GetRequiredService<IQueueService>();

        var publisher_1 = queueService.CreatePublisher(option =>
        {
            option.ExchangeOrQueue = ExchangeOrQueueEnum.Exchange;
            option.ExchangeName = "ExchangeTest.2";
            option.ExchangeType = ExchangeTypeEnum.direct;
            option.AutoDelete = false;
            option.RoutingKeys.Add("queue.3");
        });

        var message = new RabbitMessageOutbound()
        {
            Message = ""
        };
        publisher_1.SendMessage(message);

        Console.ReadKey();
    }
}