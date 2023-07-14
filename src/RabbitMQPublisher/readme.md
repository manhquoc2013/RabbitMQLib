# RabbitMQ Client library for .NET Core

Library Version: v1.0.0

Breaking changes in version 1

If you want to use specific version then Install

```powershell
Install-Package RabbitMQ.Publisher -Version 1.0.0
```

## Installation

```powershell
Install-Package RabbitMQ.Publisher
```

## Usage

RabbitMQ.NET is a simple library to Publish easily in .NET Core applications.

### Setup DI

``` C#
var serviceProvider = new ServiceCollection()
        .AddLogging(loggingBuilder =>
        {
            loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        })
        .AddRabbitMQCore(option =>
        {
            option.HostName = "localhost";
            option.UserName = "guest";
            option.Password = "guest";
            option.VirtualHost = "/";
            option.Port = 5672;
        })
        .BuildServiceProvider();
```

### Get QueueService

``` C#
var rmq = serviceProvider.GetRequiredService<IQueueService>();
```

### Publisher Examples

#### Publish on Exchange

``` C#
var pub1 = rmq.CreatePublisher(option =>
{
    option.ExchangeOrQueue = RabbitMqCore.Enums.ExchangeOrQueue.Exchange;
    option.ExchangeName = "exchange.1";
    option.ExchangeType = RabbitMqCore.Enums.ExchangeType.direct;
});
var obj = new SimpleObject() { ID = 1, Name = "One" };
var message = new RabbitMessageOutbound()
{
    Message = JsonConvert.SerializeObject(obj)
};
pub1.SendMessage(message);
```

#### Publish on Exchange with Routing Key

``` #
var pub1 = rmq.CreatePublisher(option =>
{
    option.ExchangeOrQueue = ExchangeOrQueueEnum.Exchange;
    option.ExchangeName = "exchange.1";
    option.QueueName = "queue.1";
    option.ExchangeType = ExchangeTypeEnum.direct;
    option.RoutingKeys.Add("routing.key");
});
var obj1 = new SimpleObject() { ID = 2, Name = "Two" };
var message1 = new RabbitMessageOutbound()
{
    Message = JsonConvert.SerializeObject(obj1)
};
pub1.SendMessage(message1);
```

#### Publish on Exchange with Header

``` #
var pub2 = rmq.CreatePublisher(option =>
{
    option.ExchangeOrQueue = ExchangeOrQueueEnum.Exchange;
    option.ExchangeName = "exchange.2";
    option.QueueName = "queue.2";
    option.ExchangeType = ExchangeTypeEnum.headers;
    option.BindArguments.Add("x-match", "any");
    option.BindArguments.Add("header", "some header");
});
var obj2 = new SimpleObject() { ID = 2, Name = "Two" };
var message2 = new RabbitMessageOutbound()
{
    Message = JsonConvert.SerializeObject(obj2)
};
pub2.SendMessage(message2);
```

#### Publish on Queue

``` C#
var pub3 = rmq.CreatePublisher(option =>
{
    option.ExchangeOrQueue = RabbitMqCore.Enums.ExchangeOrQueue.Queue;
    option.QueueName = "queue.3";
});
pub3.SendMessage(message);
```

## License

This library licensed under the MIT license.
