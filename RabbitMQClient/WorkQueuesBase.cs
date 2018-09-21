using Microsoft.Extensions.Logging;
using System;

namespace RabbitMQClient
{
    /// <summary>
    /// 消息队列基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WorkQueuesBase<T> where T : class, new()
    {
        private string _queue;

        /// <summary>
        /// mq客户端
        /// </summary>
        protected RabbitMQClient client;

        /// <summary>
        /// 日记操作
        /// </summary>
        protected ILogger logger;

        /// <summary>
        /// 消息队列基类
        /// </summary>
        /// <param name="client"></param>
        /// <param name="queue"></param>
        /// <param name="logger"></param>
        public WorkQueuesBase(RabbitMQClient client, ILogger logger, string queue)
        {
            _queue = queue;
            this.client = client;
            this.logger = logger;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="msg"></param>
        protected void Publish(T msg)
        {
            this.client.WorkQueuesPublish(msg, _queue);
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
            this.client.WorkQueuesSubscribe<T>(_queue, m =>
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