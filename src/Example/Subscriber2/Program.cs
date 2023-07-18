using RabbitMQSubscriber.Common.Enums;
using RabbitMQSubscriber.DependencyInjectionExtensions;
using RabbitMQSubscriber.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
})
.AddRabbitMQCore(option =>
{
    option.HostName = "10.10.74.40";
    option.UserName = "rabbitmq";
    option.Password = "Epay2023";
    option.Port = 5672;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

var queueService = app.Services.GetRequiredService<IQueueService>();

var subcriber = queueService.CreateSubscriber(option =>
{
    option.ExchangeOrQueue = ExchangeOrQueueEnum.Exchange;
    option.ExchangeName = "ExchangeTest.1";
    option.QueueName = "queue.1";
    option.ExchangeType = ExchangeTypeEnum.headers;
    option.AutoDelete = false;
    option.AutoAck = false;
    option.RoutingKeys = new HashSet<string>()
    {
        "queue.1"
    };
    option.BindArguments.Add("x-match", "any");
    option.BindArguments.Add("q1", "queue 1");
});

subcriber.Subscribe(opt =>
{
    Console.WriteLine("sub 1 called: {0}", opt.ToString());
    subcriber.Acknowledge(opt.DeliveryTag);
});

app.Run();

