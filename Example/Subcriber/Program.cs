using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQSubcriber.Common.Enums;
using RabbitMQSubcriber.DependencyInjectionExtensions;
using RabbitMQSubcriber.Interface;

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

        var subcriber = queueService.CreateSubscriber(option =>
        {
            option.ExchangeOrQueue = ExchangeOrQueueEnum.Exchange;
            option.ExchangeName = "ExchangeTest.1";
            option.QueueName = "queue.2";
            option.ExchangeType = ExchangeTypeEnum.headers;
            option.AutoDelete = false;
            option.AutoAck = false;
            option.RoutingKeys = new HashSet<string>()
            {
                "queue.2"
            };
            option.BindArguments.Add("x-match", "any");
            option.BindArguments.Add("q2", "queue 2");
        });

        subcriber.Subscribe(opt =>
        {
            Console.WriteLine("sub 1 called: {0}", opt.ToString());
            subcriber.Acknowledge(opt.DeliveryTag);
        });

        Console.ReadKey();
    }
}