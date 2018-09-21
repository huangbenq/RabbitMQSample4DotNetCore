using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace RabbitMQClient
{
    /// <summary>
    /// RabbitMQ客户端管理类
    /// </summary>
    public class RabbitMQClient
    {
        private RabbitMQConfig _config;

        /// <summary>
        /// RabbitMQ客户端管理类
        /// </summary>
        /// <param name="configuration"></param>
        public RabbitMQClient(IConfiguration configuration)
        {
            this._config = configuration.GetSection("RabbitMQConfig").Get<RabbitMQConfig>();
        }

        /// <summary>
        /// RabbitMQ客户端管理类
        /// </summary>
        /// <param name="config"></param>
        public RabbitMQClient(RabbitMQConfig config)
        {
            this._config = config;
        }

        private static ConnectionFactory _connectionFactory;

        private ConnectionFactory ConnectionFactory
        {
            get
            {
                if (_connectionFactory == null)
                {
                    _connectionFactory = new ConnectionFactory
                    {
                        //HostName = _config.HostName,
                        Port = _config.Port,
                        UserName = _config.UserName,
                        Password = _config.Password,
                        Protocol = Protocols.DefaultProtocol,
                        AutomaticRecoveryEnabled = true, //自动重连
                        RequestedFrameMax = uint.MaxValue,
                        RequestedHeartbeat = ushort.MaxValue //心跳超时时间
                    };
                }
                return _connectionFactory;
            }
            set
            {
                _connectionFactory = value;
            }
        }

        private static IConnection _connection;

        /// <summary>
        /// mq连接
        /// </summary>
        public IConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = ConnectionFactory.CreateConnection(_config.HostName.Split(','));
                }
                return _connection;
            }
        }
    }
}