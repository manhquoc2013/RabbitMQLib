﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQPublisher.Common.Enums;
using RabbitMQPublisher.Common.Events;
using RabbitMQPublisher.DependencyInjectionExtensions;
using RabbitMQPublisher.Interface;
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

        var publisher = queueService.CreatePublisher(option =>
        {
            option.ExchangeOrQueue = ExchangeOrQueueEnum.Exchange;
            option.ExchangeName = "PrefTest";
            option.QueueName = "queue.UI";
            option.ExchangeType = ExchangeTypeEnum.headers;
            option.AutoDelete = false;
            option.RoutingKeys.Add("queue.UI");
            option.BindArguments.Add("x-match", "any");
            option.BindArguments.Add("ui", "queue ui");
        });

        Console.WriteLine("Start program Send message. Input max times to send and press enter to start...");

        bool _quitFlag = false;
        while (!_quitFlag)
        {
            ConsoleKeyInfo key;
            int maxLength = 500;
            int maxRequestPerSec = 100;
            int threadNum = 2;
            string _val = "";

            Console.WriteLine("Input request times");
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace)
                {
                    double val = 0;
                    bool _x = double.TryParse(key.KeyChar.ToString(), out val);
                    if (_x)
                    {
                        _val += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && _val.Length > 0)
                    {
                        _val = _val.Substring(0, (_val.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);

            int.TryParse(_val, out maxLength);
            _val = string.Empty;

            Console.WriteLine();
            Console.WriteLine("Input request per seconds");
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace)
                {
                    double val = 0;
                    bool _x = double.TryParse(key.KeyChar.ToString(), out val);
                    if (_x)
                    {
                        _val += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && _val.Length > 0)
                    {
                        _val = _val.Substring(0, (_val.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);

            int.TryParse(_val, out maxRequestPerSec);
            _val = string.Empty;

            Console.WriteLine();
            Console.WriteLine("Input thread to run");
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace)
                {
                    double val = 0;
                    bool _x = double.TryParse(key.KeyChar.ToString(), out val);
                    if (_x)
                    {
                        _val += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && _val.Length > 0)
                    {
                        _val = _val.Substring(0, (_val.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receiving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);

            int.TryParse(_val, out threadNum);
            _val = string.Empty;

            Console.WriteLine();

            Console.WriteLine($"Request {maxLength.ToString("#,##0")} with {maxRequestPerSec.ToString("#,##0")} per seconds and {threadNum} thread");

            var tasks = new List<Task>();
            for (int threadNo = 0; threadNo < threadNum; threadNo++)
            {
                int maxRequestPerThread = (maxLength - 1) / threadNum;
                if (threadNo == threadNum - 1)
                {
                    maxRequestPerThread = (maxLength - 1) - ((threadNum - 1) * maxRequestPerThread);
                }

                tasks.Add(Task.Run(() =>
                {
                    int i = 0;

                    while (i < maxRequestPerThread)
                    {
                        Thread.Sleep(1000 / maxRequestPerSec);
                        DateTime dateNow = DateTime.Now;
                        var message = new RabbitMessageOutbound()
                        {
                            Message = JsonSerializer.Serialize(new
                            {
                                Msg = $"Msg no. {(i + 1).ToString("000")}. Start time: {dateNow.ToString("yyyy/MM/dd HH:mm:ss.ffff")}",
                                Content = "There are many different kinds of animals that live in China. Tigers and leopards are animals that live in China's forests in the north",
                                Type = "UI",
                                IsEnd = false,
                                Index = i,
                                Id = Guid.NewGuid(),
                                Date = dateNow
                            })
                        };

                        publisher.SendMessage(message);

                        i++;
                    }
                }));
            }

            Task.WhenAll(tasks.ToArray()).ContinueWith(t =>
            {
                DateTime dateNow = DateTime.Now;
                var message = new RabbitMessageOutbound()
                {
                    Message = JsonSerializer.Serialize(new
                    {
                        Msg = $"Msg no. {maxLength.ToString("000")}. Start time: {dateNow.ToString("yyyy/MM/dd HH:mm:ss.ffff")}",
                        Content = "There are many different kinds of animals that live in China. Tigers and leopards are animals that live in China's forests in the north",
                        Type = "UI",
                        IsStart = false,
                        IsEnd = true,
                        Index = maxLength,
                        Id = Guid.NewGuid(),
                        Date = dateNow
                    })
                };

                publisher.SendMessage(message);
                Console.WriteLine();
            });
        }
    }
}