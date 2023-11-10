using System;

namespace RabbitMQMessageReader.Message
{
    public class RabbitMQMessage
    {
        public string Exchange { get; private set; }
        public string RoutingKey { get; private set; }
        public ulong DeliveryTag { get; private set; }
        public ReadOnlyMemory<byte> Body { get; private set; }

        public RabbitMQMessage(string exchange, string routingKey, ulong deliveryTag, ReadOnlyMemory<byte> body)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            DeliveryTag = deliveryTag;
            Body = body;
        }

    }
}