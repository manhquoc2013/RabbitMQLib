# RabbitMQ Client library for .NET Core

Library Version: v1.0.0

Breaking changes in version 1

If you want to use specific version then Install

```powershell
Install-Package RabbitMQ.Subscriber -Version 1.0.0
```

## Installation

```powershell
Install-Package RabbitMQ.Subscriber
```

## Usage

RabbitMQ.NET is a simple library to Subscribe easily in .NET Core applications.

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

### Subscriber Examples

#### Subscribe with Exchange, Queue and with Routing Key

``` C#
var sub1 = rmq.CreateSubscriber(option =>
{
    option.ExchangeOrQueue = RabbitMqCore.Enums.ExchangeOrQueue.Exchange;
    option.ExchangeName = "exchange.1";
    option.QueueName = "queue.1";
    option.RoutingKeys.Add("routing.key.1");
});
sub1.Subscribe(opt => { Console.WriteLine("sub 1 called: {0}", opt.ToString()); });
```

#### Subscribe with Queue

``` C#
var sub3 = rmq.CreateSubscriber(option =>
{
    option.ExchangeOrQueue = RabbitMqCore.Enums.ExchangeOrQueue.Queue;
    option.QueueName = "queue.3";
});
sub3.Subscribe(opt => { Console.WriteLine("sub 3 message:{0}", opt.Message); });
```

#### Subscribe with Exchange. Temporary queue will be created automatically

``` C#
var sub4 = rmq.CreateSubscriber(option =>
{
    option.ExchangeOrQueue = RabbitMqCore.Enums.ExchangeOrQueue.Exchange;
    option.ExchangeName = "exchange.1";
});
sub4.Subscribe(opt => { Console.WriteLine("sub 4 called: {0}", opt.ToString()); });
```

#### Subscribe to Exchange with Routing key

``` C#
var sub5 = rmq.CreateSubscriber(option =>
{
    option.ExchangeOrQueue = RabbitMqCore.Enums.ExchangeOrQueue.Exchange;
    option.ExchangeName = "exchange.1";
    option.RoutingKeys.Add("routing.key.2");
});
sub5.Subscribe(opt => { Console.WriteLine("sub 5 called: {0}", opt.ToString()); });
```

#### Subscribe to Exchange and Queue with TTL

``` C#
var sub6 = rmq.CreateSubscriber(option =>
{
    option.ExchangeOrQueue = RabbitMqCore.Enums.ExchangeOrQueue.Exchange;
    option.ExchangeName = "exchange.1";
    option.QueueName = "queue.4";
    option.MessageTTL = 5000;
});
sub6.Subscribe(opt => { Console.WriteLine("sub 6 called: {0}", opt.ToString()); });
```

#### Subscribe to Exchange and Queue with header

``` C#
var sub7 = rmq.CreateSubscriber(option =>
{
    option.ExchangeOrQueue = RabbitMqCore.Enums.ExchangeOrQueue.Exchange;
    option.ExchangeName = "exchange.1";
    option.ExchangeType = ExchangeTypeEnum.headers;
    option.QueueName = "queue.4";
    option.BindArguments.Add("x-match", "any");
    option.BindArguments.Add("header", "some header");
});
sub7.Subscribe(opt => { Console.WriteLine("sub 7 called: {0}", opt.ToString()); });
```

## License

This library licensed under the MIT license.
