using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQClient
{
    /// <summary>
    /// 竞争消费者模式
    /// </summary>
    public static class WorkQueues
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        /// <param name="queue"></param>
        public static void WorkQueuesPublish<T>(this RabbitMQClient client, T msg, string queue) where T : class, new()
        {
            using (var channel = client.Connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var message = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: queue,
                                     basicProperties: properties,
                                     body: body);
            }
        }

        /// <summary>
        /// 工作者订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="queue"></param>
        /// <param name="handler"></param>
        public static void WorkQueuesSubscribe<T>(this RabbitMQClient client, string queue, Action<T> handler) where T : class, new()
        {
            var channel = client.Connection.CreateModel();
            channel.QueueDeclare(queue: queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                T msg = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message);

                handler(msg);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: queue,
                                 autoAck: false,
                                 consumer: consumer);
        }
    }
}