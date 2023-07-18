using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQPublisher.Common.Enums;
using RabbitMQPublisher.Common.Events;
using RabbitMQPublisher.DependencyInjectionExtensions;
using RabbitMQPublisher.Interface;
using System.Text.Json;
internal class Program
{
    static Random random = new Random();

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

        IPublisher publisher = null!;

        Console.WriteLine("Start program Send message. Input max times to send and press enter to start...");

        List<PublisherOptionsModel> queues = new List<PublisherOptionsModel>() {
            new PublisherOptionsModel()
            {
                ExchangeName = "PrefTest",
                Name = "PrefTest.UI",
                Args = new Dictionary<string, object>() { { "x-match", "any" }, { "UI", "UI Control" } },
                RoutingKey = "UI"
            },
            new PublisherOptionsModel()
            {
                ExchangeName = "PrefTest",
                Name = "PrefTest.VMS",
                Args = new Dictionary<string, object>() { { "x-match", "any" }, { "VMS", "VMS Control" } },
                RoutingKey = "VMS"
            },
            new PublisherOptionsModel()
            {
                ExchangeName = "PrefTest",
                Name = "PrefTest.PLC",
                Args = new Dictionary<string, object>() { { "x-match", "any" }, { "PLC", "Barrier Control" } },
                RoutingKey = "PLC"
            },
            new PublisherOptionsModel()
            {
                ExchangeName = "PrefTest",
                Name = "PrefTest.POS",
                Args = new Dictionary<string, object>() { { "x-match", "any" }, { "POS", "POS Control" } },
                RoutingKey = "POS"
            },
            new PublisherOptionsModel()
            {
                ExchangeName = "PrefTest",
                Name = "PrefTest.CardPay",
                Args = new Dictionary<string, object>() { { "x-match", "any" }, { "CardPay", "Tap & Go payment Control" } },
                RoutingKey = "CardPay"
            },
            new PublisherOptionsModel()
            {
                ExchangeName = "PrefTest",
                Name = "PrefTest.RFIDPay",
                Args = new Dictionary<string, object>() { { "x-match", "any" }, { "RFIDPay", "RFID payment Control" } },
                RoutingKey = "RFIDPay"
            },
            new PublisherOptionsModel()
            {
                ExchangeName = "PrefTest",
                Name = "PrefTest.CashPay",
                Args = new Dictionary<string, object>() { { "x-match", "any" }, { "CashPay", "Cash payment Control" } },
                RoutingKey = "CashPay"
            }
        };
        List<string> plateTypes = new List<string>() { "While", "Blue", "Red", "Yellow" };

        int index = 0;
        string queueName = "";
        DateTime nextTimeToChangePublisher = DateTime.Now;
        DateTime nextTimeToSendMessage = DateTime.Now;

        while (true)
        {
            while (publisher == null || nextTimeToChangePublisher < DateTime.Now)
            {
                PublisherOptionsModel selectedQueue = queues[random.Next(queues.Count)];

                queueService.CreatePublisher(option =>
                {
                    option.ExchangeOrQueue = ExchangeOrQueueEnum.Exchange;
                    option.ExchangeName = selectedQueue.ExchangeName;
                    option.QueueName = selectedQueue.Name;
                    option.ExchangeType = selectedQueue.ExchangeType;
                    option.AutoDelete = selectedQueue.AutoDelete;
                    option.RoutingKeys.Add(selectedQueue.RoutingKey);
                    option.BindArguments = selectedQueue.Args;
                });

                nextTimeToChangePublisher = DateTime.Now.AddMinutes(1);
            }

            while (nextTimeToSendMessage == DateTime.Now)
            {
                DateTime dateNow = DateTime.Now;
                index++;

                string stationId = $"0{RandomString(1, true)}";
                string laneId = $"{stationId}{RandomString(2, true)}";

                DemoModel data = new DemoModel()
                {
                    TransactionId = RandomString(30),
                    Name = WordFinder2(random.Next(100)),
                    LaneId = $"laneId",
                    ChargeAmount = random.Next(10000, 100000),
                    EmployeeId = $"{stationId}{RandomString(4, true)}",
                    ShiftId = $"{laneId}{RandomString(2, true)}",
                    LaneOutRecogPlate = $"30A{RandomString(5, true)}",
                    PlateType = plateTypes[random.Next(plateTypes.Count)],
                    RFID = $"{RandomString(28, true)}",
                    TicketId = $"{RandomString(15, true)}"
                };

                var message = new RabbitMessageOutbound()
                {
                    Message = JsonSerializer.Serialize(data)
                };

                publisher.SendMessage(message);

                Console.WriteLine($"Start send message to queue {queueName} with id \"{data.Id}\"");

                nextTimeToSendMessage = index == 1000 ? dateNow.AddMinutes(2) : dateNow.AddMilliseconds(1000 / 50);
            }
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
        public DateTime Date { set; get; } = DateTime.Now;
    }

    class PublisherOptionsModel
    {
        public string Name { set; get; } = string.Empty;
        public string ExchangeName { set; get; } = string.Empty;
        public ExchangeTypeEnum ExchangeType { set; get; } = ExchangeTypeEnum.headers;
        public IDictionary<string, object> Args { set; get; } = new Dictionary<string, object>();
        public bool AutoDelete { set; get; } = false;
        public string RoutingKey { set; get; } = string.Empty;
    }

    static string RandomString(int length, bool onlyNumber = false)
    {
        string chars = $"{(onlyNumber == false ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ" : "")}0123456789";

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    static string WordFinder2(int requestedLength)
    {
        Random rnd = new Random();
        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
        string[] vowels = { "a", "e", "i", "o", "u" };

        string word = "";

        if (requestedLength == 1)
        {
            word = GetRandomLetter(rnd, vowels);
        }
        else
        {
            for (int i = 0; i < requestedLength; i += 2)
            {
                word += GetRandomLetter(rnd, consonants) + GetRandomLetter(rnd, vowels);
            }

            word = word.Replace("q", "qu").Substring(0, requestedLength); // We may generate a string longer than requested length, but it doesn't matter if cut off the excess.
        }

        return word;
    }

    static string GetRandomLetter(Random rnd, string[] letters)
    {
        return letters[rnd.Next(0, letters.Length - 1)];
    }
}