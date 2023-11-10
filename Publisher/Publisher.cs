using RabbitMQ.Client;
using RabbitMQMessageReader.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace RabbitMQMessageReader.Publisher
{
    public class Publisher : IDisposable
    {
        private readonly IModel channel;

        public Publisher([NotNull] IModel channel)
        {
            this.channel = channel;
        }

        public void PublishMessage(string exchange, string routingkey, object messageBody)
        {
            ReadOnlyMemory<byte> body = new(JsonSerializer.SerializeToUtf8Bytes(messageBody));

            PublishMessage(exchange, routingkey, body);
        }

        public void PublishMessage(string exchange, string routingkey, ReadOnlyMemory<byte> messageBody)
        {
            channel.BasicPublish(exchange, routingkey, channel.CreateBasicProperties(), messageBody);
        }

        public void PublishMessages(IList<RabbitMQMessage> messages)
        {
            IBasicPublishBatch batch = channel.CreateBasicPublishBatch();

            foreach (RabbitMQMessage message in messages)
            {
                batch.Add(message.Exchange, message.RoutingKey, false, channel.CreateBasicProperties(), message.Body);

                batch.Publish();
            }
        }

        public void Dispose()
        {
            if (channel.IsOpen)
            {
                channel.Close();
            }
        }

    }
}