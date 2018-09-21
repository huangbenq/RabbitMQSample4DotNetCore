using RabbitMQClient;
using System;
using System.Threading;

namespace RabbitMQSubscribe2
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
            client.Subscribe<Message>("logs", m =>
            {
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(m));
                Thread.Sleep(3000);
            });
            Console.ReadLine();
        }

        public class Message
        {
            public string a { get; set; }

            public string b { get; set; }
        }
    }
}