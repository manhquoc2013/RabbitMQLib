using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQLib.Interface;
using RabbitMQLib.Model;
using RabbitMQLib.Service;

namespace RabbitMQLib
{
    public static partial class StartupExtension
    {
        public static IServiceCollection AddCommonService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqConfiguration>(a => configuration.GetSection(nameof(RabbitMqConfiguration)).Bind(a));
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddSingleton<IConsumerService, ConsumerService>();
            services.AddSingleton<IPublisherService<object>, PublisherService>();

            return services;
        }
    }
}