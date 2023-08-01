using RabbitMQ.Client;
using System.Collections.Generic;

namespace RabbitMQMessageReader
{
    public class Configurer
    {
        public void Configure(IModel channel)
        {
            channel.ExchangeDeclare("test-exchange", ExchangeType.Topic, true);

            QueueDeclareOk queueDeclare = channel.QueueDeclare("test-queue", true, false, false);

            channel.QueueBind(queueDeclare.QueueName, "test-exchange", "test-rk");
        }
    }
}