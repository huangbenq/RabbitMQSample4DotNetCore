using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQClient
{
    /// <summary>
    /// 发布订阅模式
    /// </summary>
    public static class PublishSubscribe
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        /// <param name="exchange"></param>
        public static void Publish<T>(this RabbitMQClient client, T msg, string exchange) where T : class, new()
        {
            using (var channel = client.Connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchange, type: "fanout");

                var message = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: exchange,
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);
            }
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="exchange"></param>
        /// <param name="handler"></param>
        public static void Subscribe<T>(this RabbitMQClient client, string exchange, Action<T> handler) where T : class, new()
        {
            var channel = client.Connection.CreateModel();
            channel.ExchangeDeclare(exchange: exchange, type: "fanout");

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: exchange,
                              routingKey: "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var msg = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message);

                handler(msg);
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }
    }
}