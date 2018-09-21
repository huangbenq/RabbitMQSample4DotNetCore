using RabbitMQClient;
using System;

namespace RabbitMQSample4DotNetCore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RabbitMQClient.RabbitMQClient client = new RabbitMQClient.RabbitMQClient(new RabbitMQConfig()
            {
                HostName = "192.168.8.102",
                Port = 5672,
                UserName = "hmj",
                Password = "123456"
            });

            for (int i = 0; i < 20; i++)
            {
                Message message = new Message()
                {
                    a = i.ToString(),
                    b = "a" + i
                };
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(message));
                client.WorkQueuesPublish<Message>(message, "queue_task");
            }

            for (int i = 20; i < 40; i++)
            {
                Message message = new Message()
                {
                    a = i.ToString(),
                    b = "a" + i
                };
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(message));
                client.Publish<Message>(message, "logs");
            }

            Console.ReadLine();
        }
    }

    public class Message
    {
        public string a { get; set; }

        public string b { get; set; }
    }
}