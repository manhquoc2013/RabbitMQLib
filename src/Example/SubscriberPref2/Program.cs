using Microsoft.Extensions.Configuration;
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
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Dictionary<string, long> reports = new Dictionary<string, long>();

        var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile($"appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                        .AddEnvironmentVariables();

        IConfiguration config = builder.Build();

        var serviceProvider = new ServiceCollection()
                    .AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                    })
                    .AddRabbitMQCore(config)
                    .BuildServiceProvider();

        var queueService = serviceProvider.GetRequiredService<IQueueService>();
        SubscriberOptionsModel? queueOptions = config.GetSection("SubscriberConfiguration").Get<SubscriberOptionsModel>();

        Console.WriteLine($"Start program Receive message from queue {queueOptions?.Name} and exchange {queueOptions?.ExchangeName}");
        Console.WriteLine();

        var subscriber = queueService.CreateSubscriber(option =>
        {
            option.ExchangeOrQueue = ExchangeOrQueueEnum.Exchange;
            option.ExchangeName = queueOptions?.ExchangeName ?? string.Empty;
            option.QueueName = queueOptions?.Name ?? string.Empty;
            option.ExchangeType = queueOptions?.ExchangeType ?? ExchangeTypeEnum.headers;
            option.AutoDelete = queueOptions?.AutoDelete ?? false;
            option.AutoAck = queueOptions?.AutoAck ?? false;
            option.RoutingKeys.Add(queueOptions?.RoutingKey ?? string.Empty);
            option.BindArguments = queueOptions?.Args ?? new Dictionary<string, object>();
        });

        string dicKey = "Less than 10ms";
        int tableWidth = 75;
        if (!reports.ContainsKey(dicKey))
            reports.Add(dicKey, 0);

        dicKey = "Less than 20ms";
        if (!reports.ContainsKey(dicKey))
            reports.Add(dicKey, 0);

        dicKey = "Less than 50ms";
        if (!reports.ContainsKey(dicKey))
            reports.Add(dicKey, 0);

        dicKey = "Less than 100ms";
        if (!reports.ContainsKey(dicKey))
            reports.Add(dicKey, 0);

        dicKey = "Over 100ms";
        if (!reports.ContainsKey(dicKey))
            reports.Add(dicKey, 0);

        while (true)
        {
            subscriber.Subscribe(opt =>
            {
                DemoModel? data = JsonSerializer.Deserialize<DemoModel>(opt.Message);
                DateTime dateNow = DateTime.Now;
                double totalMilliseconds = (dateNow - (data?.Date ?? dateNow)).TotalMilliseconds;

                Console.WriteLine($"Received message response time {totalMilliseconds}ms");
                Console.WriteLine($"Id: {data?.Id}, TransactionId: {data?.TransactionId}, LaneOutRecogPlate: {data?.LaneOutRecogPlate}, ChargeAmount: {data?.ChargeAmount}");
                Console.WriteLine($"Start send message time: {data?.Date.ToString("dd/MM/yyyy HH:mm:ss.fffff")}      Start receiving message time: {dateNow.ToString("dd/MM/yyyy HH:mm:ss.fffff")}");
                Console.WriteLine();

                if (totalMilliseconds < 10)
                {
                    dicKey = "Less than 10ms";
                    reports[dicKey]++;
                }
                else if (totalMilliseconds < 20)
                {
                    dicKey = "Less than 20ms";
                    reports[dicKey]++;
                }
                else if (totalMilliseconds < 50)
                {
                    dicKey = "Less than 50ms";
                    reports[dicKey]++;
                }
                else if (totalMilliseconds < 100)
                {
                    dicKey = "Less than 100ms";
                    reports[dicKey]++;
                }
                else
                {
                    dicKey = "Over 100ms";
                    reports[dicKey]++;
                }

                PrintLine(tableWidth);
                PrintRow(tableWidth, "Response time", "Count");
                PrintLine(tableWidth);

                foreach (var item in reports)
                {
                    PrintRow(tableWidth, item.Key, item.Value.ToString("#,##0"));
                    PrintLine(tableWidth);
                }
                Console.WriteLine();
                Console.WriteLine();

                subscriber.Acknowledge(opt.DeliveryTag);
            });

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
                Console.WriteLine();
        }
    }

    class DemoModel
    {
        public Guid Id { set; get; } = Guid.NewGuid();
        public string TransactionId { set; get; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LaneId { get; set; } = string.Empty;
        public string ShiftId { set; get; } = string.Empty;
        public string EmployeeId { set; get; } = string.Empty;
        public string? PlateType { set; get; }
        public string? LaneOutRecogPlate { set; get; }
        public string? TicketId { set; get; }
        public int ChargeAmount { set; get; } = 0;
        public string? RFID { set; get; }
        public DateTime Date { set; get; }
    }

    class SubscriberOptionsModel
    {
        public string Name { set; get; } = string.Empty;
        public string ExchangeName { set; get; } = string.Empty;
        public ExchangeTypeEnum ExchangeType { set; get; } = ExchangeTypeEnum.headers;
        public IDictionary<string, object> Args { set; get; } = new Dictionary<string, object>();
        public bool AutoDelete { set; get; } = false;
        public bool AutoAck { set; get; } = false;
        public string RoutingKey { set; get; } = string.Empty;
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