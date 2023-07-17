using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RabbitMQPublisher.Common.Enums;
using RabbitMQPublisher.Common.Events;
using RabbitMQPublisher.Common.Exceptions;
using RabbitMQPublisher.Interface;
using System.Text.Json;

namespace Publisher.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IPublisher _publisher;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IQueueService queueService)
        {
            _logger = logger;
            _publisher = queueService.CreatePublisher(option =>
            {
                option.ExchangeOrQueue = ExchangeOrQueueEnum.Exchange;
                option.ExchangeName = "ExchangeTest.2";
                option.ExchangeType = ExchangeTypeEnum.direct;
                option.AutoDelete = false;
                option.RoutingKeys.Add("queue.3");
            });
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            var data = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            try
            {
                var message = new RabbitMessageOutbound()
                {
                    Message = JsonSerializer.Serialize(data)
                };
                _publisher.SendMessage(message);
                Console.WriteLine("pub called: {0}", message.Message);
            }
            catch (OutboundMessageFailedException ex)
            {
                Console.WriteLine("OutboundMessageFailedException Message failed {0}:{1}", data.Count(), ex.RabbitMessageOutbound.Message);
            }
            catch (NotConnectedException)
            {
                Console.WriteLine("NotConnectedException Message failed {0}", data.Count());
            }
            catch (Exception)
            {
                Console.WriteLine("Message failed {0}", data.Count());
            }


            return Ok(data);
        }
    }
}