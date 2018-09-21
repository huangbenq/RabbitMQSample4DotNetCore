using Microsoft.Extensions.Logging;
using RabbitMQClient;

namespace RabbitMQSubscribe3.RabbitMQ
{
    public class QueueTest : PublishSubscribeBase<Message>
    {
        public QueueTest(RabbitMQClient.RabbitMQClient client, ILogger logger) : base(client, logger, "logs")
        {
        }

        public override void ExceptionHandler(Message msg)
        {
        }

        public override void Subscribe(Message msg)
        {
            logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(msg));
            System.Threading.Thread.Sleep(1000);
            // throw new NotImplementedException();
        }
    }

    public class Message
    {
        public string a { get; set; }

        public string b { get; set; }
    }
}
