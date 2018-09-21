using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RabbitMQClient
{
    public class RabbitMQMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        ///
        /// </summary>
        /// <param name="next"></param>
        public RabbitMQMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="configuration"></param>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            //_services.AddTransient<RabbitMQClient, RabbitMQClient>();

            var work_queue_types = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes().Where(t => t.BaseType != null && t.BaseType.Name == typeof(WorkQueuesBase<>).Name))
                                .ToArray();

            RabbitMQClient client = new RabbitMQClient(configuration);
            foreach (var type in work_queue_types)
            {
                var logger = loggerFactory.CreateLogger(type);
                object obj = Activator.CreateInstance(type, client, logger);
                type.InvokeMember("Execute", BindingFlags.Default | BindingFlags.InvokeMethod, null, obj, null);
            }

            var publish_subscribe_types = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes().Where(t => t.BaseType != null && t.BaseType.Name == typeof(PublishSubscribeBase<>).Name))
                                .ToArray();
            foreach (var type in publish_subscribe_types)
            {
                var logger = loggerFactory.CreateLogger(type);
                object obj = Activator.CreateInstance(type, client, logger);
                type.InvokeMember("Execute", BindingFlags.Default | BindingFlags.InvokeMethod, null, obj, null);
            }

            await _next(httpContext);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public static class UseRabbitMQExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRabbitMQ(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RabbitMQMiddleware>();
        }
    }
}