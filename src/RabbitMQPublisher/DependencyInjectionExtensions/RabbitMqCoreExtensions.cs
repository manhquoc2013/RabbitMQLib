using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQPublisher.Common.Options;
using RabbitMQPublisher.Interface;
using RabbitMQPublisher.Services;

namespace RabbitMQPublisher.DependencyInjectionExtensions
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
    }
}
