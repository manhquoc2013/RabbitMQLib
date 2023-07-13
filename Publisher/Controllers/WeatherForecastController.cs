using Microsoft.AspNetCore.Mvc;
using RabbitMQLib.Constants;
using RabbitMQLib.Interface;

namespace Publisher.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IPublisherService<object> _publisherService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IPublisherService<object> publisherService)
        {
            _logger = logger;
            _publisherService = publisherService;
        }

        [HttpPost(Name = "SendMessage")]
        public IActionResult Get(WeatherForecast weather)
        {
            _publisherService.SendMessgaes(QueueNameConstant.DefaultExchangeName, QueueNameConstant.DefaultQueueName, (object)weather);

            return Ok(weather);
        }
    }
}