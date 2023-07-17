using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQSubscriber.Common.Enums;
using RabbitMQSubscriber.DependencyInjectionExtensions;
using RabbitMQSubscriber.Interface;
using System.Text.Json;

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

        Console.WriteLine("Start program Receive message to UI");

        var subscriber = queueService.CreateSubscriber(option =>
        {
            option.ExchangeOrQueue = ExchangeOrQueueEnum.Exchange;
            option.ExchangeName = "PrefTest";
            option.QueueName = "queue.UI";
            option.ExchangeType = ExchangeTypeEnum.headers;
            option.AutoDelete = false;
            option.AutoAck = false;
            option.RoutingKeys.Add("queue.UI");
            option.BindArguments.Add("x-match", "any");
            option.BindArguments.Add("ui", "queue ui");
        });

        Dictionary<string, long> reports = new Dictionary<string, long>();

        while (true)
        {
            subscriber.Subscribe(opt =>
            {
                ConsumerReceived(JsonSerializer.Deserialize<MsgModel>(opt.Message), reports);

                subscriber.Acknowledge(opt.DeliveryTag);
            });

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
                Console.WriteLine();
        }
    }

    private class MsgModel
    {
        public string? Msg { get; set; }
        public string? Content { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsStart { get; set; } = false;
        public bool IsEnd { get; set; } = false;
        public long Index { get; set; } = 0;
        public Guid Id { get; set; } = Guid.Empty;
    }
    private static void ConsumerReceived(MsgModel? msg, Dictionary<string, long> reports)
    {
        try
        {
            if (msg?.IsStart == true)
            {
                reports.Clear();
            }

            DateTime dateNow = DateTime.Now;
            double totalMilliseconds = (dateNow - (msg?.Date ?? dateNow)).TotalMilliseconds;

            string dicKey = "";
            if (totalMilliseconds < 10)
            {
                dicKey = "Less than 10ms";
                if (reports.ContainsKey(dicKey))
                {
                    reports[dicKey]++;
                }
                else
                {
                    reports.Add(dicKey, 1);
                }
            }
            else if (totalMilliseconds < 20)
            {
                dicKey = "Less than 20ms";
                if (reports.ContainsKey(dicKey))
                {
                    reports[dicKey]++;
                }
                else
                {
                    reports.Add(dicKey, 1);
                }
            }
            else if (totalMilliseconds < 50)
            {
                dicKey = "Less than 50ms";
                if (reports.ContainsKey(dicKey))
                {
                    reports[dicKey]++;
                }
                else
                {
                    reports.Add(dicKey, 1);
                }
            }
            else if (totalMilliseconds < 100)
            {
                dicKey = "Less than 100ms";
                if (reports.ContainsKey(dicKey))
                {
                    reports[dicKey]++;
                }
                else
                {
                    reports.Add(dicKey, 1);
                }
            }
            else
            {
                dicKey = "Over 100ms";
                if (reports.ContainsKey(dicKey))
                {
                    reports[dicKey]++;
                }
                else
                {
                    reports.Add(dicKey, 1);
                }
            }

            Console.WriteLine($"Message: {msg?.Msg}. End time: {dateNow.ToString("yyyy/MM/dd HH:mm:ss.ffff")}. Response time: {(dateNow - (msg?.Date ?? dateNow)).TotalMilliseconds}ms");

            if (msg?.IsEnd == true)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                int tableWidth = 50;

                PrintLine(tableWidth);
                PrintRow(tableWidth, "Response time", "Count");
                PrintLine(tableWidth);

                foreach (var item in reports)
                {
                    PrintRow(tableWidth, item.Key, item.Value.ToString("#,##0"));
                    PrintLine(tableWidth);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static void PrintLine(int tableWidth)
    {
        Console.WriteLine(new string('-', tableWidth));
    }

    static void PrintRow(int tableWidth, params string[] columns)
    {
        int width = (tableWidth - columns.Length) / columns.Length;
        string row = "|";

        foreach (string column in columns)
        {
            row += AlignCentre(column, width) + "|";
        }

        Console.WriteLine(row);
    }

    static string AlignCentre(string text, int width)
    {
        text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

        if (string.IsNullOrEmpty(text))
        {
            return new string(' ', width);
        }
        else
        {
            return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
        }
    }
}