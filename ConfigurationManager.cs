using RabbitMQ.Client;

namespace RabbitMQMessageReader
{
    public class ConfigurationManager
    {
        public static readonly string MainExchange = "test-main-exchange";
        public static readonly string DeadLetterExchange = "test-deadletter-exchange";
        public static readonly string MainRoutingKey = "test-main-routingkey";
        public static readonly string DeadLetterRoutingKey = "test-deadletter-routingkey";

        public static void Configure(IModel channel)
        {
            channel.ExchangeDeclare(MainExchange, ExchangeType.Topic, true);
            channel.ExchangeDeclare(DeadLetterExchange, ExchangeType.Topic, true);

            QueueDeclareOk mainQueue = channel.QueueDeclare("test-main-queue", true, false, false);
            QueueDeclareOk deadletterQueue = channel.QueueDeclare("test-deadletter-queue", true, false, false);

            channel.QueueBind(mainQueue.QueueName, MainExchange, MainRoutingKey);
            channel.QueueBind(deadletterQueue.QueueName, DeadLetterExchange, MainRoutingKey);
        }
    }
}