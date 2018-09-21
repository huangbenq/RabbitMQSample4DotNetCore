using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitMQClient
{
    /// <summary>
    /// RabbitMQ依赖注入扩展
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// 依赖注入使用RabbitMQ
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<RabbitMQClient, RabbitMQClient>();
        }
    }
}