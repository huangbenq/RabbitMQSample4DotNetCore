using Microsoft.Extensions.Logging;
using System;

namespace RabbitMQClient
{
    /// <summary>
    /// 发布订阅模式基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PublishSubscribeBase<T> where T : class, new()
    {
        private string _exchange;

        /// <summary>
        /// mq客户端
        /// </summary>
        protected RabbitMQClient client;

        /// <summary>
        /// 日志操作
        /// </summary>
        protected ILogger logger;

        /// <summary>
        /// 发布订阅模式基类
        /// </summary>
        /// <param name="client"></param>
        /// <param name="exchange"></param>
        /// <param name="logger"></param>
        public PublishSubscribeBase(RabbitMQClient client, ILogger logger, string exchange)
        {
            _exchange = exchange;
            this.client = client;
            this.logger = logger;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="msg"></param>
        protected void Publish(T msg)
        {
            this.client.Publish(msg, _exchange);
        }

        /// <summary>
        /// 订阅
        /// </summary>
        public abstract void Subscribe(T msg);

        /// <summary>
        /// 异常处理事件，发生异常时调用，调用方可通过该方法进行通知或处理该队列
        /// </summary>
        /// <param name="msg"></param>
        public virtual void ExceptionHandler(T msg)
        {
        }

        /// <summary>
        ///
        /// </summary>
        public void Execute()
        {
            this.client.Subscribe<T>(_exchange, m =>
            {
                try
                {
                    Subscribe(m);
                }
                catch (Exception ex)
                {
                    ExceptionHandler(m);
                    logger.LogError(ex, ex.Message);
                }
            });
        }
    }
}