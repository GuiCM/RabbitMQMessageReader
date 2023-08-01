using RabbitMQ.Client;

namespace RabbitMQMessageReader
{
    public class Connector
    {
        public IConnection GetConnection()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                Port = 5672,
            };

            return connectionFactory.CreateConnection();
        }
    }
}