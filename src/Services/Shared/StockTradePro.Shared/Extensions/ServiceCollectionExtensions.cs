using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockTradePro.Shared.Messaging;

namespace StockTradePro.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
            return services;
        }
    }
}