using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQSubcriber.Common.Options;
using RabbitMQSubcriber.Interface;
using RabbitMQSubcriber.Services;

namespace RabbitMQSubcriber.DependencyInjectionExtensions
{
    public static class RabbitMqCoreExtensions
    {
        public static IServiceCollection AddRabbitMQCore(this IServiceCollection services)
        {
            var temp = new RabbitMQCoreOptions();
            services.AddSingleton(temp);
            services.TryAddSingleton<IQueueService, QueueService>();
            return AddRabbitMQCore(services);
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
