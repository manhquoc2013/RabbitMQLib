using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQSubscriber.Common.Options;
using RabbitMQSubscriber.Interface;
using RabbitMQSubscriber.Services;

namespace RabbitMQSubscriber.DependencyInjectionExtensions
{
    public static class RabbitMqCoreExtensions
    {
        public static IServiceCollection AddRabbitMQCore(this IServiceCollection services)
        {
            var temp = new RabbitMQCoreOptions();
            services.AddSingleton(temp);
            services.TryAddSingleton<IQueueService, QueueService>();
            return services.AddRabbitMQCore();
        }

        public static IServiceCollection AddRabbitMQCore(this IServiceCollection services, Action<RabbitMQCoreOptions> options)
        {
            var temp = new RabbitMQCoreOptions();
            options(temp);
            services.AddSingleton(temp);
            services.TryAddSingleton<IQueueService, QueueService>();
            return services;
        }

        public static IServiceCollection AddRabbitMQCore(this IServiceCollection services, IConfiguration configuration)
        {
            var temp = new RabbitMQCoreOptions();
            configuration.GetSection("RabbitMqConfiguration").Bind(temp);

            services.AddSingleton(temp);
            services.TryAddSingleton<IQueueService, QueueService>();
            return services;
        }
    }
}
