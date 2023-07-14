using RabbitMQPublisher.DependencyInjectionExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Set up RabbitMQ
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

app.Run();
